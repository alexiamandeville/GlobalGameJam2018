using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Clickable : MonoBehaviour {

    // Use this for initialization
    BodyController bod = BodyController.GetInstance();
    ToolBox toolBox = ToolBox.GetInstance();

    //ToolBoxController tools;


	void Start () {
	    //TODO: Initialize Body Controller	
        //TODO: Initialize ToolBox Controller
	}
	
	// Update is called once per frame
	void Update () {
		
	} 
    // On hit, we talk to the owning parent
    void OnMouseDown() {

        Debug.Log("The parent was clicked! Tag: " + tag + "Name: " + name );
        switch (tag)
        {
            //Body parts:
            case "Body":
                //Tell the body that the current Tool has touched it
				BodyPartType part = BodyController.GetBodyPart(name);
                bod.applyCure(toolBox.selectedTool, part);
                break;
            case "Tool":
                toolBox.selectTool(name);
                break;
            case "Background":
                toolBox.selectTool("None");
                break;
        }
		// Communicate to owning script:
	}
}
