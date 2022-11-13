using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement; // シーン遷移用(フェードマネージャー作成時削除)

public class TitleManager : MonoBehaviour
{
    #region 変数群
    LoginManager loginManagerCS;

    // ゲストログイン
    bool isGuestLogin = false;                                          // ゲストログインフラグ
    string guestLoginURL = "http://localhost/user/guest/login";         // ゲストログインURL
    #endregion

    void Start()
    {
        loginManagerCS = GameObject.FindObjectOfType<LoginManager>();
    }

    void Update()
    {

    }

    // ゲストログインボタンが押されたとき(GuestLogin用UIのEventTriggerのPointerClickに使う)
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
        // POST送信用のフォームを作成
        WWWForm postData = new WWWForm();

        // POSTでデータ送信
        using UnityWebRequest request = UnityWebRequest.Post(guestLoginURL, postData);
        request.timeout = 10;
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            print(request.error);
        }
        else
        {
            print(request.downloadHandler.text);        // JSON確認用出力

            var resData = JsonUtility.FromJson<Root>(request.downloadHandler.text);
            loginManagerCS.SetUserData(resData.guest_data.manage_id, resData.guest_data.login_id, resData.guest_data.user_name,
                resData.guest_data.last_login, resData.guest_data.created, resData.guest_data.modified, resData.guest_data.connection_status);
            
            SceneManager.LoadScene("Lobby");        // シーン遷移
        }
    }
}


// ゲストログインの結果JSONを変換するためのクラス
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
