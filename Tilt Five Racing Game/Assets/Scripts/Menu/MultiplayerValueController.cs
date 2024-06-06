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

    public static bool isDay;
    public static bool isFoggy;
    public static bool isRaining;

    public static string player1Name;
    public static string player2Name;

    // Call this function on the "Play" button click
    public void StartGame()
    {
        isDay = dayToggle;
        isFoggy = fogToggle;
        isRaining = rainToggle;

        player1Name = player1NameInput.text;
        player2Name = player2NameInput.text;
        SceneManager.LoadScene(1);

    }
}
