using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MailProjectile : MonoBehaviour
{
    public bool CanTrigger { get; set; } = true;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") && CanTrigger)
        {
            DestroySequence();
        }
    }

    private void DestroySequence()
    {
        Sequence shatterSeq = DOTween.Sequence();

        shatterSeq.Append(transform.DOShakeScale(0.2f, 0.5f, 10, 90, true))
                  .Append(transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack));

        if (TryGetComponent<SpriteRenderer>(out SpriteRenderer sr))
        {
            sr.transform.DOScale(new Vector3(0.4f, 0.4f), 0.3f).SetDelay(0.1f);
            sr.DOFade(0f, 0.3f).SetEase(Ease.InSine).SetDelay(0.1f);
        }

        Destroy(gameObject, 0.6f);
    }
}
