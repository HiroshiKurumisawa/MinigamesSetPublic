using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement; // シーン確認用

public class QuietGame : NetworkBaseManager
{
    #region インスタンス関係
    public static QuietGame Instance { get; private set; }
    bool isVectory;
    public bool IsVectory { get { return isVectory; } set { isVectory = value; } }
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

    private void OnApplicationQuit()
    {
        loginManagerCS = GameObject.FindObjectOfType<LoginManager>();
        if (SceneManager.GetActiveScene().name != "Title")
        {
            StartCoroutine(LogOutProcess());
        }
    }
}
