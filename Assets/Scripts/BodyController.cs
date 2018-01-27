using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class BodyController : MonoBehaviour{
  

	/*** Symptoms ***/

	// List of possible symptoms. These can be anywhere
	enum Symptom {

		// All good: default state
		None,

		BloodSpurts,
		Pain,
		Heartbeat,
		SkinRashes
	};

    public enum BodyPart
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

    // Parallel array that maps BodyPart to the body part GameObjects
    GameObject[] bodyPartObjects;

	// List of current symptoms on the body
	Symptom[] bodyPartSymptoms;

    public static BodyPart GetBodyPart (string name)
    {
        BodyPart part = BodyPart.None;
        switch (name)
        {
            case "Head":
                part = BodyPart.Head;
                break;
            case "Left Arm":
                part = BodyPart.LeftArm;
                break;
            case "Right Arm":
                part = BodyPart.RightArm;
                break;
            case "Left Leg":
                part = BodyPart.LeftLeg;
                break;
            case "Right Leg":
                part = BodyPart.RightLeg;
                break;
            case "Groin":
                part = BodyPart.Groin;
                break;
            default:
                part = BodyPart.None;
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
	
    public void applyCure(ToolBox.Tool tool, BodyPart part)
    {
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
		int bodyPartCount = System.Enum.GetNames(typeof(BodyPart)).Length;
		for (int i = 0; i < bodyPartCount; i++) {
			Symptom symptom = bodyPartSymptoms [i];
			GameObject bodyPart = bodyPartObjects [i];
			if (symptom != Symptom.None && bodyPart != null) {
				Vector3 pos = Camera.main.WorldToScreenPoint ( bodyPart.transform.position );
				pos.y = Screen.height - pos.y;
				GUI.Label (new Rect(pos.x - 25, pos.y, 50, 50), "Symptom: " + symptom, style );
			}
		}
	}

	void OnMouseDown() {
		Debug.Log ( "The parent was clicked!" );
	}


	/*** Symptoms Setup ***/

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
		int targetSymptomCount = Random.Range( 3, 6 );

		// Shitty performance / approach
		while( targetSymptomCount > 0 )
		{
			// Pick random body part. If it's not yet assigned, assign it now
			int bodyPartIndex = Random.Range ( 0, bodyPartCount );
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

	void SetupVisuals()
	{
		// First, grab all body parts 
		int bodyPartCount = System.Enum.GetNames(typeof(BodyPart)).Length;
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
			Symptom symptom = bodyPartSymptoms [i];
			if (symptom != Symptom.None) {
				bodyPartObjects [i].GetComponent< Renderer > ().material = redMaterial;
			} else {
				bodyPartObjects [i].GetComponent< Renderer > ().material = newMaterial;
			}
		}
	}
}
