using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SwordImpactDetector : MonoBehaviour
{
    private Animator playerAnim;
    private Player_Controller controller;
    private Player_Sound_Controller sound_Controller;
    //private AudioSource sparksSource; // audioSource que ira tocar o som, aleatoriamente
    //public GameObject sparkEffect; // prefab das faiscas
    //public AudioClip[] sparkSound; // som quando a espada bate em algo
    private BoxCollider swordCollider;
    public LayerMask hitLayers;
    public float checkSize = 0.12f;

    // Start is called before the first frame update
    void Start()
    {
        //sparksSource = this.GetComponent<AudioSource>();
        controller = GetComponentInParent<Player_Controller>();
        sound_Controller = GetComponentInParent<Player_Sound_Controller>();
        playerAnim = GetComponentInParent<Animator>();
        swordCollider = this.GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
       CheckTrail();
    }

    public struct BufferObj 
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 size;
        public Vector3 scale;
    }

    private LinkedList<BufferObj> trailList = new LinkedList<BufferObj>();
    private int maxFrameBuffer = 2;

    private void CheckTrail()
    {
        BufferObj bo = new BufferObj();
        bo.size = swordCollider.size;
        bo.scale = swordCollider.transform.localScale;
        bo.rotation = swordCollider.transform.rotation;
        bo.position = swordCollider.transform.position + swordCollider.transform.TransformDirection(swordCollider.center) / 2;
        trailList.AddFirst(bo);
        if (trailList.Count > maxFrameBuffer)
        {
            trailList.RemoveLast();
        }

        DetectTrailCollisions();
    }

    private void DetectTrailCollisions()
    {
        bool isFirstRound = true;
        BufferObj lastBo = new BufferObj();
        foreach (BufferObj bo in trailList) // para cada um dos azuis
        {
            if (!isFirstRound)
            {
                LinkedList<BufferObj> calculated = FillTrail(bo, lastBo);
                foreach (BufferObj cbo in calculated)
                {
                    Collider[] hits = Physics.OverlapBox(cbo.position, cbo.size / 2, cbo.rotation, hitLayers, QueryTriggerInteraction.Ignore);

                    if (hits.Length > 0)
                    {
                        //if (hits[0].gameObject.GetComponent<Destructible>() != null)
                        //    hits[0].gameObject.GetComponent<Destructible>().SwordTrailDetectedMe();

                        if (playerAnim.GetBool("Attacking") && hits[0].gameObject.GetComponent<Boss_Controller>() != null)
                        {

                            hits[0].gameObject.GetComponentInParent<Boss_Controller>().BossTakeDamage();
                            sound_Controller.PlayImpact();
                            controller.PlaySparkVFX();
                        }
                    }
                }
            }
            isFirstRound = false;
            lastBo = bo;
        }
    }

    private LinkedList<BufferObj> FillTrail(BufferObj from, BufferObj to) // preenche os gaps e retorna uma lista
    {
        LinkedList<BufferObj> fillerList = new LinkedList<BufferObj>();
        float distance = Mathf.Abs((from.position - to.position).magnitude);

        if (distance > checkSize)
        {
            float steps = Mathf.Ceil(distance / checkSize);
            float stepsAmount = 1 / (steps + 1);
            float stepValue = 0;
            for (int i = 0; i < (int)steps; i++)
            {
                stepValue += stepsAmount;
                BufferObj tmpBo = new BufferObj();
                tmpBo.size = swordCollider.size;
                tmpBo.position = Vector3.Lerp(from.position, to.position, stepValue);
                tmpBo.rotation = Quaternion.Lerp(from.rotation, to.rotation, stepValue);
                fillerList.AddFirst(tmpBo);
            }
        }
        return fillerList;
    }
}
