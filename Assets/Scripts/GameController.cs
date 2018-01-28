using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// This is the high level state manager for the game. It controls the setup of the body,
// the player's life counter, and win / end state.
public class GameController : MonoBehaviour {

    BodyController body;
	/*** UI Labels ***/


    [Header("Menus")]
    [SerializeField]
    private GameObject TitleScreen;
    [SerializeField]
    private GameObject GameFinishedScreen;

    [Header("Game object")]
    [SerializeField]
    private GameObject MainGameObjs;

	/*** UI Labels ***/
    [Header("UI Labels")]
    [SerializeField]
	private Text healthText;
    [SerializeField]
	private Text timeText;
    [SerializeField]
    private Text heartRateText;

    [SerializeField]
    private Heartbeat HeartScript;
	/*** Game State ***/

	// Game constants
	const float kTotalTime = 60.0f;
	const int kHeartCount = 3;
    private SoundController sound;

	// Starting time of the game
	float startingTime;

   public bool isGameFinished = true;

	// Each game starts with 3 wrong moves. On the third, game is over
	int heartCount;

    /*** Unity Methods ***/

    void Start()
    {
        GameObject gameController = GameObject.FindGameObjectWithTag("GameController");
        //TODO: Initialize Body Controller	
        body = gameController.GetComponent(typeof(BodyController)) as BodyController;
        sound = gameController.GetComponent(typeof(SoundController)) as SoundController;
    }

    // Update is called once per frame
    void Update()
    {

        if (isGameFinished)
        {
            return;
        }
        // If the patient ever has no heartCount left, game over
        float secondsLeft = kTotalTime - (Time.time - startingTime);
        if (secondsLeft < 0) { secondsLeft = 0; }
		if ( heartCount <= 0 || secondsLeft <= 0.0f)
        {

            healthText.text = "DEAD";
            timeText.text = "00:00.00";
            StartCoroutine(GoToGameFinishedMenu());
        }
		// Else, win!
		else if( body.IsFullyHealed() )
		{
			// TODO: WIN WIN WIN!
		}
        // Else not dead, keep playing
        else
        {
            // Update health
            healthText.text = "heartCount left: " + heartCount;

            double minutesLeft = Mathf.Floor(secondsLeft / 60.0f);
            int fractionsLeft = (int)((secondsLeft - Mathf.Floor(secondsLeft)) * 100.0f);
            if (minutesLeft < 0) { minutesLeft = 0; }
            if (fractionsLeft < 0) { fractionsLeft = 0; }
            if (secondsLeft < 0) { secondsLeft = 0; }
            
			timeText.text = string.Format("{0}:{1}.{2}", minutesLeft.ToString("00"), ((int)secondsLeft % 60).ToString("00"), fractionsLeft.ToString("00"));
        }
    }

	// Called by BodyController when player fails to do the right application of medicine
	public void FailedCureAttempt()
	{
		heartCount--;
	}

	public void OnDebugClick( GameObject sender )
	{
		if (sender.tag.CompareTo ("RightButton") == 0) {
			// Do nothing yet..
		} else if (sender.tag.CompareTo ("WrongButton") == 0) {

			// Patient loses a heart
			heartCount--;
		}
	}

	public void ResetGame()
	{
        sound.stopBGM();
        //Deactivate uneeded gameobjects and activate needed gameobjects
        isGameFinished = false;
        TitleScreen.SetActive(false);
        MainGameObjs.SetActive(true);
		// Initial values to restart game
		startingTime = Time.time;
		heartCount = kHeartCount;

		// Reset rules system, since it is static but stateful
		RulesSystem.Initialize();

		// Reset body as well		
		body.Reset ();

        HeartScript.StartHeart();


	}
    
    public void ReturnToTitleMenu()
    {
        sound.playBGM(SoundController.Music.StartScreen);
        GameFinishedScreen.SetActive(false);
        TitleScreen.SetActive(true);
    }

    public IEnumerator GoToGameFinishedMenu()
    {
        sound.playBGM(SoundController.Music.LosingTheme);
        isGameFinished = true;
        HeartScript.StopHeart();
        yield return new WaitForSeconds(2f);
        MainGameObjs.SetActive(false);
        GameFinishedScreen.SetActive(true);
    }
}
