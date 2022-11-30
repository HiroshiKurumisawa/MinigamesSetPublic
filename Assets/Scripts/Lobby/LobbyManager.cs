using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyManager : MonoBehaviour
{
    #region �ϐ��Q
    LoginManager loginManagerCS;

    // ���[�U�[���\��
    [SerializeField] GameObject userNameText;
    // ���[���쐬�֌W
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

    // ���[�U�[�l�[���\��
    void DisplayUserName()
    {
        if (loginManagerCS != null) { userNameText.GetComponent<TextMeshProUGUI>().text = loginManagerCS.User_name; }
        else { userNameText.GetComponent<TextMeshProUGUI>().text = "No Data"; }
    }

    #region ���[���쐬�֌W
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
