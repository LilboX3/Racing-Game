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

    public GameObject[] slots;
    [SerializeField] private TextMeshProUGUI[] nameTexts;
    [SerializeField] private TextMeshProUGUI[] timeTexts;

    private List<Racer> racers = new List<Racer>();
    [SerializeField] private int amountFinished = 0;
    [SerializeField] private int shouldFinish;

    void Start()
    {
        //nameTexts = slots.Select(s => s.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>()).ToArray();
        //timeTexts = slots.Select(s => s.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>()).ToArray();
    }

    public void AddScore(string name, float time)
    {
        amountFinished++;
        Debug.Log("Amount finished is " + amountFinished + "  should be "+shouldFinish);
        racers.Add(new Racer { name = name, time = time });
        racers = racers.OrderBy(r => r.time).ToList();
        UpdateLeaderboard();
        if (amountFinished >= shouldFinish)
        {
            Time.timeScale = 0.2f;
            Debug.Log("all finished!!");
            gameObject.SetActive(true);
        }
    }

    void UpdateLeaderboard()
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