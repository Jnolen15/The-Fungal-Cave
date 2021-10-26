using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    private Queue<string> sentences;

    public Text dialogueText;
    public bool dialogueOver = false;
    public Animator animatior;
    public AudioManager audioManager;

    void Start()
    {
        sentences = new Queue<string>();
    }

    public void StartDialogue(Dialogue dialogue)
    {
        Debug.Log("Starting Dialogue");

        animatior.SetBool("IsOpen", true);

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
        StopAllCoroutines();
        StartCoroutine(TypeSenctence(sentence));

    }

    IEnumerator TypeSenctence (string sentence)
    {
        dialogueText.text = "";
        int count = 0;
        foreach (char letter in sentence.ToCharArray())
        {
            count++;
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.05f);
            if (count % 5 == 0)
            {
                if (Random.value < 0.5f)
                    audioManager.source.PlayOneShot(audioManager.Talk1);
                else
                    audioManager.source.PlayOneShot(audioManager.Talk2);
            }
        }
    }

    private void EndDialogue()
    {
        Debug.Log("Ended");
        dialogueOver = true;
        animatior.SetBool("IsOpen", false);
    }
}
