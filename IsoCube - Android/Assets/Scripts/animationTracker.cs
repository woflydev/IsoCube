using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class animationTracker : MonoBehaviour
{
    private static animationTracker instance = null;
    private buttonManager btnMngrScript;
    public bool startMenuButtonsActive;

    public Animator startAnim;

    public GameObject[] uiToBeHidden;

    public bool startAnimationPlayed;

    public string nameOfMenuScene;

    // this script will not be destroyed, and will keep track of whether the animation has been played or not
    // every time the scene is reloaded, it will check if there's already an instance of the script, and if so, delete the new one and keep the old one.
    private void Awake()
    {
        btnMngrScript = FindObjectOfType<buttonManager>();

        startMenuButtonsActive = false;

        // sets the animation object to this one, so that it won't be destroyed on loading another
        startAnim.transform.SetParent(transform);

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
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

    // Start is called before the first frame update
    private void Start()
    {
        // sets the animation played to false, since this script will only be initialized once (i.e. the start() function will only be called once)
        startAnimationPlayed = false;

        if (!startAnimationPlayed)
        {
            StartCoroutine(onCompleteStartAnimation());
        }
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == nameOfMenuScene)
        {
            executeAction(true);

            startMenuButtonsActive = true;
        }

        else
        {
            executeAction(false);

            startMenuButtonsActive = false;
        }
    }

    private void executeAction(bool state)
    {
        // iterates through every single different object stored within UITOBEHIDDEN and sets it to active
        foreach (GameObject obj in uiToBeHidden)
        {
            obj.SetActive(state);
        }
    }

    // this waits until the beginning animation has finished, then does stuff
    private IEnumerator onCompleteStartAnimation()
    {
        while (startAnim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }

        startAnim.speed = 0;

        startMenuButtonsActive = true;
        btnMngrScript.keyboardNavCheck = true;
    }
}
