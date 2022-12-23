using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ReversiManager : MonoBehaviour
{
    #region 変数群
    // ゲーム状況関連
    [SerializeField] GameObject endForm;
    [SerializeField] TextMeshProUGUI endFormText;
    [SerializeField, Header("黒石総数")]
    TextMeshProUGUI blackStonesText;
    [SerializeField, Header("白石総数")]
    TextMeshProUGUI whiteStonesText;
    const int blackTurnNum = 0;
    const int WhiteTrunNum = 1;
    const int blackWinNum = 2;
    const int whiteWinNum = 3;
    const int drowNum = 4;
    int thisStatusNum;
    int allstonesNum = 0;
    // 石配置、盤面用
    [SerializeField, Header("ベースステージ")]
    GameObject beaseStage;
    [SerializeField, Header("タイル生成ポイント")]
    GameObject tileCreatePoint;
    [SerializeField, Header("タイルのPrefab")]
    GameObject tilePrefab;
    List<Vector2> tilePoint;
    [SerializeField, Header("方向リスト")]
    List<Vector2> DirectionList;        // 左上は(-1,1)
    [SerializeField, Header("石のPrefab")]
    GameObject stonePrefab;
    const int stoneLayer = 6;
    // ターン関係
    [SerializeField, Header("黒試行中UI")]
    GameObject BlackSpeachBalloonUI;
    [SerializeField, Header("白試行中UI")]
    GameObject whiteSpeachBalloon;
    [SerializeField, Header("現在のターンを表示するテキスト")]
    TextMeshProUGUI turnText;
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
        CreateStage();
        StartStonePut();
    }

    void Update()
    {

    }

    void FixedUpdate()
    {
        LimitTime(limitTime);
        StoneCount();
        EndGame();
    }

    void EndGame()
    {
        if (thisStatusNum == whiteWinNum)
        {
            endForm.SetActive(true);
            endFormText.text = "ユーザー白の勝利";
        }
        else if (thisStatusNum == blackWinNum)
        {
            endForm.SetActive(true);
            endFormText.text = "ユーザー黒の勝利";
        }
        else if (thisStatusNum == drowNum)
        {
            endForm.SetActive(true);
            endFormText.text = "引き分け";
        }
        else { return; }
    }



    void StoneCount()
    {
        var whiteStones = GameObject.FindGameObjectsWithTag("White");
        var blackStones = GameObject.FindGameObjectsWithTag("Black");

        whiteStonesText.text = whiteStones.Length.ToString();
        blackStonesText.text = blackStones.Length.ToString();

        allstonesNum = whiteStones.Length + blackStones.Length;

        if ((allstonesNum >= 64 && whiteStones.Length > blackStones.Length) || blackStones.Length <= 0)
        {
            thisStatusNum = whiteWinNum;
        }
        else if ((allstonesNum >= 64 && whiteStones.Length < blackStones.Length) || whiteStones.Length <= 0)
        {
            thisStatusNum = blackWinNum;
        }
        else if (allstonesNum > 4 && whiteStones.Length == blackStones.Length)
        {
            thisStatusNum = drowNum;
        }

    }

    // ステージ生成
    private void CreateStage()
    {
        var stageSize = beaseStage.GetComponent<Image>().rectTransform.rect;
        tilePoint = new List<Vector2>();
        for (int i = 0; i < (int)stageSize.height && i < 8; i++)
        {
            for (int j = 0; j < (int)stageSize.width && j < 8; j++)
            {
                tilePoint.Add(new Vector2(j, i));
                GameObject tileClone = Instantiate(tilePrefab, tileCreatePoint.transform.position + new Vector3(j, i, 0), Quaternion.identity, beaseStage.transform);
                tileClone.name = i.ToString() + "-" + j.ToString();
            }
        }
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
        if (!isPutOrPass && (thisStatusNum == blackTurnNum || thisStatusNum == WhiteTrunNum) && ((pointerObject.transform.childCount == 0 && IsPutImpossible()) || pointerObject.name == "PassButton"))
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
                stoneClone.name = "BlackStone";
                stoneClone.tag = "Black";
            }
            else if (turnStatusNum == WhiteTrunNum)
            {
                stoneClone.GetComponent<Image>().color = Color.white;
                stoneClone.name = "WhiteStone";
                stoneClone.tag = "White";
            }
            ReverseStoneProcess();
        }
        else { return; }
    }

    bool IsPutImpossible()
    {
        for (int i = 0; i < DirectionList.Count; i++)
        {
            //RaycastAllの引数（PointerEventData）作成
            PointerEventData pointData = new PointerEventData(EventSystem.current);
            //RaycastAllの結果格納用List
            List<RaycastResult> rayResult = new List<RaycastResult>();
            List<GameObject> reverseObj = new List<GameObject>();
            if (PutImpossible(pointData, rayResult, reverseObj, i, 0))
            {
                return true;
            }
        }
        return false;
    }

    bool PutImpossible(PointerEventData point, List<RaycastResult> ray, List<GameObject> reverseObjList, int directionNum, int count)
    {
        var keepRay = ray;
        var keepObj = reverseObjList;
        var errorNum = 0;
        Vector3 addPoint = new Vector3(DirectionList[directionNum].x, DirectionList[directionNum].y, 0);
        point.position = Input.mousePosition + addPoint + (addPoint * count);
        EventSystem.current.RaycastAll(point, keepRay);
        foreach (RaycastResult result in keepRay)
        {
            var targetObj = result.gameObject;
            if (targetObj.layer == stoneLayer && thisStatusNum == blackTurnNum)
            {
                if (targetObj.name.Contains("Black"))
                {
                    if (count != 0)
                    {
                        return true;
                    }
                    else { return false; }

                }
                else if (targetObj.name.Contains("White"))
                {
                    count++;
                    keepObj.Add(targetObj);
                    if (PutImpossible(point, keepRay, keepObj, directionNum, count))
                    {
                        errorNum = 1;
                    }
                    break;
                }
                else { break; }
            }
            else if (targetObj.layer == stoneLayer && thisStatusNum == WhiteTrunNum)
            {
                if (targetObj.name.Contains("Black"))
                {
                    count++;
                    keepObj.Add(targetObj);
                    if (PutImpossible(point, keepRay, keepObj, directionNum, count))
                    {
                        errorNum = 1;
                    }
                    break;
                }
                else if (targetObj.name.Contains("White"))
                {
                    if (count != 0)
                    {
                        return true;
                    }
                    else { return false; }
                }
                else { break; }
            }
            else { errorNum = 2; ; break; }
        }
        if (count != 0 && errorNum == 1) { return true; }
        else { return false; }
    }

    void ReverseStoneProcess()
    {
        for (int i = 0; i < DirectionList.Count; i++)
        {
            //RaycastAllの引数（PointerEventData）作成
            PointerEventData pointData = new PointerEventData(EventSystem.current);
            //RaycastAllの結果格納用List
            List<RaycastResult> rayResult = new List<RaycastResult>();
            List<GameObject> reverseObj = new List<GameObject>();
            ReverseStone(pointData, rayResult, reverseObj, i, 0);
        }
    }
    void ReverseStone(PointerEventData point, List<RaycastResult> ray, List<GameObject> reverseObjList, int directionNum, int count)
    {
        var keepRay = ray;
        var keepObj = reverseObjList;
        Vector3 addPoint = new Vector3(DirectionList[directionNum].x, DirectionList[directionNum].y, 0);
        point.position = Input.mousePosition + addPoint + (addPoint * count);
        EventSystem.current.RaycastAll(point, keepRay);
        foreach (RaycastResult result in keepRay)
        {
            var targetObj = result.gameObject;
            if (targetObj.layer == stoneLayer && thisStatusNum == blackTurnNum)
            {
                if (targetObj.name.Contains("Black"))
                {
                    if (count != 0)
                    {
                        for (int i = 0; keepObj.Count > i; i++)
                        {
                            keepObj[i].GetComponent<Image>().color = Color.black;
                            keepObj[i].name = "BlackStone";
                            keepObj[i].tag = "Black";
                        }
                        keepObj.Clear();
                    }

                }
                else if (targetObj.name.Contains("White"))
                {
                    count++;
                    keepObj.Add(targetObj);
                    ReverseStone(point, keepRay, keepObj, directionNum, count);
                    break;
                }
            }
            else if (targetObj.layer == stoneLayer && thisStatusNum == WhiteTrunNum)
            {
                if (targetObj.name.Contains("Black"))
                {
                    count++;
                    keepObj.Add(targetObj);
                    ReverseStone(point, keepRay, keepObj, directionNum, count);
                    break;
                }
                else if (targetObj.name.Contains("White"))
                {
                    if (count != 0)
                    {
                        for (int i = 0; keepObj.Count > i; i++)
                        {
                            keepObj[i].GetComponent<Image>().color = Color.white;
                            keepObj[i].name = "WhiteStone";
                            keepObj[i].tag = "White";
                        }
                        keepObj.Clear();
                    }
                }
            }
            else { break; }
        }

    }

    void StartStonePut()
    {
        endForm.SetActive(false);
        StonePut("3-4", blackTurnNum);
        StonePut("3-3", WhiteTrunNum);
        StonePut("4-4", WhiteTrunNum);
        StonePut("4-3", blackTurnNum);
        turnText.text = "黒のターンです";
        whiteSpeachBalloon.SetActive(false);
    }


}
