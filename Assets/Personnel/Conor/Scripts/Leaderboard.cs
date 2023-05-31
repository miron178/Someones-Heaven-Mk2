using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


public class Leaderboard : MonoBehaviour
{
    //Back Panel
    [SerializeField] Image m_mainPanel;
    [SerializeField] Color32 m_mainColor;
    bool m_reverseR = false;
    bool m_reverseG = false;
    bool m_reverseB = false;

    //Panels
    [SerializeField] GameObject m_mainMenu;
    [SerializeField] GameObject m_leaderboard;

    //Leaderboard
    [SerializeField] GameObject m_leaderboardGrid;
    [SerializeField] GameObject m_playerCardPrefab;
    [SerializeField] List<PlayerCard> m_playerCards;
    [SerializeField] List<PlayerInfo> m_playerInfos;

    int m_comxSeed = 9999;

    void Awake()
    {
        m_mainColor = new Color(UnityEngine.Random.Range(0, 255), UnityEngine.Random.Range(0, 255), UnityEngine.Random.Range(0, 255), 1);
        StartCoroutine(SetBackgroundColor());

        for(int i = 0; i < 25; i++)
        {
            m_playerCards.Add(Instantiate(m_playerCardPrefab, new Vector3(0,0,0), Quaternion.identity, m_leaderboardGrid.transform).GetComponent<PlayerCard>());
            m_playerCards[i].SetPosition(i + 1);
        }
    }

    public void OpenLeaderboard()
    {
        m_mainMenu.SetActive(false);
        m_leaderboard.SetActive(true);

        // StartCoroutine(FetchResults());

        InvokeRepeating("FetchResultsBootstrap", 0f, 10f);
    }

    void FetchResultsBootstrap() { StartCoroutine(FetchResults()); }
    IEnumerator FetchResults()
    {
        UnityWebRequest www = UnityWebRequest.Get($"https://halfempty.conorlewis.com/getTimes?seed={m_comxSeed}");

        yield return www.SendWebRequest();

        Debug.Log("Recieved Request:" + www.downloadHandler.text);

        string rawData = www.downloadHandler.text.Substring(1, www.downloadHandler.text.Length - 2);
        string[] rawDataList = rawData.Split(',');

        for(int i = 0; i < rawDataList.Length; i+=2)
        {
            string data = rawDataList[i] + "," + rawDataList[i+1];

            JsonObject jsonData = JsonNode.Parse(data).AsObject();

            PlayerInfo newInfo = new PlayerInfo() { Name = (string)jsonData["name"], Time = (float)jsonData["time"] };

            if(!m_playerInfos.Contains(newInfo)) { m_playerInfos.Add(newInfo); }
        }

        List<PlayerInfo> sortedList = m_playerInfos.OrderBy(o => o.Time).ToList();

        for(int i = 0; i < sortedList.Count; i++)
        {
            m_playerCards[i].SetName(sortedList[i].Name);
            m_playerCards[i].SetTime(sortedList[i].Time);
        }
    }

    IEnumerator SetBackgroundColor()
    {
        while(true)
        {
            switch(UnityEngine.Random.Range(0, 3))
            {
                case 0:
                {
                    if(!m_reverseR) { m_mainColor.r += 1; if(m_mainColor.r == 255) { m_reverseR = true; } }
                    else if(m_reverseR) { m_mainColor.r -= 1; if(m_mainColor.r == 0) { m_reverseR = false; } }

                    break;
                }
                case 1:
                {
                    if(!m_reverseG) { m_mainColor.g += 1; if(m_mainColor.g == 255) { m_reverseG = true; } }
                    else if(m_reverseG) { m_mainColor.g -= 1; if(m_mainColor.g == 0) { m_reverseG = false; } }

                    break;
                }
                case 2:
                {
                    if(!m_reverseB) { m_mainColor.b += 1; if(m_mainColor.b == 255) { m_reverseB = true; } }
                    else if(m_reverseB) { m_mainColor.b -= 1; if(m_mainColor.b == 0) { m_reverseB = false; } }

                    break;
                }
            }

            m_mainPanel.color = m_mainColor;
            yield return new WaitForSeconds(0.01f);
        }
    }
}

[Serializable]
public struct PlayerInfo
{
    public string Name;
    public float Time;
}