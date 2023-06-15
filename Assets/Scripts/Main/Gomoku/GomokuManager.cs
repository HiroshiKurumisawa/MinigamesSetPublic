using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using TMPro;
using SoundSystem;
using JsonClass;

public class GomokuManager : NetworkBaseManager
{
    #region 変数群
    // ゲーム状況関連
    [SerializeField] Canvas m_canvas;
    [SerializeField, Header("終了時のUI")]
    GameObject endForm;
    [SerializeField, Header("終了時のUIに表示するText")]
    TextMeshProUGUI endFormText;
    [SerializeField, Header("終了後ロビーに戻るまでの時間表示Text")]
    TextMeshProUGUI returnLobbyCount;
    [SerializeField, Header("降参確認画面")]
    GameObject surrenderFormUI;
    [SerializeField, Header("相手が降参した時のText")]
    GameObject surrenderText;
    [SerializeField, Header("黒石ユーザー")]
    TextMeshProUGUI blackStonesUser;
    [SerializeField, Header("白石ユーザー")]
    TextMeshProUGUI whiteStonesUser;
    [SerializeField, Header("オプション")]
    GameObject Option;
    const int blackTurnNum = 0;
    const int WhiteTrunNum = 1;
    const int blackWinNum = 2;
    const int whiteWinNum = 3;
    const int blackSurrenderNum = 4;
    const int whiteSurrenderNum = 5;
    const int drowNum = 6;
    int thisStatusNum = 0;
    bool isGameEnd = false;
    bool surrender = false;
    bool surrenderForm = false;
    bool updateGame = false;
    bool sceneMove = false;
    const float returnLobbyCountNum = 10f;
    float returnLobbycountDownValue;
    // 石配置、盤面用
    [SerializeField, Header("ベースステージ")]
    GameObject beaseStage;
    [SerializeField, Header("タイル生成ポイント")]
    GameObject tileCreatePoint;
    [SerializeField, Header("タイルのPrefab")]
    GameObject tilePrefab;
    List<GameObject> tilePoint;
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

    //テスト用
    const string User_name1 = "test1", User_name2 = "test2";
    const string Host_user = "test1", Guest_user = "test2";

    string putPoint = "";

    void Start()
    {
        SoundManager.Instance.PlayBGMWithFadeIn("Main", 1f);
        loginManagerCS = GameObject.FindObjectOfType<LoginManager>();
        roomDataManagerCS = GameObject.FindObjectOfType<RoomDataManager>();
        CreateStage();
        StartStonePut();
    }

    void FixedUpdate()
    {
        LimitTime(limitTime);
        UpdateGame();
        EndGame();
    }

    // 一番最初に起こる処理
    void StartStonePut()
    {
        blackStonesUser.text = roomDataManagerCS.User_host;
        whiteStonesUser.text = roomDataManagerCS.User_entry;
        surrenderText.SetActive(false);
        endForm.SetActive(false);
        StonePut("3-4", blackTurnNum);
        StonePut("3-3", WhiteTrunNum);
        StonePut("4-4", WhiteTrunNum);
        StonePut("4-3", blackTurnNum);
        turnText.text = "黒のターンです";
        whiteSpeachBalloon.SetActive(false);
    }

    // ゲーム情報更新
    void UpdateGame()
    {
        if (!updateGame)
        {
            updateGame = true;
            StartCoroutine(GameUpdateProcess());
        }
    }
    IEnumerator GameUpdateProcess()
    {
        // POST送信用のフォームを作成
        WWWForm postData = new WWWForm();
        postData.AddField("room_name", roomDataManagerCS.Room_name);

        // POSTでデータ送信
        using UnityWebRequest request = UnityWebRequest.Post(reversiUpdateGameURL, postData);
        request.timeout = 10;
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            print(request.error);
        }
        else
        {
            UpdateGame resData = JsonUtility.FromJson<UpdateGame>(request.downloadHandler.text);
            if (resData.result == 0 && resData.gameData.room_name == roomDataManagerCS.Room_name)
            {
                int status = int.Parse(resData.gameData.game_state);
                if (status == blackTurnNum || status == WhiteTrunNum)
                {
                    var selectObj = GameObject.Find(resData.gameData.set_point);
                    if (selectObj != null && ((selectObj.transform.childCount == 0) || selectObj.name == "PassButton"))
                    {
                        isPutOrPass = true;
                        thisStatusNum = status;
                        StonePut(selectObj.name, thisStatusNum);
                    }
                    else if (selectObj != null && selectObj.transform.childCount != 0)
                    {
                        updateGame = false;
                        yield break;
                    }
                }
                else { thisStatusNum = status; }
            }
            updateGame = false;
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

    void EndGame()
    {
        switch (thisStatusNum)
        {
            case blackWinNum:
                ReturnLobby(returnLobbyCountNum, roomDataManagerCS.User_host + "(黒)の勝利");
                break;
            case whiteWinNum:
                ReturnLobby(returnLobbyCountNum, roomDataManagerCS.User_entry + "(白)の勝利");
                break;
            case blackSurrenderNum:
                surrender = true;
                ReturnLobby(returnLobbyCountNum, roomDataManagerCS.User_entry + "(白)の勝利");
                break;
            case whiteSurrenderNum:
                surrender = true;
                ReturnLobby(returnLobbyCountNum, roomDataManagerCS.User_host + "(黒)の勝利");
                break;
            case drowNum:
                ReturnLobby(returnLobbyCountNum, "引き分け");
                break;
        }
    }

    // 石の設置
    void StonePut(string point, int turnStatusNum)
    {
        if (point != "PassButton")
        {
            SoundManager.Instance.PlayOneShotSe("reversi_put");
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

            var rectpos = GameObject.Find(point).GetComponent<RectTransform>();
            var putWorldPos = GetWorldPositionFromRectPosition(rectpos);

            ReverseStoneProcess(putWorldPos);
        }
        else
        {
            putPoint = "";
        }
    }

    public void PutStoneOrPass(BaseEventData data)
    {
        var pointerObject = (data as PointerEventData).pointerClick;
        if (!isPutOrPass && pointerObject.name == "PassButton")
        {
            putPoint = pointerObject.name;
        }

    }

    IEnumerator PutStonProcess(string point)
    {
        var statusString = thisStatusNum.ToString();
        // POST送信用のフォームを作成
        WWWForm postData = new WWWForm();
        postData.AddField("room_name", roomDataManagerCS.Room_name);
        postData.AddField("put_point", point);
        postData.AddField("game_status", statusString);

        // POSTでデータ送信
        using UnityWebRequest request = UnityWebRequest.Post( gomokuPutStoneURL, postData);
        request.timeout = 10;
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            print(request.error);
        }
    }

    // 石の反転処理
    void ReverseStoneProcess(Vector2 putCell)
    {
        for (int i = 0; i < DirectionList.Count; i++)
        {
            //RaycastAllの引数（PointerEventData）作成
            PointerEventData pointData = new PointerEventData(EventSystem.current);
            //RaycastAllの結果格納用List
            List<RaycastResult> rayResult = new List<RaycastResult>();
            List<GameObject> reverseObj = new List<GameObject>();
            ReverseStone(pointData, rayResult, reverseObj, putCell, i, 0);
        }
    }

    void ReverseStone(PointerEventData point, List<RaycastResult> ray, List<GameObject> reverseObjList, Vector3 inputPos, int directionNum, int count)
    {
        var keepRay = ray;
        var keepObj = reverseObjList;
        Vector3 addPoint = transform.TransformPoint(new Vector3(DirectionList[directionNum].x, DirectionList[directionNum].y, 0));
        point.position = transform.TransformPoint(inputPos) + addPoint + (addPoint * count);
        EventSystem.current.RaycastAll(point, keepRay);
        foreach (RaycastResult result in keepRay)
        {
            var targetObj = result.gameObject;
            if (targetObj.layer == stoneLayer && thisStatusNum == blackTurnNum)
            {
                if (targetObj.name.Contains("Black"))
                {
                    if (count < 5)
                    {
                        count++;
                        keepObj.Add(targetObj);
                        ReverseStone(point, keepRay, keepObj, inputPos, directionNum, count);
                        break;
                    }
                    else if (count >= 5)
                    {
                        print("黒の勝利");
                        keepObj.Clear();
                    }

                }
                else if (targetObj.name.Contains("White"))
                {
                    break;
                }
            }
            else if (targetObj.layer == stoneLayer && thisStatusNum == WhiteTrunNum)
            {
                if (targetObj.name.Contains("Black"))
                {
                    break;
                }
                else if (targetObj.name.Contains("White"))
                {
                    if (count < 5)
                    {
                        count++;
                        keepObj.Add(targetObj);
                        ReverseStone(point, keepRay, keepObj, inputPos, directionNum, count);
                        break;
                    }
                    else if (count >= 5)
                    {
                        print("白の勝利");
                        keepObj.Clear();
                    }
                }
            }
            else { break; }
        }

    }

    private Vector3 GetWorldPositionFromRectPosition(RectTransform rect)
    {
        //UI座標からスクリーン座標に変換
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(m_canvas.worldCamera, rect.position);
        return screenPos;
    }

    // ステージ生成
    private void CreateStage()
    {
        var stageSize = beaseStage.GetComponent<Image>().rectTransform.rect;
        tilePoint = new List<GameObject>();
        for (int i = 0; i < (int)stageSize.height && i < 8; i++)
        {
            for (int j = 0; j < (int)stageSize.width && j < 8; j++)
            {
                GameObject tileClone = Instantiate(tilePrefab, tileCreatePoint.transform.position + new Vector3(j, i, 0), Quaternion.identity, beaseStage.transform);
                tileClone.name = i.ToString() + "-" + j.ToString();
                tilePoint.Add(tileClone);
            }
        }
    }

    public void OpenSurrenderForm()
    {
        if (!surrenderForm && !isGameEnd)
        {
            surrenderForm = true;
            surrenderFormUI.SetActive(true);
        }
    }

    public void Surrender()
    {
        if (!surrender)
        {
            surrenderFormUI.SetActive(false);
            if (roomDataManagerCS.User_host == loginManagerCS.User_name) // 黒が降参したとき
            {
                StartCoroutine(SurrenderProcess(blackSurrenderNum));
            }
            else if (roomDataManagerCS.User_entry == loginManagerCS.User_name) // 白が降参したとき
            {
                StartCoroutine(SurrenderProcess(whiteSurrenderNum));
            }
        }
    }
    IEnumerator SurrenderProcess(int status)
    {
        var statusString = status.ToString();
        // POST送信用のフォームを作成
        WWWForm postData = new WWWForm();
        postData.AddField("room_name", roomDataManagerCS.Room_name);
        postData.AddField("game_status", statusString);

        // POSTでデータ送信
        using UnityWebRequest request = UnityWebRequest.Post(reversiSurrenderURL, postData);
        request.timeout = 10;
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            print(request.error);
        }
    }


    public void NotSurrender()
    {
        if (surrenderForm)
        {
            surrenderForm = false;
            surrenderFormUI.SetActive(false);
        }
    }

    // ゲーム終了後のカウントダウン(シーン遷移するための)
    void ReturnLobby(float retrunSceneCountNum, string resultString)
    {
        if (!isGameEnd)
        {
            isGameEnd = true;
            Option.SetActive(false);
            endForm.SetActive(true);
            endFormText.text = resultString;
            returnLobbycountDownValue = retrunSceneCountNum;
            returnLobbyCount.text = returnLobbycountDownValue.ToString("00");
            if (surrender)
            {
                surrenderText.SetActive(true);
            }
        }

        if (returnLobbycountDownValue <= 0 && !sceneMove)
        {
            sceneMove = true;
            StartCoroutine(EndGameProcess());
        }
        else
        {
            returnLobbycountDownValue -= Time.deltaTime;
            returnLobbyCount.text = returnLobbycountDownValue.ToString("00");
        }
    }
    IEnumerator EndGameProcess()
    {
        // POST送信用のフォームを作成
        WWWForm postData = new WWWForm();
        postData.AddField("room_name", roomDataManagerCS.Room_name);

        // POSTでデータ送信
        using UnityWebRequest request = UnityWebRequest.Post(endGameURL, postData);
        request.timeout = 10;
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            print(request.error);
        }
        else
        {
            SoundManager.Instance.StopBGMWithFadeOut(1f);
            FadeManager.Instance.LoadScene("Lobby", 0.5f);
        }
    }
}
