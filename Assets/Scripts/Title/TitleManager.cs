using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // �V�[���J�ڗp(�t�F�[�h�}�l�[�W���[�쐬���폜)

public class TitleManager : MonoBehaviour
{
    #region �ϐ��Q
    bool isGuestLogin = false;
    #endregion

    void Start()
    {

    }

    void Update()
    {

    }

    // �Q�X�g���O�C��
    public void GuestLogin()
    {
        if(!isGuestLogin)
        {
            isGuestLogin = true;
            SceneManager.LoadScene("Lobby");
        }
    }

}
