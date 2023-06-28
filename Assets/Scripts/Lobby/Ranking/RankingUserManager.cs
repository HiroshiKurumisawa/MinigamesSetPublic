using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RankingUserManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI userNameText;
    [SerializeField] TextMeshProUGUI userPointText;
    [SerializeField] TextMeshProUGUI userRankText;

    string userName;
    string userPoint;
    public string UserPoint { get { return userPoint; } }
    string userRank;

    public void SetAccountData(string name, string point)
    {
        userName = name;
        userPoint = point;

        userNameText.text = userName;
        userPointText.text = userPoint;
    }

    public void SetRankingNum(string rank)
    {
        userRank = rank;

        userRankText.text = userRank;

        if(int.Parse(userRank) <= 1)
        {
            userRankText.color = Color.yellow;
        }
        else if (int.Parse(userRank) <= 2)
        {
            userRankText.color = new Color(0.6f, 0.6f, 0.6f);
        }
        else if (int.Parse(userRank) <= 3)
        {
            userRankText.color = new Color(1f, 0.3f, 0f);
        }
        else
        {
            userRankText.color = Color.green;
        }
    }

}
