using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class pauseControl : MonoBehaviour
{
    public static pauseControl instance;
    private environmentLoader envLoaderScript;

    public bool isPaused;
    public bool pauseAllowed;
    public bool isInGame;

    public GameObject pauseMenu;
    private UIUpdater uiUpdaterScript;

    private void Awake()
    {
        uiUpdaterScript = FindObjectOfType<UIUpdater>();
        envLoaderScript = FindObjectOfType<environmentLoader>();

        instance = this;

        isPaused = false;
        pauseAllowed = true;
    }

    private void Start()
    {
        pauseMenu.SetActive(false);
    }

    private void Update()
    {
        if (isInGame && pauseAllowed)
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                isPaused = !isPaused;
                pauseGameControl();
            }
        }

        if (Input.GetKeyUp(KeyCode.Backspace) && isPaused)
        {
            Time.timeScale = 1;
            SceneManager.LoadSceneAsync(0);
        }
    }

    // pausing the game and resuming it
    public void pauseGameControl()
    {
        if (isPaused)
        {
            pauseMenu.SetActive(true);
            uiUpdaterScript.inGameUIIsVisible(false);
            Time.timeScale = 0f;
        }

        else
        {
            pauseMenu.SetActive(false);
            uiUpdaterScript.inGameUIIsVisible(true);
            Time.timeScale = 1;
        }
    }
}
