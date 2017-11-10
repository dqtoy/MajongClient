using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MajongSoundController : MonoBehaviour
{
    public static MajongSoundController ins;

    private AudioSource audioSource;
    private Dictionary<string, AudioClip> nameSoundDict = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        ins = this;
        LoadSounds();
        audioSource = transform.GetComponent<AudioSource>();
    }

    void Start ()
	{
	    
    }
	
    public void PlayMajongSound(string name)
    {
        AudioClip curAudioClip = nameSoundDict[name];
        audioSource.clip = curAudioClip;
        audioSource.Play();
    }

    void LoadSounds()
    {
        Object[] soundObjects = Resources.LoadAll("MajongSound");
        foreach (Object ac in soundObjects)
        {
            nameSoundDict.Add(ac.name, (AudioClip)ac);
        }
    }
}
