using UnityEngine;

public class PlayerCollisions : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject dedPanel; 
    public GameObject wonPanel; 
    public AudioClip pickupSound;
    public AudioClip screamSound;

    private AudioSource audioSource;


    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("DedEnd-1"))
        {
            transform.position = new Vector3(-16.3f,0f,2.1f);
        }
        else if (collision.gameObject.CompareTag("DedEnd-2"))
        {
            transform.position = new Vector3(-7.6f,0f,2.7f);
        }
        else if (collision.gameObject.CompareTag("trash"))
        {
            if (audioSource && pickupSound)
            {
                audioSource.PlayOneShot(pickupSound);
            }
            gameManager.addScore();
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("evil-trash"))
        {
            if (audioSource && pickupSound)
            {
                audioSource.PlayOneShot(screamSound);
            }
            gameManager.Died();
        }
    }

}
