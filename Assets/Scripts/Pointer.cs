using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Pointer : MonoBehaviour
{
    GameObject thisSelectUI;

    private void Start()
    {
        thisSelectUI = this.gameObject;
    }

    // ポインター処理
    public void PointerEnter()
    {
        thisSelectUI.GetComponent<Image>().color = new Color(1f, 0.5f, 0f, 1f);
    }

    public void PointerExit()
    {
        thisSelectUI.GetComponent<Image>().color = new Color(0f, 0.5f, 1f, 1f);
    }
}
