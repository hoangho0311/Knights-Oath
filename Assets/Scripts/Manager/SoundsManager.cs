using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundsManager : MonoBehaviour
{
    public static SoundsManager instance;
    public AudioSource battleTheme;

    private void Awake()
    {
        instance = this;
    }
    public void PlayBattleTheme()
    {
        battleTheme.Play();
    }
    public void StopBattleTheme()
    {
        battleTheme.Stop();
    }
}
