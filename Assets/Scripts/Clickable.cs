using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Clickable : MonoBehaviour {
    SoundController sound;
    static GameObject activeTool;
    static bool isToolSelected = false;
    static Vector3 oldPoint;

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
		if (isToolSelected)
        {
            //oldPoint = activeTool.transform.position;
            Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            newPosition.y = newPosition.z;
            newPosition.y = oldPoint.y;
            activeTool.transform.position = newPosition;
        }
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
                sound.playBodyEffect(toolBox.selectedTool, false);
                break;
            case "Tool":
                ToolBox.Tool oldTool = toolBox.selectedTool;
                toolBox.selectTool(name);
                if (toolBox.selectedTool == oldTool)
                {
                    Debug.Log("Yes we just got the same thing.");
                    return;
                }
                if (toolBox.selectedTool != ToolBox.Tool.None)
                {
                    if (isToolSelected)
                    {
                        activeTool.transform.position = oldPoint;
                        activeTool.GetComponent<Collider>().enabled = true;
                        activeTool = null;
                    }
                    activeTool = GameObject.Find(name) as GameObject;
                    activeTool.GetComponent<Collider>().enabled = false;
                    oldPoint = activeTool.transform.position;
                    sound.playToolPickupEffect(toolBox.selectedTool);
                    isToolSelected = true;
                } else
                {
                    if (isToolSelected)
                    {
                        isToolSelected = false;
                        activeTool.transform.position = oldPoint;
                        activeTool.GetComponent<Collider>().enabled = true;
                        activeTool = null;
                    }

                }
                break;
            case "Background":
                if (isToolSelected)
                {
                    isToolSelected = false;
                    activeTool.transform.position = oldPoint;
                    activeTool.GetComponent<Collider>().enabled = true;
                    activeTool = null;
                }
                toolBox.selectTool("None");
                break;

        }
        // Communicate to owning script:
    }
}
