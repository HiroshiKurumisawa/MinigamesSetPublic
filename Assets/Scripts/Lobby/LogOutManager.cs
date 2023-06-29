using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using SoundSystem;
using UnityEngine.Networking;

public class LogOutManager : NetworkBaseManager
{
    #region �ϐ��Q

    [SerializeField]GameObject logOutForm;
    bool openLogOutForm = false;
    bool isLogOut = false;                                     // ���O�A�E�g�t���O

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

    // ���O�A�E�g�{�^���������ꂽ�Ƃ�(LogOut�pUI��EventTrigger��PointerClick�Ɏg��)
    public void LogOut()
    {
        if (!isLogOut && openLogOutForm)
        {
            isLogOut = true;
            StartCoroutine(LogOutProcess());
        }
    }
}
