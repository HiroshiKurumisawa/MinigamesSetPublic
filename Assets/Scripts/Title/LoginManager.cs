using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginManager : MonoBehaviour
{
    #region �C���X�^���X�֌W
    public static LoginManager Instance { get; private set; }
    #endregion

    #region �ϐ��Q
    // ���[�U�[�f�[�^----------
    int manage_id;
    public int Manage_id { get { return manage_id; } }
    string login_id;
    public string Login_id { get { return login_id; } }
    string user_name;
    public string User_name { get { return user_name; } }
    string last_login;
    public string Last_login { get { return last_login; } }
    string created;
    public string Created { get { return created; } }
    string modified;
    public string Modified { get { return modified; } }
    bool connection_status;
    public bool Connection_status { get { return connection_status; } }
    //------------------------
    // �ڑ����Ă���A�J�E���g�̎��(0=�A�J�E���g���[�U�[�A 1=�Q�X�g���[�U�[)
    int user_Type; 
    public int User_Type { get { return user_Type; } }
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

    // ���[�U�[�f�[�^���Z�b�g����
    public void SetUserData(int id, string loginId, string userName,
        string lastLogin, string createdTime, string midfiedTime, bool connectionStatus, int userType)
    {
        manage_id = id;
        login_id = loginId;
        user_name = userName;
        last_login = lastLogin;
        created = createdTime;
        modified = midfiedTime;
        connection_status = connectionStatus;
        user_Type = userType;
    }
}
