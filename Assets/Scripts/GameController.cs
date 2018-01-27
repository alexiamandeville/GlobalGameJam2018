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

	/*** Game State ***/

	// Game constants
	const float kTotalTime = 10.0f;
	const int kHeartCount = 3;

	// Starting time of the game
	float startingTime;

   public bool isGameFinished = true;

	// Each game starts with 3 wrong moves. On the third, game is over
	int heartCount;

	/*** Symptoms ***/


	// Use this for initialization
	

	// List of possible symptoms. These can be anywhere
	enum Symptom {

		// All good: default state
		None,

		// Blood
		BloodBlue,
		BloodGreen,

		// Heartbeat
		HeartbeatFast,
		HeartbeatSlow,

		// Skin color
		SkinColorRash,
		SkinColorBoils,

		// Pain
		PainIntense,
		PainNumb
	};

	// Six body parts, listed as 0-indexed enum for easier setting
	enum BodyPart {

		Head = 0,
		LeftArm,
		RightArm,
		LeftLeg,
		RightLeg,
		Groin
	};

	// List of current symptoms on the body
	Symptom[] bodyPartSymptoms;

    /*** Unity Methods ***/

    void Start()
    {
        GameObject gameController = GameObject.FindGameObjectWithTag("GameController");
        //TODO: Initialize Body Controller	
        body = gameController.GetComponent(typeof(BodyController)) as BodyController;
        // Setup UI
        //healthText = GameObject.Find("HealthText").GetComponent<Text>();
        //timeText = GameObject.Find("TimeText").GetComponent<Text>();

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
        if ((heartCount <= 0 || secondsLeft <= 0.0f))
        {

            healthText.text = "DEAD";
            timeText.text = "00:00.00";
            StartCoroutine(GoToGameFinishedMenu());
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
        //Deactivate uneeded gameobjects and activate needed gameobjects
        isGameFinished = false;
        TitleScreen.SetActive(false);
        MainGameObjs.SetActive(true);
		// Initial values to restart game
		startingTime = Time.time;
		heartCount = kHeartCount;

		// Reset body as well		
		body.Reset ();

		SetupSymptoms ();

	}
    
    public void ReturnToTitleMenu()
    {
        GameFinishedScreen.SetActive(false);
        TitleScreen.SetActive(true);
    }

    public IEnumerator GoToGameFinishedMenu()
    {
        isGameFinished = true;
        yield return new WaitForSeconds(2f);
        MainGameObjs.SetActive(false);
        GameFinishedScreen.SetActive(true);
    }

	/*** Internal ***/

	void SetupSymptoms()
	{
		int bodyPartCount = System.Enum.GetNames(typeof(BodyPart)).Length;
		bodyPartSymptoms = new Symptom[ bodyPartCount ];
		for( int i = 0; i < bodyPartCount; i++ )
			bodyPartSymptoms[ i ] = Symptom.None;

		// List of symptoms we randomly shuffle: we will apply these
		int kSymptomCount = System.Enum.GetNames(typeof(Symptom)).Length;
		List< Symptom > symptoms = new List< Symptom >();
		for( int i = 0; i < kSymptomCount; i++ )
			symptoms.Add( (Symptom)i );
		symptoms.Sort((a, b)=> 1 - 2 * Random.Range(0, 1));

		// 3 - 5 symptoms. Keep trying to assign to a symptom-free body part
		int targetSymptomCount = Random.Range( 3, 5 );

		// Shitty performance / approach
		while( targetSymptomCount > 0 )
		{
			// Pick random body part. If it's not yet assigned, assign it now
			int bodyPartIndex = Random.Range ( 0, bodyPartCount - 1 );
			if (bodyPartSymptoms [bodyPartIndex] == Symptom.None) {

				// Assign a random and unique symptom
				bodyPartSymptoms[ bodyPartIndex ] = symptoms[ 0 ];
				symptoms.RemoveAt (0);
				targetSymptomCount--;
			}
		}

		// Print out for debugging
		foreach( BodyPart bodyPart in System.Enum.GetValues(typeof(BodyPart)) )
		{
			Debug.Log ( "Body part " + bodyPart + " has symptom: " + bodyPartSymptoms[ (int)bodyPart ] );
		}
	}
}
