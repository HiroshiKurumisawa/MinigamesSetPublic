using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class VolumeConfigUI : MonoBehaviour
{
    [SerializeField] Slider masterSlider;
    public float MasterSilider { get { return masterSlider.value; } private set { masterSlider.value = value; } }
    [SerializeField] Slider bgmSlider;
    public float BgmSlider { get { return bgmSlider.value; } private set { bgmSlider.value = value; } }
    [SerializeField] Slider seSlider;
    public float SeSlider { get { return seSlider.value; } private set { seSlider.value = value; } }

    // スライダーの位置をボリュームに合わせてセット
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

    public void SetVolume(float vm, float vb, float vs)
    {
        MasterSilider = vm;
        BgmSlider = vb;
        SeSlider = vs;
    }

    // スライダーに変更があったら値を反映させる(イベント)
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
