using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class LobbyManager : MonoBehaviour
{
    #region �ϐ��Q
    LoginManager loginManagerCS;

    // ���[�U�[���\��
    [SerializeField] GameObject userNameText;
    // ���[���쐬�֌W
    [SerializeField] GameObject createRoomForm;
    bool isOpenCreateRoomForm = false;                                           // ���[���쐬�t�H�[�����J���Ă��邩
    const string roomCreateURL = "http://localhost/room/create";                 // ���[���쐬URL
    string createRoom_name = "";                                                 // �쐬���[����
    string createRoom_password = "";                                             // ���O�C���p�X���[�h
    [Header("���[���쐬�֌W")]
    [SerializeField] GameObject massage_CreateRoomText;                          // ���b�Z�[�W�e�L�X�g(���[���쐬)
    [SerializeField] TMP_InputField room_nameField_CreateRoom;                   // ���[�������̓t�B�[���h(���[���쐬)
    [SerializeField] TMP_InputField passwordField_CreateRoom;                    // �p�X���[�h���̓t�B�[���h(���[���쐬)
    // ���[���Q���֌W
    bool isOpenSelectRoomForm = false;
    bool updateSelectForm = false;
    const string updateSelectFormURL = "http://localhost/room/select_form_update";
    [Header("���[���Q���֌W")]
    [SerializeField] GameObject roomsSelectForm;
    [SerializeField] GameObject roomUIprefab;
    // ���[���֌W
    int entryRoomUsers;
    [Header("���[���֌W")]
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
        roomsSelectForm.SetActive(false);
        loginManagerCS = GameObject.FindObjectOfType<LoginManager>();
        DisplayUserName();
    }

    void Update()
    {

    }

    // ���[�U�[�l�[���\��
    void DisplayUserName()
    {
        if (loginManagerCS != null) { userNameText.GetComponent<TextMeshProUGUI>().text = loginManagerCS.User_name; }
        else { userNameText.GetComponent<TextMeshProUGUI>().text = "No Data"; }
    }

    #region ���[���쐬�֌W
    // ���[����input
    public void InputRoomNameCreateRoom()
    {
        createRoom_name = room_nameField_CreateRoom.text;
    }
    // �p�X���[�hinput
    public void InputPasswordCreateRoom()
    {
        createRoom_password = passwordField_CreateRoom.text;
    }
    // ���[���쐬�t�H�[���\��
    public void OpenCreateRoomForm()
    {
        if (!isOpenCreateRoomForm)
        {
            isOpenCreateRoomForm = true;
            createRoomForm.SetActive(true);
        }
    }
    // ���[���쐬�t�H�[����\��
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
        // POST���M�p�̃t�H�[�����쐬
        WWWForm postData = new WWWForm();
        postData.AddField("room_name", createRoom_name);
        postData.AddField("room_password", createRoom_password);
        postData.AddField("room_max_users", "2");
        postData.AddField("room_host_user", loginManagerCS.User_name);

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
                // ���[����ʕ\��
                roomForm.SetActive(true);
                roomName.GetComponent<TextMeshProUGUI>().text = resData.roomData.room_name;
                if (resData.roomData.room_password != "") { roomPasswordText.GetComponent<TextMeshProUGUI>().text = resData.roomData.room_password; }
                else { roomPasswordText.GetComponent<TextMeshProUGUI>().text = "�Ȃ�"; }
                entryUsersUI[0].GetComponent<TextMeshProUGUI>().text = resData.roomData.user_host;
                //---------------------------------

            }
            else if (resData.requestMessage == 1)                   // �G���[���Ԃ��Ă����Ƃ�
            {
                isOpenCreateRoomForm = true;
                massage_CreateRoomText.GetComponent<TextMeshProUGUI>().color = new Color(255, 0, 0);
                massage_CreateRoomText.GetComponent<TextMeshProUGUI>().text = "�쐬���s(���͓��e���s�K�؂ł�)";
            }
        }

    }
    #endregion
    #region ���[���Q���֌W
    // ���[���I����ʗp
    // ���[���I���t�H�[���\��
    public void OpenSelectRoomForm()
    {
        if (!isOpenSelectRoomForm)
        {
            isOpenSelectRoomForm = true;
            roomsSelectForm.SetActive(true);
            StartCoroutine(UpdateRoomSelectFormProcess());
        }
    }
    // ���[���I���t�H�[����\��
    public void CloseSelectRoomForm()
    {
        if (isOpenSelectRoomForm)
        {
            isOpenSelectRoomForm = false;
            roomsSelectForm.SetActive(false);
        }
    }
    // ���[�����X�V
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
                GameObject roomUIclone = Instantiate(roomUIprefab, transform.position, Quaternion.identity, roomsSelectForm.transform);
                roomUIclone.name = "Room_" + i.ToString();
                SelectRoomManager selectRoomManagerCS = roomUIclone.GetComponent<SelectRoomManager>();
                selectRoomManagerCS.SetRoomData(resData.allRoomList[i].room_name, resData.allRoomList[i].in_room_users, resData.allRoomList[i].max_room_users);
            }
            updateSelectForm = false;
        }

    }
    #endregion
    #region ���[���֌W
    public void RoomLeave()
    {

    }
    #endregion

    #region JSON�ϊ��N���X
    // ���[���쐬�쐬����JSON�ϊ��N���X
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

    // ���[���I�����
    [Serializable]
    public class AllRoomRoot
    {
        public int result;
        public List<RoomData> allRoomList = new List<RoomData>();
    }
    #endregion
}
