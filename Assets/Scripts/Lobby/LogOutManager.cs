using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using SoundSystem;
using UnityEngine.Networking;

public class LogOutManager : MonoBehaviour
{
    #region 変数群
    LoginManager loginManagerCS;

    [SerializeField]GameObject logOutForm;

    bool openLogOutForm = false;
    bool isLogOut = false;                                     // ログアウトフラグ
    const string logOutURL = "http://localhost/user/logout";         // ログアウトURL
    //const string logOutURL = "http://54.168.79.41/user/logout";         // ログアウトURL
    #endregion

    void Start()
    {
        logOutForm.SetActive(false);
        loginManagerCS = GameObject.FindObjectOfType<LoginManager>();
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

    // ログアウトボタンが押されたとき(LogOut用UIのEventTriggerのPointerClickに使う)
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
        else
        {
            SoundManager.Instance.StopBGMWithFadeOut(1f);
            FadeManager.Instance.LoadScene("Title", 0.5f);
        }
    }
}
