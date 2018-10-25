using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour
{

    public Text[] playerScoreText;
    MatchManager matchManager;
    

   
    void Start()
    {
        matchManager = GameObject.Find("MatchManager").GetComponent<MatchManager>();
    }

  
    void Update()
    {
        for (int i = 1; i <= 4; i++)
        {
            if (matchManager.players.ContainsKey(i))
            {
                playerScoreText[i - 1].text = matchManager.players[i].score.ToString();
                
            }
        }   
    }
}
