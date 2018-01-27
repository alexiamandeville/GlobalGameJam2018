using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/*** Body Enum Types ***/

// List of possible symptoms. These can be anywhere, but only one per body part
public enum Symptom
{
	None = 0, // All good: default state
	BloodSpurts,
	Pain,
	Heartbeat,
	SkinRashes
};

// Full list of body parts
public enum BodyPartType
{
	Head = 0,
	LeftArm,
	RightArm,
	LeftLeg,
	RightLeg,
	Groin,
	Chest,
	None
};

// Official colors
public enum BodyPartColor
{
	Normal = 0, // Default skin
	Green,
	Red,
	Blue,
	White,
};

// Body part has a type, symptom, and color
public class BodyPart
{
	public BodyPartType bodyPartType;
	public Symptom symptom;
};

public class BodyController : MonoBehaviour{
  
	/*** Overall Body ***/

	// Body has heartbeat and an overall color
	int heartbeat;
	public BodyPartColor bodyColor;

	// List of all body parts, their symptoms, and color
	// This array is indexed via BodyPartType
	BodyPart[] bodyParts;

	// Parallel array that maps BodyPart to the body part GameObjects
	GameObject[] bodyPartObjects;

	/*** Private ***/

	public static BodyPartType GetBodyPart (string name)
    {
        BodyPartType part = BodyPartType.None;
        switch (name)
        {
            case "Head":
                part = BodyPartType.Head;
                break;
            case "Left Arm":
                part = BodyPartType.LeftArm;
                break;
            case "Right Arm":
                part = BodyPartType.RightArm;
                break;
            case "Left Leg":
                part = BodyPartType.LeftLeg;
                break;
            case "Right Leg":
                part = BodyPartType.RightLeg;
                break;
            case "Groin":
                part = BodyPartType.Groin;
                break;
            case "Chest":
                part = BodyPartType.Chest;
                break;
            default:
                part = BodyPartType.None;
                break;
                
        }

        return part;
    }


    // Use this for initialization
    void Start () {

		Reset ();

	}

	public void Reset() {

		// When a body is placed down, we setup its symptoms and visuals
		SetupSymptoms();
		SetupVisuals();
	}
	
	public void applyCure(ToolBox.Tool tool, BodyPartType part)
    {
		// We have a TOOL, a BODY part, and a SYMPTOM on this body part..
		Symptom symptom = bodyParts[ (int)part ].symptom;

		// How many of each symptom do we have?
		int bloodSpurtCount = CountSymptom( Symptom.BloodSpurts );
		int painCount = CountSymptom( Symptom.Pain );
		int heartbeatCount = CountSymptom( Symptom.Heartbeat );
		int skinRashesCount = CountSymptom( Symptom.SkinRashes );

		// We now have to apply a set of rules:

        Debug.Log("Applying cure with tool: " + tool + "To part: " + part);
        //Stub for now.
    }

	// Update is called once per frame
	void Update () {
	}

	void OnGUI() {

		GUIStyle style = new GUIStyle ();
		style.fontSize = 10;

		// For each diseasd body part, slap it on visually
		int bodyPartCount = System.Enum.GetNames(typeof(BodyPartType)).Length;
		for (int i = 0; i < bodyPartCount; i++) {
			BodyPart bodyPart = bodyParts [i];
			GameObject bodyPartObject = bodyPartObjects [i];
			if (bodyPart.symptom != Symptom.None && bodyPartObject != null) {
				Vector3 pos = Camera.main.WorldToScreenPoint ( bodyPartObject.transform.position );
				pos.y = Screen.height - pos.y;
				GUI.Label (new Rect(pos.x - 25, pos.y, 50, 50), "Symptom: " + bodyPart.symptom, style );
			}
		}
	}

	void OnMouseDown() {
		Debug.Log ( "The parent was clicked!" );
	}


	/*** Symptoms Setup ***/

	// Helper function: returns the number of body parts that have the given symptom
	int CountSymptom( Symptom symptom )
	{
		// I'm sure there is a lambda way of doing this
		int count = 0;

		int bodyPartCount = System.Enum.GetNames(typeof(BodyPartType)).Length;
		for (int i = 0; i < bodyPartCount; i++) {
			if (bodyParts [i].symptom == symptom)
				count++;
		}

		return count;
	}

	void SetupSymptoms()
	{
		// 50% chance that the heartbeat is abnormal
		if (Random.Range (0, 2) == 0)
			heartbeat = 80;
		else
			heartbeat = Random.Range( 60, 101 );

		// 50% chance of abnormal color
		int colorCount = System.Enum.GetNames(typeof(BodyPartColor)).Length;
		if (Random.Range (0, 2) == 0)
			bodyColor = BodyPartColor.Normal;
		else
			bodyColor = (BodyPartColor)Random.Range (1, colorCount + 1);
	
		// Initialize body parts with no symptom
		int bodyPartCount = System.Enum.GetNames(typeof(BodyPartType)).Length;
		bodyParts = new BodyPart[ bodyPartCount ];
		for (int i = 0; i < bodyPartCount; i++) {
			bodyParts [i].bodyPartType = (BodyPartType)i;
			bodyParts [i].symptom = Symptom.None;
		}

		// List of symptoms we randomly shuffle: we will apply these
		int kSymptomCount = System.Enum.GetNames(typeof(Symptom)).Length;
		List< Symptom > symptoms = new List< Symptom >();
		for( int i = 0; i < kSymptomCount; i++ )
			symptoms.Add( (Symptom)i );
		symptoms.Sort((a, b)=> 1 - 2 * Random.Range(0, 1));

		// 3 - 5 symptoms. Keep trying to assign to a symptom-free body part
		int targetSymptomCount = Random.Range( 3, 6 );

		// Shitty performance / approach
		while( targetSymptomCount > 0 )
		{
			// Pick random body part. If it's not yet assigned, assign it now
			int bodyPartIndex = Random.Range ( 0, bodyPartCount );
			if (bodyParts [bodyPartIndex].symptom == Symptom.None) {

				// Assign a random and unique symptom
				bodyParts[ bodyPartIndex ].symptom = symptoms[ 0 ];
				symptoms.RemoveAt (0);
				targetSymptomCount--;
			}
		}

		// Print out for debugging
		foreach( BodyPart bodyPart in bodyParts )
		{
			Debug.Log ( "Body part " + bodyPart.bodyPartType + " has symptom: " + bodyPart.symptom );
		}
	}

	void SetupVisuals()
	{
		// First, grab all body parts 
		int bodyPartCount = System.Enum.GetNames(typeof(BodyPartType)).Length;
		bodyPartObjects = new GameObject[ bodyPartCount ];

		// Todo: not be hardcoded
		bodyPartObjects[ 0 ] = GameObject.Find( "Head" );
		bodyPartObjects[ 1 ] = GameObject.Find( "Left Arm" );
		bodyPartObjects[ 2 ] = GameObject.Find( "Right Arm" );
		bodyPartObjects[ 3 ] = GameObject.Find( "Left Leg" );
		bodyPartObjects[ 4 ] = GameObject.Find( "Right Leg" );
		bodyPartObjects[ 5 ] = GameObject.Find( "Groin" );

		// For each diseasd body part, slap it on visually
		Material redMaterial = AssetDatabase.LoadAssetAtPath("Assets/RedMaterial.mat", typeof(Material)) as Material;
		Material newMaterial = AssetDatabase.LoadAssetAtPath("Assets/New Material.mat", typeof(Material)) as Material;
		for (int i = 0; i < bodyPartCount; i++) {
			Symptom symptom = bodyParts [i].symptom;
			if (symptom != Symptom.None) {
				bodyPartObjects [i].GetComponent< Renderer > ().material = redMaterial;
			} else {
				bodyPartObjects [i].GetComponent< Renderer > ().material = newMaterial;
			}
		}
	}
}
