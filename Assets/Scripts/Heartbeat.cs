using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Heartbeat : MonoBehaviour {
    [SerializeField]
    private Text HeartBeatRate;
    [SerializeField]
    private AudioSource HeartBeatAudioSource;

    [Header("Heart beat sound pitch rates")]
    [Tooltip("1 is the default for normal audio speed, audio speed is a float")]
    [SerializeField]
    private float NormalHeatBeat = 1;
    [SerializeField]
    private float FastHeartBeat = 1.5f;
    [SerializeField]
    private float SlowHeartBeat = 0.5f;

	// Use this for initialization
	void Start () {
        StartCoroutine(StartHeart());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator StartHeart()
    {
        HeartBeat();
        yield return new WaitForSeconds(17f);
        StartCoroutine(StartHeart());

    }
    public void HeartBeat()
    {
        int HeartBeatRateValue;
        int.TryParse(HeartBeatRate.text, out HeartBeatRateValue); 
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

}
