using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement; // �V�[���m�F�p

public class QuietGame : MonoBehaviour
{
    #region �C���X�^���X�֌W
    public static QuietGame Instance { get; private set; }
    bool isVectory;
    public bool IsVectory { get { return isVectory; } set { isVectory = value; } }
    #endregion
    #region �ϐ��Q
    LoginManager loginManagerCS;
    //const string logOutURL = "http://localhost/user/logout";         // ���O�A�E�gURL
    const string logOutURL = "http://ik1-423-43506.vs.sakura.ne.jp/user/logout";         // ���O�A�E�gURL
    #endregion

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }
    }

    private void OnApplicationQuit()
    {
        loginManagerCS = GameObject.FindObjectOfType<LoginManager>();
        if (SceneManager.GetActiveScene().name!="Title")
        {
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
    }
}
