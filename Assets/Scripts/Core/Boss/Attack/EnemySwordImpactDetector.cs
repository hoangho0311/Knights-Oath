using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySwordImpactDetector : MonoBehaviour
{
    private BoxCollider swordCollider;
    public float checkSize;
    public LayerMask hitLayers;

    private void Start()
    {
        swordCollider = GetComponent<BoxCollider>();
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

    private void Update()
    {
        CheckTrail();
    }

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
        foreach (BufferObj bo in trailList)
        {
            if (!isFirstRound)
            {
                LinkedList<BufferObj> calculated = FillTrail(bo, lastBo);
                foreach (BufferObj cbo in calculated)
                {
                    Collider[] hits = Physics.OverlapBox(cbo.position, Vector3.Scale(bo.size, bo.scale), cbo.rotation, hitLayers, QueryTriggerInteraction.Ignore);

                    if (hits.Length > 0)
                    {
                        GreatSwordFiller(hits[0].gameObject);
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

    public bool damageOn;
    public float damageAmount;

    public float GetDamage()
    {
        return damageAmount;
    }

    public void GreatSwordFiller(GameObject other)
    {
        if (!damageOn) return;

        if (other.gameObject.CompareTag("Player"))
        {
            if (other.GetComponent<Animator>().GetBool("Intangible")) return;
            other.transform.GetComponent<Player_Controller>().TakeDamage(damageAmount);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!damageOn) return;

        if (other.gameObject.CompareTag("Player"))
        {
            if (other.GetComponent<Animator>().GetBool("Intangible")) return;
            other.transform.GetComponent<Player_Controller>().TakeDamage(damageAmount);
        }
    }
}
