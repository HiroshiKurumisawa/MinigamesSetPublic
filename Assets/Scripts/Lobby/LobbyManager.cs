using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; // �V�[���J�ڗp(�t�F�[�h�}�l�[�W���[�쐬���폜)

public class LobbyManager : MonoBehaviour
{
    #region �ϐ��Q
    // �K�v�ȃf�[�^�ێ����邽�߂̃X�N���v�g
    LoginManager loginManagerCS;
    RoomDataManager roomDataManagerCS;

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
    bool isOpenSelectRoomForm = false;                                                  // �Q���p�̃t�H�[�����J���Ă��邩
    bool updateSelectForm = false;                                                      // �Q���I����ʂ��X�V����
    bool isOpenInputRoomPasswordForm = false;                                           // �Q���p�X���[�h���͉�ʂ��\������Ă��邩
    bool isEntryRoomInPass = false;                                                     // �p�X���[�h���͉�ʂ̎Q���̃{�^������������
    const string entryRoomURL = "http://localhost/room/entry";                          // ���[���Q��URL
    const string updateSelectFormURL = "http://localhost/room/select_form_update";      // ���[���I����ʍX�VURL
    string inputRoomPasswordFormRoomName = "";                                          // �p�X���[�h���͉�ʂ̎Q�����悤�Ƃ��Ă��郋�[�����\�L�p
    string inputRoomPassword = "";                                                      // �Q�����悤�Ƃ��Ă��郋�[���̃p�X���[�h(���͂��ꂽ����)
    [Header("���[���Q���֌W")]
    [SerializeField] public GameObject roomsSelectForm;                                 // ���[���I���t�H�[��
    [SerializeField] GameObject roomUIprefab;                                           // ���[��UI��Prefab
    [SerializeField] GameObject roomScrollView;                                         // �쐬���ꂽ���[�����\�������ScrollView;
    [SerializeField] GameObject inputRoomPasswordForm;                                  // �Q�����[���̃p�X���[�h���͉��
    [SerializeField] GameObject inputRoomPasswordRoomNameText;                          // �p�X���[�h���͉�ʂ̃��[����
    [SerializeField] GameObject inputRoomPasswordMessageText;                           // �p�X���[�h���͉�ʂ̃��b�Z�[�W�e�L�X�g
    [SerializeField] TMP_InputField passwordField_EntryRoom;                            // �p�X���[�h���̓t�B�[���h(���[���Q��)
    // ���[���ҋ@�֌W
    bool updateRoomForm = false;                                                        // ���[�����X�V�t���O
    bool gameStart = false;                                                             // �Q�[���J�n�{�^������������
    bool isHostReady = false;                                                           // �z�X�g����������
    bool isEntryRedy = false;                                                           // �Q���҂�����������
    bool isReady = false;                                                               // ���������{�^������������
    bool leaveRoom = false;                                                             // �ޏo�{�^������������
    const string readyURL = "http://localhost/room/ready_user_room";                    // ���������ύXURL
    const string leaveRoomURL = "http://localhost/room/leave_room";                     // �ޏo�pURL
    const string updateRoomFormURL = "http://localhost/room/room_form_update";          // ���[�����X�V�pURL
    const string gameStartURL = "http://localhost/room/gamestart_room";                 // �Q�[���J�nURL
    [Header("���[���ҋ@�֌W")]
    [SerializeField] GameObject[] entryUsersUI;                                         // �ҋ@���̃��[�U�[�\�L
    [SerializeField] GameObject[] userRadyIcon;                                         // ���������̕\�L
    [SerializeField] GameObject roomForm;                                               // �ҋ@���̃��[���t�H�[��
    [SerializeField] GameObject roomNameTextUI;                                         // ���[�����e�L�X�g
    [SerializeField] GameObject roomKeyRockIcon;                                        // ���[���̌��A�C�R��(�s�K�v�ɂȂ�\������)
    [SerializeField] GameObject roomPasswordText;                                       // ���[���̃p�X���[�h�\��
    [SerializeField] GameObject message_RoomText;                                       // ���[���̃��b�Z�[�W�e�L�X�g
    [SerializeField] GameObject gameStartButton;                                        // �Q�[���J�n�{�^��
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
        roomDataManagerCS = GameObject.FindObjectOfType<RoomDataManager>();
        DisplayUserName();
    }

    void Update()
    {

    }

    private void FixedUpdate()
    {
        RoomKeepUpdate();
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
                OpenRoomForm(resData.roomData.room_name, resData.roomData.room_password, resData.roomData.user_host, resData.roomData.user_entry, resData.roomData.ready_status_host, resData.roomData.ready_status_entry, resData.roomData.game_status);
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
    // ���[���p�X���[�h���͉��
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
    // �p�X���[�hinput
    public void InputPasswordEntryRoom()
    {
        inputRoomPassword = passwordField_EntryRoom.text;
    }
    public void EntryRoomInPass()
    {
        if (!isEntryRoomInPass)
        {
            isEntryRoomInPass = true;
            StartCoroutine(EntryRoomInPassProcess());
        }
    }
    IEnumerator EntryRoomInPassProcess()
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
                OpenRoomForm(resData.roomData.room_name, resData.roomData.room_password, resData.roomData.user_host, resData.roomData.user_entry, resData.roomData.ready_status_host, resData.roomData.ready_status_entry, resData.roomData.game_status);
            }
            else if (resData.requestMessage == 2)                   // �G���[2���Ԃ��Ă����Ƃ�
            {
                inputRoomPasswordMessageText.GetComponent<TextMeshProUGUI>().text = "�p�X���[�h���Ⴂ�܂�";
            }
            else
            {
                print("error");
            }
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
                GameObject roomUIclone = Instantiate(roomUIprefab, transform.position, Quaternion.identity, roomScrollView.transform);
                roomUIclone.name = "Room_" + i.ToString();
                SelectRoomManager selectRoomManagerCS = roomUIclone.GetComponent<SelectRoomManager>();
                selectRoomManagerCS.SetRoomData(resData.allRoomList[i].room_name, resData.allRoomList[i].room_password, resData.allRoomList[i].in_room_users, resData.allRoomList[i].max_room_users);
            }
            updateSelectForm = false;
        }

    }
    #endregion
    #region ���[���֌W
    // ���[����ʕ\��
    public void OpenRoomForm(string roomName, string roomPass, string hostUser, string EntryUser, bool isReadyHost, bool isReadyEntry, bool gameStart)
    {
        roomForm.SetActive(true);
        roomNameTextUI.GetComponent<TextMeshProUGUI>().text = roomName;
        if (roomPass != "") { roomPasswordText.GetComponent<TextMeshProUGUI>().text = roomPass; }
        else { roomPasswordText.GetComponent<TextMeshProUGUI>().text = "�Ȃ�"; }
        entryUsersUI[0].GetComponent<TextMeshProUGUI>().text = hostUser;
        entryUsersUI[1].GetComponent<TextMeshProUGUI>().text = EntryUser;
        isHostReady = isReadyHost;
        isEntryRedy = isReadyEntry;
        if (isHostReady) { userRadyIcon[0].SetActive(true); }
        else { userRadyIcon[0].SetActive(false); }
        if (isEntryRedy) { userRadyIcon[1].SetActive(true); }
        else { userRadyIcon[1].SetActive(false); }
        if (loginManagerCS.User_name == hostUser) { gameStartButton.SetActive(true); }
        else { gameStartButton.SetActive(false); }
        if (gameStart)
        {

            SceneManager.LoadScene("Main"); /*�V�[���J��*/
        }
    }

    // ��������
    public void Ready()
    {
        if (!isReady)
        {
            isReady = true;
            StartCoroutine(ReadyProcess());
        }
    }
    IEnumerator ReadyProcess()
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
            isReady = false;
        }
    }
    // �Q�[���J�n
    public void GameStart()
    {
        if (!gameStart && isHostReady && isEntryRedy)
        {
            gameStart = true;
            StartCoroutine(GameStartProcess());
        }
    }
    IEnumerator GameStartProcess()
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
            gameStart = false;
        }
    }
    // �ޏo
    public void RoomLeave()
    {
        if (!leaveRoom)
        {
            leaveRoom = true;
            StartCoroutine(RoomLeaveProcess());
        }
    }
    IEnumerator RoomLeaveProcess()
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
                isOpenSelectRoomForm = false;
            }
            leaveRoom = false;
        }
    }
    // ���[�������ŐV�ɂ�������
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
                OpenRoomForm(resData.roomData.room_name, resData.roomData.room_password, resData.roomData.user_host, resData.roomData.user_entry, resData.roomData.ready_status_host, resData.roomData.ready_status_entry, resData.roomData.game_status);
                roomDataManagerCS.SetRoomData(resData.roomData.room_name, resData.roomData.user_host, resData.roomData.user_entry);
            }
            updateRoomForm = false;
        }
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
        public bool ready_status_host;
        public bool ready_status_entry;
        public bool game_status;
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

    // ���[���\����
    [Serializable]
    public class UpdateRoom
    {
        public int result;
        public RoomData roomData;
    }

    // ���[���Q��(�p�X���[�h������ꍇ)
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
        public bool ready_status_host;
        public bool ready_status_entry;
        public bool game_status;
    }
    // ���[���Q��
    [Serializable]
    public class EntryRoomInpassRoot
    {
        public int result;
        public int requestMessage;
        public EntryRoomDataInPass roomData;
    }

    // ���[���ޏo
    [Serializable]
    public class LeaveRoom
    {
        public int result;
    }
    #endregion
}
