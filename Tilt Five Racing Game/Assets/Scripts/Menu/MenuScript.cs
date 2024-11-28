using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class NameData
{
    public int lastPlayerCount;
}

public class MenuScript : MonoBehaviour
{
    public string currentPlayerName;
    public static int playerCount = 0;
    private string previousPlayerName;
    public GameObject nextFirstSelected;
    public EventSystem eventSystem;

    public TextMeshProUGUI nameShowcase;

    public void Start()
    {
        LoadPlayerCount();
        if(nameShowcase != null)
        {
            ++playerCount;

            string autoName = "Player" + playerCount;
            RacerScript.playerName = autoName; //auto generated name because theres no keyboard...
            nameShowcase.text = "You are: "+autoName;
        }
        SavePlayerCount();
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

    public void ChangeFirstSelected()
    {
        if(eventSystem != null)
        {
            eventSystem.SetSelectedGameObject(nextFirstSelected, new BaseEventData(eventSystem));
            Debug.Log("Now selected " + nextFirstSelected.name);
        }
    }

    //Persist playercount
    public static void SavePlayerCount()
    {
        NameData data = new NameData { lastPlayerCount = playerCount };
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("Playercount", json);
        PlayerPrefs.Save();
        Debug.Log("Playercount saved: " + json);
    }

    public static void LoadPlayerCount()
    {
        if (PlayerPrefs.HasKey("Playercount"))
        {
            string json = PlayerPrefs.GetString("Playercount");
            NameData data = JsonUtility.FromJson<NameData>(json);
            playerCount = data.lastPlayerCount;
            Debug.Log("Playercount loaded: " + json);
        }
    }
}
