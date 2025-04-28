using UnityEngine;

public class Recipient : MonoBehaviour
{
    [SerializeField] private LayerMask mailLayer;
    [SerializeField] private GameObject notif;
    [SerializeField] private ParticleSystem confetti;

    private bool hasReceivedMail = false;
    
    void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if (mailLayer.Contains(otherCollider) && !hasReceivedMail)
        {
            hasReceivedMail = true;
            //dialogueBubble.SetActive(false);

            if (GameManager.instance != null)
            {
                GameManager.instance.TriggerLevelEnd();
            }

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlaySoundEffect("mail_give", 1);
            }

            confetti.Play();
            Destroy(otherCollider.gameObject);
            Destroy(notif);
        }
    }
}
