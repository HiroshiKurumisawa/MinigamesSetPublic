using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ReversiManager : MonoBehaviour
{
    #region �ϐ��Q
    // �Δz�u
    [SerializeField, Header("�΂�Prefab")]
    GameObject stonePrefab;
    // �^�[���֌W
    [SerializeField, Header("�����s��UI")]
    GameObject BlackSpeachBalloonUI;
    [SerializeField, Header("�����s��UI")]
    GameObject whiteSpeachBalloon;
    [SerializeField, Header("���݂̃^�[����\������e�L�X�g")]
    TextMeshProUGUI turnText;
    const int blackTurnNum = 0;
    const int WhiteTrunNum = 1;
    int thisStatusNum;
    // ���Ԑ����֌W
    [SerializeField, Header("�������ԕ\���e�L�X�g")]
    TextMeshProUGUI limitTimeText;
    const float limitTime = 60f;
    float countTime;
    bool isTimeCountStart = false;
    bool isPutOrPass = false;
    #endregion

    void Start()
    {
        StartStonePut();
        turnText.text = "���̃^�[���ł�";
        whiteSpeachBalloon.SetActive(false);
    }

    void Update()
    {

    }

    void FixedUpdate()
    {
        LimitTime(limitTime);
    }

    // ��������
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

        if (countTime < 0 && (thisStatusNum == blackTurnNum || thisStatusNum == WhiteTrunNum))        // �������Ԃ�0�ɂȂ����Ƃ�
        {
            TruneChange(thisStatusNum);
            isTimeCountStart = false;
        }
        else if (countTime >= 0 && (thisStatusNum == blackTurnNum || thisStatusNum == WhiteTrunNum))  // �������ԃJ�E���g
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
            turnText.text = "���̃^�[���ł�";
        }
        else if (statusNum == WhiteTrunNum)
        {
            thisStatusNum = blackTurnNum;
            whiteSpeachBalloon.SetActive(false);
            BlackSpeachBalloonUI.SetActive(true);
            turnText.text = "���̃^�[���ł�";
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

    // �΂̐ݒu
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
