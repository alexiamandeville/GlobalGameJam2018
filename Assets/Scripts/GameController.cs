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

	public GameObject GameFinishedPanel;
    public Sprite[] finishFaces;
    public GameObject finishText;
    public GameObject finishImage;

    private string[] winStrings =
    {
        "All those weeks of Medical school have paid off!",
        "Your parents are super proud of you!",
        "Mommy Wow! I'm. A. Doctor. Now!",
        "We are very impressed!",
        "You have received 30xp."
    };

    private string[] loseStrings =
    {
    
        "How could you?",
        "We worked so hard on this just for you to fail.",
        "Well, they were probably a jerk anyway.",
        "This patient had a family to support."
    };

    [Header("Game object")]
    [SerializeField]
    private GameObject MainGameObjs;

	// Game hearts
	public GameObject[] GameHearts;

	/*** UI Labels ***/
    [Header("UI Labels")]

    [SerializeField]
	private Text timeText;

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

            timeText.text = "00:00.00";
            body.setPainLevel(BodyPainLevel.Dead);
            GoToGameFinishedMenu(false);
        }
		// Else, win!
		else if( body.IsFullyHealed() )
		{
            body.setPainLevel(BodyPainLevel.Cured);
            GoToGameFinishedMenu(true);
			// TODO: WIN WIN WIN!
			//Debug.Log( "Winning!" );
		}
        // Else not dead, keep playing
        else
        {
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

		// Disable this heart now..
		Sprite deadHeart = UnityEditor.AssetDatabase.LoadAssetAtPath( "Assets/UI/T_noHeart.png", typeof( Sprite ) ) as Sprite;
		GameHearts [2 - heartCount].GetComponent< Image >().sprite = deadHeart;
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

		// Reset texture
		Sprite deadHeart = UnityEditor.AssetDatabase.LoadAssetAtPath( "Assets/UI/T_heart.png", typeof( Sprite ) ) as Sprite;
		for( int i = 0; i < 3; i++ )
			GameHearts [ i ].GetComponent< Image >().sprite = deadHeart;

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

    public void GoToGameFinishedMenu(bool didIWin)
    {
        //Setup win/lose screen.
        if (didIWin)
        {
            sound.playBGM(SoundController.Music.WinningTheme);
            int randomIndex = Random.Range(0, winStrings.Length);
            finishText.GetComponent<Text>().text = "Wow, your patient lived! "  + winStrings[randomIndex];
            finishImage.GetComponent<Image>().sprite = finishFaces[1];
        }
        else
        {
            sound.playBGM(SoundController.Music.LosingTheme);
            int randomIndex = Random.Range(0, loseStrings.Length);
            finishText.GetComponent<Text>().text = "Your patient has died. " + loseStrings[randomIndex];
            finishImage.GetComponent<Image>().sprite = finishFaces[0];
        }


        isGameFinished = true;
		HeartScript.StopHeart();
		GameFinishedScreen.SetActive(true);

		//MainGameObjs.SetActive(false);

		Image image = GameFinishedPanel.GetComponent < Image >();
		image.canvasRenderer.SetAlpha (0.0f);
		image.CrossFadeAlpha (1.0f, 1.0f, false);

		// TODO: Zoom in on dead face	
    }
}
