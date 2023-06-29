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
    protected const string accountLoginURL = "http://localhost/user/account/login";                 // アカウントログインURL
    protected const string logOutURL = "http://localhost/user/logout";                              // ログアウトURL
    protected const string guestLoginURL = "http://localhost/user/guest/login";                     // ゲストログインURL
    protected const string createAccountURL = "http://localhost/user/account/create";               // アカウント作成URL
    protected const string entryRoomURL = "http://localhost/room/entry";                            // ルーム参加URL
    protected const string updateSelectFormURL = "http://localhost/room/select_form_update";        // ルーム選択画面更新URL
    protected const string roomCreateURL = "http://localhost/room/create";                          // ルーム作成URL
    protected const string readyURL = "http://localhost/room/ready_user_room";                      // 準備完了変更URL
    protected const string leaveRoomURL = "http://localhost/room/leave_room";                       // 退出用URL
    protected const string updateRoomFormURL = "http://localhost/room/room_form_update";            // ルーム情報更新用URL
    protected const string gameStartURL = "http://localhost/room/gamestart_room";                   // ゲーム開始URL  
    // リバーシ
    protected const string reversiUpdateGameURL = "http://localhost/game/update_game";
    protected const string reversiPutStoneURL = "http://localhost/game/putStone_game";
    protected const string reversiSurrenderURL = "http://localhost/game/surrender_game";
    //
    // 五目並べ
    protected const string gomokuUpdateGameURL = "http://localhost/game/update_game";
    protected const string gomokuPutStoneURL = "http://localhost/game/putStone_game";
    protected const string gomokuSurrenderURL = "http://localhost/game/surrender_game";
    //
    protected const string endGameURL = "http://localhost/game/end_game";                           //対戦終了URL
    protected const string rankingUpdateURL = "http://localhost/ranking/ranking_view";              // ランキング情報更新
    protected const string accountPointUpdateURL = "http://localhost/ranking/ranking_point_update";
    #endregion
    #region ServerURL
    //protected const string accountLoginURL = "http://ik1-423-43506.vs.sakura.ne.jp/user/account/login";               // アカウントログインURL
    //protected const string logOutURL = "http://ik1-423-43506.vs.sakura.ne.jp/user/logout";                            // ログアウトURL
    //protected const string guestLoginURL = "http://ik1-423-43506.vs.sakura.ne.jp/user/guest/login";                   // ゲストログインURL
    //protected const string createAccountURL = "http://ik1-423-43506.vs.sakura.ne.jp/user/account/create";             // アカウント作成URL
    //protected const string updateSelectFormURL = "http://ik1-423-43506.vs.sakura.ne.jp/room/select_form_update";      // ルーム選択画面更新URL
    //protected const string entryRoomURL = "http://ik1-423-43506.vs.sakura.ne.jp/room/entry";                          // ルーム参加URL
    //protected const string roomCreateURL = "http://ik1-423-43506.vs.sakura.ne.jp/room/create";                        // ルーム作成URL
    //protected const string readyURL = "http://ik1-423-43506.vs.sakura.ne.jp/room/ready_user_room";                    // 準備完了変更URL
    //protected const string leaveRoomURL = "http://ik1-423-43506.vs.sakura.ne.jp/room/leave_room";                     // 退出用URL
    //protected const string updateRoomFormURL = "http://ik1-423-43506.vs.sakura.ne.jp/room/room_form_update";          // ルーム情報更新用URL
    //protected const string gameStartURL = "http://ik1-423-43506.vs.sakura.ne.jp/room/gamestart_room";                 // ゲーム開始URL
    // リバーシ
    //protected const string reversiUpdateGameURL = "http://ik1-423-43506.vs.sakura.ne.jp/game/update_game";
    //protected const string reversiPutStoneURL = "http://ik1-423-43506.vs.sakura.ne.jp/game/putStone_game";
    //protected const string reversiSurrenderURL = "http://ik1-423-43506.vs.sakura.ne.jp/game/surrender_game";
    //
    // 五目並べ
    //protected const string gomokuUpdateGameURL = "http://ik1-423-43506.vs.sakura.ne.jp/game/update_game";
    //protected const string gomokuPutStoneURL = "http://ik1-423-43506.vs.sakura.ne.jp/game/putStone_game";
    //protected const string gomokuSurrenderURL = "http://ik1-423-43506.vs.sakura.ne.jp/game/surrender_game";
    //
    //protected const string endGameURL = "http://ik1-423-43506.vs.sakura.ne.jp/game/end_game";                         // 対戦終了URL
    //protected const string rankingUpdateURL = "http://ik1-423-43506.vs.sakura.ne.jp/ranking/ranking_view";
    //protected const string accountPointUpdate = "http://ik1-423-43506.vs.sakura.ne.jp/ranking/ranking_point_update";
    #endregion


    #region 変数群
    // 必要なデータ保持するためのスクリプト
    protected LoginManager loginManagerCS;
    protected RoomDataManager roomDataManagerCS;
    const int AccountUser = 0, GuestUser = 1;
    protected const int Win = 0, Lose = 1;
    #endregion
    #region ログイン関係
    // ゲストログイン
    protected IEnumerator GuestLoginProcess()
    {
        // POST送信用のフォームを作成
        WWWForm postData = new WWWForm();

        // POSTでデータ送信
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
    // アカウントログイン
    protected virtual IEnumerator AccountLoginProcess(string loginUser_name, string loginUser_password, TextMeshProUGUI messageLogin, Action<bool> callback)
    {
        // POST送信用のフォームを作成
        WWWForm postData = new WWWForm();
        postData.AddField("user_name", loginUser_name);
        postData.AddField("password", loginUser_password);
        // POSTでデータ送信
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
                if (resData.requestMessage == 1)           // エラーメッセージだった時(入力内容エラー)
                {
                    messageLogin.color = new Color(255, 0, 0);
                    messageLogin.text = "ログイン失敗(入力内容に誤りがあります)";
                    callback(false);
                }
                else if (resData.requestMessage == 2)   // パスワードが違う場合状態
                {
                    messageLogin.color = new Color(255, 0, 0);
                    messageLogin.text = "ログイン失敗(パスワードに誤りがあります)";
                    callback(false);
                }
                else if (resData.requestMessage == 3)   // すでにログインされている状態
                {
                    messageLogin.color = new Color(255, 0, 0);
                    messageLogin.text = "ログイン失敗(既にログインしています)";
                    callback(false);
                }
                else if (resData.requestMessage == 4)   // アカウントがない場合
                {
                    messageLogin.color = new Color(255, 0, 0);
                    messageLogin.text = "ログイン失敗(入力されたアカウントが存在しません)";
                    callback(false);
                }
                else                                    // ログインできるとき
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
    // アカウント作成
    protected IEnumerator CreateAccountProcess(string createUser_name, string createUser_password, string createUser_rePassword,
        TMP_InputField user_nameField, TMP_InputField passwordField, TMP_InputField rePasswordField, GameObject massageText, Action<bool> callback)
    {
        // POST送信用のフォームを作成
        WWWForm postData = new WWWForm();
        postData.AddField("user_name", createUser_name);
        postData.AddField("password", createUser_password);
        postData.AddField("repassword", createUser_rePassword);

        // POSTでデータ送信
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

            if (resData.requestMessage == 0)    // 成功
            {
                user_nameField.text = "";
                passwordField.text = "";
                rePasswordField.text = "";
                massageText.GetComponent<TextMeshProUGUI>().color = new Color(0, 0, 255);
                massageText.GetComponent<TextMeshProUGUI>().text = "作成成功";
                yield return new WaitForSeconds(0.8f);
                TitleManager titleManager = GameObject.FindObjectOfType<TitleManager>();
                titleManager.CloseCrateFormUI();
            }
            else if (resData.requestMessage == 1)   // ユーザーが既に登録されている
            {
                massageText.GetComponent<TextMeshProUGUI>().color = new Color(255, 0, 0);
                massageText.GetComponent<TextMeshProUGUI>().text = "作成失敗\n(ユーザー名が入力されていません)";
            }
            else if (resData.requestMessage == 2)   // ユーザー名に禁止ワードが入っている
            {
                massageText.GetComponent<TextMeshProUGUI>().color = new Color(255, 0, 0);
                massageText.GetComponent<TextMeshProUGUI>().text = "作成失敗\n(同じユーザー名が既に登録されています)";
            }
            else if (resData.requestMessage == 3)   // ユーザー名が入力されていない
            {
                massageText.GetComponent<TextMeshProUGUI>().color = new Color(255, 0, 0);
                massageText.GetComponent<TextMeshProUGUI>().text = "作成失敗\n(ユーザー名に\"ゲスト\"が含まれています)";
            }
            else if (resData.requestMessage == 4)   // パスワードに不備がある
            {
                massageText.GetComponent<TextMeshProUGUI>().color = new Color(255, 0, 0);
                massageText.GetComponent<TextMeshProUGUI>().text = "作成失敗\n(パスワードが正しく入力されていません)";
            }
            else if (resData.requestMessage == 5)
            {
                massageText.GetComponent<TextMeshProUGUI>().color = new Color(255, 0, 0);
                massageText.GetComponent<TextMeshProUGUI>().text = "作成失敗\n(ユーザー名とパスワード両方に不備があります)";
            }
        }
        callback(false);
    }
    // ログアウト
    protected IEnumerator LogOutProcess()
    {
        // POST送信用のフォームを作成
        WWWForm postData = new WWWForm();
        postData.AddField("manageID", loginManagerCS.Manage_id);
        postData.AddField("userName", loginManagerCS.User_name);
        postData.AddField("userType", loginManagerCS.User_Type);

        // POSTでデータ送信
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
    #region ルーム関係
    // ルーム作成
    protected IEnumerator CreateRoomProcess(string createRoom_name, string createRoom_password, string createRoom_gameRule, TMP_InputField room_nameField_CreateRoom,
        TMP_InputField passwordField_CreateRoom, GameObject massage_CreateRoomText, GameObject createRoomForm, Action<bool> isOpenCreateRoomForm)
    {
        // POST送信用のフォームを作成
        WWWForm postData = new WWWForm();
        postData.AddField("room_name", createRoom_name);
        postData.AddField("room_password", createRoom_password);
        postData.AddField("room_max_users", "2");
        postData.AddField("room_host_user", loginManagerCS.User_name);
        postData.AddField("room_game_rule", createRoom_gameRule);

        // POSTでデータ送信
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

            if (resData.requestMessage == 0) // 成功したとき
            {
                room_nameField_CreateRoom.text = "";
                passwordField_CreateRoom.text = "";
                massage_CreateRoomText.GetComponent<TextMeshProUGUI>().text = "";
                createRoomForm.SetActive(false);
                LobbyManager lobbyManager = FindObjectOfType<LobbyManager>();
                lobbyManager.OpenRoomForm(resData.roomData.room_name, resData.roomData.room_password, resData.roomData.user_host,
                    resData.roomData.user_entry, resData.roomData.ready_status_host, resData.roomData.ready_status_entry, resData.roomData.game_status, resData.roomData.game_rule);
            }
            else if (resData.requestMessage == 1)                   // エラーが返ってきたとき
            {
                isOpenCreateRoomForm(true);
                massage_CreateRoomText.GetComponent<TextMeshProUGUI>().color = new Color(255, 0, 0);
                massage_CreateRoomText.GetComponent<TextMeshProUGUI>().text = "作成失敗(入力内容が不適切です)";
            }
        }

    }

    // ルームエントリー
    protected IEnumerator EntryRoomInPassProcess(string inputRoomPasswordFormRoomName, string inputRoomPassword, TMP_InputField passwordField_EntryRoom,
        GameObject roomsSelectForm, GameObject inputRoomPasswordMessageText, GameObject inputRoomPasswordForm, Action<bool> isOpenInputRoomPasswordForm, Action<bool> isEntryRoomInPassRetrun, bool isEntryRoomInPass)
    {
        // POST送信用のフォームを作成
        WWWForm postData = new WWWForm();
        postData.AddField("room_name", inputRoomPasswordFormRoomName);
        postData.AddField("room_password", inputRoomPassword);
        postData.AddField("room_entry_user", loginManagerCS.User_name);

        // POSTでデータ送信
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

            if (resData.requestMessage == 0) // 成功したとき
            {
                passwordField_EntryRoom.text = "";
                roomsSelectForm.SetActive(false);
                LobbyManager lobbyManager = FindObjectOfType<LobbyManager>();
                lobbyManager.OpenRoomForm(resData.roomData.room_name, resData.roomData.room_password, resData.roomData.user_host, resData.roomData.user_entry, resData.roomData.ready_status_host, resData.roomData.ready_status_entry, resData.roomData.game_status, resData.roomData.game_rule);
                isOpenInputRoomPasswordForm(false);
                inputRoomPasswordMessageText.GetComponent<TextMeshProUGUI>().text = "";
                inputRoomPasswordForm.SetActive(false);
            }
            else if (resData.requestMessage == 2)                   // エラー2が返ってきたとき
            {
                inputRoomPasswordMessageText.GetComponent<TextMeshProUGUI>().text = "パスワードが違います";
            }
            else
            {
                print("error");
            }

            isEntryRoomInPass = isEntryRoomInPass == true ? false : true;
            isEntryRoomInPassRetrun(isEntryRoomInPass);
        }
    }

    // ルームの総数更新
    protected IEnumerator UpdateRoomSelectFormProcess(GameObject roomUIprefab, GameObject roomScrollView)
    {
        // 更新前のルーム情報を削除
        GameObject[] rooms = GameObject.FindGameObjectsWithTag("Room");
        foreach (GameObject room in rooms)
        {
            Destroy(room);
        }

        // POST送信用のフォームを作成
        WWWForm postData = new WWWForm();

        // POSTでデータ送信
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

    // 準備完了
    protected IEnumerator ReadyProcess(GameObject roomNameTextUI, Action<bool> isReady)
    {
        // POST送信用のフォームを作成
        WWWForm postData = new WWWForm();
        postData.AddField("room_name", roomNameTextUI.GetComponent<TextMeshProUGUI>().text);
        postData.AddField("ready_user", loginManagerCS.User_name);

        // POSTでデータ送信
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

    // ゲーム開始
    protected IEnumerator GameStartProcess(GameObject roomNameTextUI, Action<bool> gameStart)
    {
        // POST送信用のフォームを作成
        WWWForm postData = new WWWForm();
        postData.AddField("room_name", roomNameTextUI.GetComponent<TextMeshProUGUI>().text);

        // POSTでデータ送信
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

    // ルーム退出
    protected IEnumerator RoomLeaveProcess(GameObject roomNameTextUI, GameObject roomForm, Action<bool> isOpenSelectRoomForm, Action<bool> leaveRoom)
    {
        // POST送信用のフォームを作成
        WWWForm postData = new WWWForm();
        postData.AddField("room_name", roomNameTextUI.GetComponent<TextMeshProUGUI>().text);
        postData.AddField("room_leave_user", loginManagerCS.User_name);

        // POSTでデータ送信
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

    // 参加中のルーム更新
    protected IEnumerator RoomKeepUpdateProcess(GameObject roomNameTextUI, GameObject roomForm, Action<bool> updateRoomForm)
    {
        // POST送信用のフォームを作成
        WWWForm postData = new WWWForm();
        postData.AddField("room_name", roomNameTextUI.GetComponent<TextMeshProUGUI>().text);

        // POSTでデータ送信
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
    #region ランキング
    // ルームの総数更新
    protected IEnumerator UpdateRankingFormProcess(GameObject acountUIprefab, GameObject rankigScrollView)
    {
        // 更新前のルーム情報を削除
        GameObject[] rankings = GameObject.FindGameObjectsWithTag("Ranking");
        foreach (GameObject room in rankings)
        {
            Destroy(room);
        }

        // POST送信用のフォームを作成
        WWWForm postData = new WWWForm();

        // POSTでデータ送信
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
    #region ポイント増減
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
        // POST送信用のフォームを作成
        WWWForm postData = new WWWForm();
        postData.AddField("user_name", loginUser_name);
        postData.AddField("user_point", point);
        // POSTでデータ送信
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
