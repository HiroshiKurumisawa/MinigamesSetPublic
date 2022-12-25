using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.SceneManagement; // �V�[���J�ڗp(�t�F�[�h�}�l�[�W���[�쐬���폜)

public class ReversiManager : MonoBehaviour
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
    [SerializeField, Header("���Α���")]
    TextMeshProUGUI blackStonesText;
    [SerializeField, Header("���Α���")]
    TextMeshProUGUI whiteStonesText;
    [SerializeField, Header("���΃��[�U�[")]
    TextMeshProUGUI blackStonesUser;
    [SerializeField, Header("���΃��[�U�[")]
    TextMeshProUGUI whiteStonesUser;
    const int blackTurnNum = 0;
    const int WhiteTrunNum = 1;
    const int blackWinNum = 2;
    const int whiteWinNum = 3;
    const int drowNum = 4;
    int thisStatusNum;
    int allstonesNum = 0;
    bool isGameEnd = false;
    bool surrender = false;
    bool surrenderForm = false;
    bool updateGame = false;
    bool sceneMove = false;
    const float returnLobbyCountNum = 10f;
    float returnLobbycountDownValue;
    const string updateGameURL = "http://localhost/game/update_game";
    const string putStoneURL = "http://localhost/game/putStone_game";
    const string surrenderURL = "http://localhost/game/surrender_game";
    // �Δz�u�A�Ֆʗp
    [SerializeField, Header("�x�[�X�X�e�[�W")]
    GameObject beaseStage;
    [SerializeField, Header("�^�C�������|�C���g")]
    GameObject tileCreatePoint;
    [SerializeField, Header("�^�C����Prefab")]
    GameObject tilePrefab;
    List<Vector2> tilePoint;
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

    // �ꎞ�I��URL
    const string endGameURL= "http://localhost/game/end_game";

    LoginManager loginManagerCS;
    RoomDataManager roomDataCS;
    #endregion

    void Start()
    {
        loginManagerCS = GameObject.FindObjectOfType<LoginManager>();
        roomDataCS = GameObject.FindObjectOfType<RoomDataManager>();
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

    void EndGame()
    {
        if (thisStatusNum == whiteWinNum)
        {
            if(allstonesNum<=64)
            {
                surrender = true;
            }
            ReturnLobby(returnLobbyCountNum, roomDataCS.User_entry + "(��)�̏���");
        }
        else if (thisStatusNum == blackWinNum)
        {
            if (allstonesNum <= 64)
            {
                surrender = true;
            }
            ReturnLobby(returnLobbyCountNum, roomDataCS.User_host + "(��)�̏���");
        }
        else if (thisStatusNum == drowNum)
        {
            ReturnLobby(returnLobbyCountNum, "��������");
        }
        else { return; }
    }


    // �΂̐��𐔂���
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
        else if (allstonesNum >= 64 && whiteStones.Length == blackStones.Length)
        {
            thisStatusNum = drowNum;
        }

    }

    // �Q�[���I����̃J�E���g�_�E��(�V�[���J�ڂ��邽�߂�)
    void ReturnLobby(float retrunSceneCountNum, string resultString)
    {
        if (!isGameEnd)
        {
            isGameEnd = true;
            endForm.SetActive(true);
            endFormText.text = resultString;
            returnLobbycountDownValue = retrunSceneCountNum;
            returnLobbyCount.text = returnLobbycountDownValue.ToString("00");
            if (surrender)
            {
                surrenderText.SetActive(true);
            }
        }

        if (returnLobbycountDownValue <= 0&& !sceneMove)
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
        postData.AddField("room_name", roomDataCS.Room_name);

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
            SceneManager.LoadScene("Lobby"); /*�V�[���J��*/
        }
    }

    // �X�e�[�W����
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
        if (!isPutOrPass && (thisStatusNum == blackTurnNum && roomDataCS.User_host == loginManagerCS.User_name || thisStatusNum == WhiteTrunNum && roomDataCS.User_entry == loginManagerCS.User_name) && ((pointerObject.transform.childCount == 0 && IsPutImpossible()) || pointerObject.name == "PassButton"))
        {
            StartCoroutine(PutStonProcess(pointerObject.name));
        }

    }
    IEnumerator PutStonProcess(string point)
    {
        var statusString = thisStatusNum.ToString();
        // POST���M�p�̃t�H�[�����쐬
        WWWForm postData = new WWWForm();
        postData.AddField("room_name", roomDataCS.Room_name);
        postData.AddField("put_point", point);
        postData.AddField("game_status", statusString);

        // POST�Ńf�[�^���M
        using UnityWebRequest request = UnityWebRequest.Post(putStoneURL, postData);
        request.timeout = 10;
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            print(request.error);
        }
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
        postData.AddField("room_name", roomDataCS.Room_name);

        // POST�Ńf�[�^���M
        using UnityWebRequest request = UnityWebRequest.Post(updateGameURL, postData);
        request.timeout = 10;
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            print(request.error);
        }
        else
        {
            UpdateGame resData = JsonUtility.FromJson<UpdateGame>(request.downloadHandler.text);
            if (resData.result == 0 && resData.gameData.room_name == roomDataCS.Room_name)
            {
                int status = int.Parse(resData.gameData.game_state);
                if (status == blackTurnNum || status == WhiteTrunNum)
                {
                    var selectObj = GameObject.Find(resData.gameData.set_point);
                    if (selectObj != null && selectObj.transform.childCount == 0)
                    {
                        isPutOrPass = true;
                        thisStatusNum = status;
                        StonePut(resData.gameData.set_point, thisStatusNum);
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
            var world = GetWorldPositionFromRectPosition(rectpos);

            ReverseStoneProcess(world);
        }
        else { return; }

    }

    private Vector3 GetWorldPositionFromRectPosition(RectTransform rect)
    {
        //UI���W����X�N���[�����W�ɕϊ�
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(m_canvas.worldCamera, rect.position);
        return screenPos;
    }

    // �΂̐ݒu���o���邩�̔���
    bool IsPutImpossible()
    {
        for (int i = 0; i < DirectionList.Count; i++)
        {
            //RaycastAll�̈����iPointerEventData�j�쐬
            PointerEventData pointData = new PointerEventData(EventSystem.current);
            //RaycastAll�̌��ʊi�[�pList
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
        Vector3 addPoint = transform.TransformPoint(new Vector3(DirectionList[directionNum].x, DirectionList[directionNum].y, 0));
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
                    ReverseStone(point, keepRay, keepObj, inputPos, directionNum, count);
                    break;
                }
            }
            else if (targetObj.layer == stoneLayer && thisStatusNum == WhiteTrunNum)
            {
                if (targetObj.name.Contains("Black"))
                {
                    count++;
                    keepObj.Add(targetObj);
                    ReverseStone(point, keepRay, keepObj, inputPos, directionNum, count);
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

    // ��ԍŏ��ɋN���鏈��
    void StartStonePut()
    {
        blackStonesUser.text = roomDataCS.User_host;
        whiteStonesUser.text = roomDataCS.User_entry;
        surrenderText.SetActive(false);
        endForm.SetActive(false);
        StonePut("3-4", blackTurnNum);
        StonePut("3-3", WhiteTrunNum);
        StonePut("4-4", WhiteTrunNum);
        StonePut("4-3", blackTurnNum);
        turnText.text = "���̃^�[���ł�";
        whiteSpeachBalloon.SetActive(false);
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
            if (roomDataCS.User_host == loginManagerCS.User_name) // �����~�Q�����Ƃ�
            {
                StartCoroutine(SurrenderProcess(whiteWinNum));
            }
            else if (roomDataCS.User_entry == loginManagerCS.User_name) // �����~�Q�����Ƃ�
            {
                StartCoroutine(SurrenderProcess(blackWinNum));
            }
        }
    }
    IEnumerator SurrenderProcess(int status)
    {
        var statusString = status.ToString();
        // POST���M�p�̃t�H�[�����쐬
        WWWForm postData = new WWWForm();
        postData.AddField("room_name", roomDataCS.Room_name);
        postData.AddField("game_status", statusString);

        // POST�Ńf�[�^���M
        using UnityWebRequest request = UnityWebRequest.Post(surrenderURL, postData);
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
}

// �Q�[���f�[�^�\����
[Serializable]
public class UpdateGame
{
    public int result;
    public GameData gameData;
}
[Serializable]
public class GameData
{
    public int id;
    public string room_name;
    public string set_point;
    public string game_state;
    public string created_at;
    public string updated_at;
}

[Serializable]
public class DeleteRoom
{
    public int result;
}