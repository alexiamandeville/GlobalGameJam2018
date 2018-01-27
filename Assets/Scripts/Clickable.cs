using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Clickable : MonoBehaviour {
    public SoundController sound;

    // Use this for initialization
    BodyController bod;
    ToolBox toolBox;

    //ToolBoxController tools;


	void Start () {
        GameObject gameController = GameObject.FindGameObjectWithTag("GameController");
        //TODO: Initialize Body Controller	
        bod = gameController.GetComponent(typeof(BodyController)) as BodyController;

        //TODO: Initialize ToolBox Controller
        toolBox = gameController.GetComponent(typeof(ToolBox)) as ToolBox;
        //Initializing Sound Engine
        sound = gameController.GetComponent(typeof(SoundController)) as SoundController;

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
                BodyController.BodyPart part = BodyController.GetBodyPart(name);
                bod.applyCure(toolBox.selectedTool, part);
                sound.playBodyEffect(toolBox.selectedTool, false);
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
