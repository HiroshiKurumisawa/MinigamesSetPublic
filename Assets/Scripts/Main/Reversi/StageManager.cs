using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StageManager : MonoBehaviour, IPointerClickHandler
{
    ReversiManager reversiManagerCS;

    private void Start()
    {
        reversiManagerCS = GameObject.FindObjectOfType<ReversiManager>();
    }

    public void OnPointerClick(PointerEventData pointerData)
    {
        reversiManagerCS.PutStoneOrPass(pointerData);
    }
}
