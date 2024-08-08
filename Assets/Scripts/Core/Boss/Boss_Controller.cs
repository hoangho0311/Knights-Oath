using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public enum EnemyType
{
    DualSwordBoss, KatanaBoss
}
public class Boss_Controller : MonoBehaviour
{
    [Header("Control")]
    public bool AI;

    [Header("References")]
    public Animator anim;
    public Transform model;
    public Transform player;
    private Animator playerAnim;
    public EnemySwordImpactDetector greatSword;
    public CameraShaker shaker;
    public EnemyType enemyType;
    private SoundsManager soundsManager;
    //public GameManagerScript gameManager;

    [Header("Attacks")]
    public GameObject dragonFlamePrefab;
    public ParticleSystem slashObj;

    [Header("AI Manager")]
    public float nearValue;
    public float farValue;
    public float chillTime;
    private string action;
    private float lastActionTime;
    private float distance;
    private float chillDirection;
    private bool canBeginAI;
    private int lastAttack = 0; 

    // SlowBossDown
    private bool slowDown;
    private string actionAfterSlowDown;

    [Header("Take Damage")]
    public AudioClip[] takeDamageSound;
    public BossLifeController bossLifeScript;

    private float rotationSpeed = 6;
    private float lastDamageTakenTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        anim = model.GetComponent<Animator>();
        playerAnim = player.GetComponent<Animator>();
        soundsManager = SoundsManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (anim.GetBool("Dead")) return;

        Rotate();

        distance = Vector3.Distance(model.transform.position, player.transform.position);
        if (distance < 20 && !anim.GetBool("Equipped"))
        {
            anim.SetTrigger("DrawSword");
            soundsManager.PlayBattleTheme();
            StartCoroutine(StartAI());
        }

        if (!anim.GetBool("Equipped")) return;

        if (!canBeginAI) return;

        if (AI && !playerAnim.GetBool("Dead"))
        {
            AI_Manager();
        }

        greatSword.damageOn = anim.GetBool("Attacking"); 
    }
    IEnumerator StartAI()
    {
        yield return new WaitForSeconds(1.3f);
        canBeginAI = true;
    }

    private void Rotate()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("idle") || !anim.GetBool("CanRotate"))
        {
            Vector3 rotationOffset = player.transform.position - model.position;
            rotationOffset.y = 0;
            float lookDirection = Vector3.SignedAngle(model.forward, rotationOffset, Vector3.up);
            anim.SetFloat("LookDirection", lookDirection);
        }
        else if (!anim.GetBool("Attacking") && anim.GetBool("CanRotate"))
        {
            model.transform.LookAt(player.transform.position);

            var targetRotation = Quaternion.LookRotation(player.transform.position - model.transform.position);

            // Smoothly rotate towards the target point.
            model.transform.rotation = Quaternion.Slerp(model.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        model.transform.eulerAngles = new Vector3(0, model.transform.eulerAngles.y, 0);
    }  

    private void AI_Manager()
    {
        if (action == "Wait" || anim.GetBool("Dead")) return;

        if (action == "Move")
        {
            MoveToPlayer();
        }

        if (action == "WaitForPlayer")
        {
            WaitForPlayer();
        }

        if (action == "FarAttack")
        {
            if (!anim.GetBool("TakingDamage"))
                FarAttack();
        }

        if (action == "NearAttack")
        {
            if (!anim.GetBool("TakingDamage"))
                NearAttack();            
        }
    }

    private void MoveToPlayer()
    {
        anim.SetFloat("Horizontal", 0);


        float speedValue = distance / 15;
        if (speedValue > 1) speedValue = 1;

        if (slowDown)
        {
            SlowBossDown();
            return;
        }

        if (distance < nearValue) 
        {
            actionAfterSlowDown = "CallNextMove";
            slowDown = true;
        }
        else if (Time.time - lastActionTime > chillTime)
        {
            actionAfterSlowDown = "FarAttack";
            slowDown = true;
        }
        else
        {
            anim.SetFloat("Vertical", speedValue);  
        }
    }   

    private void SlowBossDown()
    {
        if (anim.GetFloat("Vertical") <= 0.4f)
        {
            slowDown = false;
            if (actionAfterSlowDown == "CallNextMove")
            {
                action = "Wait";
                anim.SetFloat("Vertical", 0);
                anim.SetFloat("Horizontal", 0);
                StartCoroutine(WaitAfterNearMove());
            }
            else if (actionAfterSlowDown == "FarAttack")
            {
                action = "FarAttack";
            }
        }
        else
        {
            anim.SetFloat("Vertical", Mathf.Lerp(anim.GetFloat("Vertical"), 0, 1 * Time.deltaTime));
        }
    }

    private void FarAttack()
    {
        anim.SetFloat("Vertical", 0);
        anim.SetFloat("Horizontal", 0);
        if (enemyType == EnemyType.DualSwordBoss)
        {
            int rand = 0;
            //do
            //{
            //    rand = Random.Range(0, 1);
            //} while (rand == lastAttack);
            //lastAttack = rand;

            switch (rand)
            {
                case 0:
                    anim.SetTrigger("FarSlash"); // Fireball
                    break;
                case 1:
                    anim.SetTrigger("Scream");
                    break;
                default:
                    break;
            }
        }
        else if (enemyType == EnemyType.KatanaBoss)
        {
            anim.SetTrigger("FarSlash");
        }

            action = "Wait";
    }

    private void NearAttack()
    {
        anim.SetFloat("Vertical", 0);
        anim.SetFloat("Horizontal", 0);

        if (enemyType == EnemyType.DualSwordBoss)
        {
            //int rand = Random.Range(0, 5);
            int rand = 0;
            do
            {
                rand = Random.Range(0, 5);
            } while (rand == lastAttack);
            lastAttack = rand;

            switch (rand)
            {
                case 0:
                    anim.SetTrigger("SpinAttack");
                    break;
                case 1:
                    anim.SetTrigger("ForwardAttack");
                    break;
                case 2:
                    anim.SetTrigger("ForwardAttack1");
                    break;
                case 3:
                    anim.SetTrigger("ForwardAttack2");
                    break;
                case 4:
                    anim.SetTrigger("Combo");
                    break;
                case 5:
                    anim.SetTrigger("Combo2");
                    break;
                default:
                    break;
            }
        }
        else if (enemyType == EnemyType.KatanaBoss)
        {
            int rand = 0;
            do
            {
                rand = Random.Range(0, 3);
            } while (rand == lastAttack);
            lastAttack = rand;

            switch (rand)
            {
                case 0:
                    anim.SetTrigger("ForwardAttack");
                    break;
                case 1:
                    anim.SetTrigger("AT1");
                    break;
                case 2:
                    anim.SetTrigger("AT2");
                    break;
                case 3:
                    anim.SetTrigger("AT3");
                    break;
                default:
                    break;
            }
        }

         action = "Wait";
    }

    private void WaitForPlayer()
    {
        anim.SetFloat("Horizontal", chillDirection);
        anim.SetFloat("Vertical", 0);

        if (distance <= nearValue && Time.time - lastActionTime > chillTime)
        {
            CallNextMove();
        }
        else

        if (distance > farValue && Time.time - lastActionTime > chillTime)
        {
            FarAttack();
        }
    }

    IEnumerator WaitAfterNearMove()
    {
        slowDown = false;
        action = "Wait";
        anim.SetFloat("Vertical", 0);
        anim.SetFloat("Horizontal", 0);
        float maxWaitTime = 6;
        float possibility = 2;
        if (anim.GetBool("Phase2"))
        {
            maxWaitTime = 5.5f;
            possibility = 2;
        }
        float waitTime;
        float decision = Random.Range(0, possibility); 
        if (decision == 0) waitTime = Random.Range(2.5f, maxWaitTime);
        else waitTime = 0;
        yield return new WaitForSeconds(waitTime);
        action = "NearAttack";
        CallNextMove();
    }

    public void CallNextMove()
    {
        lastActionTime = Time.time;

        if (distance >= farValue && !anim.GetBool("Dead"))
        {
            action = "Move";
        }
        else if (distance > nearValue && distance < farValue && !anim.GetBool("Dead"))
        {
            int rand = Random.Range(0, 2);
            if (rand == 0) chillDirection = -0.5f;
            if (rand == 1) chillDirection = 0.5f;
            action = "WaitForPlayer";
        }
        else if (distance <= nearValue && !anim.GetBool("Dead"))
        {
            action = "NearAttack";
        }
    }

    #region Skill
    public void DragonFlame()
    {
        dragonFlamePrefab.SetActive(true);
        shaker.ShakeCamera(1.5f);
    }

    public void StopDragonFlame()
    {
        dragonFlamePrefab.SetActive(false);
    }
    
    public void PlayFarSlashEffects()
    {
        slashObj.Play();
        shaker.ShakeCamera(1.5f);
    }

    #endregion

    #region Event 
    private void SetNotAttackingFalse() // a cada inicio de animacao de ataque
    {
        anim.SetBool("NotAttacking", false);
    }

    private void SetNotAttackingTrue() // setado pelo None e alguns finais de animacao
    {
        anim.SetBool("NotAttacking", true);
    }

    private void SetCanRotateTrue() // Boss podera olhar para o player
    {
        anim.SetBool("CanRotate", true);
    }

    private void SetCanRotateFalse()
    {
        anim.SetBool("CanRotate", false);
    }
    #endregion

    #region Take Damage
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Sword" && other.gameObject.GetComponentInParent<Animator>().GetBool("Attacking") && !anim.GetBool("Attacking") && DamageInterval() && !anim.GetBool("Dead"))
        {
            lastDamageTakenTime = Time.time;
            //CreateAndPlay(takeDamageSound[UnityEngine.Random.Range(0, takeDamageSound.Length)], 2); // som de dano
            StopAllCoroutines();
            if (!anim.GetBool("TakingDamage") && !anim.GetBool("Attacking") && anim.GetBool("NotAttacking"))
                anim.SetTrigger("TakeDamage");
            other.GetComponentInParent<Player_Controller>().PlaySparkVFX();
            bossLifeScript.UpdateLife(-1);
        }
    }

    public void BossTakeDamage()
    {
        if (!anim.GetBool("Attacking") && DamageInterval() && !anim.GetBool("Dead")) // ja esta sendo conferido se o jogador esta atacando antes de vir pra ca
        {
            lastDamageTakenTime = Time.time;
            //CreateAndPlay(takeDamageSound[UnityEngine.Random.Range(0, takeDamageSound.Length)], 2); // som de dano
            StopAllCoroutines(); // reinicia o timer de 2seg do texto
            if (!anim.GetBool("TakingDamage") && !anim.GetBool("Attacking") && anim.GetBool("NotAttacking")) // caso ja nao esteja tocando a animacao de dano
                anim.SetTrigger("TakeDamage"); // animacao de dano

            bossLifeScript.UpdateLife(-1);
        }
    }
    private bool DamageInterval()
    {
        return (Time.time > lastDamageTakenTime + 0.7f);
    }

    #endregion
}
