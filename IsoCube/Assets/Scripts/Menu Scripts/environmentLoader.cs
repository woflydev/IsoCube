using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using TMPro;

public class environmentLoader : MonoBehaviour
{
    private persistentVariables persVar;
    buttonManager btnMngr;

    public PlayableDirector directorForStart;

    public PlayableDirector directorForCredits;

    public Camera cutSceneCamera;
    public Camera menuCamera;

    public GameObject[] cutSceneObjects;
    public GameObject[] menuObjects;

    public string[] sceneNames;

    public GameObject startMenu;
    public GameObject settingsMenu;
    public GameObject creditMenu;
    public GameObject helpMenu;

    public GameObject loadingScreen;
    public TextMeshProUGUI humourLoadingText;
    public TextMeshProUGUI progressText;
    public Slider loadingBar;

    public string[] humourText;

    private void Awake()
    {
        persVar = FindObjectOfType<persistentVariables>();
        btnMngr = FindObjectOfType<buttonManager>();

        loadSettings();
    }

    private void Start()
    {
        creditMenu.SetActive(false);
        loadingScreen.SetActive(false);

        startCheck();
    }

    public void loadSettings()
    {
        
    }

    // this is called every single time the scene is loaded
    public void startCheck()
    {
        if (!persVar.startCutscenePlayed)
        {
            btnMngr.keyboardNavCheck = false;

            executeActionArray(cutSceneObjects, true);
            executeActionArray(menuObjects, false);

            // plays the actual start cutscene
            directorForStart.Play();

            cutSceneCamera.enabled = true;
            menuCamera.enabled = false;

            //cannot toggle the persVar.startCutscenePlayed variable here, since it happens on the same frame as the below if function
        }

        if (persVar.startCutscenePlayed)
        {
            loadCutsceneObjectPosition();
            btnMngr.keyboardNavCheck = true;
        }
    }

    // this is toggled through finishing the starting cutscene (signal receiver)
    public void enableMenuInput()
    {
        btnMngr.keyboardNavCheck = true;

        // sets it to true so that it doesn't play again
        persVar.startCutscenePlayed = true;
    }

    // this will only be called once
    public void saveCutsceneObjectPosition()
    {
        for (int iteration = 0; iteration < cutSceneObjects.Length; iteration++)
        {
            if (!persVar.cutscenePositionSaveDictionary.ContainsKey(iteration))
            {
                persVar.cutscenePositionSaveDictionary.Add(iteration, cutSceneObjects[iteration].transform.position);
            }

            if (!persVar.cutsceneRotationSaveDictionary.ContainsKey(iteration))
            {
                persVar.cutsceneRotationSaveDictionary.Add(iteration, cutSceneObjects[iteration].transform.rotation);
            }
        }

        persVar.cutscenePositionSaveDictionary.Add(cutSceneObjects.Length + 1, cutSceneCamera.transform.position);
        persVar.cutsceneRotationSaveDictionary.Add(cutSceneObjects.Length + 1, cutSceneCamera.transform.rotation);

        loadCutsceneObjectPosition();
    }

    public void loadCutsceneObjectPosition()
    {
        for (int iteration = 0; iteration < menuObjects.Length; iteration++)
        {
            if (persVar.cutscenePositionSaveDictionary.ContainsKey(iteration))
            {
                menuObjects[iteration].transform.position = persVar.cutscenePositionSaveDictionary[iteration];
            }

            if (persVar.cutsceneRotationSaveDictionary.ContainsKey(iteration))
            {
                menuObjects[iteration].transform.rotation = persVar.cutsceneRotationSaveDictionary[iteration];
            }
        }

        menuCamera.transform.position = persVar.cutscenePositionSaveDictionary[menuObjects.Length + 1];
        menuCamera.transform.rotation = persVar.cutsceneRotationSaveDictionary[menuObjects.Length + 1];

        switchObjects();
    }

    public void switchObjects()
    {
        // sets the menu camera to the exact same place as the cutscene camera, then disables the cutscene one to make it even lesssss confusing
        menuCamera.enabled = true;
        cutSceneCamera.enabled = false;

        executeActionArray(menuObjects, true);
        executeActionArray(cutSceneObjects, false);
    }

    // function to help with repetitive stuffs
    public void executeActionArray(GameObject[] array, bool state)
    {
        foreach (GameObject obj in array)
        {
            obj.SetActive(state);
        }
    }

    public void showMainMenu()  
    {
        StartCoroutine(loadingAsync(sceneNames[0]));
    }

    // the settings and stuff will be in the same scene, don't need to change or use the scenemanagement
    public void showSettings()
    {
        startMenu.SetActive(false);
        settingsMenu.SetActive(true);

        btnMngr.keyboardNavCheck = false;
        btnMngr.isInSettings = true;

        // this makes it so that when user exits from settings menu, selection arrow defaults to the begin thingy
        btnMngr.arrayCurrentNumber = 0;
    }

    public void settingsFinished()
    {
        startMenu.SetActive(true);
        settingsMenu.SetActive(false);

        btnMngr.keyboardNavCheck = true;
        btnMngr.isInSettings = false;
    }

    public void showCredits()
    {
        creditMenu.SetActive(true);
        directorForCredits.Play();

        btnMngr.keyboardNavCheck = false;
    }

    public void creditsTimelineFinished()
    {
        directorForCredits.Stop();
        creditMenu.SetActive(false);

        btnMngr.keyboardNavCheck = true;
    }

    public void showHelpScreen()
    {
        startMenu.SetActive(false);
        helpMenu.SetActive(true);

        btnMngr.keyboardNavCheck = false;
        btnMngr.isInHelpMenu = true;
    }

    public void helpScreenFinished()
    {
        startMenu.SetActive(true);
        helpMenu.SetActive(false);

        btnMngr.keyboardNavCheck = true;
        btnMngr.isInHelpMenu = false;
    }
    public void beginGame()
    {
        startMenu.SetActive(false);
        StartCoroutine(loadingAsync(sceneNames[1]));
    }

    private IEnumerator loadingAsync(string name)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(name);

        humourLoadingText.text = humourText[Random.Range(0, humourText.Length)];

        while (!operation.isDone)
        {
            float progressValue = Mathf.Clamp01(operation.progress / 0.9f);

            progressText.text = progressValue * 100 + "%";

            loadingScreen.SetActive(true);
            loadingBar.value = progressValue;

            yield return null;
        }
    }
}
