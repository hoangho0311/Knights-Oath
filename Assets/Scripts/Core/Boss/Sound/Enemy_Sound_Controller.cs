using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Sound_Controller : MonoBehaviour
{
    public AudioClip SlashSword;
    public AudioClip CircleSlash;
    public AudioClip FarSlash;
    public AudioClip[] takeDamage;
    public AudioClip[] footStep;
    public AudioClip[] greatSwordHit;
    public AudioSource footSoundSource;
    private Animator anim;

    private void Start()
    {
        anim = this.GetComponent<Animator>();
    }

    public void PlaySlashSword()
    {
        CreateAndPlay(SlashSword, 2);
    }

    public void PlayCircleSlash()
    {
        CreateAndPlay(CircleSlash, 0.7f);
    }

    public void PlayFarSlash()
    {
        CreateAndPlay(FarSlash, 2);
    }

    public void PlayTakeDamage()
    {
        CreateAndPlay(takeDamage[Random.Range(0, takeDamage.Length)], 2);
    }

    public void PlayFootStep()
    {
        footSoundSource.transform.position = this.transform.root.GetChild(0).transform.position;
        if (!footSoundSource.isPlaying && !anim.GetBool("Dead") && anim.GetBool("CanRotate"))
            footSoundSource.Play();
    }

    public void PlayGreatSwordHit()
    {
        CreateAndPlay(greatSwordHit[Random.Range(0, greatSwordHit.Length)], 1.5f, 1, 10);
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
}
