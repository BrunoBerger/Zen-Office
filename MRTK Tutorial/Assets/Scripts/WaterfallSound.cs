using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterfallSound : MonoBehaviour
{

    [SerializeField] 
    AudioClip[] WaterfallSoundLib;

    // Start is called before the first frame update
    void Start()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.clip = WaterfallSoundLib[Random.Range(0, WaterfallSoundLib.Length - 1)];

        audioSource.PlayDelayed(Random.Range(0f, 2f));
    }
}
