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
        UIopen(logOutForm, NoActionCol());
        openLogOutForm = true;
    }

    public void CloseLogOutForm()
    {
        UIclose(logOutForm);
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
