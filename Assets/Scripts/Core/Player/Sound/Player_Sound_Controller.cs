using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Sound_Controller : MonoBehaviour
{
    public AudioClip SlashSword1;
    public AudioClip SlashSword2;
    public AudioClip SlashSword3;
    public AudioClip footStep;
    public AudioClip impact;
    public AudioClip takeDamage;
    public AudioSource footSoundSource;
    private Animator anim;
    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void CreateAndPlay(AudioClip clip, float destructionTime, float volume = 1f, float minDistance = 15f)
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.spatialBlend = 1;
        audioSource.minDistance = minDistance;
        audioSource.maxDistance = 50;
        audioSource.Play();
        Destroy(audioSource, destructionTime);
    }

    public void PlaySlashSword1()
    {
        CreateAndPlay(SlashSword1, 2);
    }
    public void PlaySlashSword2()
    {
        CreateAndPlay(SlashSword2, 2);
    }
    public void PlaySlashSword3()
    {
        CreateAndPlay(SlashSword3, 2);
    }
    public void PlayImpact()
    {
        CreateAndPlay(impact, 0.3F, 0.3f);
    }
    public void PlayTakeDamage()
    {
        CreateAndPlay(takeDamage, 0.4f, 0.5f);
    }
    public void PlayFootStep()
    {
        footSoundSource.transform.position = this.transform.root.GetChild(0).transform.position;
        if (!footSoundSource.isPlaying && !anim.GetBool("Dead"))
            footSoundSource.Play();
    }
}
