using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement; // シーン確認用

public class QuietGame : MonoBehaviour
{
    #region インスタンス関係
    public static QuietGame Instance { get; private set; }
    bool isVectory;
    public bool IsVectory { get { return isVectory; } set { isVectory = value; } }
    #endregion
    #region 変数群
    LoginManager loginManagerCS;
    //const string logOutURL = "http://localhost/user/logout";         // ログアウトURL
    const string logOutURL = "http://ik1-423-43506.vs.sakura.ne.jp/user/logout";         // ログアウトURL
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
        // POST送信用のフォームを作成
        WWWForm postData = new WWWForm();
        postData.AddField("manageID", loginManagerCS.Manage_id);
        postData.AddField("userName", loginManagerCS.User_name);
        postData.AddField("userType", loginManagerCS.User_Type);

        // POSTでデータ送信
        using UnityWebRequest request = UnityWebRequest.Post(logOutURL, postData);
        request.timeout = 10;
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            print(request.error);
        }
    }
}
