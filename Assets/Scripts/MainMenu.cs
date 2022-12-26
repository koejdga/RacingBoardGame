using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene("Game");
    }

    public void Settings()
    {
        // тут я ще не придумала що буде
    }

    public void Exit()
    {
        Debug.Log("Вихід");
        Application.Quit();
    }
}
