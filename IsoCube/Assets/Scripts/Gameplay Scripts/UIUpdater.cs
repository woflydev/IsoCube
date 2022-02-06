using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UIUpdater : MonoBehaviour
{
    private playerMovements playerMovementScript;

    // for player stuff
    public GameObject player;

    // DONT CHANGE THIS UNLESS YOU KNOW WHAT YOU'RE DOING
    public float playerNormalSpeed;

    // for the in-game UI
    public GameObject inGameUI;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI playerCoordinates;
    public TextMeshProUGUI dashCounterText;

    // for fps showing and debugging/performance purposes
    public TextMeshProUGUI fpsText;
    public float FPSRefreshRate;
    private bool fpsCounterActive;

    // for the score stuff
    private Vector3 startPos;
    public float scoreMultiplier = 5f;
    public float scoreDivider;
    public float scoreAdditiveOverTime = 0.001f;
    private float scoreVariable;

    public bool scoreCounterActive = true;

    private float currentScore;

    // for bonuses and stuff like that
    public float minBonusCoolTime;
    public float maxBonusCoolTime;
    private float nextBonusTime;

    public float minAirtimeBonus;
    public float maxAirtimeBonus;

    public string[] bonusMessageArray;
    public string bonusMessageSuffix;
    public GameObject bonusTextObject;
    public Animator bonusTextAnimator;

    private float preventBonusAtStartTime;

    // for the loseGameUI
    public GameObject confirmDelHighScore;

    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI highScoreMessageText;
    public GameObject deathTextObject;
    public TextMeshProUGUI deathText;

    // for the animations and text switching etc
    public Animator deathTextAnimator;
    public Animator scoreTextAnimator;
    public Animator highScoreAnimator;
    public float textSwitchDelay;

    // high score UI messages
    public string newHighScoreMsg;
    public string highScoreMsg;

    // for death. yay! glorious death.
    public string deathPrefix;
    public string[] deathMessages;

    private void Awake()
    {
        // sets the startpos the moment the script is initialized
        startPos = player.transform.position;

        bonusTextObject.GetComponent<TextMeshProUGUI>().text = "";
    }

    // Start is called before the first frame update
    private void Start()
    {
        scoreCounterActive = false;

        // assigns the movement script so that we can actually useeeeeeee it
        playerMovementScript = FindObjectOfType<playerMovements>();

        // prevents the thing from doing weird spasms with the score
        preventBonusAtStartTime = 2f;
         
        // sets the ingameUI to visible in case it was manually set to inactive during scene making or smth
        inGameUIIsVisible(true);
    }

    // Update is called once per frame
    private void Update()
    {
        // see function for details
        updateInGameHUD();
        FPSUpdater();
    }

    private void FPSUpdater()
    {
        if (!pauseControl.instance.isPaused && !fpsCounterActive && inGameUI.activeInHierarchy)
        {
            InvokeRepeating(nameof(refreshFPS), 1, FPSRefreshRate);
            fpsCounterActive = true;
        }

        else
        {
            if (pauseControl.instance.isPaused || !inGameUI.activeInHierarchy)
            {
                CancelInvoke(nameof(refreshFPS));
                fpsCounterActive = false;
            }
        }
    }

    private void refreshFPS()
    {
        int fps = (int)(1f / Time.deltaTime);
        fpsText.text = "FPS: " + fps;
    }

    private void updateInGameHUD()
    {
        if (scoreCounterActive && !pauseControl.instance.isPaused)
        {
            float airtimeBonus = 0;

            // for airtime, bonus, using the dash for extra points, etc.
            if (player.transform.position.y >= 3 && Time.time >= preventBonusAtStartTime && Time.time >= nextBonusTime)
            {
                // randomly chooses a bonus amount from predetermined range
                airtimeBonus += Random.Range(minAirtimeBonus, maxAirtimeBonus);

                // randomly chooses a bonus message from the array
                string bonusMsg = bonusMessageArray[Random.Range(0, bonusMessageArray.Length)] + bonusMessageSuffix + " ";

                // actually displays that via a functino and passing in params
                displayBonusText(bonusMsg, airtimeBonus);

                // sets the next bonus time so we won't be spammed with a bunch of stuff
                nextBonusTime = Time.time + Random.Range(minBonusCoolTime, maxBonusCoolTime);
            }

            currentScore += airtimeBonus + scoreMultiplier;

            // updates the score text referenced in the UI
            scoreText.text = currentScore.ToString("0");
            scoreVariable = currentScore;

            // adjusts the scoreAdditiveOverTime with the player's velocity (how fast they're moving or whether they're using the dash function)
            scoreMultiplier += scoreAdditiveOverTime / scoreDivider;
        }

        // shows the stuff in the text box
        playerCoordinates.text = player.transform.position.ToString("0");

        // updates the dash counter
        dashCounterText.text = playerMovementScript.dashCounter.ToString("0");

        //coolDownSlider.value = playerMovementScript.dashCoolDownTime / 100f;
    }

    public void displayBonusText(string type, float amount)
    {
        if (Time.time > preventBonusAtStartTime)
        {
            bonusTextObject.SetActive(true);
            bonusTextObject.GetComponent<TextMeshProUGUI>().text = type + "+" + Mathf.RoundToInt(amount);

            bonusTextAnimator.SetTrigger("startBonusFlash");
        }
    }


    public void updateDeathScreen()
    {
        // doesn't allow to pause anymore, since that would be weird.
        pauseControl.instance.pauseAllowed = false;

        string conjoinedDeathText = deathPrefix + deathMessages[Random.Range(0, deathMessages.Length)];

        // sets it to true (active) to avoid a stupid error if it was disabled in the editor for UI editing
        deathTextObject.SetActive(true);
        deathText.text = conjoinedDeathText;

        // sets the final score (on the death screen) to the score previously displayed in the inGameUI
        finalScoreText.text = scoreText.text;

        updateHighScore();
    }

    private void updateHighScore()
    {
        float tempHighScore;
        // this is if no other high score has been set on the client computer before.
        try
        {
            // attempts to fetch high score
            tempHighScore = PlayerPrefs.GetFloat("highScore", 0);

            // for the highscore saving and stuff itself
            if (Mathf.RoundToInt(scoreVariable) > tempHighScore)
            {
                // sets the highscore value to the current score, if it's bigger than the previous highscore
                PlayerPrefs.SetFloat("highScore", Mathf.RoundToInt(scoreVariable));

                highScoreMessageText.text = newHighScoreMsg;
                highScoreText.text = scoreVariable.ToString("0");
            }

            else
            {
                // actually disaplays the highscore as well as the appropriate message, see below as well for another example
                highScoreMessageText.text = highScoreMsg;
                highScoreText.text = tempHighScore.ToString("0");
            }
            
        }

        catch
        {
            // making the highscore playerprefs thingy
            PlayerPrefs.SetFloat("highScore", Mathf.RoundToInt(scoreVariable));
            // saving the values in case of application crash
            PlayerPrefs.Save();

            highScoreMessageText.text = newHighScoreMsg;
            highScoreText.text = scoreVariable.ToString("0");
        }
    }

    public void resetHighScore()
    {
        // resets the stored highscore value
        PlayerPrefs.SetFloat("highScore", 0);

        // technically this will already be set active (with the lose game panel and canvas), but just in case so no stupid errors.
        confirmDelHighScore.SetActive(true);
        highScoreAnimator.SetBool("reset", true);
    }

    public void resetCurrentScore()
    {
        currentScore = 0;
    }

    public void resetLoseUI()
    {
        deathTextAnimator.SetBool("deathTextMove", false);
        scoreTextAnimator.SetBool("scoreTextFadeIn", false);
    }

    // misc functions for setting and moving ui around with animations and stuff
    public void changeTextInLoseUI()
    {
        deathTextAnimator.SetBool("deathTextMove", true);
        delayExecution(textSwitchDelay);
        scoreTextAnimator.SetBool("scoreTextFadeIn", true);
    }

    public void inGameUIIsVisible(bool isVisible)
    {
        inGameUI.SetActive(isVisible);
        scoreCounterActive = true;
    }

    // this is just a local 'wait' function so i dont have to make a new one every single time
    private IEnumerator delayExecution(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
}
