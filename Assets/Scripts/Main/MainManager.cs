using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainManager : MonoBehaviour
{
    // �K�v�ȃf�[�^�ێ����邽�߂̃X�N���v�g
    LoginManager loginManagerCS;
    RoomDataManager roomDataManagerCS;


    void Start()
    {
        loginManagerCS = GameObject.FindObjectOfType<LoginManager>();
        roomDataManagerCS = GameObject.FindObjectOfType<RoomDataManager>();

    }

    void Update()
    {
        
    }
}
