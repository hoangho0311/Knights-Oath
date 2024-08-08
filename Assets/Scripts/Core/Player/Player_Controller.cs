using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.VFX;
using DG.Tweening.Core.Easing;


public class Player_Controller : MonoBehaviour
{
    [Header("Controll")]
    public GameManager gameManager;

    [Header("Reference")]
    public PlayerLifeController lifeBarScript;
    public Player_Sound_Controller player_Sound;
    public Transform model;
    public Transform boss;
    public Animator bossAnim;

    private float moveSpeed = 4;
    private Animator anim;
    private Vector3 stickDirection;
    private Camera mainCamera;

    private CapsuleCollider capsuleCol;
    private Rigidbody rb;

    //public AudioClip swordDamageSound;

    private float lastDamageTakenTime = 0;
    private Vector3 forwardLocked;

    [Header("Camera")]
    public CameraShaker shaker;
    
    [Header("Target")]
    private EnemyDetection enemyDetection;
    private CinemachineImpulseSource impulseSource;
    private Boss_Controller lockedTarget;

    [Header("Combat Settings")]
    [SerializeField] private float attackCooldown;
    public Vector2 moveAxis;
    string[] attacks;

    [Header("Heal")]
    public Slider healSlider;

    [Header("VFX Slash")]
    public List<Slash> slashList;

    [Header("VFX")]
    public VisualEffect sparkVFX;
    public VisualEffect abilityVFX;
    public VisualEffect abilityHitVFX;
    public VisualEffect healVFX;
    void Start()
    {
        gameManager = GameManager.Instance;

        anim = model.GetComponent<Animator>();
        mainCamera = Camera.main;
        capsuleCol = GetComponentInChildren<CapsuleCollider>();
        rb = GetComponentInParent<Rigidbody>();

        enemyDetection = GetComponentInChildren<EnemyDetection>();
        player_Sound = GetComponent<Player_Sound_Controller>();
        impulseSource = GetComponentInChildren<CinemachineImpulseSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.GetGameOver() || gameManager.GetGamePause()) return;
        stickDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        moveAxis.x = stickDirection.x;
        moveAxis.y = stickDirection.y;
        Move();
        Rotation();
        Attack();
        Dodge();
        Healing();
        //Block();
    }


    private void Move()
    {
        float x = mainCamera.transform.TransformDirection(stickDirection).x;
        float z = mainCamera.transform.TransformDirection(stickDirection).z;
        if (x > 1) x = 1;
        if (z > 1) z = 1;

        if (anim.GetBool("CanMove"))
        {
            if (Mathf.Abs(anim.GetFloat("Speed")) > 0.15f)
                model.position += new Vector3(x * moveSpeed * Time.deltaTime, 0, z * moveSpeed * Time.deltaTime);
            float clampValue = 1;
            anim.SetFloat("Speed", Vector3.ClampMagnitude(stickDirection, clampValue).magnitude, 0.02f, Time.deltaTime);
            anim.SetFloat("Horizontal", stickDirection.x); // lockedCamera
            anim.SetFloat("Vertical", stickDirection.z); // lockedCamera
            //if (anim.GetBool("Drinking") && anim.GetFloat("Speed") > 0.25f) anim.SetFloat("Speed", 0.25f); // desacelera o jogador caso ele esteja bebendo
            //if (anim.GetBool("Drinking") && anim.GetFloat("Vertical") > 0.25f) anim.SetFloat("Vertical", 0.25f); // desacelera o jogador caso ele esteja bebendo
        }
    }

    private void Rotation()
    {
        if (anim.GetBool("Attacking")) return;

        if (!anim.GetBool("LockedCamera")) // camera livre
        {
            Vector3 rotationOffset = mainCamera.transform.TransformDirection(stickDirection) * 4f;
            rotationOffset.y = 0;
            model.forward += Vector3.Lerp(model.forward, rotationOffset, Time.deltaTime * 30f);
        }
        else // camera locked
        {
            //DodgeController();
            model.forward += Vector3.Lerp(model.forward, forwardLocked, Time.deltaTime * 20f);
        }

    }

    #region Heal
    private void Healing()
    {
        if (Input.GetKeyDown(KeyCode.R) && healSlider.value > 0 && !anim.GetBool("Drinking") && !anim.GetBool("Dodging"))
        {
            anim.SetTrigger("Drink");
            StartCoroutine(HealingCoroutine());
        }
    }

    IEnumerator HealingCoroutine()
    {
        PlayHealVFX();
        yield return new WaitForSeconds(1f);
        if (!anim.GetBool("Dead"))
        {
            healSlider.value -= 0.5f;
            lifeBarScript.UpdateLife(3);
        }
        yield return new WaitForSeconds(3f);
    }
    #endregion

    #region Block
    private void Block()
    {
        if (Input.GetKey(KeyCode.Q) && !anim.GetBool("Drinking") && !anim.GetBool("Dodging"))
        {
            anim.SetBool("Block", Input.GetKey(KeyCode.Q));
            //StartCoroutine(HealingCoroutine());
        }
        else
        {
            anim.SetBool("Block", Input.GetKey(KeyCode.Q));
        }   
    }
    #endregion

    #region Attack
    private void Attack()
    {
        //if (Input.GetKeyDown(KeyCode.Mouse1) && anim.GetBool("CanAttack"))
        //{
        //    autoTarget_Controller.UpdateTarget();
        //    //anim.SetTrigger("LightAttack");

        //}
        if (Input.GetKeyDown(KeyCode.Mouse0) && anim.GetBool("CanAttack") && !anim.GetBool("Drinking") && !anim.GetBool("Blocking"))
        {
            //autoTarget_Controller.UpdateTarget();
            //anim.SetTrigger("HeavyAttack");
            AttackCheck();           
        }
        //if (Input.GetKeyDown(KeyCode.Tab) && !GameManager.Instance.GetIsBossDead())
        //{
        //    anim.SetBool("LockedCamera", !anim.GetBool("LockedCamera"));
        //}
    }

    IEnumerator SlashAttack(float delay, ParticleSystem visual)
    {
        yield return new WaitForSeconds(delay);
        visual.gameObject.SetActive(true);
        visual.Play();
        yield return new WaitForSeconds(0.4f);
        visual.gameObject.SetActive(false);
    }
    #endregion

    #region Dodge
    private bool CanDodge()
    {
        return !anim.GetCurrentAnimatorStateInfo(1).IsName("Sprinting Forward");
    }

    private void Dodge()
    {
        if (Input.GetKeyDown(KeyCode.Space) && CanDodge())
        {
            anim.SetTrigger("Dodge");
        }
    }

    #endregion

    private void OnParticleCollision(GameObject other)
    {
        //if (other.gameObject.name.Contains("Shock") && !anim.GetBool("Intangible"))
        //{
        //    TakeDamage(4);
        //    return;
        //}

        Debug.Log(other.transform.name);

        //if (other.transform.name.Contains("Flame") && !anim.GetBool("Intangible"))
        //{
        //    TakeDamage(4.2f);
        //    return;
        //}

        if (other.transform.name.Contains("Pivot") && !anim.GetBool("Intangible"))
        {
            TakeDamage(3f);
            return;
        }
    }

    #region Take Damage
    private bool DamageInterval()
    {
        return (Time.time > lastDamageTakenTime + 0.25f);
    }

    public void TakeDamage(float damageAmount)
    {
        if (damageAmount == 0 || anim.GetBool("Intangible") || !DamageInterval() || bossAnim.GetBool("Dead")) return;

        anim.SetFloat("Speed", 0);
        anim.SetFloat("Vertical", 0);
        anim.SetFloat("Horizontal", 0);
        lastDamageTakenTime = Time.time;
        capsuleCol.isTrigger = true;
        rb.isKinematic = true;
        anim.SetBool("Intangible", true);
        anim.SetBool("CanMove", false);
        player_Sound.PlayTakeDamage();
        lifeBarScript.UpdateLife(-damageAmount);
        DamageAnimation(damageAmount);
        shaker.ShakeCamera(0.3f);
    }

    private void DamageAnimation(float damageAmount)
    {
        if (damageAmount >= 3)
        {
            Vector3 dir = (boss.transform.position - model.transform.position).normalized;
            float dot = Vector3.Dot(dir, model.transform.forward);

            if (dot >= 0)
                anim.SetTrigger("FallDamage");
            else if (dot < 0)
                anim.SetTrigger("FallForward");
            return;
        }

        anim.SetTrigger("TakeDamage");
    }
    #endregion

    #region Combat

    void AttackCheck()
    {
        //If the player is moving the movement input, use the "directional" detection to determine the enemy
        if (enemyDetection.InputMagnitude() > .2f)
            lockedTarget = enemyDetection.CurrentTarget();

        //Extra check to see if the locked target was set
        if (lockedTarget == null || lockedTarget.anim.GetBool("Dead"))
        {
            lockedTarget = enemyDetection.GetNearestEnemy();
            enemyDetection.SetCurrentTarget(lockedTarget);
        }

        if (lockedTarget == null)
        {
            Attack(null, 0);
            return;
        }

        Attack(lockedTarget, TargetDistance(lockedTarget));
    }

    public void Attack(Boss_Controller target, float distance)
    {
        //Types of attack animation
        attacks = new string[] {"AT2", "AT1", "Dash"};

        //Attack nothing in case target is null
        if (target == null)
        {
            AttackType("AT2", .2f, null, 0);
            return;
        }

        if (distance < 15 && distance > 5)
        {
            //animationCount = (int)Mathf.Repeat((float)animationCount + 1, (float)attacks.Length);
            //string attackString = attacks[animationCount];
            AttackType("Dash", attackCooldown, target, .95f);
        }else if (distance <= 5)
        {
            AttackType("AT2", attackCooldown, target, .95f);
        }
        else
        {
            lockedTarget = null;
            AttackType("AT2", .2f, null, 0);
        }

        //Change impulse
        impulseSource.m_ImpulseDefinition.m_AmplitudeGain = Mathf.Max(3, 1 * distance);

    }

    void AttackType(string attackTrigger, float cooldown, Boss_Controller target, float movementDuration)
    {
        anim.SetTrigger(attackTrigger);

        if (target == null)
            return;

        MoveTorwardsTarget(target, movementDuration);
    }
    void MoveTorwardsTarget(Boss_Controller target, float duration)
    {
        transform.DOLookAt(target.transform.position, .2f);
        transform.DOMove(TargetOffset(target.transform), duration);
    }

    public Vector3 TargetOffset(Transform target)
    {
        Vector3 position;
        position = target.position;
        return Vector3.MoveTowards(position, transform.position, 1.3f);
    }

    float TargetDistance(Boss_Controller target)
    {
        return Vector3.Distance(transform.position, target.transform.position);
    }

    public void SetLockTarget(Boss_Controller enemy)
    {
        lockedTarget = enemy;
    }
    #endregion

    #region Play Slash Effects
    public void PlaySlashEffectAT2_1()
    {
        StartCoroutine(SlashAttack(slashList[0].delay, slashList[0].slashObj));
    }
    public void PlaySlashEffectAT2_2()
    {
        StartCoroutine(SlashAttack(slashList[1].delay, slashList[1].slashObj));
    }
    public void PlaySlashEffectAT2_3()
    {
        StartCoroutine(SlashAttack(slashList[2].delay, slashList[2].slashObj));
    }
    public void PlaySlashEffectDash()
    {
        StartCoroutine(SlashAttack(slashList[3].delay, slashList[3].slashObj));
    }
    #endregion

    #region VFX
    public void PlaySparkVFX()
    {
        sparkVFX.SendEvent("OnPlay");
    }
    
    public void PlayHealVFX()
    {
        healVFX.SendEvent("OnPlay");
    }
    #endregion
}

[System.Serializable]
public class Slash
{
    public ParticleSystem slashObj;
    public float delay;
}