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
    #region �ϐ��Q
    // �Q�[���󋵊֘A
    [SerializeField] Canvas m_canvas;
    [SerializeField, Header("�I������UI")]
    GameObject endForm;
    [SerializeField, Header("�I������UI�ɕ\������Text")]
    TextMeshProUGUI endFormText;
    [SerializeField, Header("�I���ネ�r�[�ɖ߂�܂ł̎��ԕ\��Text")]
    TextMeshProUGUI returnLobbyCount;
    [SerializeField, Header("�~�Q�m�F���")]
    GameObject surrenderFormUI;
    [SerializeField, Header("���肪�~�Q��������Text")]
    GameObject surrenderText;
    [SerializeField, Header("���΃��[�U�[")]
    TextMeshProUGUI blackStonesUser;
    [SerializeField, Header("���΃��[�U�[")]
    TextMeshProUGUI whiteStonesUser;
    [SerializeField, Header("�I�v�V����")]
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
    // �Δz�u�A�Ֆʗp
    [SerializeField, Header("�x�[�X�X�e�[�W")]
    GameObject beaseStage;
    [SerializeField, Header("�^�C�������|�C���g")]
    GameObject tileCreatePoint;
    [SerializeField, Header("�^�C����Prefab")]
    GameObject tilePrefab;
    List<GameObject> tilePoint;
    [SerializeField, Header("�������X�g")]
    List<Vector2> DirectionList;        // �����(-1,1)
    [SerializeField, Header("�΂�Prefab")]
    GameObject stonePrefab;
    const int stoneLayer = 6;
    // �^�[���֌W
    [SerializeField, Header("�����s��UI")]
    GameObject BlackSpeachBalloonUI;
    [SerializeField, Header("�����s��UI")]
    GameObject whiteSpeachBalloon;
    [SerializeField, Header("���݂̃^�[����\������e�L�X�g")]
    TextMeshProUGUI turnText;
    // ���Ԑ����֌W
    [SerializeField, Header("�������ԕ\���e�L�X�g")]
    TextMeshProUGUI limitTimeText;
    const float limitTime = 60f;
    float countTime;
    bool isTimeCountStart = false;
    bool isPutOrPass = false;
    #endregion

    //�e�X�g�p
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

    // ��ԍŏ��ɋN���鏈��
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
        turnText.text = "���̃^�[���ł�";
        whiteSpeachBalloon.SetActive(false);
    }

    // �Q�[�����X�V
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
        // POST���M�p�̃t�H�[�����쐬
        WWWForm postData = new WWWForm();
        postData.AddField("room_name", roomDataManagerCS.Room_name);

        // POST�Ńf�[�^���M
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

    void EndGame()
    {
        switch (thisStatusNum)
        {
            case blackWinNum:
                ReturnLobby(returnLobbyCountNum, roomDataManagerCS.User_host + "(��)�̏���");
                break;
            case whiteWinNum:
                ReturnLobby(returnLobbyCountNum, roomDataManagerCS.User_entry + "(��)�̏���");
                break;
            case blackSurrenderNum:
                surrender = true;
                ReturnLobby(returnLobbyCountNum, roomDataManagerCS.User_entry + "(��)�̏���");
                break;
            case whiteSurrenderNum:
                surrender = true;
                ReturnLobby(returnLobbyCountNum, roomDataManagerCS.User_host + "(��)�̏���");
                break;
            case drowNum:
                ReturnLobby(returnLobbyCountNum, "��������");
                break;
        }
    }

    // �΂̐ݒu
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
        // POST���M�p�̃t�H�[�����쐬
        WWWForm postData = new WWWForm();
        postData.AddField("room_name", roomDataManagerCS.Room_name);
        postData.AddField("put_point", point);
        postData.AddField("game_status", statusString);

        // POST�Ńf�[�^���M
        using UnityWebRequest request = UnityWebRequest.Post( gomokuPutStoneURL, postData);
        request.timeout = 10;
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            print(request.error);
        }
    }

    // �΂̔��]����
    void ReverseStoneProcess(Vector2 putCell)
    {
        for (int i = 0; i < DirectionList.Count; i++)
        {
            //RaycastAll�̈����iPointerEventData�j�쐬
            PointerEventData pointData = new PointerEventData(EventSystem.current);
            //RaycastAll�̌��ʊi�[�pList
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
                        print("���̏���");
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
                        print("���̏���");
                        keepObj.Clear();
                    }
                }
            }
            else { break; }
        }

    }

    private Vector3 GetWorldPositionFromRectPosition(RectTransform rect)
    {
        //UI���W����X�N���[�����W�ɕϊ�
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(m_canvas.worldCamera, rect.position);
        return screenPos;
    }

    // �X�e�[�W����
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
            if (roomDataManagerCS.User_host == loginManagerCS.User_name) // �����~�Q�����Ƃ�
            {
                StartCoroutine(SurrenderProcess(blackSurrenderNum));
            }
            else if (roomDataManagerCS.User_entry == loginManagerCS.User_name) // �����~�Q�����Ƃ�
            {
                StartCoroutine(SurrenderProcess(whiteSurrenderNum));
            }
        }
    }
    IEnumerator SurrenderProcess(int status)
    {
        var statusString = status.ToString();
        // POST���M�p�̃t�H�[�����쐬
        WWWForm postData = new WWWForm();
        postData.AddField("room_name", roomDataManagerCS.Room_name);
        postData.AddField("game_status", statusString);

        // POST�Ńf�[�^���M
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

    // �Q�[���I����̃J�E���g�_�E��(�V�[���J�ڂ��邽�߂�)
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
        // POST���M�p�̃t�H�[�����쐬
        WWWForm postData = new WWWForm();
        postData.AddField("room_name", roomDataManagerCS.Room_name);

        // POST�Ńf�[�^���M
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
