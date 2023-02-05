using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using SoundSystem;

public class VolumeConfigUI : MonoBehaviour
{
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider bgmSlider;
    [SerializeField] Slider seSlider;

    private void Start()
    {
        OpenOptionPanel();
    }

    private void OpenOptionPanel()
    {
        // �{�����[���̐ݒ�
        SetMasterSliderEvent(vol => SoundManager.Instance.MasterVolume = vol);
        SetBGMSliderEvent(vol => SoundManager.Instance.BGMVolume = vol);
        SetSeSliderEvent(vol => SoundManager.Instance.SEVolume = vol);
        // �X���C�_�[�̐��l���f
        SetMasterVolume(SoundManager.Instance.MasterVolume);
        SetBGMVolume(SoundManager.Instance.BGMVolume);
        SetSeVolume(SoundManager.Instance.SEVolume);
    }

    // �X���C�_�[�̈ʒu���{�����[���ɍ��킹�ăZ�b�g
    public void SetMasterVolume(float masterVolume)
    {
        masterSlider.value = masterVolume;
    }
    public void SetBGMVolume(float bgmVolume)
    {
        bgmSlider.value = bgmVolume;

    }
    public void SetSeVolume(float seVolume)
    {
        seSlider.value = seVolume;

    }

    // �X���C�_�[�ɕύX����������l�𔽉f������(�C�x���g)
    public void SetMasterSliderEvent(UnityAction<float> sliderCallback)
    {
        SetVolumeChagedEvent(masterSlider, sliderCallback);
    }
    public void SetBGMSliderEvent(UnityAction<float> sliderCallback)
    {
        SetVolumeChagedEvent(bgmSlider, sliderCallback);
    }
    public void SetSeSliderEvent(UnityAction<float> sliderCallback)
    {
        SetVolumeChagedEvent(seSlider, sliderCallback);
    }
    void SetVolumeChagedEvent(Slider slider, UnityAction<float> sliderCallback)
    {
        if (slider.onValueChanged != null)
        {
            slider.onValueChanged.RemoveAllListeners();
        }
        slider.onValueChanged.AddListener(sliderCallback);
    }
}
