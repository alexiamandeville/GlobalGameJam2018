using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ToolBox : MonoBehaviour {
    public enum Tool
    {
        Injector,
        Tourniquet,
        Pill,
        Ointment,
        None
    }

    enum Drug
    {
        Red,
        Green,
        Blue
    }


     public Tool selectedTool = Tool.None;

    public void selectTool (string toolName)
    {
        switch(toolName)
        {
            case "Injector":
                selectedTool = Tool.Injector;
                break;
            case "Tourniquet":
                selectedTool = Tool.Tourniquet;
                break;
            case "Pill":
                selectedTool = Tool.Pill;
                break;
            case "Ointment":
                selectedTool = Tool.Ointment;
                break;
            default:
                selectedTool = Tool.None;
                break;
        }
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
