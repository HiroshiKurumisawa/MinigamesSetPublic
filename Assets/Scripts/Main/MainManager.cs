using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainManager : MonoBehaviour
{
    // 必要なデータ保持するためのスクリプト
    LoginManager loginManagerCS;
    RoomDataManager roomDataManagerCS;

    // テスト
    [SerializeField] TextMeshProUGUI textRoom;
    [SerializeField] TextMeshProUGUI textName;

    void Start()
    {
        loginManagerCS = GameObject.FindObjectOfType<LoginManager>();
        roomDataManagerCS = GameObject.FindObjectOfType<RoomDataManager>();

        // テスト
        textRoom.text = "ログインしているユーザ:" + loginManagerCS.User_name;
        textName.text = "ルーム名:" + roomDataManagerCS.Room_name;
    }

    void Update()
    {
        
    }
}
