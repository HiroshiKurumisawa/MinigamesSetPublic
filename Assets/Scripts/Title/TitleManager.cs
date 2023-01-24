using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.SceneManagement; // シーン遷移用(フェードマネージャー作成時削除)

public class TitleManager : MonoBehaviour
{
    #region 変数群
    // ログイン後に必要なデータ保持するためのスクリプト
    LoginManager loginManagerCS;

    // アカウントログイン
    bool isAccountLogin = false;                                            // アカウントログインフラグ
    const string accountLoginURL = "http://localhost/user/account/login";   // アカウントログインURL
    //const string accountLoginURL = "http://54.168.79.41/user/account/login";   // アカウントログインURL
    string loginUser_name = "";                                             // ログインユーザー名
    string loginUser_password = "";                                         // ログインパスワード
    [Header("ユーザーログイン関係")]
    [SerializeField] GameObject massage_LoginText;                          // メッセージテキスト(ログイン)
    [SerializeField] TMP_InputField user_nameField_Login;                   // ユーザーネーム入力フィールド(ログイン)
    [SerializeField] TMP_InputField passwordField_Login;                    // パスワード入力フィールド(ログイン)
    int loginInputSelected = 0;
    // ゲストログイン
    bool isGuestLogin = false;                                              // ゲストログインフラグ
    const string guestLoginURL = "http://localhost/user/guest/login";       // ゲストログインURL
    //const string guestLoginURL = "http://54.168.79.41/user/guest/login";       // ゲストログインURL

    // ユーザー作成
    bool openCreateForm = false;                                            // ユーザー作成フォームが開いているか
    bool isCreateAccont = false;                                            // 作成ボタンを押したかどうか
    public bool IsCreateAccount { get { return isCreateAccont; } }
    const string createAccountURL = "http://localhost/user/account/create"; // アカウント作成URL
    //const string createAccountURL = "http://54.168.79.41/user/account/create"; // アカウント作成URL
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
        massage_LoginText.GetComponent<TextMeshProUGUI>().text = "";
        massageText.GetComponent<TextMeshProUGUI>().text = "";
        createFormUI.SetActive(false);
    }

    void Start()
    {
        loginManagerCS = GameObject.FindObjectOfType<LoginManager>();
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
    public void SelectUserNameLogin() => loginInputSelected = 0;
    // パスワードinput
    public void InputPasswordLogin()
    {
        loginUser_password = passwordField_Login.text;
    }
    // パスワードselect
    public void SelectPasswordLogin() => loginInputSelected = 1;
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
                    user_nameField_Login.Select();
                    break;
                case 1:
                    passwordField_Login.Select();
                    break;
            }
        }
    }
    // ログインボタンが押されたとき(AccountLogin用UIのEventTriggerのPointerClickに使う)
    public void AccountLogin()
    {
        if (!isAccountLogin)
        {
            isAccountLogin = true;
            {
                StartCoroutine(AccountLoginProcess());
            }
        }
    }
    IEnumerator AccountLoginProcess()
    {
        // POST送信用のフォームを作成
        WWWForm postData = new WWWForm();
        postData.AddField("user_name", loginUser_name);
        postData.AddField("password", loginUser_password);
        // POSTでデータ送信
        using UnityWebRequest request = UnityWebRequest.Post(accountLoginURL, postData);
        request.timeout = 10;
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            print(request.error);
        }
        else
        {
            AccountLoginRoot resData = JsonUtility.FromJson<AccountLoginRoot>(request.downloadHandler.text);

            if (resData.result == 0)
            {
                if (resData.requestMessage == 1)           // エラーメッセージだった時(入力内容エラー)
                {
                    massage_LoginText.GetComponent<TextMeshProUGUI>().color = new Color(255, 0, 0);
                    massage_LoginText.GetComponent<TextMeshProUGUI>().text = "ログイン失敗(入力内容に誤りがあります)";
                    isAccountLogin = false;
                }
                else if (resData.requestMessage == 2)   // すでにログインされている状態
                {
                    massage_LoginText.GetComponent<TextMeshProUGUI>().color = new Color(255, 0, 0);
                    massage_LoginText.GetComponent<TextMeshProUGUI>().text = "ログイン失敗(既にログインしています)";
                    isAccountLogin = false;
                }
                else                                    // ログインできるとき
                {
                    loginManagerCS.SetUserData(resData.account_data.manage_id, resData.account_data.login_id, resData.account_data.user_name,
                             resData.account_data.last_login, resData.account_data.created, resData.account_data.modified, resData.account_data.connection_status, 0);

                    SceneManager.LoadScene("Lobby");        // シーン遷移
                }
            }

        }
    }
    #endregion
    #region ゲストログイン関係
    // ゲストログインボタンが押されたとき(GuestLogin用UIのEventTriggerのPointerClickに使う)
    public void GuestLogin()
    {
        if (!isGuestLogin)
        {
            isGuestLogin = true;
            StartCoroutine(GuestLoginProcess());
        }
    }
    IEnumerator GuestLoginProcess()
    {
        // POST送信用のフォームを作成
        WWWForm postData = new WWWForm();

        // POSTでデータ送信
        using UnityWebRequest request = UnityWebRequest.Post(guestLoginURL, postData);
        request.timeout = 10;
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            print(request.error);
        }
        else
        {
            GuestLoginRoot resData = JsonUtility.FromJson<GuestLoginRoot>(request.downloadHandler.text);
            loginManagerCS.SetUserData(resData.guest_data.manage_id, resData.guest_data.login_id, resData.guest_data.user_name,
                resData.guest_data.last_login, resData.guest_data.created, resData.guest_data.modified, resData.guest_data.connection_status, 1);

            SceneManager.LoadScene("Lobby");        // シーン遷移
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
    public void SelectUserName() => createInputSelected = 0;
    // パスワードinput
    public void InputPassword()
    {
        createUser_password = passwordField.text;
    }
    // パスワードselect
    public void SelectPassword() => createInputSelected = 1;
    // 再入力パスワードinput
    public void InputRePassword()
    {
        createUser_rePassword = rePasswordField.text;
    }
    // 再入力パスワードselect
    public void SelectRePassword() => createInputSelected = 2;
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
                    user_nameField.Select();
                    break;
                case 1:
                    passwordField.Select();
                    break;
                case 2:
                    rePasswordField.Select();
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
            StartCoroutine(CreateAccountProcess());
        }
    }
    IEnumerator CreateAccountProcess()
    {
        // POST送信用のフォームを作成
        WWWForm postData = new WWWForm();
        postData.AddField("user_name", createUser_name);
        postData.AddField("password", createUser_password);
        postData.AddField("repassword", createUser_rePassword);

        // POSTでデータ送信
        using UnityWebRequest request = UnityWebRequest.Post(createAccountURL, postData);
        request.timeout = 10;
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            print(request.error);
        }
        else
        {
            AccountCreateRoot resData = JsonUtility.FromJson<AccountCreateRoot>(request.downloadHandler.text);

            if (resData.requestMessage == 0)
            {
                user_nameField.text = "";
                passwordField.text = "";
                rePasswordField.text = "";
                massageText.GetComponent<TextMeshProUGUI>().color = new Color(0, 0, 255);
                massageText.GetComponent<TextMeshProUGUI>().text = "作成成功";
                yield return new WaitForSeconds(0.8f);
                CloseCrateFormUI();
            }
            else if (resData.requestMessage == 1)
            {
                massageText.GetComponent<TextMeshProUGUI>().color = new Color(255, 0, 0);
                massageText.GetComponent<TextMeshProUGUI>().text = "作成失敗(入力内容が不適切です)";
            }
        }
        isCreateAccont = false;
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

#region JSON変換クラス
// アカウントログインの結果のJSONを変換するためのクラス
[Serializable]
public class AccountData
{
    public int manage_id;
    public string login_id;
    public string user_name;
    public string pass_hash;
    public string last_login;
    public string created;
    public string modified;
    public bool connection_status;
}
[Serializable]
public class AccountLoginRoot
{
    public int result;
    public int requestMessage;
    public AccountData account_data;
}

// ゲストログインの結果JSONを変換するためのクラス
[Serializable]
public class GuestData
{
    public int manage_id;
    public string login_id;
    public string user_name;
    public string last_login;
    public string created;
    public string modified;
    public bool connection_status;
}
[Serializable]
public class GuestLoginRoot
{
    public int result;
    public GuestData guest_data;
}

// アカウント作成結果JSON変換クラス
[Serializable]
public class AccountCreateRoot
{
    public int result;
    public int requestMessage;
}
#endregion