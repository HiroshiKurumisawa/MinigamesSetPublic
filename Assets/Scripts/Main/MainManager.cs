using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainManager : MonoBehaviour
{
    // �K�v�ȃf�[�^�ێ����邽�߂̃X�N���v�g
    LoginManager loginManagerCS;
    RoomDataManager roomDataManagerCS;

    // �e�X�g
    [SerializeField] TextMeshProUGUI textRoom;
    [SerializeField] TextMeshProUGUI textName;

    void Start()
    {
        loginManagerCS = GameObject.FindObjectOfType<LoginManager>();
        roomDataManagerCS = GameObject.FindObjectOfType<RoomDataManager>();

        // �e�X�g
        textRoom.text = "���O�C�����Ă��郆�[�U:" + loginManagerCS.User_name;
        textName.text = "���[����:" + roomDataManagerCS.Room_name;
    }

    void Update()
    {
        
    }
}
