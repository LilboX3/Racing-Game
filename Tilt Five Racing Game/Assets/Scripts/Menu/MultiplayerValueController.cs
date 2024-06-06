using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MultiplayerValueController : MonoBehaviour
{
    public Toggle dayToggle;
    public Toggle fogToggle;
    public Toggle rainToggle;

    public TMP_InputField player1NameInput;
    public TMP_InputField player2NameInput;

    public static bool isDay = false;
    public static bool isFoggy = false;
    public static bool isRaining = false;

    public static string player1Name;
    public static string player2Name;

    // Call this function on the "Play" button click
    public void StartGame()
    {
        if(dayToggle.isOn) isDay = true;
        if(fogToggle.isOn) isFoggy = true;
        if (rainToggle.isOn) isRaining = true;
        
        player1Name = player1NameInput.text;
        player2Name = player2NameInput.text;
        SceneManager.LoadScene(1);

    }
}
