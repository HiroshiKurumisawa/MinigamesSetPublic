using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ReversiManager : MonoBehaviour
{
    #region 変数群
    // 石配置
    [SerializeField, Header("石のPrefab")]
    GameObject stonePrefab;
    // ターン関係
    [SerializeField, Header("黒試行中UI")]
    GameObject BlackSpeachBalloonUI;
    [SerializeField, Header("白試行中UI")]
    GameObject whiteSpeachBalloon;
    [SerializeField, Header("現在のターンを表示するテキスト")]
    TextMeshProUGUI turnText;
    const int blackTurnNum = 0;
    const int WhiteTrunNum = 1;
    int thisStatusNum;
    // 時間制限関係
    [SerializeField, Header("制限時間表示テキスト")]
    TextMeshProUGUI limitTimeText;
    const float limitTime = 60f;
    float countTime;
    bool isTimeCountStart = false;
    bool isPutOrPass = false;
    #endregion

    void Start()
    {
        StartStonePut();
        turnText.text = "黒のターンです";
        whiteSpeachBalloon.SetActive(false);
    }

    void Update()
    {

    }

    void FixedUpdate()
    {
        LimitTime(limitTime);
    }

    // 制限時間
    void LimitTime(float limit)
    {
        if (!isTimeCountStart)
        {
            isTimeCountStart = true;
            countTime = limit;
            limitTimeText.text = countTime.ToString("00");
        }

        if (isPutOrPass)
        {
            isPutOrPass = false;
            countTime = 0;
        }

        if (countTime < 0 && (thisStatusNum == blackTurnNum || thisStatusNum == WhiteTrunNum))        // 制限時間が0になったとき
        {
            TruneChange(thisStatusNum);
            isTimeCountStart = false;
        }
        else if (countTime >= 0 && (thisStatusNum == blackTurnNum || thisStatusNum == WhiteTrunNum))  // 制限時間カウント
        {
            countTime -= Time.deltaTime;
            limitTimeText.text = countTime.ToString("00");
        }
        else { return; }
    }
    void TruneChange(int statusNum)
    {
        if (statusNum == blackTurnNum)
        {
            thisStatusNum = WhiteTrunNum;
            BlackSpeachBalloonUI.SetActive(false);
            whiteSpeachBalloon.SetActive(true);
            turnText.text = "白のターンです";
        }
        else if (statusNum == WhiteTrunNum)
        {
            thisStatusNum = blackTurnNum;
            whiteSpeachBalloon.SetActive(false);
            BlackSpeachBalloonUI.SetActive(true);
            turnText.text = "黒のターンです";
        }
        else { return; }
    }
    public void PutStoneOrPass(BaseEventData data)
    {
        var pointerObject = (data as PointerEventData).pointerClick;
        if (!isPutOrPass && (thisStatusNum == blackTurnNum || thisStatusNum == WhiteTrunNum) && (pointerObject.transform.childCount == 0 || pointerObject.name == "PassButton"))
        {
            isPutOrPass = true;
            StonePut(pointerObject.name, thisStatusNum);
        }
    }

    // 石の設置
    void StonePut(string point, int turnStatusNum)
    {
        if (point != "PassButton")
        {
            var putPos = GameObject.Find(point).transform;
            GameObject stoneClone = Instantiate(stonePrefab, putPos.position, Quaternion.identity, putPos);
            if (turnStatusNum == blackTurnNum)
            {
                stoneClone.GetComponent<Image>().color = Color.black;
            }
            else if (turnStatusNum == blackTurnNum)
            {
                stoneClone.GetComponent<Image>().color = Color.white;
            }
        }
        else { return; }
    }

    void StartStonePut()
    {
        StonePut("D-4", blackTurnNum);
        StonePut("D-5", WhiteTrunNum);
        StonePut("E-4", WhiteTrunNum);
        StonePut("E-5", blackTurnNum);
    }
}
