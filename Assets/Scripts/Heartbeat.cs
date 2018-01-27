using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Heartbeat : MonoBehaviour {

    private static Heartbeat HeartInstance;

    public static Heartbeat GetHeartInstance()
    {
        if(HeartInstance == null)
        {
            HeartInstance = new Heartbeat();
        }

        return HeartInstance;
    }

    [SerializeField]
    private AudioSource HeartBeatAudioSource;
    [SerializeField]
    private BodyController Body;
    [SerializeField]
    private GameController Controller;

    [Header("Heart beat sound pitch rates")]
    [Tooltip("1 is the default for normal audio speed, audio speed is a float")]
    [SerializeField]
    private float NormalHeatBeat = 1;
    [SerializeField]
    private float FastHeartBeat = 1.5f;
    [SerializeField]
    private float SlowHeartBeat = 0.5f;	

    public void StartHeart()
    {

        HeartBeat();

    }

    private void HeartBeat()
    {
        int HeartBeatRateValue = Body.heartbeat;
        if (HeartBeatRateValue == 80)
        {
            HeartBeatAudioSource.pitch = NormalHeatBeat;
            HeartBeatAudioSource.Play();
        }
        else if(HeartBeatRateValue < 80)
        {
            HeartBeatAudioSource.pitch = SlowHeartBeat;
            HeartBeatAudioSource.Play();
        }
        else
        {
            HeartBeatAudioSource.pitch = FastHeartBeat;
            HeartBeatAudioSource.Play();
        }
    }

    public void StopHeart()
    {
        HeartBeatAudioSource.Stop();
    }

}
