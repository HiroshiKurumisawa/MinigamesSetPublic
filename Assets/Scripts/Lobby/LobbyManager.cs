using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyManager : MonoBehaviour
{
    #region 変数群
    LoginManager loginManagerCS;

    // テスト用
    [SerializeField]GameObject userNameText;
    #endregion

    void Start()
    {
        loginManagerCS = GameObject.FindObjectOfType<LoginManager>();
        userNameText.GetComponent<TextMeshProUGUI>().text = loginManagerCS.User_name;
    }

    void Update()
    {
        
    }
}
