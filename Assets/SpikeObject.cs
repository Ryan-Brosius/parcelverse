using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SpikeObject : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (GameManager.instance != null)
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
