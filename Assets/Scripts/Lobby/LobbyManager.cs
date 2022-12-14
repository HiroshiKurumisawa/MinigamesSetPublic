using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class LobbyManager : MonoBehaviour
{
    #region 変数群
    LoginManager loginManagerCS;

    // ユーザー名表示
    [SerializeField] GameObject userNameText;
    // ルーム作成関係
    [SerializeField] GameObject createRoomForm;
    bool isOpenCreateRoomForm = false;                                           // ルーム作成フォームが開いているか
    const string roomCreateURL = "http://localhost/room/create";                 // ルーム作成URL
    string createRoom_name = "";                                                 // 作成ルーム名
    string createRoom_password = "";                                             // ログインパスワード
    [Header("ルーム作成関係")]
    [SerializeField] GameObject massage_CreateRoomText;                          // メッセージテキスト(ルーム作成)
    [SerializeField] TMP_InputField room_nameField_CreateRoom;                   // ルーム名入力フィールド(ルーム作成)
    [SerializeField] TMP_InputField passwordField_CreateRoom;                    // パスワード入力フィールド(ルーム作成)
    // ルーム参加関係
    bool isOpenSelectRoomForm = false;
    bool updateSelectForm = false;
    bool isOpenInputRoomPasswordForm = false;
    bool isEntryRoomInPass = false;
    const string entryRoomURL = "http://localhost/room/entry";
    const string updateSelectFormURL = "http://localhost/room/select_form_update";
    string inputRoomPasswordFormRoomName = "";
    string inputRoomPassword = "";
    [Header("ルーム参加関係")]
    [SerializeField] public GameObject roomsSelectForm;
    [SerializeField] GameObject roomUIprefab;
    [SerializeField] GameObject roomScrollView;
    [SerializeField] GameObject inputRoomPasswordForm;
    [SerializeField] GameObject inputRoomPasswordRoomNameText;
    [SerializeField] GameObject inputRoomPasswordMessageText;
    [SerializeField] TMP_InputField passwordField_EntryRoom;                    // パスワード入力フィールド(ルーム参加)
    // ルーム関係
    bool updateRoomForm = false;
    const string updateRoomFormURL = "http://localhost/room/room_form_update";
    [Header("ルーム関係")]
    [SerializeField] GameObject[] entryUsersUI;
    [SerializeField] GameObject[] userRadyIcon;
    [SerializeField] GameObject roomForm;
    [SerializeField] GameObject roomNameTextUI;
    [SerializeField] GameObject roomKeyRockIcon;
    [SerializeField] GameObject roomPasswordText;
    [SerializeField] GameObject message_RoomText;
    #endregion

    void Start()
    {
        inputRoomPasswordMessageText.GetComponent<TextMeshProUGUI>().text = "";
        massage_CreateRoomText.GetComponent<TextMeshProUGUI>().text = "";
        message_RoomText.GetComponent<TextMeshProUGUI>().text = "";
        entryUsersUI[0].GetComponent<TextMeshProUGUI>().text = "";
        entryUsersUI[1].GetComponent<TextMeshProUGUI>().text = "";
        roomForm.SetActive(false);
        createRoomForm.SetActive(false);
        roomsSelectForm.SetActive(false);
        loginManagerCS = GameObject.FindObjectOfType<LoginManager>();
        DisplayUserName();
    }

    void Update()
    {

    }

    private void FixedUpdate()
    {
        RoomKeepUpdate();
    }

    // ユーザーネーム表示
    void DisplayUserName()
    {
        if (loginManagerCS != null) { userNameText.GetComponent<TextMeshProUGUI>().text = loginManagerCS.User_name; }
        else { userNameText.GetComponent<TextMeshProUGUI>().text = "No Data"; }
    }

    #region ルーム作成関係
    // ルーム名input
    public void InputRoomNameCreateRoom()
    {
        createRoom_name = room_nameField_CreateRoom.text;
    }
    // パスワードinput
    public void InputPasswordCreateRoom()
    {
        createRoom_password = passwordField_CreateRoom.text;
    }
    // ルーム作成フォーム表示
    public void OpenCreateRoomForm()
    {
        if (!isOpenCreateRoomForm)
        {
            isOpenCreateRoomForm = true;
            createRoomForm.SetActive(true);
        }
    }
    // ルーム作成フォーム非表示
    public void CloseCreateRoomForm()
    {
        if (isOpenCreateRoomForm)
        {
            isOpenCreateRoomForm = false;
            massage_CreateRoomText.GetComponent<TextMeshProUGUI>().text = "";
            createRoomForm.SetActive(false);
        }
    }
    public void CreateRoom()
    {
        if (isOpenCreateRoomForm)
        {
            isOpenCreateRoomForm = false;
            StartCoroutine(CreateRoomProcess());
        }
    }
    IEnumerator CreateRoomProcess()
    {
        // POST送信用のフォームを作成
        WWWForm postData = new WWWForm();
        postData.AddField("room_name", createRoom_name);
        postData.AddField("room_password", createRoom_password);
        postData.AddField("room_max_users", "2");
        postData.AddField("room_host_user", loginManagerCS.User_name);

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
                OpenRoomForm(resData.roomData.room_name, resData.roomData.room_password, resData.roomData.user_host, resData.roomData.user_entry);
            }
            else if (resData.requestMessage == 1)                   // エラーが返ってきたとき
            {
                isOpenCreateRoomForm = true;
                massage_CreateRoomText.GetComponent<TextMeshProUGUI>().color = new Color(255, 0, 0);
                massage_CreateRoomText.GetComponent<TextMeshProUGUI>().text = "作成失敗(入力内容が不適切です)";
            }
        }

    }
    #endregion
    #region ルーム参加関係
    // ルーム選択画面用
    // ルーム選択フォーム表示
    public void OpenSelectRoomForm()
    {
        if (!isOpenSelectRoomForm)
        {
            isOpenSelectRoomForm = true;
            roomsSelectForm.SetActive(true);
            StartCoroutine(UpdateRoomSelectFormProcess());
        }
    }
    // ルーム選択フォーム非表示
    public void CloseSelectRoomForm()
    {
        if (isOpenSelectRoomForm)
        {
            isOpenSelectRoomForm = false;
            roomsSelectForm.SetActive(false);
        }
    }
    // ルームパスワード入力画面
    public void OpenInputRoomPasswordForm(string roomName)
    {
        if (!isOpenInputRoomPasswordForm)
        {
            isOpenInputRoomPasswordForm = true;
            inputRoomPasswordFormRoomName = roomName;
            inputRoomPasswordForm.SetActive(true);
            inputRoomPasswordRoomNameText.GetComponent<TextMeshProUGUI>().text = inputRoomPasswordFormRoomName;
        }
    }
    public void CloseInputRoomPasswordForm()
    {
        if (isOpenInputRoomPasswordForm)
        {
            isOpenInputRoomPasswordForm = false;
            inputRoomPasswordForm.SetActive(false);
        }
    }
    // パスワードinput
    public void InputPasswordEntryRoom()
    {
        inputRoomPassword = passwordField_EntryRoom.text;
    }
    public void EntryRoomInPass()
    {
        if(!isEntryRoomInPass)
        {
            isEntryRoomInPass = true;
            StartCoroutine(EntryRoomInPassProcess());
        }
    }
    IEnumerator EntryRoomInPassProcess()
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
                OpenRoomForm(resData.roomData.room_name, resData.roomData.room_password, resData.roomData.user_host, resData.roomData.user_entry);
            }
            else if (resData.requestMessage == 2)                   // エラー2が返ってきたとき
            {
                inputRoomPasswordMessageText.GetComponent<TextMeshProUGUI>().text = "パスワードが違います";
            }
            else
            {
                print("error");
            }
        }
    }

    // ルーム情報更新
    public void UpdateTotalRooms()
    {
        if (!updateSelectForm)
        {
            updateSelectForm = true;
            StartCoroutine(UpdateRoomSelectFormProcess());
        }
    }
    IEnumerator UpdateRoomSelectFormProcess()
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
            updateSelectForm = false;
        }

    }
    #endregion
    #region ルーム関係
    // ルーム画面表示
    public void OpenRoomForm(string roomName, string roomPass, string hostUser, string EntryUser)
    {
        roomForm.SetActive(true);
        roomNameTextUI.GetComponent<TextMeshProUGUI>().text = roomName;
        if (roomPass != "") { roomPasswordText.GetComponent<TextMeshProUGUI>().text = roomPass; }
        else { roomPasswordText.GetComponent<TextMeshProUGUI>().text = "なし"; }
        entryUsersUI[0].GetComponent<TextMeshProUGUI>().text = hostUser;
        entryUsersUI[1].GetComponent<TextMeshProUGUI>().text = EntryUser;
    }
    public void RoomLeave()
    {

    }
    // ルーム情報を最新にし続ける
    private void RoomKeepUpdate()
    {
        if (roomForm.activeSelf == true && !updateRoomForm)
        {
            updateRoomForm = true;
            StartCoroutine(RoomKeepUpdateProcess());
        }
    }
    IEnumerator RoomKeepUpdateProcess()
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
            RefreshRoom resData = JsonUtility.FromJson<RefreshRoom>(request.downloadHandler.text);
            OpenRoomForm(resData.roomData.room_name, resData.roomData.room_password, resData.roomData.user_host, resData.roomData.user_entry);
            updateRoomForm = false;
        }
    }
    #endregion

    #region JSON変換クラス
    // ルーム作成作成結果JSON変換クラス
    [Serializable]
    public class RoomData
    {
        public int id;
        public string room_name;
        public string room_password;
        public string max_room_users;
        public string in_room_users;
        public string created_at;
        public string updated_at;
        public string user_host;
        public string user_entry;
    }
    [Serializable]
    public class RoomCreateRoot
    {
        public int result;
        public int requestMessage;
        public RoomData roomData;
    }

    // ルーム選択画面
    [Serializable]
    public class AllRoomRoot
    {
        public int result;
        public List<RoomData> allRoomList = new List<RoomData>();
    }

    // ルーム表示中
    [Serializable]
    public class RefreshRoom
    {
        public int result;
        public RoomData roomData;
    }

    // ルーム参加(パスワードがある場合)
    [Serializable]
    public class EntryRoomDataInPass
    {
        public int id;
        public string room_name;
        public string room_password;
        public string max_room_users;
        public string in_room_users;
        public string created_at;
        public string updated_at;
        public string user_host;
        public string user_entry;
    }
    // ルーム参加
    [Serializable]
    public class EntryRoomInpassRoot
    {
        public int result;
        public int requestMessage;
        public EntryRoomDataInPass roomData;
    }
    #endregion
}
