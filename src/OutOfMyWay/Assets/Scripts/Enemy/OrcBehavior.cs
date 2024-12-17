using System.Collections;
using UnityEngine;

public class OrcBehavior : MonoBehaviour
{
    public float deathDelay = 1f;
    private Animator animator;
    private Collider2D orcCollider;
    public AudioClip orcDeathSound;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource is missing on the chest object. Adding one dynamically.");
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;

        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component missing on the orc object.");
        }

        orcCollider = GetComponent<Collider2D>();
        if (orcCollider == null)
        {
            Debug.LogError("Collider2D component missing on the orc object.");
        }
    }

    public void TriggerDeath()
    {
        if (animator != null) { animator.SetBool("isAttacked", true); }

        if (orcCollider != null) { orcCollider.enabled = false; }

        StartCoroutine(HandleDeath());
    }

    private IEnumerator HandleDeath()
    {
        if (orcDeathSound != null && audioSource != null)
        {
            audioSource.clip = orcDeathSound;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("No openChestSound assigned or AudioSource is missing.");
        }

        float soundDuration = orcDeathSound != null ? orcDeathSound.length : 0f;
        float waitTime = Mathf.Max(soundDuration, deathDelay);

        yield return new WaitForSeconds(waitTime);

        Destroy(gameObject);
    }
}
