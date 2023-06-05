using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using SoundSystem;
using UnityEngine.Networking;

public class LogOutManager : NetworkBaseManager
{
    #region 変数群

    [SerializeField]GameObject logOutForm;
    bool openLogOutForm = false;
    bool isLogOut = false;                                     // ログアウトフラグ

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
}
