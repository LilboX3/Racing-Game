using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;


[System.Serializable]
public class LeaderboardData
{
    public List<Leaderboard.Racer> racers = new List<Leaderboard.Racer>();
}

public class Leaderboard : MonoBehaviour
{
    [System.Serializable]
    public class Racer
    {
        public string name;
        public float time;
    }

    public static GameObject[] slots = new GameObject[5];
    private static string[] nameTexts = new string[5];
    private static string[] timeTexts = new string[5];

    public TextMeshProUGUI[] nameSlots = new TextMeshProUGUI[5];
    public TextMeshProUGUI[] timeSlots = new TextMeshProUGUI[5];

    private static List<Racer> racers = new List<Racer>();

    void Start()
    {
        //nameTexts = slots.Select(s => s.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>()).ToArray();
        //timeTexts = slots.Select(s => s.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>()).ToArray();
        LoadLeaderboard();
        FillUIBoard();
    }

    public static void AddScore(string newName, float newTime, int levelNumber)
    {
        Debug.Log("RECEIVED: " + newName + " & " + newTime);
        racers.Add(new Racer { name = newName, time = newTime });
        racers = racers.OrderBy(r => r.time).ToList();
        UpdateLeaderboard();
    }

    public static void UpdateLeaderboard()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < racers.Count)
            {
                nameTexts[i] = racers[i].name;
                timeTexts[i] = racers[i].time.ToString("F2");
            }
            else
            {
                nameTexts[i] = "-";
                timeTexts[i] = "-";
            }
        }
        SaveLeaderboard();
    }

    //Persist leaderboard
    public static void SaveLeaderboard()
    {
        LeaderboardData data = new LeaderboardData { racers = racers };
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("Leaderboard", json);
        PlayerPrefs.Save();
        Debug.Log("Leaderboard saved: " + json);
    }

    public static void LoadLeaderboard()
    {
        if (PlayerPrefs.HasKey("Leaderboard"))
        {
            string json = PlayerPrefs.GetString("Leaderboard");
            LeaderboardData data = JsonUtility.FromJson<LeaderboardData>(json);
            racers = data.racers;
            Debug.Log("Leaderboard loaded: " + json);
            UpdateLeaderboard();
        }
    }

    private void FillUIBoard()
    {
        for (int i = 0; i < nameSlots.Length; i++)
        {
            nameSlots[i].text = nameTexts[i];
        }
        for (int i = 0; i < timeSlots.Length; i++)
        {
            timeSlots[i].text = timeTexts[i];
        }
    }

}