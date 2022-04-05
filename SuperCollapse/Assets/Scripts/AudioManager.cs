using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // SINGLETON
    public static AudioManager Instance = null;

    public Sound[] sounds;
    

    // Start is called before the first frame update
    void Awake() {
        
        if(Instance == null) {
            Instance = this;
        } else if (Instance != this) {
            Destroy(Instance);
        }
        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds) {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    private void Start()
    {
        Play("BGM");
    }

    public void Play(string name) {
        Sound s = Array.Find(sounds, s => s.name == name);
        if(s == null) {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.Play();
    }
}
