using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainManager : MonoBehaviour
{
    [SerializeField,Header("�\������Q�[���̃I�u�W�F�N�g")]
    GameObject[] minigames;

    const int Revasi = 0;

    // �T�[�o�[����擾���鐔�l�ɂ̂��ɕύX����
    int testNum = 0;

    void Start()
    {
        DisplayMiniGame();
    }

    void Update()
    {
        
    }

    void DisplayMiniGame()
    {
        switch(testNum)
        {
            case Revasi:
                minigames[Revasi].SetActive(true);
                break;
        }
    }
}
