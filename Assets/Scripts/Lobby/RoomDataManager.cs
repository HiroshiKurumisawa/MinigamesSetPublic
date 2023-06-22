using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDataManager : MonoBehaviour
{
    #region �C���X�^���X�֌W
    public static RoomDataManager Instance { get; private set; }
    #endregion

    #region �ϐ��Q
    // ���[���f�[�^(�K�v�Œ��)
    string room_name;
    public string Room_name { get { return room_name; } }
    string user_host;
    public string User_host { get { return user_host; } }
    string user_entry;
    public string User_entry { get { return user_entry; } }
    string game_rule;
    public string Game_rule { get { return game_rule; } }
    #endregion

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }
    }

    // ���[���f�[�^���Z�b�g����
    public void SetRoomData(string roomName, string host, string entry,string gameRule)
    {
        room_name = roomName;
        user_host = host;
        user_entry = entry;
        game_rule = gameRule;
    }
}
