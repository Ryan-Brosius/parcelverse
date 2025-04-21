using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Offset Settings")]
    public Vector2 offset = new Vector2(0f, 2f);

    [Header("Follow Settings")]
    public float followSpeed = 5f;

    [Header("Smoothing")]
    public bool useSmoothFollow = true;

    private Vector3 currentVelocity;

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("CameraFollow2D: No target set! Please assign the player Transform.");
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = new Vector3(
            target.position.x + offset.x,
            target.position.y + offset.y,
            transform.position.z
        );

        if (useSmoothFollow)
        {
            transform.DOMove(targetPosition, 1f / followSpeed).SetEase(Ease.OutQuad);
        }
        else
        {
            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPosition,
                ref currentVelocity,
                1f / followSpeed
            );
        }
    }
}
