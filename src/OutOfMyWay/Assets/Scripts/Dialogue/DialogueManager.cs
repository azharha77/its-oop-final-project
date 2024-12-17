using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Text nameText;
    public Text dialogueText;

    public Animator animator;

    private Queue<string> sentences;

    // Start is called before the first frame update
    void Start()
    {
        sentences = new Queue<string>();
    }

    public void StartDialogue(Dialogue dialogue)
    {
        animator.SetBool("isOpen", true);
        
        nameText.text = dialogue.name;
        sentences.Clear();

        foreach(string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if(sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        dialogueText.text = sentence;
    }

    void EndDialogue()
    {
        animator.SetBool("isOpen", false);
    
    // Try to find StairsBehavior
    StairsBehavior stairs = FindObjectOfType<StairsBehavior>();
    if (stairs != null)
    {
        stairs.DialogueEnd();
    }
    else
    {
        // If StairsBehavior is not found, try ChestBehavior
        ChestBehavior chest = FindObjectOfType<ChestBehavior>();
        if (chest != null)
        {
            chest.DialogueEnd();
        }
        else
        {
            Debug.LogWarning("Neither StairsBehavior nor ChestBehavior was found!");
        }
    }
    
    Debug.Log("Ending dialogue");
    }

}
