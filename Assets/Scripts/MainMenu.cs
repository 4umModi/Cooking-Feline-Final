using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour{
    // event for the main menu play button
    // brings app to next scene, where the game is played
    public void PlayGame(){
        SceneManager.LoadScene(1);
    }

    public void SettingsGame()
    {
        SceneManager.LoadScene(3);
    }

    public void GoMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    // event for the main menu quit button; quits game
    public void QuitGame(){
        Debug.Log("Quit!");
        Application.Quit();
    }
}
