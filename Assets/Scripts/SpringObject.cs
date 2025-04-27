using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringObject : MonoBehaviour
{
    [SerializeField] private float height = 10f;

    private void OnTriggerEnter2D(Collider2D collision)
    {

        Rigidbody2D rb = collision.attachedRigidbody;
        if (rb != null)
        {
            float gravity = Physics2D.gravity.y;
            float velocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * height);
            Vector2 force = transform.up.normalized * velocity;
            if (collision.gameObject.TryGetComponent<MoveComponent>(out MoveComponent mc))
            {
                mc.SetCurrentSpeed(new Vector2(mc.GetCurrentSpeed().x, 0.0f));
                mc.AddExternalForce(force * 1.7f);  // idk why the math isnt lined up nor do I feel like trying
            }
            else
            {
                rb.velocity = new Vector2((rb.velocity.x * .5f) + force.x, force.y);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //OnTriggerEnter2D(collision);
    }
}
