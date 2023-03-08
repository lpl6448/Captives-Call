using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void ToInstructions()
    {
        SceneManager.LoadScene("Instructions");
    }
    public void StartGame()
    {
        SceneManager.LoadScene("1");
    }
    public void ToMainMenu()
    {
        SceneManager.LoadScene("Title");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
