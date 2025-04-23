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
            if (collision.gameObject.TryGetComponent<MoveComponent>(out MoveComponent mc))
            {
                var force = new Vector2(0.0f, velocity);
                mc.SetCurrentSpeed(new Vector2(mc.GetCurrentSpeed().x, 0.0f));
                mc.AddExternalForce(force * 1.5f);  // idk why the math isnt lined up nor do I feel like trying
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, velocity);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        OnTriggerEnter2D(collision);
    }
}
