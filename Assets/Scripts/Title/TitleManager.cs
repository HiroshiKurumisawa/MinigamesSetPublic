using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using SoundSystem;

public class TitleManager : NetworkBaseManager
{
    #region 変数群
    // アカウントログイン
    bool isAccountLogin = false;                                            // アカウントログインフラグ
    string loginUser_name = "";                                             // ログインユーザー名
    string loginUser_password = "";                                         // ログインパスワード
    [Header("ユーザーログイン関係")]
    [SerializeField] TextMeshProUGUI massage_LoginText;                          // メッセージテキスト(ログイン)
    [SerializeField] TMP_InputField user_nameField_Login;                   // ユーザーネーム入力フィールド(ログイン)
    [SerializeField] TMP_InputField passwordField_Login;                    // パスワード入力フィールド(ログイン)
    int loginInputSelected = 0;
    // ゲストログイン
    bool isGuestLogin = false;                                              // ゲストログインフラグ

    // ユーザー作成
    bool openCreateForm = false;                                            // ユーザー作成フォームが開いているか
    bool isCreateAccont = false;                                            // 作成ボタンを押したかどうか
    public bool IsCreateAccount { get { return isCreateAccont; } }
    string createUser_name = "";                                            // アカウント作成ユーザ名
    string createUser_password = "";                                        // アカウント作成パスワード
    string createUser_rePassword = "";                                      // アカウント作成パスワード再入力
    [Header("ユーザー作成関係")]
    [SerializeField] GameObject createFormUI;                               // ユーザー作成フォーム
    [SerializeField] GameObject massageText;                                // メッセージテキスト
    [SerializeField] TMP_InputField user_nameField;                         // ユーザーネーム入力フィールド
    [SerializeField] TMP_InputField passwordField;                          // パスワード入力フィールド
    [SerializeField] TMP_InputField rePasswordField;                        // パスワード再入力フィールド
    int createInputSelected = 0;

    [Header("ゲーム終了関係")]
    [SerializeField] GameObject quitGameForm;
    bool isOpenQuitGameForm = false;
    #endregion

    private void Awake()
    {
        massage_LoginText.text = "";
        massageText.GetComponent<TextMeshProUGUI>().text = "";
        createFormUI.SetActive(false);
    }

    void Start()
    {
        loginManagerCS = GameObject.FindObjectOfType<LoginManager>();
        SoundManager.Instance.PlayBGMWithFadeIn("Title_Lobby", 1f);
    }

    private void Update()
    {
        InputSelectLogin();
        InputSelectCreateUser();
    }

    #region アカウントログイン関係
    // ユーザー名input
    public void InputUserNameLogin()
    {
        loginUser_name = user_nameField_Login.text;
    }
    // ユーザー名select
    public void SelectUserNameLogin() => loginInputSelected = 1;
    // パスワードinput
    public void InputPasswordLogin()
    {
        loginUser_password = passwordField_Login.text;
    }
    // パスワードselect
    public void SelectPasswordLogin() => loginInputSelected = 0;
    // Tabで切り替え
    private void InputSelectLogin()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            loginInputSelected++;
            if (loginInputSelected > 1) { loginInputSelected = 0; }

            switch (loginInputSelected)
            {
                case 0:
                    passwordField_Login.Select();
                    break;
                case 1:
                    user_nameField_Login.Select();
                    break;
            }
        }
    }
    // ログインボタンが押されたとき(AccountLogin用UIのEventTriggerのPointerClickに使う)
    public void AccountLogin()
    {
        if (!isAccountLogin && !isGuestLogin)
        {
            isAccountLogin = true;
            StartCoroutine(AccountLoginProcess(loginUser_name, loginUser_password, massage_LoginText, x => isAccountLogin = x));
        }
    }

    #endregion
    #region ゲストログイン関係
    // ゲストログインボタンが押されたとき(GuestLogin用UIのEventTriggerのPointerClickに使う)
    public void GuestLogin()
    {
        if (!isGuestLogin && !isAccountLogin)
        {
            isGuestLogin = true;
            StartCoroutine(GuestLoginProcess());
        }
    }
    #endregion
    #region アカウント作成関係
    // ユーザー名input
    public void InputUserName()
    {
        createUser_name = user_nameField.text;
    }
    // ユーザー名select
    public void SelectUserName() => createInputSelected = 1;
    // パスワードinput
    public void InputPassword()
    {
        createUser_password = passwordField.text;
    }
    // パスワードselect
    public void SelectPassword() => createInputSelected = 2;
    // 再入力パスワードinput
    public void InputRePassword()
    {
        createUser_rePassword = rePasswordField.text;
    }
    // 再入力パスワードselect
    public void SelectRePassword() => createInputSelected = 0;
    // Tabで切り替え
    private void InputSelectCreateUser()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            createInputSelected++;
            if (createInputSelected > 2) { createInputSelected = 0; }

            switch (createInputSelected)
            {
                case 0:
                    rePasswordField.Select();
                    break;
                case 1:
                    user_nameField.Select();
                    break;
                case 2:
                    passwordField.Select();
                    break;
            }
        }
    }

    // 新規作成ボタンを押したとき
    public void OpenCreateFormUI()
    {
        if (!openCreateForm)
        {
            openCreateForm = true;
            createInputSelected = 0;
            createFormUI.SetActive(true);
        }
    }
    // 新規作成フォーム内の戻るボタンを押したとき
    public void CloseCrateFormUI()
    {
        if (openCreateForm)
        {
            openCreateForm = false;
            massageText.GetComponent<TextMeshProUGUI>().text = "";
            createFormUI.SetActive(false);
        }
    }
    // 新規作成フォーム内の作成ボタンを押したとき
    public void CreateAccountButton()
    {
        if (!isCreateAccont)
        {
            isCreateAccont = true;
            createInputSelected = 0;
            StartCoroutine(CreateAccountProcess(createUser_name,createUser_password,createUser_rePassword,user_nameField,passwordField,rePasswordField, massageText, x => isCreateAccont = x));
        }
    }
    
    #endregion

    #region ゲーム終了関係
    public void OpenQuitGameForm()
    {
        if (!isOpenQuitGameForm)
        {
            isOpenQuitGameForm = true;
            quitGameForm.SetActive(true);
        }
    }

    public void CloseQuitGameForm()
    {
        if (isOpenQuitGameForm)
        {
            isOpenQuitGameForm = false;
            quitGameForm.SetActive(false);
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
    Application.Quit();//ゲームプレイ終了
#endif
    }
    #endregion
}