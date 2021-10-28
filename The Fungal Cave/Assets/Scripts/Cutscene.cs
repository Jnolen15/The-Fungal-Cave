using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cutscene : MonoBehaviour
{
    public Vector3 speed;
    public DialogueTrigger dialogueTrigger;
    public DialogueManager dialogueManager;

    private bool startedDialogue = false;

    // Update is called once per frame
    void Update()
    {
        if(transform.position.x > -11 && !startedDialogue)
        {
            dialogueTrigger.TriggerDialogue();
            startedDialogue = true;
        }

        if (Input.GetKeyDown(KeyCode.Escape)) SceneManager.LoadScene("Level");

        if (Input.GetKeyDown(KeyCode.Z)) dialogueManager.DisplayNextSentence();

        if (dialogueManager.dialogueOver && transform.position.x > 14)
        {
            SceneManager.LoadScene("Level");
        }
    }

    private void FixedUpdate()
    {
        transform.position += speed;
    }
}
