/*
    referenced tutorial: "Introduction to AUDIO in Unity" by Brackeys YT channel
    https://www.youtube.com/watch?v=6OT43pvUyfY&list=PL-UICBkD9yr0etwHc5deLx5xkVGJVO9rF&index=9
*/

using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound {

    public string name;

    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume;
    [Range(.1f, 3f)]
    public float pitch;

    public bool loop; // really just for background cooking music

    [HideInInspector]
    public AudioSource source;

}
