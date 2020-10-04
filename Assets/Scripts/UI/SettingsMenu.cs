using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public TMP_Dropdown resolutionsDropdown;
    public TMP_Dropdown graphicsQualityDropdown;
    public Toggle fullScreenToggle;
    public Slider soundEffectVolumeSlider;
    public Slider musicVolumeSlider;

    public AudioMixer mainMixer;


    private Resolution[] resolutions = new Resolution[0];

    
    public void SetResolution(int index)
    {
        Screen.SetResolution(resolutions[index].width, resolutions[index].height, Screen.fullScreen);
    }

    public void SetQualityLevel(int level)
    {
        QualitySettings.SetQualityLevel(level);
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

    public void SetSoundEffectVolume(float volume)
    {
        mainMixer.SetFloat("soundEffectVolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        mainMixer.SetFloat("musicVolume", volume);
    }


    private void InitResolutions()
    {
        resolutions = Screen.resolutions;

        resolutionsDropdown.ClearOptions();

        List<string> options = new List<string>();

        Resolution currentResolution = Screen.currentResolution;
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; ++i)
        {
            string option = resolutions[i].ToString();
            options.Add(option);

            if (resolutions[i].width == currentResolution.width &&
                resolutions[i].height == currentResolution.height &&
                resolutions[i].refreshRate == currentResolution.refreshRate)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionsDropdown.AddOptions(options);
        resolutionsDropdown.value = currentResolutionIndex;
        resolutionsDropdown.RefreshShownValue();
    }

    private void InitGraphicsQuality()
    {
        string[] qualityNames = QualitySettings.names;

        graphicsQualityDropdown.ClearOptions();
        graphicsQualityDropdown.AddOptions(new List<string>(qualityNames));
        graphicsQualityDropdown.value = QualitySettings.GetQualityLevel();
        graphicsQualityDropdown.RefreshShownValue();
    }


    private void Start()
    {
        InitResolutions();
        InitGraphicsQuality();

        fullScreenToggle.isOn = Screen.fullScreen;

        mainMixer.GetFloat("soundEffectVolume", out float soundEffectVolume);
        soundEffectVolumeSlider.value = soundEffectVolume;

        mainMixer.GetFloat("musicVolume", out float musicVolume);
        musicVolumeSlider.value = musicVolume;
    }
}
