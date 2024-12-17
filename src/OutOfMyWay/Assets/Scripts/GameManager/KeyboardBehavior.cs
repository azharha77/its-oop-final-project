using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KeyboardBehavior : MonoBehaviour
{
    public float resetDelay = 6f;
    private PlayerMovement playerMovement;
    private FadeController fadeController;

    private void Start()
    {
        playerMovement = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        if (playerMovement == null) { Debug.LogError("PlayerMovement component not found on the Player object."); }

        fadeController = FindObjectOfType<FadeController>();
        if (fadeController == null) { Debug.LogError("No FadeController found in the scene."); }
    }

    private void Update()
    {
        HandleRestart();
    }

    private void HandleRestart()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Restart triggered by player.");
            StartCoroutine(RestartWithDeath());
        }
    }

    private IEnumerator RestartWithDeath()
    {
        if (playerMovement != null)
        {
            playerMovement.TriggerDeath();
            Debug.Log("Player death animation triggered via PlayerMovement.");
        }
        else
        {
            Debug.LogWarning("PlayerMovement not found, skipping death trigger.");
        }

        yield return new WaitForSeconds(resetDelay);
        if (fadeController != null) { yield return fadeController.FadeOut(); }

        Debug.Log("Restarting scene...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
