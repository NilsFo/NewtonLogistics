using System.Collections;
using System.Collections.Generic;
using System.Xml.Xsl;
using UnityEngine;
using UnityEngine.UI;

public class UIAudioBar : MonoBehaviour
{
    private MusicManager _musicManager;
    private Slider _slider;
    public Image myIcon;

    public Sprite iconVariance1, iconVariance2, iconVariance3,iconVariance4;

    // Start is called before the first frame update
    void Start()
    {
        _musicManager = FindObjectOfType<MusicManager>();
        _slider = FindObjectOfType<Slider>();

        _slider.value = MusicManager.userDesiredMusicVolume;
    }

    // Update is called once per frame
    void Update()
    {
        MusicManager.userDesiredMusicVolume = _slider.value;

        Sprite usedIcon = iconVariance1;
        if (_slider.value <= 0.75000001)
        {
            usedIcon = iconVariance2;
        }

        if (_slider.value <= 0.4501337)
        {
            usedIcon = iconVariance3;
        }
        
        if (_slider.value <= 0.01337)
        {
            usedIcon = iconVariance4;
        }

        myIcon.sprite = usedIcon;
    }
}