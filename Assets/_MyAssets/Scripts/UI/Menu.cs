using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup.alpha = 0f;
    }

    public void ToggeMenu(bool toggle)
    {
        if (toggle)
        {
            canvasGroup.DOFade(1f, 1f);
            canvasGroup.blocksRaycasts = true;
        } else
        {
            canvasGroup.DOFade(0f, 1f);
            canvasGroup.blocksRaycasts = false;
        }
    }


    public void ReturnToTitleScreen()
    {
        SceneManager.LoadSceneAsync(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
