using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JsonClass
{
    #region JSON�ϊ��N���X(�A�J�E���g�֌W)
    // �A�J�E���g���O�C���̌��ʂ�JSON��ϊ����邽�߂̃N���X
    [Serializable]
    public class AccountData
    {
        public int manage_id;
        public string login_id;
        public string user_name;
        public string pass_hash;
        public string last_login;
        public string created;
        public string modified;
        public bool connection_status;
    }
    [Serializable]
    public class AccountLoginRoot
    {
        public int result;
        public int requestMessage;
        public AccountData account_data;
    }

    // �Q�X�g���O�C���̌���JSON��ϊ����邽�߂̃N���X
    [Serializable]
    public class GuestData
    {
        public int manage_id;
        public string login_id;
        public string user_name;
        public string last_login;
        public string created;
        public string modified;
        public bool connection_status;
    }
    [Serializable]
    public class GuestLoginRoot
    {
        public int result;
        public GuestData guest_data;
    }

    // �A�J�E���g�쐬����JSON�ϊ��N���X
    [Serializable]
    public class AccountCreateRoot
    {
        public int result;
        public int requestMessage;
    }
    #endregion
    #region JSON�ϊ��N���X(���[���֌W)
    // ���[���쐬�쐬����JSON�ϊ��N���X
    [Serializable]
    public class RoomData
    {
        public int id;
        public string room_name;
        public string room_password;
        public string max_room_users;
        public string in_room_users;
        public string created_at;
        public string updated_at;
        public string user_host;
        public string user_entry;
        public bool ready_status_host;
        public bool ready_status_entry;
        public bool game_status;
        public string game_rule;
    }
    [Serializable]
    public class RoomCreateRoot
    {
        public int result;
        public int requestMessage;
        public RoomData roomData;
    }

    // ���[���I�����
    [Serializable]
    public class AllRoomRoot
    {
        public int result;
        public List<RoomData> allRoomList = new List<RoomData>();
    }

    // ���[���\����
    [Serializable]
    public class UpdateRoom
    {
        public int result;
        public RoomData roomData;
    }

    // ���[���Q��(�p�X���[�h������ꍇ)
    [Serializable]
    public class EntryRoomDataInPass
    {
        public int id;
        public string room_name;
        public string room_password;
        public string max_room_users;
        public string in_room_users;
        public string created_at;
        public string updated_at;
        public string user_host;
        public string user_entry;
        public bool ready_status_host;
        public bool ready_status_entry;
        public bool game_status;
        public string game_rule;
    }
    // ���[���Q��
    [Serializable]
    public class EntryRoomInpassRoot
    {
        public int result;
        public int requestMessage;
        public EntryRoomDataInPass roomData;
    }

    // ���[���ޏo
    [Serializable]
    public class LeaveRoom
    {
        public int result;
    }

    // ���[���Q��
    [Serializable]
    public class EntryRoomData
    {
        public int id;
        public string room_name;
        public string room_password;
        public string max_room_users;
        public string in_room_users;
        public string created_at;
        public string updated_at;
        public string user_host;
        public string user_entry;
        public bool ready_status_host;
        public bool ready_status_entry;
        public bool game_status;
        public string game_rule;
    }
    [Serializable]
    public class EntryRoomRoot
    {
        public int result;
        public int requestMessage;
        public EntryRoomData roomData;
    }
    #endregion

    #region JSON�ϊ��N���X(���o�[�V)
    // �Q�[���f�[�^�\����
    [Serializable]
    public class UpdateGame
    {
        public int result;
        public GameData gameData;
    }
    [Serializable]
    public class GameData
    {
        public int id;
        public string room_name;
        public string set_point;
        public string game_state;
        public string created_at;
        public string updated_at;
    }

    [Serializable]
    public class DeleteRoom
    {
        public int result;
    }
    #endregion
    #region JSON�ϊ��N���X(�ܖڕ���)
    
    #endregion
}
