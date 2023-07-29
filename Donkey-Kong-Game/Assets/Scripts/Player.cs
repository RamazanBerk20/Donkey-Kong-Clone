using UnityEngine;

public class Player : MonoBehaviour
{
    private GameManager gameManager;
    private AudioSource audioSource;
    private GameObject heart;

    private SpriteRenderer spriteRenderer;
    public Sprite[] runSprites;
    public Sprite climbSprite;
    private int spriteIndex = 0;

    private Rigidbody2D rb2d;
    private Vector2 direction;

    private Collider2D collider2d;
    private Collider2D[] results;

    private bool grounded;
    private bool climbing;

    private int runClipIndex;
    private int climbClipIndex;

    public AudioClip jumpClip;
    public AudioClip deathClip;
    public AudioClip winClip;
    public AudioClip[] runClips;
    public AudioClip[] climbClips;

    public float speed = 5f;
    public float jumpForce = 5f;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        gameManager = FindObjectOfType<GameManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb2d = GetComponent<Rigidbody2D>();
        collider2d = GetComponent<Collider2D>();
        results = new Collider2D[4];
    }
    void Start()
    {
        heart = GameObject.Find("Heart");
    }

    private void OnEnable()
    {
        InvokeRepeating("AnimateSprite", 1/12f, 1/12f);
        InvokeRepeating("RunningSound", 1/3f, 1/3f);
        InvokeRepeating("ClimbingSound", 1/3f, 1/3f);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void CheckCollision()
    {
        grounded = false;
        climbing = false;

        Vector2 size = collider2d.bounds.size;
        size.y += 0.1f;
        size.x /= 2f;

        int amount = Physics2D.OverlapBoxNonAlloc(transform.position, size, 0f, results);

        for (int i = 0; i < amount; i++)
        {
            GameObject hit = results[i].gameObject;

            if(hit.layer == LayerMask.NameToLayer("Ground"))
            {
                grounded = hit.transform.position.y < (transform.position.y - 0.5f);

                Physics2D.IgnoreCollision(collider2d, results[i], !grounded);
            } 
            else if (hit.layer == LayerMask.NameToLayer("Ladder"))
            {
                climbing = true;
            }
        }
    }

    private void Update()
    {
        CheckCollision();

        if(climbing)
        {
            direction.y = Input.GetAxisRaw("Vertical") * speed;
        }
        else if(grounded && Input.GetButtonDown("Jump"))
        {
            direction = Vector2.up * jumpForce;
            audioSource.PlayOneShot(jumpClip);
        } 
        else
        {
            direction += Physics2D.gravity * Time.deltaTime;
        }

        direction.x = Input.GetAxisRaw("Horizontal") * speed;
        if(grounded)
            direction.y = Mathf.Max(direction.y, -1f);

        if (direction.x > 0f)
        {
            transform.eulerAngles = Vector3.zero;
        } 
        else if (direction.x < 0f)
        {
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }
    }

    private void FixedUpdate()
    {
        rb2d.MovePosition(rb2d.position + direction * Time.fixedDeltaTime);
    }

    private void AnimateSprite()
    {
        if(climbing)
        {
            spriteRenderer.sprite = climbSprite;
        }
        else if (direction.x != 0f)
        {
            spriteIndex++;
            
            if(spriteIndex >= runSprites.Length)
            {
                spriteIndex = 0;
            }

            spriteRenderer.sprite = runSprites[spriteIndex];
        }
        else
        {
            spriteRenderer.sprite = runSprites[0];
        }
    }

    private void RunningSound()
    {
        if (direction.x != 0f && grounded)
        {
            runClipIndex++;

            if (runClipIndex >= runClips.Length)
            {
                runClipIndex = 0;
            }

            audioSource.PlayOneShot(runClips[runClipIndex]);
        }
    }

    private void ClimbingSound()
    {
        if(direction.y != 0f && climbing)
        {
              climbClipIndex++;

            if (climbClipIndex >= climbClips.Length)
            {
                climbClipIndex = 0;
            }

            audioSource.PlayOneShot(climbClips[climbClipIndex]);
        }
    }

    private void DeathSound()
    {
        audioSource.PlayOneShot(deathClip);
    }

    private void WinSound()
    {
        audioSource.PlayOneShot(winClip);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.CompareTag("Princess"))
        {
            enabled = false;
            heart.GetComponent<SpriteRenderer>().enabled = true;
            WinSound();
            gameManager.GetComponent<GameManager>().LevelCompleted();
        }
        else if(collision.transform.CompareTag("Barrel"))
        {
            enabled = false;
            DeathSound();
            gameManager.GetComponent<GameManager>().LevelFailed();
        }
    }
}
