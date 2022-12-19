using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainManager : MonoBehaviour
{
    // 必要なデータ保持するためのスクリプト
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
