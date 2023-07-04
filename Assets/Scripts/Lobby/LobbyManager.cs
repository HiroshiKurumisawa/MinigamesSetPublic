using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using SoundSystem;

public class LobbyManager : NetworkBaseManager
{
    #region �ϐ��Q
    const int Reversi = 0, Gomoku = 1;

    // ���[�U�[���\��
    [SerializeField] GameObject userNameText;
    // ���[���쐬�֌W
    [SerializeField] GameObject createRoomForm;
    bool isOpenCreateRoomForm = false;                                           // ���[���쐬�t�H�[�����J���Ă��邩

    string createRoom_name = "";                                                 // �쐬���[����
    string createRoom_password = "";                                             // ���O�C���p�X���[�h
    [Header("���[���쐬�֌W")]
    [SerializeField] GameObject massage_CreateRoomText;                          // ���b�Z�[�W�e�L�X�g(���[���쐬)
    [SerializeField] GameObject[] ruleTextObjs;                                  // ���[����\������e�L�X�g�I�u�W�F�N�g
    [SerializeField] TMP_InputField room_nameField_CreateRoom;                   // ���[�������̓t�B�[���h(���[���쐬)
    [SerializeField] TMP_InputField passwordField_CreateRoom;                    // �p�X���[�h���̓t�B�[���h(���[���쐬)
    [SerializeField] TextMeshProUGUI gameRuleNameText;                           // �Q�[�����[�h���e�L�X�g
    int createRoomInputSelected = 0;
    int gameRuleNum = 0;                                                         // �Q�[�����[�h�؂�ւ��p
    // ���[���Q���֌W
    bool isOpenSelectRoomForm = false;                                                  // �Q���p�̃t�H�[�����J���Ă��邩
    bool updateSelectForm = false;                                                      // �Q���I����ʂ��X�V����
    bool isOpenInputRoomPasswordForm = false;                                           // �Q���p�X���[�h���͉�ʂ��\������Ă��邩
    bool isEntryRoomInPass = false;                                                     // �p�X���[�h���͉�ʂ̎Q���̃{�^������������
    bool isUpdateSelectFormWait = false;
    float waitTimeValue;                                                                // �X�V���Ԃ̒l
    const float waitTime = 10f;                                                         // �X�V����
    string inputRoomPasswordFormRoomName = "";                                          // �p�X���[�h���͉�ʂ̎Q�����悤�Ƃ��Ă��郋�[�����\�L�p
    string inputRoomPassword = "";                                                      // �Q�����悤�Ƃ��Ă��郋�[���̃p�X���[�h(���͂��ꂽ����)
    [Header("���[���Q���֌W")]
    [SerializeField] public GameObject roomsSelectForm;                                 // ���[���I���t�H�[��
    [SerializeField] GameObject roomUIprefab;                                           // ���[��UI��Prefab
    [SerializeField] GameObject roomScrollView;                                         // �쐬���ꂽ���[�����\�������ScrollView;
    [SerializeField] GameObject inputRoomPasswordForm;                                  // �Q�����[���̃p�X���[�h���͉��
    [SerializeField] GameObject inputRoomPasswordRoomNameText;                          // �p�X���[�h���͉�ʂ̃��[����
    [SerializeField] GameObject inputRoomPasswordMessageText;                           // �p�X���[�h���͉�ʂ̃��b�Z�[�W�e�L�X�g
    [SerializeField] GameObject waitTimeMessage;                                        // �ōX�V�\�܂ł̃��b�Z�[�W�e�L�X�g
    [SerializeField] GameObject updateRoomButton;                                       // ���[���X�V�{�^��
    [SerializeField] TMP_InputField passwordField_EntryRoom;                            // �p�X���[�h���̓t�B�[���h(���[���Q��)
    // ���[���ҋ@�֌W
    bool updateRoomForm = false;                                                        // ���[�����X�V�t���O
    bool gameStart = false;                                                             // �Q�[���J�n�{�^������������
    bool mainSceneChange = false;
    bool isHostReady = false;                                                           // �z�X�g����������
    bool isEntryRedy = false;                                                           // �Q���҂�����������
    bool isReady = false;                                                               // ���������{�^������������
    bool leaveRoom = false;                                                             // �ޏo�{�^������������

    [Header("���[���ҋ@�֌W")]
    [SerializeField] GameObject[] entryUsersUI;                                         // �ҋ@���̃��[�U�[�\�L
    [SerializeField] GameObject[] userRadyIcon;                                         // ���������̕\�L
    [SerializeField] GameObject roomForm;                                               // �ҋ@���̃��[���t�H�[��
    [SerializeField] GameObject roomNameTextUI;                                         // ���[�����e�L�X�g
    [SerializeField] GameObject roomKeyRockIcon;                                        // ���[���̌��A�C�R��(�s�K�v�ɂȂ�\������)
    [SerializeField] GameObject roomPasswordText;                                       // ���[���̃p�X���[�h�\��
    [SerializeField] GameObject message_RoomText;                                       // ���[���̃��b�Z�[�W�e�L�X�g
    [SerializeField] GameObject gameStartButton;                                        // �Q�[���J�n�{�^��
    [SerializeField] TextMeshProUGUI gameRuleText;                                      // �Q�[�����[���\��
    #endregion

    void Start()
    {
        SoundManager.Instance.PlayBGMWithFadeIn("Title_Lobby", 1f);
        inputRoomPasswordMessageText.GetComponent<TextMeshProUGUI>().text = "";
        massage_CreateRoomText.GetComponent<TextMeshProUGUI>().text = "";
        message_RoomText.GetComponent<TextMeshProUGUI>().text = "";
        entryUsersUI[0].GetComponent<TextMeshProUGUI>().text = "";
        entryUsersUI[1].GetComponent<TextMeshProUGUI>().text = "";
        waitTimeMessage.GetComponent<TextMeshProUGUI>().text = "";
        roomForm.SetActive(false);
        createRoomForm.SetActive(false);
        roomsSelectForm.SetActive(false);
        loginManagerCS = GameObject.FindObjectOfType<LoginManager>();
        roomDataManagerCS = GameObject.FindObjectOfType<RoomDataManager>();
        DisplayUserName();
    }

    void Update()
    {
        InputSelectCreateRoom();
    }

    private void FixedUpdate()
    {
        RoomKeepUpdate();
        UpdateRoomSelectFormWaitTime(waitTime);
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
    // ���[����select
    public void SelectRoomNameCreateRoom() => createRoomInputSelected = 1;
    // ���[���p�X���[�hinput
    public void InputPasswordCreateRoom()
    {
        createRoom_password = passwordField_CreateRoom.text;
    }
    // ���[���p�X���[�hselect
    public void SelecttPasswordCreateRoom() => createRoomInputSelected = 0;
    // Tab�Ő؂�ւ�
    private void InputSelectCreateRoom()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            createRoomInputSelected++;
            if (createRoomInputSelected > 1) { createRoomInputSelected = 0; }

            switch (createRoomInputSelected)
            {
                case 0:
                    passwordField_CreateRoom.Select();
                    break;
                case 1:
                    room_nameField_CreateRoom.Select();
                    break;
            }
        }
    }
    // ���[���쐬�t�H�[���\��
    public void OpenCreateRoomForm()
    {
        if (!isOpenCreateRoomForm)
        {
            isOpenCreateRoomForm = true;
            gameRuleNum = 0;
            UIopen(createRoomForm, NoActionCol());
            RuleView(gameRuleNum);
        }
    }
    // ���[���쐬�t�H�[����\��
    public void CloseCreateRoomForm()
    {
        if (isOpenCreateRoomForm)
        {
            isOpenCreateRoomForm = false;
            massage_CreateRoomText.GetComponent<TextMeshProUGUI>().text = "";
            UIclose(createRoomForm);
        }
    }
    public void CreateRoom()
    {
        if (isOpenCreateRoomForm)
        {
            isOpenCreateRoomForm = false;
            StartCoroutine(CreateRoomProcess(createRoom_name, createRoom_password, gameRuleNum.ToString(), room_nameField_CreateRoom,
        passwordField_CreateRoom, massage_CreateRoomText, createRoomForm, x => isOpenCreateRoomForm = x));
        }
    }

    // �Q�[�����ǂ̐؂�ւ�
    public void RuleChangeUPcount()
    {
        gameRuleNum++;
        if (gameRuleNum > 1)
        {
            gameRuleNum = 0;
        }
        RuleView(gameRuleNum);
    }
    public void RuleChangeDowncount()
    {
        gameRuleNum--;
        if (gameRuleNum < 0)
        {
            gameRuleNum = 1;
        }

        RuleView(gameRuleNum);
    }

    private void RuleView(int ruleNum)
    {
        ruleTextObjs[Reversi].SetActive(false);
        ruleTextObjs[Gomoku].SetActive(false);
        switch (ruleNum)
        {
            case Reversi:
                ruleTextObjs[Reversi].SetActive(true);
                gameRuleNameText.text = "�I�Z��";
                break;
            case Gomoku:
                ruleTextObjs[Gomoku].SetActive(true);
                gameRuleNameText.text = "�ܖڕ���";
                break;
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
            UpdateRoomSelectFormWaitTimeReset();
            RoomClear();
            UIopen(roomsSelectForm, UpdateRoomSelectFormProcess(roomUIprefab, roomScrollView));
        }
    }
    // ���[���I���t�H�[����\��
    public void CloseSelectRoomForm()
    {
        if (isOpenSelectRoomForm)
        {
            isOpenSelectRoomForm = false;
            updateSelectForm = false;
            isUpdateSelectFormWait = false;
            updateRoomButton.GetComponent<Image>().color = new Color(0f, 0.5f, 1f, 1f);
            waitTimeMessage.GetComponent<TextMeshProUGUI>().text = "";
            RoomClear();
            UIclose(roomsSelectForm);
        }
    }
    // ���[���p�X���[�h���͉��
    public void OpenInputRoomPasswordForm(string roomName)
    {
        if (!isOpenInputRoomPasswordForm)
        {
            isOpenInputRoomPasswordForm = true;
            inputRoomPasswordFormRoomName = roomName;
            UIopen(inputRoomPasswordForm, NoActionCol());
            inputRoomPasswordRoomNameText.GetComponent<TextMeshProUGUI>().text = inputRoomPasswordFormRoomName;
        }
    }
    public void CloseInputRoomPasswordForm()
    {
        if (isOpenInputRoomPasswordForm)
        {
            isOpenInputRoomPasswordForm = false;
            passwordField_EntryRoom.text = "";
            inputRoomPasswordMessageText.GetComponent<TextMeshProUGUI>().text = "";
            UIclose(inputRoomPasswordForm);
            UpdateRoomSelectFormWaitTimeReset();
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
            StartCoroutine(EntryRoomInPassProcess(inputRoomPasswordFormRoomName, inputRoomPassword, passwordField_EntryRoom,
        roomsSelectForm, inputRoomPasswordMessageText, inputRoomPasswordForm, x => isOpenInputRoomPasswordForm = x, x => isEntryRoomInPass = x, isEntryRoomInPass));
        }
    }

    // ���[�����X�V
    public void UpdateTotalRooms()
    {
        if (!updateSelectForm)
        {
            updateSelectForm = true;
            SoundManager.Instance.PlayOneShotSe("ui_click");
            RoomClear();
            StartCoroutine(UpdateRoomSelectFormProcess(roomUIprefab, roomScrollView));
        }
    }

    void RoomClear()
    {
        // �X�V�O�̃��[�������폜
        GameObject[] rooms = GameObject.FindGameObjectsWithTag("Room");
        foreach (GameObject room in rooms)
        {
            Destroy(room);
        }
    }

    void UpdateRoomSelectFormWaitTime(float time)
    {
        if (!updateSelectForm) { return; }
        else
        {
            updateRoomButton.GetComponent<Image>().color = new Color(0f, 0.5f, 1f, 0.2f);
            if (!isUpdateSelectFormWait)
            {
                isUpdateSelectFormWait = true;
                waitTimeValue = time;
            }

            if (waitTimeValue <= 0)
            {
                UpdateRoomSelectFormWaitTimeReset();
            }
            else
            {
                waitTimeValue -= Time.deltaTime;
                waitTimeMessage.GetComponent<TextMeshProUGUI>().text = "�čX�V�\�܂�\n" + waitTimeValue.ToString("00");
            }
        }
    }
    void UpdateRoomSelectFormWaitTimeReset() // ���[�������X�V�ҋ@��Ԃ����Z�b�g����
    {
        updateSelectForm = false;
        isUpdateSelectFormWait = false;
        waitTimeMessage.GetComponent<TextMeshProUGUI>().text = "";
        updateRoomButton.GetComponent<Image>().color = new Color(0f, 0.5f, 1f, 1f);
    }
    // �|�C���^�[����
    public void UpdateRoomButtonPointerEnter()
    {
        if (!updateSelectForm)
        {
            SoundManager.Instance.PlayOneShotSe("ui_enter");
            updateRoomButton.GetComponent<Image>().color = new Color(1f, 0.5f, 0f, 1f);
        }
        else { return; }
    }
    public void UpdateRoomButtonPointerExit()
    {
        if (!updateSelectForm)
        {
            updateRoomButton.GetComponent<Image>().color = new Color(0f, 0.5f, 1f, 1f);
        }
        else { return; }
    }
    #endregion
    #region ���[���֌W
    // ���[����ʕ\��
    public void OpenRoomForm(string roomName, string roomPass, string hostUser, string EntryUser, bool isReadyHost, bool isReadyEntry, bool gameStart,string game_rule)
    {
        roomForm.SetActive(true);
        roomNameTextUI.GetComponent<TextMeshProUGUI>().text = roomName;
        if (roomPass != "") { roomPasswordText.GetComponent<TextMeshProUGUI>().text = roomPass; }
        else { roomPasswordText.GetComponent<TextMeshProUGUI>().text = "�Ȃ�"; }
        entryUsersUI[0].GetComponent<TextMeshProUGUI>().text = hostUser;
        entryUsersUI[1].GetComponent<TextMeshProUGUI>().text = EntryUser;
        switch(int.Parse(game_rule))
        {
            case Reversi:
                gameRuleText.text = "�I�Z��";
                break;
            case Gomoku:
                gameRuleText.text = "�ܖڕ���";
                break;
        }
        isHostReady = isReadyHost;
        isEntryRedy = isReadyEntry;
        if (isHostReady) { userRadyIcon[0].SetActive(true); }
        else { userRadyIcon[0].SetActive(false); }
        if (isEntryRedy) { userRadyIcon[1].SetActive(true); }
        else { userRadyIcon[1].SetActive(false); }
        if (loginManagerCS.User_name == hostUser) { gameStartButton.SetActive(true); }
        else { gameStartButton.SetActive(false); }
        if (gameStart && !mainSceneChange)
        {
            mainSceneChange = true;
            SoundManager.Instance.StopBGMWithFadeOut(1f);
            FadeManager.Instance.LoadScene("Main", 0.5f);
        }
    }

    // ��������
    public void Ready()
    {
        if (!isReady)
        {
            isReady = true;
            StartCoroutine(ReadyProcess(roomNameTextUI, x => isReady = x));
        }
    }

    // �Q�[���J�n
    public void GameStart()
    {
        if (!gameStart && isHostReady && isEntryRedy)
        {
            gameStart = true;
            StartCoroutine(GameStartProcess(roomNameTextUI, x => gameStart = x));
        }
    }

    // �ޏo
    public void RoomLeave()
    {
        if (!leaveRoom)
        {
            leaveRoom = true;
            StartCoroutine(RoomLeaveProcess(roomNameTextUI, roomForm, x => isOpenSelectRoomForm = x, x => leaveRoom = x));
        }
    }

    // ���[�������ŐV�ɂ�������
    private void RoomKeepUpdate()
    {
        if (roomForm.activeSelf == true && !updateRoomForm)
        {
            updateRoomForm = true;
            StartCoroutine(RoomKeepUpdateProcess(roomNameTextUI, roomForm, x => updateRoomForm = x));
        }
    }
    #endregion
}
