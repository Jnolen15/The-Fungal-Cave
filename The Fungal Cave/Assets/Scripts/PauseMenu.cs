using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool isPaused = false;

    public GameObject pauseMenuUI;
    public GameObject optionsUI;
    public GameObject tutorialUI;

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

    void Resume()
    {
        pauseMenuUI.SetActive(false);
        optionsUI.SetActive(false);
        tutorialUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    void Paused()
    {
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
