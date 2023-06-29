using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainManager : MonoBehaviour
{
    [SerializeField, Header("表示するゲームのオブジェクト")]
    GameObject[] minigames;

    const int Revasi = 0, Gomoku = 1;

    RoomDataManager roomDataManagerCS;

    private void Start()
    {
        roomDataManagerCS = GameObject.FindObjectOfType<RoomDataManager>();
        DisplayMiniGame();
    }

    void DisplayMiniGame()
    {
        switch (int.Parse(roomDataManagerCS.Game_rule))
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
