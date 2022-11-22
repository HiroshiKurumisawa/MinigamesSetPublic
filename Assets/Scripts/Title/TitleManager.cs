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
    LoginManager loginManagerCS;

    // �Q�X�g���O�C��
    bool isGuestLogin = false;                                          // �Q�X�g���O�C���t���O
    const string guestLoginURL = "http://localhost/user/guest/login";   // �Q�X�g���O�C��URL

    // ���[�U�[�쐬
    bool openCreateForm = false;                                        // ���[�U�[�쐬�t�H�[�����J���Ă��邩
    bool isCreateAccont = false;                                        // �쐬�{�^�������������ǂ���
    public bool IsCreateAccount { get { return isCreateAccont; } }
    const string createAccountURL = "http://localhost/user/account/create";         // �A�J�E���g�쐬URL
    string createUser_name = "";
    string createUser_password = "";
    string createUser_rePassword = "";
    [Header("���[�U�[�쐬�֌W")]
    [SerializeField] GameObject createFormUI;                           // ���[�U�[�쐬�t�H�[��
    [SerializeField] GameObject massageText;                            // ���b�Z�[�W�e�L�X�g
    [SerializeField] TMP_InputField user_nameField;                     // ���[�U�[�l�[�����̓t�B�[���h
    [SerializeField] TMP_InputField passwordField;                      // �p�X���[�h���̓t�B�[���h
    [SerializeField] TMP_InputField rePasswordField;                    // �p�X���[�h�ē��̓t�B�[���h
    #endregion

    private void Awake()
    {
        massageText.GetComponent<TextMeshProUGUI>().text = "";
        createFormUI.SetActive(false);
    }

    void Start()
    {
        loginManagerCS = GameObject.FindObjectOfType<LoginManager>();
    }

    void Update()
    {

    }

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
                resData.guest_data.last_login, resData.guest_data.created, resData.guest_data.modified, resData.guest_data.connection_status);

            print(resData.guest_data.user_name);

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
    // �p�X���[�hinput
    public void InputPassword()
    {
        createUser_password = passwordField.text;
    }
    // �ē��̓p�X���[�hinput
    public void InputRePassword()
    {
        createUser_rePassword = rePasswordField.text;
    }
    // �V�K�쐬�{�^�����������Ƃ�
    public void OpenCreateFormUI()
    {
        if (!openCreateForm)
        {
            openCreateForm = true;
            createFormUI.SetActive(true);
        }
    }
    // �V�K�쐬�t�H�[�����̖߂�{�^�����������Ƃ�
    public void CloseCrateFormUI()
    {
        if (openCreateForm)
        {
            openCreateForm = false;
            createFormUI.SetActive(false);
        }
    }
    // �V�K�쐬�t�H�[�����̍쐬�{�^�����������Ƃ�
    public void CreateAccountButton()
    {
        if (!isCreateAccont)
        {
            isCreateAccont = true;
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
            }
            else if(resData.requestMessage == 1)
            {
                massageText.GetComponent<TextMeshProUGUI>().color = new Color(255, 0, 0);
                massageText.GetComponent<TextMeshProUGUI>().text = "�쐬���s(���͓��e���s�K�؂ł�)";
            }
        }
        isCreateAccont = false;
    }
    #endregion
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
