using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.SceneManagement; // �V�[���J�ڗp(�t�F�[�h�}�l�[�W���[�쐬���폜)

public class TitleManager : MonoBehaviour
{
    #region �ϐ��Q
    // ���O�C����ɕK�v�ȃf�[�^�ێ����邽�߂̃X�N���v�g
    LoginManager loginManagerCS;

    // �A�J�E���g���O�C��
    bool isAccountLogin = false;                                            // �A�J�E���g���O�C���t���O
    const string accountLoginURL = "http://localhost/user/account/login";   // �A�J�E���g���O�C��URL
    //const string accountLoginURL = "http://54.168.79.41/user/account/login";   // �A�J�E���g���O�C��URL
    string loginUser_name = "";                                             // ���O�C�����[�U�[��
    string loginUser_password = "";                                         // ���O�C���p�X���[�h
    [Header("���[�U�[���O�C���֌W")]
    [SerializeField] GameObject massage_LoginText;                          // ���b�Z�[�W�e�L�X�g(���O�C��)
    [SerializeField] TMP_InputField user_nameField_Login;                   // ���[�U�[�l�[�����̓t�B�[���h(���O�C��)
    [SerializeField] TMP_InputField passwordField_Login;                    // �p�X���[�h���̓t�B�[���h(���O�C��)
    int loginInputSelected = 0;
    // �Q�X�g���O�C��
    bool isGuestLogin = false;                                              // �Q�X�g���O�C���t���O
    const string guestLoginURL = "http://localhost/user/guest/login";       // �Q�X�g���O�C��URL
    //const string guestLoginURL = "http://54.168.79.41/user/guest/login";       // �Q�X�g���O�C��URL

    // ���[�U�[�쐬
    bool openCreateForm = false;                                            // ���[�U�[�쐬�t�H�[�����J���Ă��邩
    bool isCreateAccont = false;                                            // �쐬�{�^�������������ǂ���
    public bool IsCreateAccount { get { return isCreateAccont; } }
    const string createAccountURL = "http://localhost/user/account/create"; // �A�J�E���g�쐬URL
    //const string createAccountURL = "http://54.168.79.41/user/account/create"; // �A�J�E���g�쐬URL
    string createUser_name = "";                                            // �A�J�E���g�쐬���[�U��
    string createUser_password = "";                                        // �A�J�E���g�쐬�p�X���[�h
    string createUser_rePassword = "";                                      // �A�J�E���g�쐬�p�X���[�h�ē���
    [Header("���[�U�[�쐬�֌W")]
    [SerializeField] GameObject createFormUI;                               // ���[�U�[�쐬�t�H�[��
    [SerializeField] GameObject massageText;                                // ���b�Z�[�W�e�L�X�g
    [SerializeField] TMP_InputField user_nameField;                         // ���[�U�[�l�[�����̓t�B�[���h
    [SerializeField] TMP_InputField passwordField;                          // �p�X���[�h���̓t�B�[���h
    [SerializeField] TMP_InputField rePasswordField;                        // �p�X���[�h�ē��̓t�B�[���h
    int createInputSelected = 0;

    [Header("�Q�[���I���֌W")]
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

    #region �A�J�E���g���O�C���֌W
    // ���[�U�[��input
    public void InputUserNameLogin()
    {
        loginUser_name = user_nameField_Login.text;
    }
    // ���[�U�[��select
    public void SelectUserNameLogin() => loginInputSelected = 0;
    // �p�X���[�hinput
    public void InputPasswordLogin()
    {
        loginUser_password = passwordField_Login.text;
    }
    // �p�X���[�hselect
    public void SelectPasswordLogin() => loginInputSelected = 1;
    // Tab�Ő؂�ւ�
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
    // ���O�C���{�^���������ꂽ�Ƃ�(AccountLogin�pUI��EventTrigger��PointerClick�Ɏg��)
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
        // POST���M�p�̃t�H�[�����쐬
        WWWForm postData = new WWWForm();
        postData.AddField("user_name", loginUser_name);
        postData.AddField("password", loginUser_password);
        // POST�Ńf�[�^���M
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
                if (resData.requestMessage == 1)           // �G���[���b�Z�[�W��������(���͓��e�G���[)
                {
                    massage_LoginText.GetComponent<TextMeshProUGUI>().color = new Color(255, 0, 0);
                    massage_LoginText.GetComponent<TextMeshProUGUI>().text = "���O�C�����s(���͓��e�Ɍ�肪����܂�)";
                    isAccountLogin = false;
                }
                else if (resData.requestMessage == 2)   // ���łɃ��O�C������Ă�����
                {
                    massage_LoginText.GetComponent<TextMeshProUGUI>().color = new Color(255, 0, 0);
                    massage_LoginText.GetComponent<TextMeshProUGUI>().text = "���O�C�����s(���Ƀ��O�C�����Ă��܂�)";
                    isAccountLogin = false;
                }
                else                                    // ���O�C���ł���Ƃ�
                {
                    loginManagerCS.SetUserData(resData.account_data.manage_id, resData.account_data.login_id, resData.account_data.user_name,
                             resData.account_data.last_login, resData.account_data.created, resData.account_data.modified, resData.account_data.connection_status, 0);

                    SceneManager.LoadScene("Lobby");        // �V�[���J��
                }
            }

        }
    }
    #endregion
    #region �Q�X�g���O�C���֌W
    // �Q�X�g���O�C���{�^���������ꂽ�Ƃ�(GuestLogin�pUI��EventTrigger��PointerClick�Ɏg��)
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
        // POST���M�p�̃t�H�[�����쐬
        WWWForm postData = new WWWForm();

        // POST�Ńf�[�^���M
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

            SceneManager.LoadScene("Lobby");        // �V�[���J��
        }
    }
    #endregion
    #region �A�J�E���g�쐬�֌W
    // ���[�U�[��input
    public void InputUserName()
    {
        createUser_name = user_nameField.text;
    }
    // ���[�U�[��select
    public void SelectUserName() => createInputSelected = 0;
    // �p�X���[�hinput
    public void InputPassword()
    {
        createUser_password = passwordField.text;
    }
    // �p�X���[�hselect
    public void SelectPassword() => createInputSelected = 1;
    // �ē��̓p�X���[�hinput
    public void InputRePassword()
    {
        createUser_rePassword = rePasswordField.text;
    }
    // �ē��̓p�X���[�hselect
    public void SelectRePassword() => createInputSelected = 2;
    // Tab�Ő؂�ւ�
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

    // �V�K�쐬�{�^�����������Ƃ�
    public void OpenCreateFormUI()
    {
        if (!openCreateForm)
        {
            openCreateForm = true;
            createInputSelected = 0;
            createFormUI.SetActive(true);
        }
    }
    // �V�K�쐬�t�H�[�����̖߂�{�^�����������Ƃ�
    public void CloseCrateFormUI()
    {
        if (openCreateForm)
        {
            openCreateForm = false;
            massageText.GetComponent<TextMeshProUGUI>().text = "";
            createFormUI.SetActive(false);
        }
    }
    // �V�K�쐬�t�H�[�����̍쐬�{�^�����������Ƃ�
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
        // POST���M�p�̃t�H�[�����쐬
        WWWForm postData = new WWWForm();
        postData.AddField("user_name", createUser_name);
        postData.AddField("password", createUser_password);
        postData.AddField("repassword", createUser_rePassword);

        // POST�Ńf�[�^���M
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
                massageText.GetComponent<TextMeshProUGUI>().text = "�쐬����";
                yield return new WaitForSeconds(0.8f);
                CloseCrateFormUI();
            }
            else if (resData.requestMessage == 1)
            {
                massageText.GetComponent<TextMeshProUGUI>().color = new Color(255, 0, 0);
                massageText.GetComponent<TextMeshProUGUI>().text = "�쐬���s(���͓��e���s�K�؂ł�)";
            }
        }
        isCreateAccont = false;
    }
    #endregion

    #region �Q�[���I���֌W
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
        UnityEditor.EditorApplication.isPlaying = false;//�Q�[���v���C�I��
#else
    Application.Quit();//�Q�[���v���C�I��
#endif
    }
    #endregion
}

#region JSON�ϊ��N���X
// �A�J�E���g���O�C���̌��ʂ�JSON��ϊ����邽�߂̃N���X
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

// �Q�X�g���O�C���̌���JSON��ϊ����邽�߂̃N���X
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

// �A�J�E���g�쐬����JSON�ϊ��N���X
[Serializable]
public class AccountCreateRoot
{
    public int result;
    public int requestMessage;
}
#endregion