using UnityEngine;

public class Recipient : MonoBehaviour
{
    [SerializeField] private LayerMask mailLayer;
    [SerializeField] private GameObject dialogueBubble;
    
    private bool hasReceivedMail = false;
    
    void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if (mailLayer.Contains(otherCollider))
        {
            hasReceivedMail = true;
            dialogueBubble.SetActive(false);

            if (GameManager.instance != null)
            {
                GameManager.instance.TriggerLevelEnd();
            }
        }
    }
}
