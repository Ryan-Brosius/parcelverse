using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PitTrigger : MonoBehaviour
{
    [SerializeField] private GameObject pit;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            DestroyPit();
        }
    }

    void DestroyPit()
    {
        foreach (Transform t in pit.transform)
        {
            GameObject child = t.gameObject;
            Sequence shatterSeq = DOTween.Sequence();

            if (child.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
            {
                rb.constraints = RigidbodyConstraints2D.None;
            }

            shatterSeq.Append(child.transform.DOScale(Vector3.zero, 0.6f));

            Destroy(child, 0.6f);
        }
    }
}
