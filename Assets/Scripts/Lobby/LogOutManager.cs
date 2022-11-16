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

    // �Q�X�g���O�C��
    bool isLogOut = false;                                          // ���O�A�E�g�t���O
    string logOutURL = "http://localhost/user/logout";         // ���O�A�E�gURL
    #endregion

    void Start()
    {
        loginManagerCS = GameObject.FindObjectOfType<LoginManager>();
    }

    void Update()
    {
        
    }

    // ���O�A�E�g�{�^���������ꂽ�Ƃ�(LogOut�pUI��EventTrigger��PointerClick�Ɏg��)
    public void LogOut()
    {
        if (!isLogOut)
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
