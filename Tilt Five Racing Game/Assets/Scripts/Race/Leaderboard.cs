using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class Leaderboard : MonoBehaviour
{
    [System.Serializable]
    public class Racer
    {
        public string name;
        public float time;
    }

    public static GameObject[] slots = new GameObject[5];
    [SerializeField] private static TextMeshProUGUI[] nameTexts = new TextMeshProUGUI[5];
    [SerializeField] private static TextMeshProUGUI[] timeTexts = new TextMeshProUGUI[5];

    private static List<Racer> racers = new List<Racer>();

    void Start()
    {
        //nameTexts = slots.Select(s => s.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>()).ToArray();
        //timeTexts = slots.Select(s => s.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>()).ToArray();
    }

    public static void AddScore(string name, float time)
    {
        racers.Add(new Racer { name = name, time = time });
        racers = racers.OrderBy(r => r.time).ToList();
        UpdateLeaderboard();
    }

    public static void UpdateLeaderboard()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < racers.Count)
            {
                nameTexts[i].text = racers[i].name;
                timeTexts[i].text = racers[i].time.ToString("F2");
            }
            else
            {
                nameTexts[i].text = "-";
                timeTexts[i].text = "-";
            }
        }
    }

}