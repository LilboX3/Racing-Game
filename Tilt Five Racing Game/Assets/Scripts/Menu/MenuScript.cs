using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public string currentPlayerName;
    public static int playerCount = 0;
    private string previousPlayerName;

    public TextMeshProUGUI nameShowcase;

    public void Start()
    {
        if(nameShowcase != null)
        {
            ++playerCount;

            string autoName = "Player" + playerCount;
            RacerScript.playerName = autoName; //auto generated name because theres no keyboard...
            nameShowcase.text = "You are: "+autoName;
        }
    }

    public void Update()
    {
        //Restart game with R key (or A with controller) 
        if (Input.GetButton("Restart"))
        {
            RestartGame();
        }
    }
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void OpenMainMenu()
    {
        SceneManager.LoadSceneAsync(0);
    }

    public void OpenLeaderboard()
    {
        SceneManager.LoadSceneAsync(4);
    }

    public void RestartGame()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
}
