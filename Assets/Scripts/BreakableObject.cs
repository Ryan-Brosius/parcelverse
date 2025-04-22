using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    [SerializeField] private string targetTag = "MailProjectile";

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
    }

    private void DestroySequence()
    {
        Sequence shatterSeq = DOTween.Sequence();

        shatterSeq.Append(transform.DOShakeScale(0.1f, .2f, 10, 90, true))
                  .Append(transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack));

        Destroy(gameObject, 0.3f);
    }
}
