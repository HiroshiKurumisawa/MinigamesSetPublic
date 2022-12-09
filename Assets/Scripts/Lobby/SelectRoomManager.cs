using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SelectRoomManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI roomNameText;
    [SerializeField] TextMeshProUGUI roomUserCountText;

    string roomName;
    string roomUserEntryNum;
    string roomUserMaxNum;

    public void SetRoomData(string name, string entryNum, string maxNum)
    {
        roomName = name;
        roomUserEntryNum = entryNum;
        roomUserMaxNum = maxNum;

        roomNameText.text = roomName;
        roomUserCountText.text = roomUserEntryNum + "/" + roomUserMaxNum;
    }
}
