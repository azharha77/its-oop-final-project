using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StairsBehavior : MonoBehaviour
{
    public string nextSceneName;
    public float delayBeforeTransition = 1f;
    public Dialogue dialogue;
    public Achievements achievements;
    private bool isTransitioning = false;
    private FadeController fadeController;
    private DialogueTrigger dialogueTrigger;

    private void Start()
    {
        fadeController = FindObjectOfType<FadeController>();
        if (fadeController == null) { Debug.LogError("No FadeController found in the scene."); }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isTransitioning)
        {
            TriggerDialogue();
            TriggerAchievement();
        }
    }

    public void DialogueEnd()
    {
        StartCoroutine(TransitionToNextScene());
    }

    public void TriggerDialogue()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }

    public void TriggerAchievement()
    {
        FindObjectOfType<AchievementManager>().OpenAchievement(achievements);
    }

    private IEnumerator TransitionToNextScene()
    {
        isTransitioning = true;

        if (fadeController != null) { yield return fadeController.FadeOut(); }

        yield return new WaitForSeconds(delayBeforeTransition);

        if (!string.IsNullOrEmpty(nextSceneName)) { SceneManager.LoadScene(nextSceneName); }
        else { Debug.LogError("Next scene name is not set in the StairsBehavior script."); }
    }
}
