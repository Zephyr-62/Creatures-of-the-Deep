using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    public void Setup(float oxygenValue)
    {
        Time.timeScale = 0f;

        gameObject.SetActive(true);
    }

    public void RestartButton()
    {        
        SceneManager.LoadScene("GaugeTestScene"); //to be changed for the actual main scene

        Time.timeScale = 1f;
    }

    public void ExitButton()
    {
        Application.Quit();
    }
}
