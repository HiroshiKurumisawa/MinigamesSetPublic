using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyManager : MonoBehaviour
{
    #region �ϐ��Q
    LoginManager loginManagerCS;

    // �e�X�g�p
    [SerializeField] GameObject userNameText;
    #endregion

    void Start()
    {
        loginManagerCS = GameObject.FindObjectOfType<LoginManager>();
        if (loginManagerCS != null)
        {
            userNameText.GetComponent<TextMeshProUGUI>().text = loginManagerCS.User_name;
        }
        else
        {
            userNameText.GetComponent<TextMeshProUGUI>().text = "No Data";
        }
    }

    void Update()
    {

    }
}
