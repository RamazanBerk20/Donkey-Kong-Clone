using UnityEngine;

public class Barrel : MonoBehaviour
{
    private Rigidbody2D rb2d;
    public float force = 5.0f;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            rb2d.AddForce(collision.transform.right * force, ForceMode2D.Impulse);
        }
    }
}
