using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour {

    public static SoundManager singleton;

    public Image img;
    public Slider slider;

    public Sprite[] sprites;

    public bool isMuted = false;
    public float volume = 0.5f;

    public AudioSource src;

    public AudioClip[] audioClips;
    public enum Clip { CLICK = 0, CONFIRM = 1, DENY = 2, PURCHASE_PROD = 3, PURCHASE_UP = 4 };

    void Awake() {
        singleton = this;
    }

    public void PlayAudio(Clip index) {
        if (isMuted) return;
        src.PlayOneShot(audioClips[(int)index], volume);
    }

    public void UpdateVolume(float volume) {
        if (volume == 0) {
            if (!isMuted) ToggleMute();
        } else {
            this.volume = volume;
            if (isMuted) ToggleMute();
        }
    }

    public void ToggleMute() {
        isMuted = !isMuted;
        if (isMuted) {
            img.sprite = sprites[1];
            slider.value = 0;
        } else {
            img.sprite = sprites[0];
            slider.value = volume;
            PlayAudio(Clip.CONFIRM);
        }
    }

}
