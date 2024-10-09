using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameOverScreen gameOverScreen;
    public GaugeController gaugeController;

    private bool gameHasEnded = false;

    public void GameOver()
    {
        if(gameHasEnded == false)
        {
            gameHasEnded = true;
            gameOverScreen.Setup(gaugeController.currentOxygenValue);
        }     
    }
}
