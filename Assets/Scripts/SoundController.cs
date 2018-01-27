using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundController : MonoBehaviour {

    AudioSource source;   
    AudioSource music;

    public AudioClip[] toolPickup;
    public AudioClip[] injectorApply;
    public AudioClip[] tourniquetApply;
    public AudioClip[] pillApply;
    public AudioClip[] ointmentApply;
    public AudioClip[] bodyReactionGood;
    public AudioClip[] bodyReactionBad;
    public AudioClip[] gameWin;
    public AudioClip[] gameLose;
    public AudioClip[] gameStart;

    public AudioClip[] bgm;

    public bool muteEffects = false;
    public bool muteMusic = false;

    public enum Music
    {
        StartScreen = 0,
        MainTheme,
        DramaticTheme
    }

    public enum Event
    {
        GameStart,
        GameWin,
        GameLose
    }

    private void playEffect (AudioClip clip)
    {
        if (!muteEffects)
            source.PlayOneShot(clip);
    }

    public void playEvent(Event e)
    {
        AudioClip[] eventClip;
        switch (e)
        {
            case Event.GameStart:
                eventClip = gameStart;
                break;
            case Event.GameWin:
                eventClip = gameWin;
                break;
            case Event.GameLose:
                eventClip = gameLose;
                break;
            default:
                eventClip = gameLose;
                break;
        }
        int randomIndex = Random.Range(0, eventClip.Length - 1);
        playEffect(eventClip[randomIndex]);
    }

    public void playBGM(Music music )
    {
        Debug.Log("Plaing Background music " + music);
    }

    public void playBodyEffect( ToolBox.Tool tool, bool correctTreatment)
    {
        Debug.Log("Playing sound effect for touching body using tool: " + tool);
        int randomEffect = Random.Range(0, pillApply.Length - 1);
        playEffect(pillApply[0]);
        AudioClip bodyNoise;
        if (correctTreatment)
        {
            bodyNoise = bodyReactionBad[0];
        } else
        {
            bodyNoise = bodyReactionGood[0];
        }
        playEffect(bodyNoise);

    }

    public void playToolPickupEffect(ToolBox.Tool tool)
    {
        Debug.Log("Playing Sound effect for tool: " + tool);
        playEffect(toolPickup[Random.Range(0, toolPickup.Length - 1)]);
    }



	// Use this for initialization
	void Start () {
        GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
        //TODO: Initialize Body Controller	
        source = camera.GetComponent(typeof(AudioSource)) as AudioSource;

        GameObject gameController = GameObject.FindGameObjectWithTag("GameController");
        music = gameController.GetComponent(typeof(AudioSource)) as AudioSource;
	}
}
