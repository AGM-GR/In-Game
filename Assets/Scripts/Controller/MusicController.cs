using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour {

    public List<AudioClip> music = new List<AudioClip>();

    private int musicIndex;
    private AudioSource audioSource;

    private void Awake() {
        audioSource = GetComponent<AudioSource>();
        musicIndex = Random.Range(0, music.Count-1);
    }

    private void Start() {
        audioSource.PlayOneShot(music[musicIndex]);
        StartCoroutine(PlayNextSong());
    }

    IEnumerator PlayNextSong() {
        yield return new WaitForEndOfFrame();

        yield return new WaitWhile(() => audioSource.isPlaying);

        musicIndex = (musicIndex + 1) % music.Count;
        audioSource.PlayOneShot(music[musicIndex]);

        StartCoroutine(PlayNextSong());
    }
}
