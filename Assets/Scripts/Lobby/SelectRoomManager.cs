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

    [SerializeField] GameObject entryButton;
    [SerializeField] GameObject lockImage;
    [SerializeField] TextMeshProUGUI roomNameText;
    [SerializeField] TextMeshProUGUI roomUserCountText;

    const string entryRoomURL = "http://54.168.79.41/room/entry";

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

        if (roomPass != "") // ���̃C���[�W��\�����邩�ۂ�
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

    // ���[���Q��
    public void EntryRoom()
    {
        if (!isRoomEntry && this.roomPass != "" && roomUserEntryNumInt < roomUserMaxNumInt) // �p�X���[�h������Ƃ��̓p�X���[�h�t�H�[�����J��
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
        // POST���M�p�̃t�H�[�����쐬
        WWWForm postData = new WWWForm();
        postData.AddField("room_name", this.roomName);
        postData.AddField("room_password", this.roomPass);
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
            EntryRoomRoot resData = JsonUtility.FromJson<EntryRoomRoot>(request.downloadHandler.text);

            if (resData.requestMessage == 0) // ���������Ƃ�
            {
                lobbyManagerCS.roomsSelectForm.SetActive(false);
                lobbyManagerCS.OpenRoomForm(resData.roomData.room_name, resData.roomData.room_password, resData.roomData.user_host, resData.roomData.user_entry, resData.roomData.ready_status_host, resData.roomData.ready_status_entry, resData.roomData.game_status);
            }
            else if (resData.requestMessage == 2)                   // �G���[2���Ԃ��Ă����Ƃ�
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
    public bool ready_status_host;
    public bool ready_status_entry;
    public bool game_status;
}
// ���[���Q��
[Serializable]
public class EntryRoomRoot
{
    public int result;
    public int requestMessage;
    public EntryRoomData roomData;
}