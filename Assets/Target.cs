using System.Collections;
using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] private AudioClip successSound;
    [SerializeField] private Color hitColor = Color.green;
    [SerializeField] private float flashDuration = 0.5f;

    private Renderer rend;
    private AudioSource audioSource;
    private Color startColor;

    void Start()
    {
        rend = GetComponent<Renderer>();
        audioSource = GetComponent<AudioSource>();
        startColor = rend.material.color;
    }

    private void OnCollisionEnter(Collision collision)
    {
       
        if (!collision.gameObject.CompareTag("Projectile")) return;

        
        StopAllCoroutines();
        StartCoroutine(Flash());

        if (audioSource != null && successSound != null)
            audioSource.PlayOneShot(successSound);

        // Bonus 
        ScoreManager.AddPoint();
    }

    private IEnumerator Flash()
    {
        rend.material.color = hitColor;
        yield return new WaitForSeconds(flashDuration);
        rend.material.color = startColor;
    }
}