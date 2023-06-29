using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using SoundSystem;

public class TitleManager : NetworkBaseManager
{
    #region �ϐ��Q
    // �A�J�E���g���O�C��
    bool isAccountLogin = false;                                            // �A�J�E���g���O�C���t���O
    string loginUser_name = "";                                             // ���O�C�����[�U�[��
    string loginUser_password = "";                                         // ���O�C���p�X���[�h
    [Header("���[�U�[���O�C���֌W")]
    [SerializeField] TextMeshProUGUI massage_LoginText;                          // ���b�Z�[�W�e�L�X�g(���O�C��)
    [SerializeField] TMP_InputField user_nameField_Login;                   // ���[�U�[�l�[�����̓t�B�[���h(���O�C��)
    [SerializeField] TMP_InputField passwordField_Login;                    // �p�X���[�h���̓t�B�[���h(���O�C��)
    int loginInputSelected = 0;
    // �Q�X�g���O�C��
    bool isGuestLogin = false;                                              // �Q�X�g���O�C���t���O

    // ���[�U�[�쐬
    bool openCreateForm = false;                                            // ���[�U�[�쐬�t�H�[�����J���Ă��邩
    bool isCreateAccont = false;                                            // �쐬�{�^�������������ǂ���
    public bool IsCreateAccount { get { return isCreateAccont; } }
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

    #region �A�J�E���g���O�C���֌W
    // ���[�U�[��input
    public void InputUserNameLogin()
    {
        loginUser_name = user_nameField_Login.text;
    }
    // ���[�U�[��select
    public void SelectUserNameLogin() => loginInputSelected = 1;
    // �p�X���[�hinput
    public void InputPasswordLogin()
    {
        loginUser_password = passwordField_Login.text;
    }
    // �p�X���[�hselect
    public void SelectPasswordLogin() => loginInputSelected = 0;
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
                    passwordField_Login.Select();
                    break;
                case 1:
                    user_nameField_Login.Select();
                    break;
            }
        }
    }
    // ���O�C���{�^���������ꂽ�Ƃ�(AccountLogin�pUI��EventTrigger��PointerClick�Ɏg��)
    public void AccountLogin()
    {
        if (!isAccountLogin && !isGuestLogin)
        {
            isAccountLogin = true;
            StartCoroutine(AccountLoginProcess(loginUser_name, loginUser_password, massage_LoginText, x => isAccountLogin = x));
        }
    }

    #endregion
    #region �Q�X�g���O�C���֌W
    // �Q�X�g���O�C���{�^���������ꂽ�Ƃ�(GuestLogin�pUI��EventTrigger��PointerClick�Ɏg��)
    public void GuestLogin()
    {
        if (!isGuestLogin && !isAccountLogin)
        {
            isGuestLogin = true;
            StartCoroutine(GuestLoginProcess());
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
    public void SelectUserName() => createInputSelected = 1;
    // �p�X���[�hinput
    public void InputPassword()
    {
        createUser_password = passwordField.text;
    }
    // �p�X���[�hselect
    public void SelectPassword() => createInputSelected = 2;
    // �ē��̓p�X���[�hinput
    public void InputRePassword()
    {
        createUser_rePassword = rePasswordField.text;
    }
    // �ē��̓p�X���[�hselect
    public void SelectRePassword() => createInputSelected = 0;
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
            StartCoroutine(CreateAccountProcess(createUser_name,createUser_password,createUser_rePassword,user_nameField,passwordField,rePasswordField, massageText, x => isCreateAccont = x));
        }
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