using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SoundSystem;

public class RankingManager : NetworkBaseManager
{
    [SerializeField] GameObject rankingFormObj;
    [SerializeField] GameObject updateRankigFormButton;
    [SerializeField] GameObject waitTimeMessage;
    [SerializeField] GameObject accountUIprefab;                                        // アカウント(ランキング)UIのPrefab
    [SerializeField] GameObject rankigScrollView;                                       // ランキング表示されるScrollView;
    float waitTimeValue;                                                                // 更新時間の値
    const float waitTime = 10f;                                                         // 更新時間

    bool isUpdateRankingForm = false;
    bool isOpenRankingForm = false;
    bool isUpdateRankingFormWait = false;

    private void FixedUpdate()
    {
        UpdateRankingFormWaitTime(waitTime);
    }

    public void OpenRankingForm()
    {
        if (!isOpenRankingForm)
        {
            isOpenRankingForm = true;

            UpdateRankingFormWaitTimeReset();
            rankingFormObj.SetActive(true);
            StartCoroutine(UpdateRankingFormProcess(accountUIprefab, rankigScrollView));
        }
    }

    public void CloseRankingForm()
    {
        if (isOpenRankingForm)
        {
            isOpenRankingForm = false;
            isUpdateRankingForm = false;
            isUpdateRankingFormWait = false;
            updateRankigFormButton.GetComponent<Image>().color = new Color(0f, 0.5f, 1f, 1f);
            waitTimeMessage.GetComponent<TextMeshProUGUI>().text = "";
            rankingFormObj.SetActive(false);
        }
    }

    public void UpdateTotalRooms()
    {
        if (!isUpdateRankingForm)
        {
            isUpdateRankingForm = true;
            SoundManager.Instance.PlayOneShotSe("ui_click");
            StartCoroutine(UpdateRankingFormProcess(accountUIprefab, rankigScrollView));
        }
    }

    void UpdateRankingFormWaitTime(float time)
    {
        if (!isUpdateRankingForm) { return; }
        else
        {
            updateRankigFormButton.GetComponent<Image>().color = new Color(0f, 0.5f, 1f, 0.2f);
            if (!isUpdateRankingFormWait)
            {
                isUpdateRankingFormWait = true;
                waitTimeValue = time;
            }

            if (waitTimeValue <= 0)
            {
                UpdateRankingFormWaitTimeReset();
            }
            else
            {
                waitTimeValue -= Time.deltaTime;
                waitTimeMessage.GetComponent<TextMeshProUGUI>().text = "再更新可能まで\n" + waitTimeValue.ToString("00");
            }
        }
    }
    void UpdateRankingFormWaitTimeReset() // ルーム検索更新待機状態をリセットする
    {
        isUpdateRankingForm = false;
        isUpdateRankingFormWait = false;
        waitTimeMessage.GetComponent<TextMeshProUGUI>().text = "";
        updateRankigFormButton.GetComponent<Image>().color = new Color(0f, 0.5f, 1f, 1f);
    }
    // ポインター処理
    public void UpdateRoomButtonPointerEnter()
    {
        if (!isUpdateRankingForm)
        {
            SoundManager.Instance.PlayOneShotSe("ui_enter");
            updateRankigFormButton.GetComponent<Image>().color = new Color(1f, 0.5f, 0f, 1f);
        }
        else { return; }
    }
    public void UpdateRoomButtonPointerExit()
    {
        if (!isUpdateRankingForm)
        {
            updateRankigFormButton.GetComponent<Image>().color = new Color(0f, 0.5f, 1f, 1f);
        }
        else { return; }
    }
}
