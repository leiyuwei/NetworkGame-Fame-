using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : SingleMonoBehaviour<SoundManager> {

	public AudioSource bgmAudioSource;
	public AudioSource seAudioSource;

	public AudioClip bgm;
	public AudioClip se;
	public AudioClip hit;

	public void PlayBGM(){
		bgmAudioSource.clip = bgm;
		bgmAudioSource.loop = true;
		bgmAudioSource.Play ();
	}

	public void PlaySE(AudioClip clip){
		seAudioSource.PlayOneShot (clip);
	}

}
