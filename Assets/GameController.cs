using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This is the high level state manager for the game. It controls the setup of the body,
// the player's life counter, and win / end state.
public class GameController : MonoBehaviour {

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

	// Use this for initialization
	void Start () {

		SetupSymptoms ();

	}


	
	// Update is called once per frame
	void Update () {
		
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
