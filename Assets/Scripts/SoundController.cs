using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour {

    public enum Music
    {
        StartScreen,
        MainTheme,
        DramaticTheme
    }

    public void playBGM(Music music )
    {
        Debug.Log("Plaing Background music " + music);
    }

    public void playBodyEffect( ToolBox.Tool tool, BodyPartType part)
    {
        Debug.Log("Playing sound effect for touching body part: " + part + " using tool: " + tool);
    }

    public void playToolPickupEffect(ToolBox.Tool tool)
    {
        Debug.Log("Playing Sound effect for tool: " + tool);
    }



	// Use this for initialization
	void Start () {
		//Load audio?
	}
}
