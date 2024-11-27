using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using static Leaderboard;


[System.Serializable]
public class LeaderboardData
{
    public List<Leaderboard.Racer> racersLevel1 = new List<Leaderboard.Racer>();
    public List<Leaderboard.Racer> racersLevel2 = new List<Leaderboard.Racer>();
    public List<Leaderboard.Racer> racersLevel3 = new List<Leaderboard.Racer>();
}

public class Leaderboard : MonoBehaviour
{
    [System.Serializable]
    public class Racer
    {
        public string name;
        public float time;
    }

    public TextMeshProUGUI[] nameSlots = new TextMeshProUGUI[5];
    public TextMeshProUGUI[] timeSlots = new TextMeshProUGUI[5];

    private static List<Racer> racersLevel1 = new List<Racer>();
    private static List<Racer> racersLevel2 = new List<Racer>();
    private static List<Racer> racersLevel3 = new List<Racer>();

    void Start()
    {
        LoadLeaderboard();
        FillUIBoard(1);
    }

    public static void AddScore(string newName, float newTime, int levelNumber)
    {
        LoadLeaderboard();
        Debug.Log("RECEIVED: " + newName + " & " + newTime);
        switch (levelNumber)
        {
            case 1:
                racersLevel1.Add(new Racer { name = newName, time = newTime });
                racersLevel1 = racersLevel1.OrderBy(r => r.time).ToList();
                break;
            case 2:
                racersLevel2.Add(new Racer { name = newName, time = newTime });
                racersLevel2 = racersLevel2.OrderBy(r => r.time).ToList();
                break;
            case 3:
                racersLevel3.Add(new Racer { name = newName, time = newTime });
                racersLevel3 = racersLevel3.OrderBy(r => r.time).ToList();
                break;
        }
        SaveLeaderboard();
    }

    //Persist leaderboard
    public static void SaveLeaderboard()
    {
        LeaderboardData data = new LeaderboardData { racersLevel1 = racersLevel1, racersLevel2 = racersLevel2, racersLevel3 = racersLevel3 };
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
            racersLevel1 = data.racersLevel1;
            racersLevel2 = data.racersLevel2;
            racersLevel3 = data.racersLevel3;
            Debug.Log("Leaderboard loaded: " + json);
        }
    }

    private void FillUIBoard(int levelNumber)
    {
        List<Racer> racers = new List<Racer>();
        switch (levelNumber)
        {
            case 1:
                racers = racersLevel1;
                break;
            case 2:
                racers = racersLevel2;
                break;
            case 3:
                racers = racersLevel3;
                break;
        }
        Debug.Log("racers filled with: " + racers[0].name);

        for (int i = 0; i < nameSlots.Length; i++)
        {
            if(i < racers.Count)
            {
                nameSlots[i].text = racers[i].name;
            } else
            {
                nameSlots[i].text = "";
            }
        }
        for (int i = 0; i < timeSlots.Length; i++)
        {
            if (i < racers.Count)
            {
                timeSlots[i].text = racers[i].time.ToString("F2");
            }
            else
            {
                timeSlots[i].text = "";
            }
        }
    }

    public void ShowLevel1()
    {
        FillUIBoard(1);
    }

    public void ShowLevel2()
    {
        FillUIBoard(2);
    }

    public void ShowLevel3()
    {
        FillUIBoard(3);
    }

}