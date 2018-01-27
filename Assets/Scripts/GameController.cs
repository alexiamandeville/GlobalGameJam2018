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

		// Reset body as well
		BodyController bodyController = GameObject.Find("Body").GetComponent<BodyController>();
		bodyController.Reset ();
	}

}
