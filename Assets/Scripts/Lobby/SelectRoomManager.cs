using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class SelectRoomManager : MonoBehaviour
{
    LoginManager loginManagerCS;
    LobbyManager lobbyManagerCS;

    [SerializeField] TextMeshProUGUI roomNameText;
    [SerializeField] TextMeshProUGUI roomUserCountText;

    const string entryRoomURL = "http://localhost/room/entry";

    string roomName;
    string roomPass;
    string roomUserEntryNum;
    string roomUserMaxNum;
    bool isRoomEntry = false;

    private void Start()
    {
        loginManagerCS = GameObject.FindObjectOfType<LoginManager>();
        lobbyManagerCS = GameObject.FindObjectOfType<LobbyManager>();
    }

    public void SetRoomData(string name, string pass, string entryNum, string maxNum)
    {
        roomName = name;
        roomPass = pass;
        roomUserEntryNum = entryNum;
        roomUserMaxNum = maxNum;

        roomNameText.text = roomName;
        roomUserCountText.text = roomUserEntryNum + "/" + roomUserMaxNum;
    }

    // ルーム参加
    public void EntryRoom()
    {
        if (!isRoomEntry && this.roomPass != "")
        {

        }
        else
        {
            isRoomEntry = true;
            StartCoroutine(EntryRoomProcess());
        }
    }
    IEnumerator EntryRoomProcess()
    {
        // POST送信用のフォームを作成
        WWWForm postData = new WWWForm();
        postData.AddField("room_name", this.roomName);
        postData.AddField("room_password", this.roomPass);
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
            EntryRoomRoot resData = JsonUtility.FromJson<EntryRoomRoot>(request.downloadHandler.text);
            
            if (resData.requestMessage == 0) // 成功したとき
            {
                lobbyManagerCS.roomsSelectForm.SetActive(false);
                lobbyManagerCS.OpenRoomForm(resData.roomData.room_name, resData.roomData.room_password, resData.roomData.user_host, resData.roomData.user_entry);
            }
            else if (resData.requestMessage == 1)                   // エラーが返ってきたとき
            {
                print("error");
            }

        }
        isRoomEntry = false;
    }
}

[Serializable]
public class EntryRoomData
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
public class EntryRoomRoot
{
    public int result;
    public int requestMessage;
    public EntryRoomData roomData;
}