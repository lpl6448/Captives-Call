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
        Destroy(GameObject.FindGameObjectsWithTag("TitleMusic")[0]);
        SceneManager.LoadScene("1");
    }
    public void ToMainMenu()
    {
        Destroy(GameObject.FindGameObjectsWithTag("LevelMusic")[0]);
        SceneManager.LoadScene("Title");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
