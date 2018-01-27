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
	Groin
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

public class BodyController : MonoBehaviour
{

    /*** Overall Body ***/

    // Body has heartbeat and an overall color
    public int heartbeat = 80;
    BodyPartColor bodyColor = BodyPartColor.Normal;

    // List of all body parts, their symptoms, and color
    // This array is indexed via BodyPartType
    BodyPart[] bodyParts;

    // Parallel array that maps BodyPart to the body part GameObjects
    [SerializeField]
    private GameObject[] bodyPartObjects;

    // Parallel array that maps BodyPartColor to materials
    Material[] bodyPartColorMaterials;

    [SerializeField]
    private Text HeartRateText;

    public static BodyPartType GetBodyPart(string name)
    {
        BodyPartType part = BodyPartType.Head;
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
            default:
                Debug.Log("ERROR: This should never happen!");
                break;

        }

        return part;
    }


    /*** Private ***/

    // Use this for initialization
    void Start()
    {

        Reset();

    }

    public void Reset()
    {

        // When a body is placed down, we setup its symptoms and visuals
        SetupSymptoms();
        SetupVisuals();
    }

    public void applyCure(ToolBox.Tool tool, BodyPartType part)
    {
		// Test out the rule system
		bool success = RulesSystem.EvaluateCure( bodyParts, tool, part );
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnGUI()
    {

        if (gameObject.GetComponent<GameController>().isGameFinished)
        {
            return;
        }
        GUIStyle style = new GUIStyle();
        style.fontSize = 10;

        // For each diseasd body part, slap it on visually
        int bodyPartCount = System.Enum.GetNames(typeof(BodyPartType)).Length;
        for (int i = 0; i < bodyPartCount; i++)
        {
            BodyPart bodyPart = bodyParts[i];
            GameObject bodyPartObject = bodyPartObjects[i];
            if (bodyPart.symptom != Symptom.None && bodyPartObject != null)
            {
                Vector3 pos = Camera.main.WorldToScreenPoint(bodyPartObject.transform.position);
                pos.y = Screen.height - pos.y;
                GUI.Label(new Rect(pos.x - 25, pos.y, 50, 50), "Symptom: " + bodyPart.symptom, style);
            }
        }
    }

    void OnMouseDown()
    {
        Debug.Log("The parent was clicked!");
    }


    /*** Symptoms Setup ***/

    void SetupSymptoms()
    {
        // 50% chance that the heartbeat is abnormal
        if (Random.Range(0, 2) == 0)
            heartbeat = 80;
        else
            heartbeat = Random.Range(60, 101);

        HeartRateText.text = heartbeat.ToString() ;

        // 50% chance of abnormal color
        int colorCount = System.Enum.GetNames(typeof(BodyPartColor)).Length;
        if (Random.Range(0, 2) == 0)
            bodyColor = BodyPartColor.Normal;
        else
            bodyColor = (BodyPartColor)Random.Range(1, colorCount + 1);

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

		// Always do three symptoms, and track once the leg or arm has been
		// set so we don't re-apply it to the opposite leg / arm
        int targetSymptomCount = 3;
		bool armsApplied = false;
		bool legsApplied = false;

        // Shitty performance / approach
        while (targetSymptomCount > 0)
        {
            // Pick random body part. If it's not yet assigned, assign it now
            int bodyPartIndex = Random.Range(0, bodyPartCount);
            if (bodyParts[bodyPartIndex].symptom == Symptom.None)
            {
				// Don't double apply..
				if (armsApplied && (bodyPartIndex == (int)BodyPartType.LeftArm || bodyPartIndex == (int)BodyPartType.RightArm))
					continue;

				if (legsApplied && (bodyPartIndex == (int)BodyPartType.LeftLeg || bodyPartIndex == (int)BodyPartType.RightLeg))
					continue;

                // Assign a random and unique symptom
                bodyParts[bodyPartIndex].symptom = symptoms[0];
                symptoms.RemoveAt(0);
                targetSymptomCount--;

				if ( bodyPartIndex == (int)BodyPartType.LeftArm || bodyPartIndex == (int)BodyPartType.RightArm )
					armsApplied = true;

				if ( bodyPartIndex == (int)BodyPartType.LeftLeg || bodyPartIndex == (int)BodyPartType.RightLeg )
					legsApplied = true;
            }
        }

        // Print out for debugging
        foreach (BodyPart bodyPart in bodyParts)
        {
            Debug.Log("Body part " + bodyPart.bodyPartType + " has symptom: " + bodyPart.symptom);
        }
    }

    void SetupVisuals()
    {
        // For each type of color, initialize colors
        int bodyPartColorCount = System.Enum.GetNames(typeof(BodyPartColor)).Length;
        bodyPartColorMaterials = new Material[bodyPartColorCount];

        bodyPartColorMaterials[0] = AssetDatabase.LoadAssetAtPath("Assets/Meshes/Materials/NormalSkin.mat", typeof(Material)) as Material;
        bodyPartColorMaterials[1] = AssetDatabase.LoadAssetAtPath("Assets/Meshes/Materials/GreenSkin.mat", typeof(Material)) as Material;
        bodyPartColorMaterials[2] = AssetDatabase.LoadAssetAtPath("Assets/Meshes/Materials/RedSkin.mat", typeof(Material)) as Material;
        bodyPartColorMaterials[3] = AssetDatabase.LoadAssetAtPath("Assets/Meshes/Materials/BlueSkin.mat", typeof(Material)) as Material;
        bodyPartColorMaterials[4] = AssetDatabase.LoadAssetAtPath("Assets/Meshes/Materials/WhiteSkin.mat", typeof(Material)) as Material;

		// For each body part, apply the appropriate color
		foreach (GameObject child in bodyPartObjects) {
			child.GetComponent<Renderer> ().material = bodyPartColorMaterials [(int)bodyColor];
		}
    }
}
