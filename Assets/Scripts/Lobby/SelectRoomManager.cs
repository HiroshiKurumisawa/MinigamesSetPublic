using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using JsonClass;

public class SelectRoomManager : NetworkBaseManager
{

    LobbyManager lobbyManagerCS;

    [SerializeField] GameObject entryButton;
    [SerializeField] GameObject lockImage;
    [SerializeField] TextMeshProUGUI roomNameText;
    [SerializeField] TextMeshProUGUI roomUserCountText;


    string roomName;
    string roomPass;
    string roomUserEntryNumString;
    string roomUserMaxNumString;
    int roomUserEntryNumInt;
    int roomUserMaxNumInt;
    bool isRoomEntry = false;

    private void Start()
    {
        loginManagerCS = GameObject.FindObjectOfType<LoginManager>();
        lobbyManagerCS = GameObject.FindObjectOfType<LobbyManager>();
    }

    private void FixedUpdate()
    {
        if (roomUserEntryNumInt < roomUserMaxNumInt)
        {
            entryButton.SetActive(true);
        }
        else
        {
            entryButton.SetActive(false);
        }

        if (roomPass != "") // 鍵のイメージを表示するか否か
        {
            lockImage.SetActive(true);
        }
        else
        {
            lockImage.SetActive(false);
        }
    }

    public void SetRoomData(string name, string pass, string entryNum, string maxNum)
    {
        roomName = name;
        roomPass = pass;
        roomUserEntryNumString = entryNum;
        roomUserMaxNumString = maxNum;
        roomUserEntryNumInt = Convert.ToInt32(entryNum);
        roomUserMaxNumInt = Convert.ToInt32(maxNum);

        roomNameText.text = roomName;
        roomUserCountText.text = roomUserEntryNumString + "/" + roomUserMaxNumString;
    }

    // ルーム参加
    public void EntryRoom()
    {
        if (!isRoomEntry && this.roomPass != "" && roomUserEntryNumInt < roomUserMaxNumInt) // パスワードがあるときはパスワードフォームを開く
        {
            lobbyManagerCS.OpenInputRoomPasswordForm(this.roomName);
        }
        else if (!isRoomEntry && this.roomPass == "" && roomUserEntryNumInt < roomUserMaxNumInt)
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
                lobbyManagerCS.OpenRoomForm(resData.roomData.room_name, resData.roomData.room_password, resData.roomData.user_host, resData.roomData.user_entry, resData.roomData.ready_status_host, resData.roomData.ready_status_entry, resData.roomData.game_status, resData.roomData.game_rule);
            }
            else if (resData.requestMessage == 2)                   // エラー2が返ってきたとき
            {
                lobbyManagerCS.UpdateTotalRooms();
            }
            else
            {
                print("error");
            }
        }
        isRoomEntry = false;
    }
}
