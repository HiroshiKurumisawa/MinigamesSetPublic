using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GomokuStageManager : MonoBehaviour, IPointerClickHandler
{
     GomokuManager gomokuManagerCS;

    private void Start()
    {
        gomokuManagerCS = GameObject.FindObjectOfType<GomokuManager>();
    }

    public void OnPointerClick(PointerEventData pointerData)
    {
        gomokuManagerCS.PutStoneOrPass(pointerData);
    }
}
