using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateBehavior : MonoBehaviour
{
    public LayerMask obstacleLayer;
    public LayerMask holeLayer;
    public float slideDuration = 0.5f;
    public float shrinkDuration = 1f;

    public AudioClip slideSound;
    public AudioClip fallSound;

    private AudioSource audioSource;
    public bool IsSliding { get; private set; } = false;

    private bool isShrinking = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogWarning("AudioSource is missing on the crate object. Adding one dynamically.");
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
    }

    public void Slide(Vector3 direction)
    {
        if (IsSliding || isShrinking) { return; }

        Vector3 targetPosition = transform.position + direction;

        Collider2D hit = Physics2D.OverlapBox(targetPosition, GetComponent<BoxCollider2D>().size, 0, obstacleLayer);
        if (hit != null) { return; }

        StartCoroutine(SlideToPosition(targetPosition));
    }

    private IEnumerator SlideToPosition(Vector3 targetPosition)
    {
        IsSliding = true;

        Vector3 startPosition = transform.position;
        float elapsedTime = 0;

        if (slideSound != null && audioSource != null)
        {
            audioSource.clip = slideSound;
            audioSource.Play();
        }

        while (elapsedTime < slideDuration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / slideDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        IsSliding = false;

        CheckIfOnHole();
    }

    private void CheckIfOnHole()
    {
        Collider2D hole = Physics2D.OverlapBox(transform.position, GetComponent<BoxCollider2D>().size, 0, holeLayer);

        if (hole != null) { StartCoroutine(ShrinkIntoHole()); }
    }

    private IEnumerator ShrinkIntoHole()
    {
        isShrinking = true;

        Vector3 initialScale = transform.localScale;
        Vector3 finalScale = Vector3.zero;

        float elapsedTime = 0;

        while (elapsedTime < shrinkDuration)
        {
            transform.localScale = Vector3.Lerp(initialScale, finalScale, elapsedTime / shrinkDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = finalScale;

        if (fallSound != null && audioSource != null)
        {
            audioSource.clip = fallSound;
            audioSource.Play();
        }

        Destroy(gameObject);
    }
}
