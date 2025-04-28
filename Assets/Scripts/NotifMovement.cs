using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class NotifMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveDistance = 0.2f;
    public float moveDuration = 0.5f;

    [Header("Scale Settings")]
    public float scaleAmount = 2f;
    public float scaleDuration = 0.7f;

    [Header("Rotation Settings")]
    public float rotationAmount = 10f;
    public float rotationDuration = 1f;

    private void Start()
    {
        gameObject.transform.rotation = Quaternion.Euler(0, 0, -rotationAmount / 2f);

        transform.DOMoveY(transform.position.y + moveDistance, moveDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);

        transform.DOScale(Vector3.one * scaleAmount, scaleDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);

        transform.DORotate(new Vector3(0, 0, rotationAmount), rotationDuration, RotateMode.LocalAxisAdd)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }
}
