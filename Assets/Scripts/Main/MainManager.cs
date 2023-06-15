using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainManager : MonoBehaviour
{
    [SerializeField, Header("表示するゲームのオブジェクト")]
    GameObject[] minigames;

    const int Revasi = 0, Gomoku = 1;

    // サーバーから取得する数値にのちに変更する
    int testNum = 0;

    private void Start()
    {
        DisplayMiniGame();
    }

    void DisplayMiniGame()
    {
        switch (testNum)
        {
            case Revasi:
                minigames[Revasi].SetActive(true);
                break;
            case Gomoku:
                minigames[Gomoku].SetActive(true);
                break;
        }
    }
}
