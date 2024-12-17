using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementManager : MonoBehaviour
{
    public Image iconImage;
    public Text descriptionText;

    public Animator animator;

    private Queue<string> sentences;

    // Start is called before the first frame update
    void Start()
    {
        sentences = new Queue<string>();
    }

    public void OpenAchievement(Achievements achievements)
    {
        animator.SetBool("isOpen", true);
        
        iconImage.sprite = achievements.imageIcon;

        sentences.Clear();

        foreach(string sentence in achievements.sentences)
        {
            sentences.Enqueue(sentence);
        }

        StartCoroutine(CloseAchievementAfterDelay(2f));
    }

    private IEnumerator CloseAchievementAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for the specified time
        animator.SetBool("isOpen", false); // Close the achievement UI
    }

}
