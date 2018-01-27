using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// This is the high level state manager for the game. It controls the setup of the body,
// the player's life counter, and win / end state.
public class GameController : MonoBehaviour {

	/*** UI Labels ***/

	Text healthText;
	Text timeText;

	/*** Game State ***/

	// Game constants
	const float kTotalTime = 10.0f;
	const int kHeartCount = 3;

	// Starting time of the game
	float startingTime;

	// Each game starts with 3 wrong moves. On the third, game is over
	int heartCount;

	/*** Symptoms ***/

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


	// List of current symptoms on the body
	Symptom[] bodyPartSymptoms;

	/*** Unity Methods ***/

	// Use this for initialization
	void Start () {

		// Setup UI
		healthText = GameObject.Find("HealthText").GetComponent<Text>();
		timeText = GameObject.Find("TimeText").GetComponent<Text>();

		// Setup game state
		ResetGame();
	}

	// Update is called once per frame
	void Update () {

		// If the patient ever has no heartCount left, game over
		float secondsLeft = kTotalTime - (Time.time - startingTime);
		if (heartCount <= 0 || secondsLeft <= 0.0f) {

			healthText.text = "DEAD";
			timeText.text = "00:00.00";
		}
		// Else not dead, keep playing
		else
		{
			// Update health
			healthText.text = "heartCount left: " + heartCount;

			double minutesLeft = Mathf.Floor (secondsLeft / 60.0f);
			int fractionsLeft = (int)((secondsLeft - Mathf.Floor (secondsLeft)) * 100.0f);
			timeText.text = string.Format ("{0}:{1}.{2}", minutesLeft, ((int)secondsLeft % 60).ToString ("00"), fractionsLeft.ToString ("00"));
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
		// Initial values to restart game
		startingTime = Time.time;
		heartCount = kHeartCount;
		SetupSymptoms ();
	}

	/*** Internal ***/

	void SetupSymptoms()
	{
		int bodyPartCount = System.Enum.GetNames(typeof(BodyController.BodyPart)).Length;
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
		foreach( BodyController.BodyPart bodyPart in System.Enum.GetValues(typeof(BodyController.BodyPart)) )
		{
			Debug.Log ( "Body part " + bodyPart + " has symptom: " + bodyPartSymptoms[ (int)bodyPart ] );
		}
	}
}
