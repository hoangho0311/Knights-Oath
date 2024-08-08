using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class BossLifeController : MonoBehaviour
{
    [Header("Reference")]
    public static GameManager GameManager;
    private SoundsManager soundsManager;
    public float maxLife;
    private float life = 0;
    private float ghost = 0;
    private Animator bossAnim;

    private float lastTime;
    private float waitTime = 1.5f;

    public Slider lifeBar;
    public Slider lifeGhost;
    public GameObject UICanvas;

    [Header("VFX")]
    public VisualEffect deadVFX;

    // Start is called before the first frame update
    void Start()
    {
        soundsManager = SoundsManager.instance;
        GameManager = GameManager.Instance;
        bossAnim = GetComponent<Animator>();
        life = maxLife;
    }

    private void FixedUpdate()
    {
        if (life > ghost)
        {
            ghost = life;
            float ghostPercentage = (ghost / maxLife) * 200;

            lifeGhost.value = ghostPercentage;
        }

        if ((Time.time > lastTime + waitTime) && ghost > life) 
        {
            ghost = Mathf.Lerp(ghost, life, 5 * Time.deltaTime);

            float ghostPercentage = (ghost / maxLife) * 200;
            lifeGhost.value = ghostPercentage;
        }

        //if (this.GetComponent<CanvasGroup>().alpha == 1 && lifeBarAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1) // desabilita a animacao da barra de vida preenchendo quando ela ja estiver cheia
        //    lifeBarAnim.enabled = false;
    }
    public void UpdateLife(float amount)
    {
        if (IsDead()) return;

        if (amount < 0)
        {
            lastTime = Time.time;
        }

        life += amount;

        if (life > maxLife) life = maxLife;
        if (life < 0) life = 0;

        if (life == 0 && !IsDead())
        {
            Die();
        }

        float lifePercentage = (life / maxLife) * 200;

        lifeBar.value = lifePercentage;
    }

    public bool IsDead()
    {
        return bossAnim.GetBool("Dead");
    }

    private void Die()
    {
        bossAnim.SetBool("Dead", true);
        soundsManager.StopBattleTheme();
        bossAnim.SetFloat("Vertical", 0);
        bossAnim.SetFloat("Horizontal", 0);
        StartCoroutine(EnemyDead());
    }

    IEnumerator EnemyDead()
    {
        yield return new WaitForSeconds(3f);
        PlayHealVFX();
        yield return new WaitForSeconds(0.4f);
        GetComponentInParent<Transform>().gameObject.SetActive(false);
    }

    public void SetUICanvas(bool d)
    {
        UICanvas.SetActive(d);
    }

    public void PlayHealVFX()
    {
        deadVFX.SendEvent("OnPlay");
    }
}
