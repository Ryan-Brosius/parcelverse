using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPlayerDirection : MonoBehaviour
{
    [SerializeField] private MailGun mailGun;
    [SerializeField] private GameObject playerSprite;

    private void LateUpdate()
    {
        playerSprite.transform.localScale = new Vector3(mailGun.CorrectSide, 1, 1);
    }
}
