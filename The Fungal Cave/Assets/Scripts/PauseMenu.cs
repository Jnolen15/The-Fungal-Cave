using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool isPaused = false;

    public GameObject pauseMenuUI;
    public GameObject optionsUI;
    public GameObject tutorialUI;

    public GameSave gameSave;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(isPaused)
            {
                Resume();
            } else
            {
                Paused();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        optionsUI.SetActive(false);
        tutorialUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    void Paused()
    {
        // Save player pos
        gameSave.SaveGame();

        // Enable ui and pause game
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void Quit()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }
}
