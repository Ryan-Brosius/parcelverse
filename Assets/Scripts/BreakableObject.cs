using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    [SerializeField] private string targetTag = "MailProjectile";
    
    [SerializeField] private LayerMask defaultLayer;
    [SerializeField] private LayerMask groundLayer;

    private void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Default");;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // lol what a fucking mess this is
        if (collision.gameObject.TryGetComponent<MailProjectile>(out MailProjectile mp))
        {
            if (mp.CanTrigger)
            {
                DestroySequence();
                mp.CanTrigger = false;
            }
        }

        // All of this is to fix an exploit where the player can jump on freshly spawned boxes, leading to infinite
        // jumps.
        if (groundLayer.Contains(collision.gameObject.layer))
        {
            gameObject.layer = LayerMask.NameToLayer("Ground"); //gameObject.layer = groundLayer;
        }
    }

    private void DestroySequence()
    {
        Sequence shatterSeq = DOTween.Sequence();

        shatterSeq.Append(transform.DOShakeScale(0.1f, .2f, 10, 90, true))
                  .Append(transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack));

        Destroy(gameObject, 0.3f);
    }

    public void ExternalDestroySequence()
    {
        DestroySequence();
    }
}
