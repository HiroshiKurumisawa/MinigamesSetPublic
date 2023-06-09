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
    int allstonesNum = 0;
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
    bool isPointResult = false;
    #endregion

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
        StoneCount();
        EndGame();
    }

    // 一番最初に起こる処理
    void StartStonePut()
    {
        blackStonesUser.text = roomDataManagerCS.User_host;
        whiteStonesUser.text = roomDataManagerCS.User_entry;

        surrenderText.SetActive(false);
        endForm.SetActive(false);
        turnText.text = "黒のターンです";
        whiteSpeachBalloon.SetActive(false);
    }

    // 石の数を数える
    void StoneCount()
    {
        var whiteStones = GameObject.FindGameObjectsWithTag("White");
        var blackStones = GameObject.FindGameObjectsWithTag("Black");
        allstonesNum = whiteStones.Length + blackStones.Length;
        if (allstonesNum >= 225)
        {
            thisStatusNum = drowNum;
        }

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
        using UnityWebRequest request = UnityWebRequest.Post(gomokuUpdateGameURL, postData);
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
            UIclose(BlackSpeachBalloonUI);
            UIopen(whiteSpeachBalloon, NoActionCol());
            turnText.text = "白のターンです";
        }
        else if (statusNum == WhiteTrunNum)
        {
            thisStatusNum = blackTurnNum;
            UIclose(whiteSpeachBalloon);
            UIopen(BlackSpeachBalloonUI, NoActionCol());
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
                if (roomDataManagerCS.User_host == loginManagerCS.User_name && !isPointResult)
                {
                    isPointResult = true;
                    UserPointResult(loginManagerCS, Win);
                }
                else if (roomDataManagerCS.User_host != loginManagerCS.User_name && !isPointResult)
                {
                    isPointResult = true;
                    UserPointResult(loginManagerCS, Lose);
                }
                break;
            case whiteWinNum:
                ReturnLobby(returnLobbyCountNum, roomDataManagerCS.User_entry + "(白)の勝利");
                if (roomDataManagerCS.User_entry == loginManagerCS.User_name && !isPointResult)
                {
                    isPointResult = true;
                    UserPointResult(loginManagerCS, Win);
                }
                else if (roomDataManagerCS.User_entry != loginManagerCS.User_name && !isPointResult)
                {
                    isPointResult = true;
                    UserPointResult(loginManagerCS, Lose);
                }
                break;
            case blackSurrenderNum:
                surrender = true;
                ReturnLobby(returnLobbyCountNum, roomDataManagerCS.User_entry + "(白)の勝利");
                if (roomDataManagerCS.User_entry == loginManagerCS.User_name && !isPointResult)
                {
                    isPointResult = true;
                    UserPointResult(loginManagerCS, Win);
                }
                else if (roomDataManagerCS.User_entry != loginManagerCS.User_name && !isPointResult)
                {
                    isPointResult = true;
                    UserPointResult(loginManagerCS, Lose);
                }
                break;
            case whiteSurrenderNum:
                surrender = true;
                ReturnLobby(returnLobbyCountNum, roomDataManagerCS.User_host + "(黒)の勝利");
                if (roomDataManagerCS.User_host == loginManagerCS.User_name && !isPointResult)
                {
                    isPointResult = true;
                    UserPointResult(loginManagerCS, Win);
                }
                else if (roomDataManagerCS.User_host != loginManagerCS.User_name && !isPointResult)
                {
                    isPointResult = true;
                    UserPointResult(loginManagerCS, Lose);
                }
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

            StoneProcess(putWorldPos);
        }
        else
        {
            StartCoroutine(PutStonProcess(""));
        }
    }

    public void PutStoneOrPass(BaseEventData data)
    {
        var pointerObject = (data as PointerEventData).pointerClick;
        if (!isPutOrPass && ((thisStatusNum == blackTurnNum && roomDataManagerCS.User_host == loginManagerCS.User_name) || (thisStatusNum == WhiteTrunNum && roomDataManagerCS.User_entry == loginManagerCS.User_name)) && ((pointerObject.transform.childCount == 0) || pointerObject.name == "PassButton"))
        {
            StartCoroutine(PutStonProcess(pointerObject.name));
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
        using UnityWebRequest request = UnityWebRequest.Post(gomokuPutStoneURL, postData);
        request.timeout = 10;
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            print(request.error);
        }
    }

    // 石の判定処理
    void StoneProcess(Vector2 putCell)
    {
        for (int i = 0; i < DirectionList.Count; i++)
        {
            //RaycastAllの引数（PointerEventData）作成
            PointerEventData pointData = new PointerEventData(EventSystem.current);
            //RaycastAllの結果格納用List
            List<RaycastResult> rayResult = new List<RaycastResult>();
            List<GameObject> reverseObj = new List<GameObject>();
            JudgeStone(pointData, rayResult, reverseObj, putCell, i, 0);
        }
    }

    void JudgeStone(PointerEventData point, List<RaycastResult> ray, List<GameObject> reverseObjList, Vector3 inputPos, int directionNum, int count)
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
                    if (count >= 3)
                    {
                        thisStatusNum = blackWinNum;
                        keepObj.Clear();

                    }
                    else
                    {
                        count++;
                        keepObj.Add(targetObj);
                        JudgeStone(point, keepRay, keepObj, inputPos, directionNum, count);
                        break;
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
                    if (count >= 3)
                    {
                        thisStatusNum = whiteWinNum;
                        keepObj.Clear();

                    }
                    else
                    {
                        count++;
                        keepObj.Add(targetObj);
                        JudgeStone(point, keepRay, keepObj, inputPos, directionNum, count);
                        break;
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
        tilePoint = new List<GameObject>();
        for (int i = 0; i < 15; i++)
        {
            for (int j = 0; j < 15; j++)
            {
                GameObject tileClone = Instantiate(tilePrefab, tileCreatePoint.transform.position + (new Vector3(j, i, 0)) * 0.6f, Quaternion.identity, beaseStage.transform);
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
            UIopen(surrenderFormUI, NoActionCol());
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
            UIclose(surrenderFormUI);
        }
    }

    // ゲーム終了後のカウントダウン(シーン遷移するための)
    void ReturnLobby(float retrunSceneCountNum, string resultString)
    {
        if (!isGameEnd)
        {
            isGameEnd = true;
            Option.SetActive(false);
            StartCoroutine(EndGameProcess());
            UIopen(endForm, NoActionCol());
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
            SoundManager.Instance.StopBGMWithFadeOut(1f);
            FadeManager.Instance.LoadScene("Lobby", 0.5f);
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
    }

    private void OnApplicationQuit()
    {
        if (roomDataManagerCS.User_host == loginManagerCS.User_name) // 黒が切断したとき
        {
            StartCoroutine(SurrenderProcess(blackSurrenderNum));
        }
        else if (roomDataManagerCS.User_entry == loginManagerCS.User_name) // 白が切断したとき
        {
            StartCoroutine(SurrenderProcess(whiteSurrenderNum));
        }
    }
}
