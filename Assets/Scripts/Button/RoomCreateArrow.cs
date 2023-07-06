using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SoundSystem;
using DG.Tweening;

public class RoomCreateArrow : MonoBehaviour
{
    GameObject thisSelectUI;

    private void Start()
    {
        thisSelectUI = this.gameObject;
    }

    // ポインター処理
    public void PointerEnter()
    {
        thisSelectUI.GetComponent<Image>().DOColor(Color.yellow, 0.2f).SetLink(gameObject);
        SoundManager.Instance.PlayOneShotSe("ui_enter");
    }

    public void PointerExit()
    {
        thisSelectUI.GetComponent<Image>().DOColor(Color.white, 0.2f).SetLink(gameObject);
    }

    public void DecisionButton()
    {
        SoundManager.Instance.PlayOneShotSe("ui_click");
    }

    public void CancelButton()
    {
        SoundManager.Instance.PlayOneShotSe("ui_cancel");
    }
}
