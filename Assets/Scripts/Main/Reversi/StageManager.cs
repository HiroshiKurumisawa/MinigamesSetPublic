using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    ReversiManager reversiManagerCS;

    private void Start()
    {
        reversiManagerCS = GameObject.FindObjectOfType<ReversiManager>();
    }


}
