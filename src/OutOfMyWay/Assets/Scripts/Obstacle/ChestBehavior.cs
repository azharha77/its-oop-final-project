using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChestBehavior : MonoBehaviour
{
    public string nextSceneName;
    public Dialogue dialogue;
    public float delayBeforeTransition = 1f;
    private FadeController fadeController;
    public AudioClip openChestSound;

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

        fadeController = FindObjectOfType<FadeController>();
        if (fadeController == null) { Debug.LogError("No FadeController found in the scene."); }
    }

    public void TriggerChest()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
        Debug.Log("Chest triggered. Preparing to load the next scene.");
    }

    public void DialogueEnd()
    {
        StartCoroutine(OpenChest());
    }

    private IEnumerator OpenChest()
    {
        if (openChestSound != null && audioSource != null)
        {
            audioSource.clip = openChestSound;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("No openChestSound assigned or AudioSource is missing.");
        }

        float waitTime = openChestSound != null ? Mathf.Max(openChestSound.length, delayBeforeTransition) : delayBeforeTransition;
        yield return new WaitForSeconds(waitTime);

        yield return new WaitForSeconds(delayBeforeTransition);
        if (fadeController != null) { yield return fadeController.FadeOut(); }

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            Debug.Log($"Loading next scene: {nextSceneName}");
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError("Next scene name is not assigned in ChestBehavior.");
        }
    }
}
