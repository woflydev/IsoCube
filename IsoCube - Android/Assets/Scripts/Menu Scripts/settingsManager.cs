using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class settingsManager : MonoBehaviour
{
    public PostProcessVolume volume;
    private Bloom bloomEffect;
    private Vignette vignetteEffect;

    public Toggle fullScreenToggle;
    public Toggle bloomToggle;
    public Toggle vignetteToggle;

    public void loadAllSettings()
    {
        volume = FindObjectOfType<PostProcessVolume>();

        getResSettings();
        getQualitySettings();
        getBloomSettings();
        getVignetteSettings();
        updateFSToggle();
    }

    public void getBloomSettings()
    {
        bool value = intToBool(PlayerPrefs.GetInt("bloom"));

        volume.profile.TryGetSettings<Bloom>(out bloomEffect);
        bloomEffect.active = value;
        bloomToggle.isOn = value;
    }

    public void saveBloomSettings(bool value)
    {
        PlayerPrefs.SetInt("bloom", boolToInt(value));
        volume.profile.TryGetSettings<Bloom>(out bloomEffect);
        bloomEffect.active = value;
        bloomToggle.isOn = value;
    }

    public void getVignetteSettings()
    {
        bool value = intToBool(PlayerPrefs.GetInt("vignette"));

        volume.profile.TryGetSettings<Vignette>(out vignetteEffect);
        vignetteEffect.active = value;
        vignetteToggle.isOn = value;
    }

    public void saveVignetteSettings(bool value)
    {
        PlayerPrefs.SetInt("vignette", boolToInt(value));
        volume.profile.TryGetSettings<Vignette>(out vignetteEffect);
        vignetteEffect.active = value;
        vignetteToggle.isOn = value;
    }

    public void getResSettings()
    {
        Screen.SetResolution(PlayerPrefs.GetInt("resWidth", 0), PlayerPrefs.GetInt("resHeight", 0), Screen.fullScreen);

        // if nothing has been saved yet, save the current res, then just load the height and width from playerprefs
        if (PlayerPrefs.GetInt("resHeight") == 0)
        {
            saveResSettings(Screen.width, Screen.height);
        }

        else
        {
            PlayerPrefs.GetInt("resHeight", 0);
            PlayerPrefs.GetInt("resWidth", 0);
        }
    }

    public void saveResSettings(int width, int height)
    {
        PlayerPrefs.SetInt("resHeight", height);
        PlayerPrefs.SetInt("resWidth", width);
    }

    public void getQualitySettings()
    {
        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("qualityIndex", 0));
    }

    public void saveQualitySettings(int index)
    {
        PlayerPrefs.SetInt("qualityIndex", index);
    }

    public void saveFullScreenState(bool value)
    {
        PlayerPrefs.SetInt("fs", boolToInt(value));

        Screen.fullScreen = value;
    }

    public void updateFSToggle()
    {
        bool inFullScreen = intToBool(PlayerPrefs.GetInt("fs"));

        fullScreenToggle.isOn = inFullScreen;
        Screen.fullScreen = inFullScreen;
    }

    private int boolToInt(bool input)
    {
        if (input)
        {
            return 1;
        }

        else
        {
            return 0;
        }
    }

    private bool intToBool(int input)
    {
        if (input != 0)
        {
            return true;
        }

        else
        {
            return false;
        }
    }
}
