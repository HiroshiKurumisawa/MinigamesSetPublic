using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyManager : MonoBehaviour
{
    #region 変数群
    LoginManager loginManagerCS;

    // ユーザー名表示
    [SerializeField] GameObject userNameText;
    // ルーム作成関係
    [SerializeField] GameObject createRoomForm;
    bool isOpenCreateRoomForm = false;
    #endregion

    void Start()
    {
        createRoomForm.SetActive(false);
        loginManagerCS = GameObject.FindObjectOfType<LoginManager>();
        DisplayUserName();
    }

    void Update()
    {

    }

    // ユーザーネーム表示
    void DisplayUserName()
    {
        if (loginManagerCS != null) { userNameText.GetComponent<TextMeshProUGUI>().text = loginManagerCS.User_name; }
        else { userNameText.GetComponent<TextMeshProUGUI>().text = "No Data"; }
    }

    #region ルーム作成関係
    public void OpenCreateRoomForm()
    {
        if (!isOpenCreateRoomForm)
        {
            isOpenCreateRoomForm = true;
            createRoomForm.SetActive(true);
        }
    }
    public void CloseCreateRoomForm()
    {
        if (isOpenCreateRoomForm)
        {
            isOpenCreateRoomForm = false;
            createRoomForm.SetActive(false);
        }
    }
    #endregion
}
