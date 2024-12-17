using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerActionTracker : MonoBehaviour
{
    public int maxActions = 10;
    private int actionCount = 0;
    public Text movesLeftText;
    public float resetDelay = 6f;

    private PlayerMovement playerMovement;
    private FadeController fadeController;

    private void Start()
    {
        playerMovement = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        if (playerMovement == null) { Debug.LogError("PlayerMovement component not found on the Player object."); }

        fadeController = FindObjectOfType<FadeController>();
        if (fadeController == null) { Debug.LogError("No FadeController found in the scene."); }

        UpdateMovesLeftUI();
    }

    public void RegisterAction()
    {
        actionCount++;
        UpdateMovesLeftUI();

        if (actionCount > maxActions) StartCoroutine(DelayedReset());
    }

    private IEnumerator DelayedReset()
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
        ResetScene();
    }

    private void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ResetActionCount()
    {
        actionCount = 0;
    }

    private void UpdateMovesLeftUI()
    {
        if (movesLeftText != null)
        {
            int movesLeft = maxActions - actionCount;
            movesLeftText.text = $"{movesLeft}";
        }
    }
}
