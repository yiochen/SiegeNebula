using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Sound {
	public string name;
	public AudioClip clip;
	[Range(0.5f, 1.0f)]
	public float volume = 0.7f;
	[Range(0.5f, 1.5f)]
	public float pitch = 1.0f;

	//This might be useful for game reptitive game noises
	[Range(0f, 0.5f)]
	public float randomVolume = 0.0f;
	[Range(0f, 0.5f)]
	public float randomPitch = 0.0f;

	//Useful for music
	public bool isLoop = false;
	[Range(0f, 2.0f)]
	public float fadeOutTime = 0.0f;

	private AudioSource source;
	[HideInInspector]
	public bool isFading = false;
	private float fadeOutTimer = 0.0f;

	public void SetSource(AudioSource source) {
		this.source = source;
		this.source.clip = clip;
		this.source.loop = isLoop;
	}

	public bool IsPlaying() {
		return source.isPlaying;
	}

	public void Play() {
		source.volume = volume * (1 + Random.Range (-randomVolume / 2f, randomVolume / 2f));
		source.pitch = pitch * (1 + Random.Range (-randomPitch / 2f, randomPitch / 2f));
		source.Play ();
	}

	public void Stop() {
		if (fadeOutTime > 0.0f)
			isFading = true;
		else
			source.Stop ();
	}

	public void FadeOut() {
		fadeOutTimer += Time.deltaTime;
		source.volume = Mathf.Lerp (source.volume, 0, fadeOutTimer / fadeOutTime);
		if (fadeOutTimer >= fadeOutTime) {
			isFading = false;
			fadeOutTimer = 0.0f;
			source.Stop ();
		}
	}
}

public class AudioManager : MonoBehaviour {

	private Dictionary<string, Sound> soundMap;
	public Sound[] sounds;

	// Use this for initialization
	void Start () {
		soundMap = new Dictionary<string, Sound> (sounds.Length);
		foreach (Sound s in sounds) {
			GameObject go = new GameObject ("Sound_" + s.name);
			go.transform.SetParent (this.transform);
			s.SetSource (go.AddComponent<AudioSource> ());
			soundMap.Add (s.name, s);
		}
		ManagerScript.Instance.BackgroudMusic ();
	}

	void Update() {
		foreach (Sound s in sounds) {
			if (s.isFading)
				s.FadeOut ();
		}
	}

	public void PlaySound(string clipName) {
		Sound soundClip;
		if (soundMap.TryGetValue (clipName, out soundClip)) {
			soundClip.Play ();
		} else
			Debug.Log ("Clip: "+ clipName+ " was not found!");
	}

	//Not necessary for short sounds. Useful for Music
	public void StopSound(string clipName) {
		Sound soundClip;
		if (soundMap.TryGetValue (clipName, out soundClip))
			soundClip.Stop ();
		else
			Debug.Log ("Clip: "+ clipName+ " was not found!");
	}

}
