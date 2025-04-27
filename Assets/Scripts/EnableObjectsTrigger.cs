using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableObjectsTrigger : MonoBehaviour
{
    [SerializeField] List<GameObject> objects = new List<GameObject>();

    private void Start()
    {
        foreach (GameObject obj in objects)
        {
            obj.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            foreach (GameObject obj in objects)
            {
                if (obj != null) obj.SetActive(true);
            }
        }
    }
}
