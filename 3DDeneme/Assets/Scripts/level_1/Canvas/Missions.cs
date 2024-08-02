using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Missions : MonoBehaviour
{
    public GameObject missionPanel;
    public GameObject gameOverPanel;

   public void PanelExitButton()
    {
        missionPanel.SetActive(false);
 
    }
    public void GameOverVoid()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(0);
    }
}
