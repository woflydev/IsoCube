using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// this script stores all the information that needs to be consistent throughout scenes
public class persistentVariables : MonoBehaviour
{
    private static persistentVariables instance = null;
    private settingsManager settingsMgr;

    public bool bloomOn;

    public bool startCutscenePlayed;

    public Dictionary<int, Vector3> cutscenePositionSaveDictionary = new Dictionary<int, Vector3>();
    public Dictionary<int, Quaternion> cutsceneRotationSaveDictionary = new Dictionary<int, Quaternion>();

    // this awake function is only called once
    // it handles all the instances (i.e. on loading the original scene, another copy of this script will be made, but then deleted by this section of code)
    // every time it awakes (meaning a change of scene), the script loads all settings from playerprefs
    private void Awake()
    {
        settingsMgr = FindObjectOfType<settingsManager>();
        settingsMgr.loadAllSettings();

        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(gameObject);
            startCutscenePlayed = false;
        }

        else
        {
            if (instance != this)
            {
                DestroyImmediate(this.gameObject);

                return;
            }
        }
    }
}
