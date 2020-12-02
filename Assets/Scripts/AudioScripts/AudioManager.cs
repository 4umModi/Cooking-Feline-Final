/*
    referenced tutorial: "Introduction to AUDIO in Unity" by Brackeys YT channel
    https://www.youtube.com/watch?v=6OT43pvUyfY&list=PL-UICBkD9yr0etwHc5deLx5xkVGJVO9rF&index=9
*/

using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    public Sound[] sounds;

    public static AudioManager instance;

    // Use this for initialization
    void Awake() {

        if(instance == null) // we only want one instance of AudioManager btwn scenes
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject); // keep same AudioManager instance btwn scenes

        // list of sounds we'll be using; edit info in the inspector
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    // to play step 2 audio when @ step 2 in recipe, go to spot
    // in code where the steps are advanced and do this:
    /*
        FindObjectOfType<AudioManager>().Play("step2");
        >> see brackeys audio tutorial @ ~10:00
        >> will have to check the current step #, then play that # step w above call
    */
    void Start(){ // plays upon scene start
        Play("cookingMusic");
        //Play("step1");
    }

    public void Play(string name){
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if(s == null){
            Debug.LogWarning("Sound: " + name + " not found!");
            return; // upon typo, don't play a sound that doesn't exit
        }
        s.source.Play();
    }

}
