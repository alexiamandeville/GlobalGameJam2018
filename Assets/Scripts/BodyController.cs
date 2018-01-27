using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BodyController {
    static BodyController singleton = null;

    public static  BodyController GetInstance()
    {
        if (singleton == null)
            singleton = new BodyController();
        return singleton;
    }

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
            case "Groin":
                part = BodyPart.Groin;
                break;
            case "Chest":
                part = BodyPart.Chest;
                break;
            default:
                part = BodyPart.None;
                break;
                
        }

        return part;
    }
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

    // Use this for initialization
    void Start () {



	}
	
    public void applyCure(ToolBox.Tool tool, BodyPart part)
    {
        Debug.Log("Applying cure with tool: " + tool + "To part: " + part);
        //Stub for now.
    }
	// Update is called once per frame
	void Update () {
		
	}

	void OnMouseDown() {
		Debug.Log ( "The parent was clicked!" );
	}
}
