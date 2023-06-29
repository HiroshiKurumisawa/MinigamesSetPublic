using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using SoundSystem;
using JsonClass;

public class NetworkBaseManager : MonoBehaviour
{
    #region LocalURL
    protected const string accountLoginURL = "http://localhost/user/account/login";                 // �A�J�E���g���O�C��URL
    protected const string logOutURL = "http://localhost/user/logout";                              // ���O�A�E�gURL
    protected const string guestLoginURL = "http://localhost/user/guest/login";                     // �Q�X�g���O�C��URL
    protected const string createAccountURL = "http://localhost/user/account/create";               // �A�J�E���g�쐬URL
    protected const string entryRoomURL = "http://localhost/room/entry";                            // ���[���Q��URL
    protected const string updateSelectFormURL = "http://localhost/room/select_form_update";        // ���[���I����ʍX�VURL
    protected const string roomCreateURL = "http://localhost/room/create";                          // ���[���쐬URL
    protected const string readyURL = "http://localhost/room/ready_user_room";                      // ���������ύXURL
    protected const string leaveRoomURL = "http://localhost/room/leave_room";                       // �ޏo�pURL
    protected const string updateRoomFormURL = "http://localhost/room/room_form_update";            // ���[�����X�V�pURL
    protected const string gameStartURL = "http://localhost/room/gamestart_room";                   // �Q�[���J�nURL  
    // ���o�[�V
    protected const string reversiUpdateGameURL = "http://localhost/game/update_game";
    protected const string reversiPutStoneURL = "http://localhost/game/putStone_game";
    protected const string reversiSurrenderURL = "http://localhost/game/surrender_game";
    //
    // �ܖڕ���
    protected const string gomokuUpdateGameURL = "http://localhost/game/update_game";
    protected const string gomokuPutStoneURL = "http://localhost/game/putStone_game";
    protected const string gomokuSurrenderURL = "http://localhost/game/surrender_game";
    //
    protected const string endGameURL = "http://localhost/game/end_game";                           //�ΐ�I��URL
    protected const string rankingUpdateURL = "http://localhost/ranking/ranking_view";              // �����L���O���X�V
    protected const string accountPointUpdateURL = "http://localhost/ranking/ranking_point_update";
    #endregion
    #region ServerURL
    //protected const string accountLoginURL = "http://ik1-423-43506.vs.sakura.ne.jp/user/account/login";               // �A�J�E���g���O�C��URL
    //protected const string logOutURL = "http://ik1-423-43506.vs.sakura.ne.jp/user/logout";                            // ���O�A�E�gURL
    //protected const string guestLoginURL = "http://ik1-423-43506.vs.sakura.ne.jp/user/guest/login";                   // �Q�X�g���O�C��URL
    //protected const string createAccountURL = "http://ik1-423-43506.vs.sakura.ne.jp/user/account/create";             // �A�J�E���g�쐬URL
    //protected const string updateSelectFormURL = "http://ik1-423-43506.vs.sakura.ne.jp/room/select_form_update";      // ���[���I����ʍX�VURL
    //protected const string entryRoomURL = "http://ik1-423-43506.vs.sakura.ne.jp/room/entry";                          // ���[���Q��URL
    //protected const string roomCreateURL = "http://ik1-423-43506.vs.sakura.ne.jp/room/create";                        // ���[���쐬URL
    //protected const string readyURL = "http://ik1-423-43506.vs.sakura.ne.jp/room/ready_user_room";                    // ���������ύXURL
    //protected const string leaveRoomURL = "http://ik1-423-43506.vs.sakura.ne.jp/room/leave_room";                     // �ޏo�pURL
    //protected const string updateRoomFormURL = "http://ik1-423-43506.vs.sakura.ne.jp/room/room_form_update";          // ���[�����X�V�pURL
    //protected const string gameStartURL = "http://ik1-423-43506.vs.sakura.ne.jp/room/gamestart_room";                 // �Q�[���J�nURL
    // ���o�[�V
    //protected const string reversiUpdateGameURL = "http://ik1-423-43506.vs.sakura.ne.jp/game/update_game";
    //protected const string reversiPutStoneURL = "http://ik1-423-43506.vs.sakura.ne.jp/game/putStone_game";
    //protected const string reversiSurrenderURL = "http://ik1-423-43506.vs.sakura.ne.jp/game/surrender_game";
    //
    // �ܖڕ���
    //protected const string gomokuUpdateGameURL = "http://ik1-423-43506.vs.sakura.ne.jp/game/update_game";
    //protected const string gomokuPutStoneURL = "http://ik1-423-43506.vs.sakura.ne.jp/game/putStone_game";
    //protected const string gomokuSurrenderURL = "http://ik1-423-43506.vs.sakura.ne.jp/game/surrender_game";
    //
    //protected const string endGameURL = "http://ik1-423-43506.vs.sakura.ne.jp/game/end_game";                         // �ΐ�I��URL
    //protected const string rankingUpdateURL = "http://ik1-423-43506.vs.sakura.ne.jp/ranking/ranking_view";
    //protected const string accountPointUpdate = "http://ik1-423-43506.vs.sakura.ne.jp/ranking/ranking_point_update";
    #endregion


    #region �ϐ��Q
    // �K�v�ȃf�[�^�ێ����邽�߂̃X�N���v�g
    protected LoginManager loginManagerCS;
    protected RoomDataManager roomDataManagerCS;
    const int AccountUser = 0, GuestUser = 1;
    protected const int Win = 0, Lose = 1;
    #endregion
    #region ���O�C���֌W
    // �Q�X�g���O�C��
    protected IEnumerator GuestLoginProcess()
    {
        // POST���M�p�̃t�H�[�����쐬
        WWWForm postData = new WWWForm();

        // POST�Ńf�[�^���M
        using UnityWebRequest request = UnityWebRequest.Post(guestLoginURL, postData);
        request.timeout = 10;
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            print(request.error);
        }
        else
        {
            GuestLoginRoot resData = JsonUtility.FromJson<GuestLoginRoot>(request.downloadHandler.text);
            loginManagerCS.SetUserData(resData.guest_data.manage_id, resData.guest_data.login_id, resData.guest_data.user_name,
                resData.guest_data.last_login, resData.guest_data.created, resData.guest_data.modified, resData.guest_data.connection_status, 1);

            SoundManager.Instance.StopBGMWithFadeOut(1f);
            FadeManager.Instance.LoadScene("Lobby", 0.5f);
        }
    }
    // �A�J�E���g���O�C��
    protected virtual IEnumerator AccountLoginProcess(string loginUser_name, string loginUser_password, TextMeshProUGUI messageLogin, Action<bool> callback)
    {
        // POST���M�p�̃t�H�[�����쐬
        WWWForm postData = new WWWForm();
        postData.AddField("user_name", loginUser_name);
        postData.AddField("password", loginUser_password);
        // POST�Ńf�[�^���M
        using UnityWebRequest request = UnityWebRequest.Post(accountLoginURL, postData);
        request.timeout = 10;
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            print(request.error);
        }
        else
        {
            AccountLoginRoot resData = JsonUtility.FromJson<AccountLoginRoot>(request.downloadHandler.text);

            if (resData.result == 0)
            {
                if (resData.requestMessage == 1)           // �G���[���b�Z�[�W��������(���͓��e�G���[)
                {
                    messageLogin.color = new Color(255, 0, 0);
                    messageLogin.text = "���O�C�����s(���͓��e�Ɍ�肪����܂�)";
                    callback(false);
                }
                else if (resData.requestMessage == 2)   // �p�X���[�h���Ⴄ�ꍇ���
                {
                    messageLogin.color = new Color(255, 0, 0);
                    messageLogin.text = "���O�C�����s(�p�X���[�h�Ɍ�肪����܂�)";
                    callback(false);
                }
                else if (resData.requestMessage == 3)   // ���łɃ��O�C������Ă�����
                {
                    messageLogin.color = new Color(255, 0, 0);
                    messageLogin.text = "���O�C�����s(���Ƀ��O�C�����Ă��܂�)";
                    callback(false);
                }
                else if (resData.requestMessage == 4)   // �A�J�E���g���Ȃ��ꍇ
                {
                    messageLogin.color = new Color(255, 0, 0);
                    messageLogin.text = "���O�C�����s(���͂��ꂽ�A�J�E���g�����݂��܂���)";
                    callback(false);
                }
                else                                    // ���O�C���ł���Ƃ�
                {
                    loginManagerCS.SetUserData(resData.account_data.manage_id, resData.account_data.login_id, resData.account_data.user_name,
                             resData.account_data.last_login, resData.account_data.created, resData.account_data.modified, resData.account_data.connection_status, 0);
                    loginManagerCS.SetAccountPoint(resData.account_data.point);
                    SoundManager.Instance.StopBGMWithFadeOut(1f);
                    FadeManager.Instance.LoadScene("Lobby", 0.5f);
                }
            }

        }
    }
    // �A�J�E���g�쐬
    protected IEnumerator CreateAccountProcess(string createUser_name, string createUser_password, string createUser_rePassword,
        TMP_InputField user_nameField, TMP_InputField passwordField, TMP_InputField rePasswordField, GameObject massageText, Action<bool> callback)
    {
        // POST���M�p�̃t�H�[�����쐬
        WWWForm postData = new WWWForm();
        postData.AddField("user_name", createUser_name);
        postData.AddField("password", createUser_password);
        postData.AddField("repassword", createUser_rePassword);

        // POST�Ńf�[�^���M
        using UnityWebRequest request = UnityWebRequest.Post(createAccountURL, postData);
        request.timeout = 10;
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            print(request.error);
        }
        else
        {
            AccountCreateRoot resData = JsonUtility.FromJson<AccountCreateRoot>(request.downloadHandler.text);

            if (resData.requestMessage == 0)    // ����
            {
                user_nameField.text = "";
                passwordField.text = "";
                rePasswordField.text = "";
                massageText.GetComponent<TextMeshProUGUI>().color = new Color(0, 0, 255);
                massageText.GetComponent<TextMeshProUGUI>().text = "�쐬����";
                yield return new WaitForSeconds(0.8f);
                TitleManager titleManager = GameObject.FindObjectOfType<TitleManager>();
                titleManager.CloseCrateFormUI();
            }
            else if (resData.requestMessage == 1)   // ���[�U�[�����ɓo�^����Ă���
            {
                massageText.GetComponent<TextMeshProUGUI>().color = new Color(255, 0, 0);
                massageText.GetComponent<TextMeshProUGUI>().text = "�쐬���s\n(���[�U�[�������͂���Ă��܂���)";
            }
            else if (resData.requestMessage == 2)   // ���[�U�[���ɋ֎~���[�h�������Ă���
            {
                massageText.GetComponent<TextMeshProUGUI>().color = new Color(255, 0, 0);
                massageText.GetComponent<TextMeshProUGUI>().text = "�쐬���s\n(�������[�U�[�������ɓo�^����Ă��܂�)";
            }
            else if (resData.requestMessage == 3)   // ���[�U�[�������͂���Ă��Ȃ�
            {
                massageText.GetComponent<TextMeshProUGUI>().color = new Color(255, 0, 0);
                massageText.GetComponent<TextMeshProUGUI>().text = "�쐬���s\n(���[�U�[����\"�Q�X�g\"���܂܂�Ă��܂�)";
            }
            else if (resData.requestMessage == 4)   // �p�X���[�h�ɕs��������
            {
                massageText.GetComponent<TextMeshProUGUI>().color = new Color(255, 0, 0);
                massageText.GetComponent<TextMeshProUGUI>().text = "�쐬���s\n(�p�X���[�h�����������͂���Ă��܂���)";
            }
            else if (resData.requestMessage == 5)
            {
                massageText.GetComponent<TextMeshProUGUI>().color = new Color(255, 0, 0);
                massageText.GetComponent<TextMeshProUGUI>().text = "�쐬���s\n(���[�U�[���ƃp�X���[�h�����ɕs��������܂�)";
            }
        }
        callback(false);
    }
    // ���O�A�E�g
    protected IEnumerator LogOutProcess()
    {
        // POST���M�p�̃t�H�[�����쐬
        WWWForm postData = new WWWForm();
        postData.AddField("manageID", loginManagerCS.Manage_id);
        postData.AddField("userName", loginManagerCS.User_name);
        postData.AddField("userType", loginManagerCS.User_Type);

        // POST�Ńf�[�^���M
        using UnityWebRequest request = UnityWebRequest.Post(logOutURL, postData);
        request.timeout = 10;
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            print(request.error);
        }
        else
        {
            SoundManager.Instance.StopBGMWithFadeOut(1f);
            FadeManager.Instance.LoadScene("Title", 0.5f);
        }
    }
    #endregion
    #region ���[���֌W
    // ���[���쐬
    protected IEnumerator CreateRoomProcess(string createRoom_name, string createRoom_password, string createRoom_gameRule, TMP_InputField room_nameField_CreateRoom,
        TMP_InputField passwordField_CreateRoom, GameObject massage_CreateRoomText, GameObject createRoomForm, Action<bool> isOpenCreateRoomForm)
    {
        // POST���M�p�̃t�H�[�����쐬
        WWWForm postData = new WWWForm();
        postData.AddField("room_name", createRoom_name);
        postData.AddField("room_password", createRoom_password);
        postData.AddField("room_max_users", "2");
        postData.AddField("room_host_user", loginManagerCS.User_name);
        postData.AddField("room_game_rule", createRoom_gameRule);

        // POST�Ńf�[�^���M
        using UnityWebRequest request = UnityWebRequest.Post(roomCreateURL, postData);
        request.timeout = 10;
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            print(request.error);
        }
        else
        {
            RoomCreateRoot resData = JsonUtility.FromJson<RoomCreateRoot>(request.downloadHandler.text);

            if (resData.requestMessage == 0) // ���������Ƃ�
            {
                room_nameField_CreateRoom.text = "";
                passwordField_CreateRoom.text = "";
                massage_CreateRoomText.GetComponent<TextMeshProUGUI>().text = "";
                createRoomForm.SetActive(false);
                LobbyManager lobbyManager = FindObjectOfType<LobbyManager>();
                lobbyManager.OpenRoomForm(resData.roomData.room_name, resData.roomData.room_password, resData.roomData.user_host,
                    resData.roomData.user_entry, resData.roomData.ready_status_host, resData.roomData.ready_status_entry, resData.roomData.game_status, resData.roomData.game_rule);
            }
            else if (resData.requestMessage == 1)                   // �G���[���Ԃ��Ă����Ƃ�
            {
                isOpenCreateRoomForm(true);
                massage_CreateRoomText.GetComponent<TextMeshProUGUI>().color = new Color(255, 0, 0);
                massage_CreateRoomText.GetComponent<TextMeshProUGUI>().text = "�쐬���s(���͓��e���s�K�؂ł�)";
            }
        }

    }

    // ���[���G���g���[
    protected IEnumerator EntryRoomInPassProcess(string inputRoomPasswordFormRoomName, string inputRoomPassword, TMP_InputField passwordField_EntryRoom,
        GameObject roomsSelectForm, GameObject inputRoomPasswordMessageText, GameObject inputRoomPasswordForm, Action<bool> isOpenInputRoomPasswordForm, Action<bool> isEntryRoomInPassRetrun, bool isEntryRoomInPass)
    {
        // POST���M�p�̃t�H�[�����쐬
        WWWForm postData = new WWWForm();
        postData.AddField("room_name", inputRoomPasswordFormRoomName);
        postData.AddField("room_password", inputRoomPassword);
        postData.AddField("room_entry_user", loginManagerCS.User_name);

        // POST�Ńf�[�^���M
        using UnityWebRequest request = UnityWebRequest.Post(entryRoomURL, postData);
        request.timeout = 10;
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            print(request.error);
        }
        else
        {
            EntryRoomInpassRoot resData = JsonUtility.FromJson<EntryRoomInpassRoot>(request.downloadHandler.text);

            if (resData.requestMessage == 0) // ���������Ƃ�
            {
                passwordField_EntryRoom.text = "";
                roomsSelectForm.SetActive(false);
                LobbyManager lobbyManager = FindObjectOfType<LobbyManager>();
                lobbyManager.OpenRoomForm(resData.roomData.room_name, resData.roomData.room_password, resData.roomData.user_host, resData.roomData.user_entry, resData.roomData.ready_status_host, resData.roomData.ready_status_entry, resData.roomData.game_status, resData.roomData.game_rule);
                isOpenInputRoomPasswordForm(false);
                inputRoomPasswordMessageText.GetComponent<TextMeshProUGUI>().text = "";
                inputRoomPasswordForm.SetActive(false);
            }
            else if (resData.requestMessage == 2)                   // �G���[2���Ԃ��Ă����Ƃ�
            {
                inputRoomPasswordMessageText.GetComponent<TextMeshProUGUI>().text = "�p�X���[�h���Ⴂ�܂�";
            }
            else
            {
                print("error");
            }

            isEntryRoomInPass = isEntryRoomInPass == true ? false : true;
            isEntryRoomInPassRetrun(isEntryRoomInPass);
        }
    }

    // ���[���̑����X�V
    protected IEnumerator UpdateRoomSelectFormProcess(GameObject roomUIprefab, GameObject roomScrollView)
    {
        // �X�V�O�̃��[�������폜
        GameObject[] rooms = GameObject.FindGameObjectsWithTag("Room");
        foreach (GameObject room in rooms)
        {
            Destroy(room);
        }

        // POST���M�p�̃t�H�[�����쐬
        WWWForm postData = new WWWForm();

        // POST�Ńf�[�^���M
        using UnityWebRequest request = UnityWebRequest.Post(updateSelectFormURL, postData);
        request.timeout = 10;
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            print(request.error);
        }
        else
        {
            AllRoomRoot resData = JsonUtility.FromJson<AllRoomRoot>(request.downloadHandler.text);

            for (int i = 0; i < resData.allRoomList.Count; i++)
            {
                GameObject roomUIclone = Instantiate(roomUIprefab, transform.position, Quaternion.identity, roomScrollView.transform);
                roomUIclone.name = "Room_" + i.ToString();
                SelectRoomManager selectRoomManagerCS = roomUIclone.GetComponent<SelectRoomManager>();
                selectRoomManagerCS.SetRoomData(resData.allRoomList[i].room_name, resData.allRoomList[i].room_password, resData.allRoomList[i].in_room_users, resData.allRoomList[i].max_room_users);
            }
        }

    }

    // ��������
    protected IEnumerator ReadyProcess(GameObject roomNameTextUI, Action<bool> isReady)
    {
        // POST���M�p�̃t�H�[�����쐬
        WWWForm postData = new WWWForm();
        postData.AddField("room_name", roomNameTextUI.GetComponent<TextMeshProUGUI>().text);
        postData.AddField("ready_user", loginManagerCS.User_name);

        // POST�Ńf�[�^���M
        using UnityWebRequest request = UnityWebRequest.Post(readyURL, postData);
        request.timeout = 10;
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            print(request.error);
        }
        else
        {
            isReady(false);
        }
    }

    // �Q�[���J�n
    protected IEnumerator GameStartProcess(GameObject roomNameTextUI, Action<bool> gameStart)
    {
        // POST���M�p�̃t�H�[�����쐬
        WWWForm postData = new WWWForm();
        postData.AddField("room_name", roomNameTextUI.GetComponent<TextMeshProUGUI>().text);

        // POST�Ńf�[�^���M
        using UnityWebRequest request = UnityWebRequest.Post(gameStartURL, postData);
        request.timeout = 10;
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            print(request.error);
        }
        else
        {
            gameStart(false);
        }
    }

    // ���[���ޏo
    protected IEnumerator RoomLeaveProcess(GameObject roomNameTextUI, GameObject roomForm, Action<bool> isOpenSelectRoomForm, Action<bool> leaveRoom)
    {
        // POST���M�p�̃t�H�[�����쐬
        WWWForm postData = new WWWForm();
        postData.AddField("room_name", roomNameTextUI.GetComponent<TextMeshProUGUI>().text);
        postData.AddField("room_leave_user", loginManagerCS.User_name);

        // POST�Ńf�[�^���M
        using UnityWebRequest request = UnityWebRequest.Post(leaveRoomURL, postData);
        request.timeout = 10;
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            print(request.error);
        }
        else
        {
            LeaveRoom resData = JsonUtility.FromJson<LeaveRoom>(request.downloadHandler.text);
            if (resData.result == 0)
            {
                roomForm.SetActive(false);
                isOpenSelectRoomForm(false);
            }
            leaveRoom(false);
        }
    }

    // �Q�����̃��[���X�V
    protected IEnumerator RoomKeepUpdateProcess(GameObject roomNameTextUI, GameObject roomForm, Action<bool> updateRoomForm)
    {
        // POST���M�p�̃t�H�[�����쐬
        WWWForm postData = new WWWForm();
        postData.AddField("room_name", roomNameTextUI.GetComponent<TextMeshProUGUI>().text);

        // POST�Ńf�[�^���M
        using UnityWebRequest request = UnityWebRequest.Post(updateRoomFormURL, postData);
        request.timeout = 10;
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            print(request.error);
        }
        else
        {
            UpdateRoom resData = JsonUtility.FromJson<UpdateRoom>(request.downloadHandler.text);
            if (roomForm.activeSelf == true)
            {
                LobbyManager lobbyManager = FindObjectOfType<LobbyManager>();
                lobbyManager.OpenRoomForm(resData.roomData.room_name, resData.roomData.room_password, resData.roomData.user_host, resData.roomData.user_entry, resData.roomData.ready_status_host, resData.roomData.ready_status_entry, resData.roomData.game_status, resData.roomData.game_rule);
                roomDataManagerCS.SetRoomData(resData.roomData.room_name, resData.roomData.user_host, resData.roomData.user_entry, resData.roomData.game_rule);
            }
            yield return new WaitForSeconds(0.3f);
            updateRoomForm(false);
        }
    }
    #endregion
    #region �����L���O
    // ���[���̑����X�V
    protected IEnumerator UpdateRankingFormProcess(GameObject acountUIprefab, GameObject rankigScrollView)
    {
        // �X�V�O�̃��[�������폜
        GameObject[] rankings = GameObject.FindGameObjectsWithTag("Ranking");
        foreach (GameObject room in rankings)
        {
            Destroy(room);
        }

        // POST���M�p�̃t�H�[�����쐬
        WWWForm postData = new WWWForm();

        // POST�Ńf�[�^���M
        using UnityWebRequest request = UnityWebRequest.Post(rankingUpdateURL, postData);
        request.timeout = 10;
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            print(request.error);
        }
        else
        {
            Ranking resData = JsonUtility.FromJson<Ranking>(request.downloadHandler.text);

            for (int i = 0; i < resData.allAcountList.Count; i++)
            {
                GameObject roomUIclone = Instantiate(acountUIprefab, transform.position, Quaternion.identity, rankigScrollView.transform);
                roomUIclone.name = "Account_" + i.ToString();
                RankingUserManager rankingUserManagerCS = roomUIclone.GetComponent<RankingUserManager>();
                rankingUserManagerCS.SetAccountData(resData.allAcountList[i].user_name, resData.allAcountList[i].point);
            }

            rankings = GameObject.FindGameObjectsWithTag("Ranking");
            List<GameObject> rankUserList = new List<GameObject>();

            for (int i = 0; i < rankings.Length; i++)
            {
                rankUserList.Add(rankings[i]);
            }
            var sortedList = rankUserList.OrderByDescending(i => i.GetComponent<RankingUserManager>().UserPoint).ToList();

            int rankNum = 0;
            int comparisonPoint = -1;
            for (int i = 0; i < sortedList.Count; i++)
            {
                if (int.Parse(sortedList[i].GetComponent<RankingUserManager>().UserPoint) != comparisonPoint)
                {
                    rankNum++;
                }
                sortedList[i].transform.SetSiblingIndex(i);
                RankingUserManager rankingUserManagerCS = sortedList[i].GetComponent<RankingUserManager>();
                rankingUserManagerCS.SetRankingNum(rankNum.ToString());
                comparisonPoint = int.Parse(sortedList[i].GetComponent<RankingUserManager>().UserPoint);
            }
        }

    }
    #endregion
    #region �|�C���g����
    protected void UserPointResult(LoginManager loginData, int stateNum)
    {
        if (loginData.User_Type == GuestUser)
        {
            return;
        }
        else
        {
            int resultPoint = 0;
            if (stateNum == Win)
            {
                resultPoint = int.Parse(loginData.Point) + 2;
                StartCoroutine(AccountPointResultProcess(loginData.User_name, resultPoint.ToString()));
            }
            else
            {
                resultPoint = int.Parse(loginData.Point) - 1;
                if (resultPoint <= 0)
                {
                    resultPoint = 0;
                }
                StartCoroutine(AccountPointResultProcess(loginData.User_name, resultPoint.ToString()));
            }
        }
    }

    protected virtual IEnumerator AccountPointResultProcess(string loginUser_name, string point)
    {
        // POST���M�p�̃t�H�[�����쐬
        WWWForm postData = new WWWForm();
        postData.AddField("user_name", loginUser_name);
        postData.AddField("user_point", point);
        // POST�Ńf�[�^���M
        using UnityWebRequest request = UnityWebRequest.Post(accountPointUpdateURL, postData);
        request.timeout = 10;
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            print(request.error);
        }
        #endregion
    }
}
