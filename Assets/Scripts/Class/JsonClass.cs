using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JsonClass
{
    #region JSON変換クラス(アカウント関係)
    // アカウントログインの結果のJSONを変換するためのクラス
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

    // ゲストログインの結果JSONを変換するためのクラス
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

    // アカウント作成結果JSON変換クラス
    [Serializable]
    public class AccountCreateRoot
    {
        public int result;
        public int requestMessage;
    }
    #endregion
    #region JSON変換クラス(ルーム関係)
    // ルーム作成作成結果JSON変換クラス
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

    // ルーム選択画面
    [Serializable]
    public class AllRoomRoot
    {
        public int result;
        public List<RoomData> allRoomList = new List<RoomData>();
    }

    // ルーム表示中
    [Serializable]
    public class UpdateRoom
    {
        public int result;
        public RoomData roomData;
    }

    // ルーム参加(パスワードがある場合)
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
    // ルーム参加
    [Serializable]
    public class EntryRoomInpassRoot
    {
        public int result;
        public int requestMessage;
        public EntryRoomDataInPass roomData;
    }

    // ルーム退出
    [Serializable]
    public class LeaveRoom
    {
        public int result;
    }

    // ルーム参加
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

    #region JSON変換クラス(リバーシ)
    // ゲームデータ表示中
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
    #region JSON変換クラス(五目並べ)
    
    #endregion
}
