using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class buttonManager : MonoBehaviour
{
    private environmentLoader envLoaderScript;
    private persistentVariables persVarScript;
    private settingsManager settingSaverScript;

    public Animator selectorAnimator;

    public string[] selectionAnimatorTriggersInOrder;
    public int arrayCurrentNumber;

    public bool keyboardNavCheck;

    public bool isInSettings;
    public bool isInHelpMenu;
    public bool isInGame;

    public bool isPaused;

    public TMP_Dropdown resDropdown;
    public Resolution[] resolutions;

    public TMP_Dropdown qualityDropdown;

    private Vector2 fingerDownPosition;
    private Vector2 fingerUpPosition;
    private float minDistanceForSwipe;
    private bool detectSwipeOnlyAfterRelease;

    private void Awake()
    {
        isInGame = false;
        isInSettings = false;

        arrayCurrentNumber = 0;
    }

    private void Start()
    {
        envLoaderScript = FindObjectOfType<environmentLoader>();
        persVarScript = FindObjectOfType<persistentVariables>();
        settingSaverScript = FindObjectOfType<settingsManager>();

        checkAvailableResolutions();
        showQuality();
    }

    private void Update()
    {
        checkNavigationKeys();

        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                fingerUpPosition = touch.position;
                fingerDownPosition = touch.position;
            }

            if (!detectSwipeOnlyAfterRelease && touch.phase == TouchPhase.Moved)
            {
                fingerDownPosition = touch.position;
                DetectSwipe();
            }

            if (touch.phase == TouchPhase.Ended)
            {
                fingerDownPosition = touch.position;
                DetectSwipe();
            }
        }
    }


    public void DetectSwipe() {
        if (SwipeDistanceCheckMet())
        {
            if (IsVerticalSwipe())
            {
                var direction = fingerDownPosition.y - fingerUpPosition.y > 0 ? SwipeDirection.Up : SwipeDirection.Down;
                Debug.Log(direction);
            }
            else
            {
                var direction = fingerDownPosition.x - fingerUpPosition.x > 0 ? SwipeDirection.Right : SwipeDirection.Left;
                Debug.Log(direction);
            }
            fingerUpPosition = fingerDownPosition;
        }
    }
    

    private void checkNavigationKeys()
    {
        // the startMenuButtonsActive variable is controlled through the animationTracker script, to make sure no weird buttons are pressed
        if (keyboardNavCheck)
        {
            // for going down the menu
            if (Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow))
            {
                // have to minus 1, since the array stuff starts at 0 and not 1, took me a long time to figure out cos im stupid lmao
                if (arrayCurrentNumber != selectionAnimatorTriggersInOrder.Length - 1)
                {
                    // must reset trigger, or it'll spasm out n stuff lol
                    selectorAnimator.ResetTrigger(selectionAnimatorTriggersInOrder[arrayCurrentNumber]);

                    arrayCurrentNumber += 1;

                    updateSelection();
                }
            }

            // for going up the menu
            if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow))
            {
                if (arrayCurrentNumber != 0)
                {
                    // see above
                    selectorAnimator.ResetTrigger(selectionAnimatorTriggersInOrder[arrayCurrentNumber]);

                    arrayCurrentNumber -= 1;

                    updateSelection();
                }
            }

            if (Input.GetKeyUp(KeyCode.H))
            {
                envLoaderScript.showHelpScreen();
            }

            // for the loading of other 'areas', based on the current array count number
            // if (Input.touchCount > 0)
            // {
            //     if (arrayCurrentNumber == 0)
            //     {
            //         isInGame = true;
            //         envLoaderScript.beginGame();
            //     }

            //     if (arrayCurrentNumber == 1)
            //     {
            //         isInGame = false;
            //         envLoaderScript.showSettings();
            //     }

            //     if (arrayCurrentNumber == 2)
            //     {
            //         isInGame = false;
            //         envLoaderScript.showCredits();
            //     }

            //     if (arrayCurrentNumber == 3)
            //     {
            //         quitGame();
            //     }
            // }
        }

        if (isInSettings)
        {
            if (Input.GetKeyUp(KeyCode.Backspace) || Input.GetKeyUp(KeyCode.Escape))
            {
                envLoaderScript.settingsFinished();
            }
        }

        if (isInHelpMenu)
        {
            if (Input.GetKeyUp(KeyCode.Backspace) || Input.GetKeyUp(KeyCode.Escape))
            {
                envLoaderScript.helpScreenFinished();
            }
        }
    }

    private void updateSelection()
    {
        // sets the trigger stuff and actually invokes the animation transition
        selectorAnimator.SetTrigger(selectionAnimatorTriggersInOrder[arrayCurrentNumber]);
    }

    // relating to the settings menu stuff
    private void checkAvailableResolutions()
    {
        resolutions = Screen.resolutions;
        resDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResIndex = i;
            }
        }

        resDropdown.AddOptions(options);

        resDropdown.value = currentResIndex;
        resDropdown.RefreshShownValue();
    }

    // every time the player changes the resolution, it's saved to playerprefs to load later
    public void setRes(int resolutionIndex)
    {
        Resolution targetRes = resolutions[resolutionIndex];

        Screen.SetResolution(targetRes.width, targetRes.height, Screen.fullScreen);

        settingSaverScript.saveResSettings(targetRes.height, targetRes.width);
    }

    // this makes sure the quality thing in the menu display is correct
    private void showQuality()
    {
        int qualityIndex = QualitySettings.GetQualityLevel();
        qualityDropdown.value = qualityIndex;
        qualityDropdown.RefreshShownValue();
    }

    // this actually sets the quality when changed by a player
    public void setQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);

        settingSaverScript.saveQualitySettings(qualityIndex);
    }

    // generic quitgame thingy
    public void quitGame()
    {
        Application.Quit();
    }

    
    private bool IsVerticalSwipe()
    {
        return VerticalMovementDistance() > HorizontalMovementDistance();
    }

    private bool SwipeDistanceCheckMet()
    {
        return VerticalMovementDistance() > minDistanceForSwipe || HorizontalMovementDistance() > minDistanceForSwipe;
    }

    private float VerticalMovementDistance()
    {
        return Mathf.Abs(fingerDownPosition.y - fingerUpPosition.y);
    }

    private float HorizontalMovementDistance()
    {
        return Mathf.Abs(fingerDownPosition.x - fingerUpPosition.x);
    }


    public enum SwipeDirection
    {
        Up,
        Down,
        Left,
        Right
    }

}