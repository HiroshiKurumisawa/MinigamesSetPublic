using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // シーン遷移用(フェードマネージャー作成時削除)

public class TitleManager : MonoBehaviour
{
    #region 変数群
    bool isGuestLogin = false;
    #endregion

    void Start()
    {

    }

    void Update()
    {

    }

    // ゲストログイン
    public void GuestLogin()
    {
        if(!isGuestLogin)
        {
            isGuestLogin = true;
            SceneManager.LoadScene("Lobby");
        }
    }

}
