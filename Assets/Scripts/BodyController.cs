using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

/*** Body Enum Types ***/

// List of possible symptoms. These can be anywhere, but only one per body part
public enum Symptom
{
	None = 0, // All good: default state
	BloodSpurts,
	Pain
};

// Full list of body parts
public enum BodyPartType
{
	Head = 0,
	Arm,
	Leg,
	Groin
};

// Official colors
public enum BodyPartColor
{
	White = 0, // Default skin!
	Green,
	Red,
	Blue,
};

// Body part has a type, symptom, and color
public class BodyPart
{
	public BodyPartType bodyPartType;
	public Symptom symptom;
    public GameObject BloodSpurt;
    public GameObject PainEffect;
};

public enum BodyPainLevel
{
    Bad = 0,
    Worse,
    Dead,
    Cured
}

public class BodyController : MonoBehaviour
{

    /*** Overall Body ***/

    // Body has heartbeat and an overall color
	public const int kTargetHeartbeat = 80;
	public int heartbeat = kTargetHeartbeat;

    BodyPartColor bodyColor = BodyPartColor.White;
    public BodyPainLevel painLevel = BodyPainLevel.Bad;
    public Texture[] bodyTextures;
    // List of all body parts, their symptoms, and color
    // This array is indexed via BodyPartType
    BodyPart[] bodyParts;
    GameObject bodyMesh;
    float ouchTime = 1.5f;
    float lastOuch;
    SoundController sound;

    


    // Parallel array that maps BodyPart to the body part GameObjects
    [SerializeField]
    private GameObject[] bodyPartObjects;

    // Parallel array that maps BodyPartColor to materials
    public Material[] bodyPartColorMaterials;

    [SerializeField]
    private Text HeartRateText;

    [Header("Effect Prefabs")]
    [SerializeField]
    private GameObject BloodSpurtPrefab;
    [SerializeField]
    private GameObject PainEffectPrefab;


    public static BodyPartType GetBodyPart(string name)
    {
        BodyPartType part = BodyPartType.Head;
        switch (name)
        {
            case "Head":
                part = BodyPartType.Head;
                break;
            case "Left Arm":
                part = BodyPartType.Arm;
                break;
            case "Right Arm":
                part = BodyPartType.Arm;
                break;
            case "Left Leg":
                part = BodyPartType.Leg;
                break;
            case "Right Leg":
                part = BodyPartType.Leg;
                break;
            case "Groin":
                part = BodyPartType.Groin;
                break;
            default:
                //Debug.Log("ERROR: This should never happen!");
                break;

        }

        return part;
    }

	// Returns true if the patient is fully healed (no more symptoms!)
	public bool IsFullyHealed()
	{
		if (bodyParts == null)
			return false;
		
		foreach( BodyPart bodyPart in bodyParts )
		{
			// We still have a problem: not fully healed
			if (bodyPart.symptom != Symptom.None)
				return false;
		}

		// Is the skin color non-white?
		if (bodyColor != BodyPartColor.White)
			return false;
        
		// Is the heartbeat cured?
		if (heartbeat != kTargetHeartbeat)
			return false;

		// Graphics change
		painLevel = BodyPainLevel.Cured;

		// Fully healed
		return true;
	}

    /*** Private ***/

    // Use this for initialization
    void Start()
    {
        GameObject gameController = GameObject.FindGameObjectWithTag("GameController");
        sound = gameController.GetComponent(typeof(SoundController)) as SoundController;
        bodyMesh = GameObject.FindGameObjectWithTag("BodyModel");
        Reset();

    }

    public void Reset()
    {
        GameObject[] OldBlood = GameObject.FindGameObjectsWithTag("Blood");
        foreach (GameObject blood in OldBlood)
        {
            Destroy(blood);
        }

        GameObject[] OldPainEffects = GameObject.FindGameObjectsWithTag("Pain");
        foreach (GameObject pain in OldPainEffects)
        {
            Destroy(pain);
        }

        // When a body is placed down, we setup its symptoms and visuals
        SetupSymptoms();
        SetupVisuals();
        
    }

    public void setPainLevel(BodyPainLevel pain)
    {
        if (pain == BodyPainLevel.Worse)
            lastOuch = Time.time;
        painLevel = pain;

        if (painLevel == BodyPainLevel.Dead || painLevel == BodyPainLevel.Cured)
        {
            foreach (BodyPart part in bodyParts)
            {
                if (part.BloodSpurt != null)
                {
                    Destroy(part.BloodSpurt, 2f);
                }
                else if(part.PainEffect != null)
                {
                    Destroy(part.PainEffect, 2f);
                }
            }
        }

                    
    }

    public void applyCure(ToolBox.Tool tool, BodyPartType part)
    {
		// Skip if no tool set
		if (tool == ToolBox.Tool.None)
			return;

		// Test out the rule system
		bool fixesColor = false;
		bool success = RulesSystem.EvaluateCure( bodyParts, tool, part, bodyColor, ref heartbeat, out fixesColor );

		// Did color get fixed?
		if (fixesColor) {
			bodyColor = BodyPartColor.White;
			SetupColor ();
		}
       
		// Tell game controller if we did this wrong
		if (!success)
        { 
			GameObject gameControllerObject = GameObject.FindGameObjectWithTag ("GameController");
			GameController gameController = gameControllerObject.GetComponent (typeof(GameController)) as GameController;
			gameController.FailedCureAttempt();
            painLevel = BodyPainLevel.Worse;
            lastOuch = Time.time;
		}
        sound.playBodyEffect(tool, success);
    }

    // Update is called once per frame
    void Update()
	{
		if( HeartRateText != null )
			HeartRateText.text = heartbeat.ToString() ;

        if (bodyMesh != null)
        {
            //Set the current pain level
            if (painLevel == BodyPainLevel.Worse)
            {
                if (Time.time - lastOuch > ouchTime)
                {
                    painLevel = BodyPainLevel.Bad;
                }
            }
            Texture newTexture = bodyTextures[(int)painLevel];
            bodyMesh.GetComponent<Renderer>().material.mainTexture = newTexture;
        }

    }
	/*
    void OnGUI()
    {

        if (gameObject.GetComponent<GameController>().isGameFinished)
        {
            return;
        }
        GUIStyle style = new GUIStyle();
        style.fontSize = 10;

        // For each diseasd body part, slap it on visually
		foreach (BodyPartType bodyPartType in System.Enum.GetValues(typeof(BodyPartType)))
        {
			BodyPart bodyPart = bodyParts[ (int)bodyPartType ];
            
			int i = 0;
			if (bodyPartType == BodyPartType.Head)
				i = 0;
			else if (bodyPartType == BodyPartType.Arm)
				i = 1;
			else if (bodyPartType == BodyPartType.Leg)
				i = 3;
			else if (bodyPartType == BodyPartType.Groin)
				i = 5;
			
			GameObject bodyPartObject = bodyPartObjects[i];

            if (bodyPart.symptom != Symptom.None && bodyPartObject != null)
            {
                Vector3 pos = Camera.main.WorldToScreenPoint(bodyPartObject.transform.position);
                pos.y = Screen.height - pos.y;
                GUI.Label(new Rect(pos.x - 25, pos.y, 50, 50), "Symptom: " + bodyPart.symptom, style);

                if (bodyPart.symptom == Symptom.BloodSpurts)
                {
                    if(bodyPart.BloodSpurt == null)
                    {
                        bodyPart.BloodSpurt = GameObject.Instantiate(BloodSpurtPrefab, new Vector3(bodyPartObject.transform.position.x, bodyPartObject.transform.position.y + 20, bodyPartObject.transform.position.z), Quaternion.identity);
                    }
                }
                else if (bodyPart.symptom == Symptom.Pain)
                {
                    if(bodyPart.PainEffect == null)
                    {
                        bodyPart.PainEffect = GameObject.Instantiate(PainEffectPrefab, new Vector3(bodyPartObject.transform.position.x, bodyPartObject.transform.position.y + 3, bodyPartObject.transform.position.z), Quaternion.Euler(90, 0, 0));
                    }
                }
              
            }
        }
    }
*/
    void OnMouseDown()
    {
        //Debug.Log("The parent was clicked!");
    }


    /*** Symptoms Setup ***/

    void SetupSymptoms()
    {
        //Re-set the pain level:
        painLevel = BodyPainLevel.Bad;
        // 50% chance that the heartbeat is abnormal
        if (Random.Range(0, 2) == 0)
			heartbeat = kTargetHeartbeat;
        else
            heartbeat = Random.Range(85, 121);
		
        // 50% chance of abnormal color
        int colorCount = System.Enum.GetNames(typeof(BodyPartColor)).Length;
        if (Random.Range(0, 2) == 0)
            bodyColor = BodyPartColor.White;
        else
            bodyColor = (BodyPartColor)Random.Range(1, colorCount);

        // Initialize body parts with no symptom
        int bodyPartCount = System.Enum.GetNames(typeof(BodyPartType)).Length;
        bodyParts = new BodyPart[bodyPartCount];
        for (int i = 0; i < bodyPartCount; i++)
        {
            bodyParts[i] = new BodyPart();
            bodyParts[i].bodyPartType = (BodyPartType)i;
            bodyParts[i].symptom = Symptom.None;
        }

        // List rof symptoms we randomly shuffle: we will apply these
        int kSymptomCount = System.Enum.GetNames(typeof(Symptom)).Length;
        List<Symptom> symptoms = new List<Symptom>();
        for (int i = 0; i < kSymptomCount; i++)
			symptoms.Add((Symptom)i);
        symptoms.Sort((a, b) => 1 - 2 * Random.Range(0, 1));

        // Shitty performance / approach
		while (symptoms.Count > 0)
        {
            // Pick random body part. If it's not yet assigned, assign it now
            int bodyPartIndex = Random.Range(0, bodyPartCount);
            if (bodyParts[bodyPartIndex].symptom == Symptom.None)
            {
                // Assign a random and unique symptom
                bodyParts[bodyPartIndex].symptom = symptoms[0];               
                symptoms.RemoveAt(0);
            }
        }

        // Print out for debugging
        foreach (BodyPart bodyPart in bodyParts)
        {
           // Debug.Log("Body part " + bodyPart.bodyPartType + " has symptom: " + bodyPart.symptom);
        }
    }

    void SetupVisuals()
    {
        // For each type of color, initialize colors
        int bodyPartColorCount = System.Enum.GetNames(typeof(BodyPartColor)).Length;

		// For each body part, apply the appropriate color
		foreach (GameObject child in bodyPartObjects) {
			//child.GetComponent<MeshRenderer> ().material = bodyPartColorMaterials [(int)bodyColor];
		}

		SetupColor ();
    }

	void SetupColor()
	{

		/*	Normal = 0, // Default skin
	Green,
	Red,
	Blue,
	White,*/
		Color newColor = Color.grey;
		switch (bodyColor) {
		case BodyPartColor.White:
			newColor = new Color (.8f, .8f, .8f, .8f);
			break;
		case BodyPartColor.Green:
			newColor = new Color (.2f, .8f, .2f, 1);
			break;
		case BodyPartColor.Blue:
			newColor = new Color (.2f, .2f, .8f, 1);
			;
			break;
		case BodyPartColor.Red:
			newColor = new Color (.5f, .2f, .2f, 1);
			break;
		}

		if (bodyMesh == null) {
			bodyMesh = GameObject.FindGameObjectWithTag ("BodyModel");
		}
		if (bodyMesh != null) {
			bodyMesh.GetComponent<Renderer> ().material.color = newColor;
		}
	}
}
