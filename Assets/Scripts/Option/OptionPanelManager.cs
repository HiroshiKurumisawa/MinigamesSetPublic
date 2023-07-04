using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoundSystem;

public class OptionPanelManager : NetworkBaseManager
{
    #region インスタンス関係
    public static SaveManager Instance { get; private set; }
    #endregion

    [SerializeField] GameObject savePrefab;
    [SerializeField] GameObject optionPanel;
    [SerializeField] GameObject optionButton;
    [SerializeField] VolumeConfigUI volumeConfigUI;
    GameObject saveObj;
    SaveManager save;

    float volMaster = 0f, volBgm = 0f, volSe = 0f;
    bool isOpenOptionPanel = false;

    void Awake()
    {
        if (Instance == null)
        {
            saveObj = Instantiate(savePrefab);
            saveObj.name = "SaveManager";
            save = saveObj.GetComponent<SaveManager>();
        }

        if (!save.SaveDataCheck()) {save.CreatesaveData(); }
    }

    private void Start()
    {
        save.LoadData(ref volMaster, ref volBgm, ref volSe);
        volumeConfigUI.SetVolume(volMaster, volBgm, volSe);
        SoundManager.Instance.MasterVolume = volMaster;
        SoundManager.Instance.BGMVolume = volBgm;
        SoundManager.Instance.SEVolume = volSe;
        // ボリュームの設定
        volumeConfigUI.SetMasterSliderEvent(vol => SoundManager.Instance.MasterVolume = vol);
        volumeConfigUI.SetBGMSliderEvent(vol => SoundManager.Instance.BGMVolume = vol);
        volumeConfigUI.SetSeSliderEvent(vol => SoundManager.Instance.SEVolume = vol);
        // スライダーの数値反映
        volumeConfigUI.SetMasterVolume(volMaster);
        volumeConfigUI.SetBGMVolume(volBgm);
        volumeConfigUI.SetSeVolume(volSe);
    }

    public void OpenOptionPanel()
    {
        if (!isOpenOptionPanel)
        {
            isOpenOptionPanel = true;
            UIopen(optionPanel, NoActionCol());
            optionButton.SetActive(false);
            save.LoadData(ref volMaster, ref volBgm, ref volSe);
        }

    }

    public void CloseOptionPanel()
    {
        if (isOpenOptionPanel)
        {
            isOpenOptionPanel = false;
            save.Save(volumeConfigUI.MasterSilider, volumeConfigUI.BgmSlider, volumeConfigUI.SeSlider);
            UIclose(optionPanel);
            optionButton.SetActive(true);
        }
    }
}
