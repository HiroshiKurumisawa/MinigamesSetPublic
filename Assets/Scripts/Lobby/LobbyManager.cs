using System;
using System.Collections;
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
    string createRoom_maxUsers = "";                                             // 参加できる最大ユーザー数
    [Header("ルーム作成関係")]
    [SerializeField] GameObject massage_CreateRoomText;                          // メッセージテキスト(ルーム作成)
    [SerializeField] TMP_InputField room_nameField_CreateRoom;                   // ルーム名入力フィールド(ルーム作成)
    [SerializeField] TMP_InputField passwordField_CreateRoom;                    // パスワード入力フィールド(ルーム作成)
    // ルーム関係
    int entryRoomUsers;
    [Header("ルーム関係")]
    [SerializeField] GameObject[] entryUsersUI;
    [SerializeField] GameObject[] userRadyIcon;
    [SerializeField] GameObject roomForm;
    [SerializeField] GameObject roomName;
    [SerializeField] GameObject roomKeyRockIcon;
    [SerializeField] GameObject roomPasswordText;
    [SerializeField] GameObject message_RoomText;
    #endregion

    void Start()
    {
        massage_CreateRoomText.GetComponent<TextMeshProUGUI>().text = "";
        message_RoomText.GetComponent<TextMeshProUGUI>().text = "";
        entryUsersUI[0].GetComponent<TextMeshProUGUI>().text = "";
        entryUsersUI[1].GetComponent<TextMeshProUGUI>().text = "";
        roomForm.SetActive(false);
        createRoomForm.SetActive(false);
        loginManagerCS = GameObject.FindObjectOfType<LoginManager>();
        DisplayUserName();
    }

    void Update()
    {

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

            if (resData.requestMessage == 0)
            {
                room_nameField_CreateRoom.text = "";
                passwordField_CreateRoom.text = "";
                massage_CreateRoomText.GetComponent<TextMeshProUGUI>().text = "";
                createRoomForm.SetActive(false);
                roomForm.SetActive(true);
                roomName.GetComponent<TextMeshProUGUI>().text = resData.roomData.room_name;
                if (resData.roomData.room_password != "") { roomPasswordText.GetComponent<TextMeshProUGUI>().text = resData.roomData.room_password; }
                else { roomPasswordText.GetComponent<TextMeshProUGUI>().text = "なし"; }
                entryUsersUI[0].GetComponent<TextMeshProUGUI>().text = loginManagerCS.User_name;

            }
            else if (resData.requestMessage == 1)
            {
                isOpenCreateRoomForm = true;
                massage_CreateRoomText.GetComponent<TextMeshProUGUI>().color = new Color(255, 0, 0);
                massage_CreateRoomText.GetComponent<TextMeshProUGUI>().text = "作成失敗(入力内容が不適切です)";
            }
        }

    }
    #endregion
    #region ルーム関係
    public void RoomLeave()
    {

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
    }
    [Serializable]
    public class RoomCreateRoot
    {
        public int result;
        public int requestMessage;
        public RoomData roomData;
    }
    #endregion
}
