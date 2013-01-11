using UnityEngine;
using System.Collections;

public class MusicSource : MonoBehaviour {

    public AudioClip clip;
    private AudioSource source;

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
        source.playOnAwake = true;
        source.loop = true;
    }

    void Start()
    {
        source.clip = clip;
        source.Play();
    }
}
