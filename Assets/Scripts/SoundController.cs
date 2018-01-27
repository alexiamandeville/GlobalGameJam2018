using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundController : MonoBehaviour {

    AudioSource source;

    public AudioClip[] toolPickup;
    public AudioClip[] injectorApply;
    public AudioClip[] tourniquetApply;
    public AudioClip[] pillApply;
    public AudioClip[] ointmentApply;
    public AudioClip[] bodyReactionGood;
    public AudioClip[] bodyReactionBad;
    public AudioClip[] gameWin;
    public AudioClip[] gameLose;

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

    public void playBodyEffect( ToolBox.Tool tool, bool correctTreatment)
    {
        Debug.Log("Playing sound effect for touching body using tool: " + tool);
    }

    public void playToolPickupEffect(ToolBox.Tool tool)
    {
        Debug.Log("Playing Sound effect for tool: " + tool);
        source.PlayOneShot(toolPickup[Random.Range(0, toolPickup.Length - 1)]);
    }



	// Use this for initialization
	void Start () {
        source = GetComponent<AudioSource>();
	}
}
