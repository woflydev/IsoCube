using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class gameManager : MonoBehaviour
{
    private bool gameHasEnded = false;

    public Transform player;

    // defining all the scripts so i can actually use them lol
    private UIUpdater uiUpdaterScript;
    private playerMovements playerMovementsScript;
    private playableSceneNavigation playableNavScript;

    public GameObject completeLevelUI;
    public GameObject loseLevelUI;

    public float stopPlayerDelay;
    public float restartDelay;
    public float loseUITextSwitchDelay;

    private void Start()
    {
        uiUpdaterScript = FindObjectOfType<UIUpdater>();
        playerMovementsScript = FindObjectOfType<playerMovements>();
        playableNavScript = FindObjectOfType<playableSceneNavigation>();

        gameHasEnded = false;
    }

    // this enables all the necessary stuff eeeee
    public void startSequence()
    {
        loseLevelUI.SetActive(false);
        playerMovementsScript.enabled = true;
        enableInGameUI();
    }

    public void gameOverSequence()
    {
        if (!gameHasEnded)
        {
            // makes sure that the Restart() func. is only called once with a boolean. this is reset when the scene is reloaded.
            gameHasEnded = true;

            // stops player from pressing anymore keys, then stops the force after stopPlayerDelay seconds
            disablePlayerInput();
            Invoke("disablePlayerMovement", stopPlayerDelay);

            // stops the score from counting upwards, turns off the XYZ coords and live score, and shows the 'you lose' GUI.
            disableScoreCount();
            disableInGameUI();
            loseLevelUI.SetActive(true);

            // updates the score from the UIUpdaterScript
            uiUpdaterScript.updateDeathScreen();

            // triggers the switch of text, then enables the keyboard navigation from the lose menu
            Invoke("triggerLoseUITextChange", loseUITextSwitchDelay);
            Invoke("enableKeyboardUINavigation", restartDelay);
        }
    }

    // for completing the level
    public void completeLevel()
    {
        disablePlayerInput();
        disableScoreCount();
        completeLevelUI.SetActive(true);
    }

    // misc functions for different usages
    //relating to level loading and scenes
    public void loadNext()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // relating to UI
    public void disableScoreCount()
    {
        // stops the stopwatch
        uiUpdaterScript.scoreCounterActive = false;
    }

    public void enableScoreCount()
    {
        uiUpdaterScript.scoreCounterActive = true;
    }

    public void disableInGameUI()
    {
        uiUpdaterScript.inGameUIIsVisible(false);
    }

    public void enableInGameUI()
    {
        uiUpdaterScript.inGameUIIsVisible(true);
    }

    public void disableKeyboardUINavigation()
    {
        playableNavScript.keyboardNavigationActive = false;
    }

    public void enableKeyboardUINavigation()
    {
        playableNavScript.keyboardNavigationActive = true;
    }

    public void triggerLoseUITextChange()
    {
        uiUpdaterScript.changeTextInLoseUI();
    }


    // relating to the player
    public void disablePlayerInput()
    {
        playerMovementsScript.playerInputEnabled = false;
    }

    public void enablePlayerInput()
    {
        playerMovementsScript.playerInputEnabled = true;
    }

    public void disablePlayerMovement()
    {
        playerMovementsScript.enabled = false;
    }

    public void enablePlayerMovement()
    {
        playerMovementsScript.enabled = true;
    }

    public void quitGame()
    {
        Application.Quit();
    }

    public void backToMenu()
    {
        // loads first scene in the build index, is presumed to be the main menu
        SceneManager.LoadScene(0);
    }
}
