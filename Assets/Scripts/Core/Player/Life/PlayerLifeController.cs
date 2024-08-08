using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class PlayerLifeController : MonoBehaviour
{
    private float life = 10;
    private float ghost = 10;
    private Animator playerAnim;

    private float lastTime;
    private float waitTime = 1.5f;

    //// Bleeding
    //public GameObject bleedingParent;
    //public Image bleedingBar;
    //private float bleeding;

    private bool SloDownTime;
    private float journeyLength = 15;
    private float startTime = -1;
    
    public GameManager gameManager;
    public UIManager uiManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        uiManager = UIManager.instance;
        playerAnim = GetComponent<Animator>();
        uiManager.UpdateLifeBar(life);
        uiManager.UpdateLifeGhost(ghost);

        //PostProcessVolume volume = Camera.main.GetComponent<PostProcessVolume>();
        //volume.profile.TryGetSettings(out colorGradingLayer);
    }

    private void FixedUpdate()
    {
        if (SloDownTime && Time.timeScale > 0.5f)
        {
            if (startTime <= 0)
                startTime = Time.time;
            float distCovered = (Time.time - startTime) * 0.1f;
            float fractionOfJourney = distCovered / journeyLength;
            Time.timeScale = Mathf.Lerp(Time.timeScale, 0.5f, fractionOfJourney);
        }

        if (life > ghost)
        {
            ghost = life;
            uiManager.UpdateLifeGhost(ghost);
        }

        if (Time.time > lastTime + waitTime && ghost > life)
        {
            ghost = Mathf.Lerp(ghost, life, 5 * Time.deltaTime);
            uiManager.UpdateLifeGhost(ghost);
        }
    }

    public void UpdateLife(float amount)
    {
        if (amount < 0)
        {
            lastTime = Time.time;
        }
        else
        {
            StopAllCoroutines();
        }

        life += amount;

        if (life > 10) life = 10;
        if (life < 0) life = 0;

        if (life == 0 && !playerAnim.GetBool("Dead"))
        {
            Die();
        }

        uiManager.UpdateLifeBar(life);
    }

    private void Die()
    {
        SoundsManager.instance.StopBattleTheme();
        playerAnim.SetFloat("Vertical", 0);
        playerAnim.SetFloat("Horizontal", 0);
        playerAnim.SetFloat("Speed", 0);
        playerAnim.SetBool("Dead", true);
        playerAnim.gameObject.GetComponent<IKFootPlacement>().SetIntangibleOn();
        gameManager.SetGameOver(true);
        gameManager.HideCursor(false);
        //if (bossLifeManager.GetBossLifeAmount() <= 4) achievementManager.TriggerAlmostThere(); // morreu e o boss tinha 10% ou menos de vida

        //StartCoroutine(ShowDeathCounter());
    }

    public bool IsDead()
    {
        return playerAnim.GetBool("Dead");
    }

    public bool GetNoDamageTaken()
    {
        return life == 10;
    }

}
