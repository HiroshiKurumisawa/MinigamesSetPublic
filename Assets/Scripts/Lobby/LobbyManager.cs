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
    #region 変数群
    const int Reversi = 0, Gomoku = 1;

    // ユーザー名表示
    [SerializeField] GameObject userNameText;
    // ルーム作成関係
    [SerializeField] GameObject createRoomForm;
    bool isOpenCreateRoomForm = false;                                           // ルーム作成フォームが開いているか

    string createRoom_name = "";                                                 // 作成ルーム名
    string createRoom_password = "";                                             // ログインパスワード
    [Header("ルーム作成関係")]
    [SerializeField] GameObject massage_CreateRoomText;                          // メッセージテキスト(ルーム作成)
    [SerializeField] GameObject[] ruleTextObjs;                                  // ルールを表示するテキストオブジェクト
    [SerializeField] TMP_InputField room_nameField_CreateRoom;                   // ルーム名入力フィールド(ルーム作成)
    [SerializeField] TMP_InputField passwordField_CreateRoom;                    // パスワード入力フィールド(ルーム作成)
    [SerializeField] TextMeshProUGUI gameRuleNameText;                           // ゲームモード名テキスト
    int createRoomInputSelected = 0;
    int gameRuleNum = 0;                                                         // ゲームモード切り替え用
    // ルーム参加関係
    bool isOpenSelectRoomForm = false;                                                  // 参加用のフォームが開いているか
    bool updateSelectForm = false;                                                      // 参加選択画面が更新中か
    bool isOpenInputRoomPasswordForm = false;                                           // 参加パスワード入力画面が表示されているか
    bool isEntryRoomInPass = false;                                                     // パスワード入力画面の参加のボタンを押したか
    bool isUpdateSelectFormWait = false;
    float waitTimeValue;                                                                // 更新時間の値
    const float waitTime = 10f;                                                         // 更新時間
    string inputRoomPasswordFormRoomName = "";                                          // パスワード入力画面の参加しようとしているルーム名表記用
    string inputRoomPassword = "";                                                      // 参加しようとしているルームのパスワード(入力されたもの)
    [Header("ルーム参加関係")]
    [SerializeField] public GameObject roomsSelectForm;                                 // ルーム選択フォーム
    [SerializeField] GameObject roomUIprefab;                                           // ルームUIのPrefab
    [SerializeField] GameObject roomScrollView;                                         // 作成されたルームが表示されるScrollView;
    [SerializeField] GameObject inputRoomPasswordForm;                                  // 参加ルームのパスワード入力画面
    [SerializeField] GameObject inputRoomPasswordRoomNameText;                          // パスワード入力画面のルーム名
    [SerializeField] GameObject inputRoomPasswordMessageText;                           // パスワード入力画面のメッセージテキスト
    [SerializeField] GameObject waitTimeMessage;                                        // 最更新可能までのメッセージテキスト
    [SerializeField] GameObject updateRoomButton;                                       // ルーム更新ボタン
    [SerializeField] TMP_InputField passwordField_EntryRoom;                            // パスワード入力フィールド(ルーム参加)
    // ルーム待機関係
    bool updateRoomForm = false;                                                        // ルーム情報更新フラグ
    bool gameStart = false;                                                             // ゲーム開始ボタンを押したか
    bool mainSceneChange = false;
    bool isHostReady = false;                                                           // ホスト準備完了か
    bool isEntryRedy = false;                                                           // 参加者が準備完了か
    bool isReady = false;                                                               // 準備完了ボタンを押したか
    bool leaveRoom = false;                                                             // 退出ボタンを押したか

    [Header("ルーム待機関係")]
    [SerializeField] GameObject[] entryUsersUI;                                         // 待機中のユーザー表記
    [SerializeField] GameObject[] userRadyIcon;                                         // 準備完了の表記
    [SerializeField] GameObject roomForm;                                               // 待機中のルームフォーム
    [SerializeField] GameObject roomNameTextUI;                                         // ルーム名テキスト
    [SerializeField] GameObject roomKeyRockIcon;                                        // ルームの鍵アイコン(不必要になる可能性あり)
    [SerializeField] GameObject roomPasswordText;                                       // ルームのパスワード表示
    [SerializeField] GameObject message_RoomText;                                       // ルームのメッセージテキスト
    [SerializeField] GameObject gameStartButton;                                        // ゲーム開始ボタン
    [SerializeField] TextMeshProUGUI gameRuleText;                                      // ゲームルール表示
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
    // ルーム名select
    public void SelectRoomNameCreateRoom() => createRoomInputSelected = 1;
    // ルームパスワードinput
    public void InputPasswordCreateRoom()
    {
        createRoom_password = passwordField_CreateRoom.text;
    }
    // ルームパスワードselect
    public void SelecttPasswordCreateRoom() => createRoomInputSelected = 0;
    // Tabで切り替え
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
    // ルーム作成フォーム表示
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
    // ルーム作成フォーム非表示
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

    // ゲームもどの切り替え
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
                gameRuleNameText.text = "オセロ";
                break;
            case Gomoku:
                ruleTextObjs[Gomoku].SetActive(true);
                gameRuleNameText.text = "五目並べ";
                break;
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
            UpdateRoomSelectFormWaitTimeReset();
            RoomClear();
            UIopen(roomsSelectForm, UpdateRoomSelectFormProcess(roomUIprefab, roomScrollView));
        }
    }
    // ルーム選択フォーム非表示
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
    // ルームパスワード入力画面
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
    // パスワードinput
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

    // ルーム情報更新
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
        // 更新前のルーム情報を削除
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
                waitTimeMessage.GetComponent<TextMeshProUGUI>().text = "再更新可能まで\n" + waitTimeValue.ToString("00");
            }
        }
    }
    void UpdateRoomSelectFormWaitTimeReset() // ルーム検索更新待機状態をリセットする
    {
        updateSelectForm = false;
        isUpdateSelectFormWait = false;
        waitTimeMessage.GetComponent<TextMeshProUGUI>().text = "";
        updateRoomButton.GetComponent<Image>().color = new Color(0f, 0.5f, 1f, 1f);
    }
    // ポインター処理
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
    #region ルーム関係
    // ルーム画面表示
    public void OpenRoomForm(string roomName, string roomPass, string hostUser, string EntryUser, bool isReadyHost, bool isReadyEntry, bool gameStart,string game_rule)
    {
        roomForm.SetActive(true);
        roomNameTextUI.GetComponent<TextMeshProUGUI>().text = roomName;
        if (roomPass != "") { roomPasswordText.GetComponent<TextMeshProUGUI>().text = roomPass; }
        else { roomPasswordText.GetComponent<TextMeshProUGUI>().text = "なし"; }
        entryUsersUI[0].GetComponent<TextMeshProUGUI>().text = hostUser;
        entryUsersUI[1].GetComponent<TextMeshProUGUI>().text = EntryUser;
        switch(int.Parse(game_rule))
        {
            case Reversi:
                gameRuleText.text = "オセロ";
                break;
            case Gomoku:
                gameRuleText.text = "五目並べ";
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

    // 準備完了
    public void Ready()
    {
        if (!isReady)
        {
            isReady = true;
            StartCoroutine(ReadyProcess(roomNameTextUI, x => isReady = x));
        }
    }

    // ゲーム開始
    public void GameStart()
    {
        if (!gameStart && isHostReady && isEntryRedy)
        {
            gameStart = true;
            StartCoroutine(GameStartProcess(roomNameTextUI, x => gameStart = x));
        }
    }

    // 退出
    public void RoomLeave()
    {
        if (!leaveRoom)
        {
            leaveRoom = true;
            StartCoroutine(RoomLeaveProcess(roomNameTextUI, roomForm, x => isOpenSelectRoomForm = x, x => leaveRoom = x));
        }
    }

    // ルーム情報を最新にし続ける
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
