using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSave : MonoBehaviour
{
    public Transform player;
    public SpeedRunTimer speedRunTimer;

    public void Awake()
    {
        speedRunTimer.elapsedTime = PlayerPrefs.GetFloat("Time");

        if (PlayerPrefs.GetFloat("playerX") == 0 && PlayerPrefs.GetFloat("playerY") == 0)
            player.position = new Vector2(-2, 2);
        else
            player.position = new Vector2 (PlayerPrefs.GetFloat("playerX"), PlayerPrefs.GetFloat("playerY"));
    }

    public void SaveGame()
    {
        PlayerPrefs.SetFloat("playerX", player.position.x);
        PlayerPrefs.SetFloat("playerY", player.position.y);
        PlayerPrefs.SetFloat("Time", speedRunTimer.elapsedTime);
    }

    public void ResetGame()
    {
        PlayerPrefs.SetFloat("playerX", -2);
        PlayerPrefs.SetFloat("playerY", 2);
        PlayerPrefs.SetFloat("Time", 0f);
        SceneManager.LoadScene("Level");
    }
}
