using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundController : MonoBehaviour {

    AudioSource source;   
    AudioSource musicSource;

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
    public bool musicPlaying = false;

    public enum Music
    {
        StartScreen = 0,
        MainTheme,
        WinningTheme,
        LosingTheme
    }

    public enum Event
    {
        GameStart,
        GameWin,
        GameLose
    }

    private void playEffect (AudioClip[] clip)
    {
        if (!muteEffects)
        {
            int randomIndex = Random.Range(0, clip.Length);
            source.PlayOneShot(clip[randomIndex]);
        }
            
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
        playEffect(eventClip);
    }

    public void playBGM(Music music)
    {
        //Debug.Log("Plaing Background music " + music);
        musicPlaying = true;
        source.clip = bgm[(int)music];
        source.Play();
    }

    public void stopBGM()
    {
        musicPlaying = false;
        source.Stop();
    }

    public void playBodyEffect( ToolBox.Tool tool, bool correctTreatment)
    {
        //Debug.Log("Playing sound effect for touching body using tool: " + tool);

        AudioClip[] toolNoise;

        switch (tool)
        {
            case ToolBox.Tool.Injector:
                toolNoise = injectorApply;
                break;
            case ToolBox.Tool.Ointment:
                toolNoise = ointmentApply;
                break;
            case ToolBox.Tool.Tourniquet:
                toolNoise = tourniquetApply;
                break;
            case ToolBox.Tool.Pill:
                toolNoise = pillApply;
                break;
            default:
                toolNoise = pillApply;
                break;
        }

        playEffect(toolNoise);

        AudioClip[] bodyNoise;
        if (correctTreatment)
        {
            bodyNoise = bodyReactionGood;
        } else
        {
            bodyNoise = bodyReactionBad;
        }
        playEffect(bodyNoise);

    }

    public void playToolPickupEffect(ToolBox.Tool tool)
    {
        //Debug.Log("Playing Sound effect for tool: " + tool);
        playEffect(toolPickup);
    }



	// Use this for initialization
	void Start () {
        GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
        //TODO: Initialize Body Controller	
        source = camera.GetComponent(typeof(AudioSource)) as AudioSource;

        //musicSource = gameController.GetComponent(typeof(AudioSource)) as AudioSource;
        playBGM(Music.StartScreen);
	}
}
