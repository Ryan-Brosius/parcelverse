using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;
using UnityEngine.UIElements;
using Color = UnityEngine.Color;

public class SpikeObject : MonoBehaviour
{
    [SerializeField] private bool trapSpike = false;
    [SerializeField] private float raycastWidth = 1f;
    [SerializeField] private float raycastRange = 10f;

    private void Update()
    {
        if (trapSpike && HitPlayer())
        {
            if (TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
            {
                rb.constraints = RigidbodyConstraints2D.None;
                rb.AddForce(Vector2.zero);
            }
        }
    }

    private bool HitPlayer()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, raycastRange);

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Player"))
            {
                return true;
            }

        }
        return false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (GameManager.instance != null)
            {
                if (!GameManager.instance.restarting)
                {
                    Time.timeScale = 0f;
                    DOVirtual.DelayedCall(1.0f, () =>
                    {
                        Time.timeScale = 1f;
                    }).SetUpdate(true);

                    DOVirtual.DelayedCall(0.5f, () =>
                    {
                        GameManager.instance.restartLevel();
                    }).SetUpdate(true);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (trapSpike)
        {
            Gizmos.color = HitPlayer() ? Color.green : Color.red;
            Gizmos.DrawLine(transform.position, transform.position + transform.up * raycastRange);
        }
    }

}
