using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement; // �V�[���J�ڗp(�t�F�[�h�}�l�[�W���[�쐬���폜)

public class TitleManager : MonoBehaviour
{
    #region �ϐ��Q
    LoginManager loginManagerCS;

    // �Q�X�g���O�C��
    bool isGuestLogin = false;                                          // �Q�X�g���O�C���t���O
    string guestLoginURL = "http://localhost/user/guest/login";         // �Q�X�g���O�C��URL
    #endregion

    void Start()
    {
        loginManagerCS = GameObject.FindObjectOfType<LoginManager>();
    }

    void Update()
    {

    }

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
            print(request.downloadHandler.text);        // JSON�m�F�p�o��

            var resData = JsonUtility.FromJson<Root>(request.downloadHandler.text);
            loginManagerCS.SetUserData(resData.guest_data.manage_id, resData.guest_data.login_id, resData.guest_data.user_name,
                resData.guest_data.last_login, resData.guest_data.created, resData.guest_data.modified, resData.guest_data.connection_status);
            
            SceneManager.LoadScene("Lobby");        // �V�[���J��
        }
    }
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
public class Root
{
    public int result;
    public GuestData guest_data;
}
