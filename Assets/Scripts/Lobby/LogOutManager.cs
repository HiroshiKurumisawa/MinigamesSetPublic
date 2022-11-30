using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement; // �V�[���J�ڗp(�t�F�[�h�}�l�[�W���[�쐬���폜)

public class LogOutManager : MonoBehaviour
{
    #region �ϐ��Q
    LoginManager loginManagerCS;

    [SerializeField]GameObject logOutForm;

    bool openLogOutForm = false;
    bool isLogOut = false;                                     // ���O�A�E�g�t���O
    string logOutURL = "http://localhost/user/logout";         // ���O�A�E�gURL
    #endregion

    void Start()
    {
        logOutForm.SetActive(false);
        loginManagerCS = GameObject.FindObjectOfType<LoginManager>();
    }

    void Update()
    {

    }

    public void OpenLogOutForm()
    {
        logOutForm.SetActive(true);
        openLogOutForm = true;
    }

    public void CloseLogOutForm()
    {
        logOutForm.SetActive(false);
        openLogOutForm = false;
    }

    // ���O�A�E�g�{�^���������ꂽ�Ƃ�(LogOut�pUI��EventTrigger��PointerClick�Ɏg��)
    public void LogOut()
    {
        if (!isLogOut && openLogOutForm)
        {
            isLogOut = true;
            StartCoroutine(LogOutProcess());
        }
    }
    IEnumerator LogOutProcess()
    {
        // POST���M�p�̃t�H�[�����쐬
        WWWForm postData = new WWWForm();
        postData.AddField("manageID", loginManagerCS.Manage_id);
        postData.AddField("userName", loginManagerCS.User_name);
        postData.AddField("userType", loginManagerCS.User_Type);

        // POST�Ńf�[�^���M
        using UnityWebRequest request = UnityWebRequest.Post(logOutURL, postData);
        request.timeout = 10;
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            print(request.error);
        }
        else
        {
            SceneManager.LoadScene("Title");        // �V�[���J��
        }
    }
}
