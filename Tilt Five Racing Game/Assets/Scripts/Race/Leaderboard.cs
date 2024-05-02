using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class Leaderboard : MonoBehaviour
{
    public Text currentName;
    public Text currentTime;
    public Text currentPos;

    public GameObject[] slots;
    private TextMeshProUGUI[] nameTexts;
    private TextMeshProUGUI[] timeTexts; 

    // Start is called before the first frame update
    void Start()
    {
        nameTexts = slots.Select(s => s.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>()).ToArray();
        timeTexts = slots.Select(s => s.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>()).ToArray();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddScore(string name, float time)
    {
        currentName.text = name;
        currentTime.text = time.ToString("F2");

        int place = CalculatePlace(time);
        if (place == -1)
        {
            currentPos.text = "Unranked";
        }
        else
        {
            currentPos.text = (place + 1).ToString();
            UpdateSlots(place);
        }
    }
    private int CalculatePlace(float time)
    {
        for(int i=0; i<slots.Length; i++)
        {
            string slotTime = timeTexts[i].text;
            Debug.Log("On position"+i+"time is: "+slotTime);
            if (slotTime == "-" || float.Parse(slotTime) > time)
            {
                return i;
            }
        }
        return -1;
    }

    private void UpdateSlots(int pos)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i == pos)
            {
                string tempName = nameTexts[i].text;
                string tempTime = timeTexts[i].text;

                nameTexts[i].text = currentName.text;
                timeTexts[i].text = currentTime.text;
                if (i != slots.Length - 1)
                {
                    nameTexts[i+1].text = tempName;
                    timeTexts[i+1].text = tempTime;
                }
                break;
            }
        }
    }
}
