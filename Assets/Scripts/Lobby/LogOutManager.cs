using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement; // シーン遷移用(フェードマネージャー作成時削除)

public class LogOutManager : MonoBehaviour
{
    #region 変数群
    LoginManager loginManagerCS;

    // ゲストログイン
    bool isLogOut = false;                                          // ログアウトフラグ
    string logOutURL = "http://localhost/user/logout";         // ログアウトURL
    #endregion

    void Start()
    {
        loginManagerCS = GameObject.FindObjectOfType<LoginManager>();
    }

    void Update()
    {
        
    }

    // ログアウトボタンが押されたとき(LogOut用UIのEventTriggerのPointerClickに使う)
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
        // POST送信用のフォームを作成
        WWWForm postData = new WWWForm();
        postData.AddField("manageID", loginManagerCS.Manage_id);
        postData.AddField("userName", loginManagerCS.User_name);

        // POSTでデータ送信
        using UnityWebRequest request = UnityWebRequest.Post(logOutURL, postData);
        request.timeout = 10;
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            print(request.error);
        }
        else
        {
            SceneManager.LoadScene("Title");        // シーン遷移
        }
    }
}
