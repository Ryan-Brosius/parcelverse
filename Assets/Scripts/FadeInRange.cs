using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class FadeInRange : MonoBehaviour
{
    [SerializeField] GameObject FadeInObject;

    private void Start()
    {
        if (FadeInObject.TryGetComponent<TextMeshPro>(out TextMeshPro tmp))
        {
            // Because im lazy LOL
            tmp.DOFade(0f, 0f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (FadeInObject.TryGetComponent<TextMeshPro>(out TextMeshPro tmp))
            {
                tmp.DOFade(1f, 1f);
            }
        }
    }
}
