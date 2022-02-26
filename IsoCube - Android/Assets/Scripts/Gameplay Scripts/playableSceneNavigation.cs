using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class playableSceneNavigation : MonoBehaviour
{
    public string nameOfMainMenuScene;
    public bool keyboardNavigationActive;

    private UIUpdater uiUpdaterScript;

    private void Start()
    {
        uiUpdaterScript = FindObjectOfType<UIUpdater>();
    }

    // Update is called once per frame
    private void Update()
    {
        checkNavKeys();
    }

    private void checkNavKeys()
    {
        if (keyboardNavigationActive)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // just reloads scene to play again
                SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
            }

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                // quits the game, used function because it's cleaner
                SceneManager.LoadSceneAsync(nameOfMainMenuScene);
            }

            if (Input.GetKeyDown(KeyCode.Delete))
            {
                // resets the high score
                uiUpdaterScript.resetHighScore();
            }
        }
    }
}
