using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    private GameManager gameManager;
    private GameObject heart;

    private SpriteRenderer spriteRenderer;
    public Sprite[] runSprites;
    public Sprite climbSprite;
    private int spriteIndex;

    private Rigidbody2D rb2d;
    private Vector2 direction;

    private Collider2D collider2d;
    private Collider2D[] results;

    private bool grounded;
    private bool climbing;

    public float speed = 5f;
    public float jumpForce = 5f;

    private void Awake()
    {
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
        } else if (direction.x < 0f)
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.CompareTag("Princess"))
        {
            enabled = false;
            heart.gameObject.GetComponent<SpriteRenderer>().enabled = true;
            gameManager.GetComponent<GameManager>().LevelCompleted();
        }
        else if(collision.transform.CompareTag("Barrel"))
        {
            enabled = false;
            gameManager.GetComponent<GameManager>().LevelFailed();
        }
    }
}
