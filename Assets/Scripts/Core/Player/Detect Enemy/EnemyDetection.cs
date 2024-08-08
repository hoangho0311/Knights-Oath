using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EnemyDetection : MonoBehaviour
{
    private Player_Controller playerInput;

    public LayerMask layerMask;

    Vector3 inputDirection;
    private Boss_Controller currentTarget;

    private List<Boss_Controller> enemies = new List<Boss_Controller>();
    private HashSet<Boss_Controller> detectedEnemies = new HashSet<Boss_Controller>();
    private Camera cam;

    [Header("Canvas Target")]
    public Image aimSprite;
    public Vector2 uiOffset;
    
    private void Start()
    {
        cam = Camera.main;
        playerInput = GetComponentInParent<Player_Controller>();
    }

    private void Update()
    {
        var camera = Camera.main;
        var forward = camera.transform.forward;
        var right = camera.transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        inputDirection = forward * playerInput.moveAxis.y + right * playerInput.moveAxis.x;
        inputDirection = inputDirection.normalized;

        RaycastHit info;

        if (Physics.SphereCast(transform.position, 3f, inputDirection, out info, 10, layerMask))
        {
            if (info.collider.transform.GetComponent<Boss_Controller>())
                currentTarget = info.collider.transform.GetComponent<Boss_Controller>();
        }

        ChangeOBTarget();
        UpdateCameraTarget();
        UserInterface();
        ShowHealthBar();
    }

    private int currentEnemyIndex = 0;
    private void ChangeOBTarget()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (enemies.Count == 0)
            {
                // No enemies to target
                currentTarget = null;
                aimSprite.color = Color.clear;
                return;
            }

            // Increment the current enemy index to select the next enemy in the list
            currentEnemyIndex = (currentEnemyIndex + 1) % enemies.Count;
            currentTarget = enemies[currentEnemyIndex];
            playerInput.SetLockTarget(currentTarget);
            // Update the UI to show the new target
            aimSprite.transform.position = Camera.main.WorldToScreenPoint(currentTarget.transform.position + (Vector3)uiOffset);
        }
    }

    private void UpdateCameraTarget()
    {
        Vector3 forwardDirection = cam.transform.forward;
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);

        // Perform a sphere cast to detect colliders within the camera's view
        Collider[] colliders = Physics.OverlapSphere(cam.transform.position, 30f, layerMask);

        foreach (Collider col in colliders)
        {
            Boss_Controller enemy = col.GetComponent<Boss_Controller>();
            if (enemy == null)
            {
                // Skip if not a BossAttack
                continue;
            }

            // Check if the enemy is visible in camera's view
            if (GeometryUtility.TestPlanesAABB(planes, col.bounds))
            {
                Vector3 directionToCollider = col.transform.position - cam.transform.position;
                if (Vector3.Dot(forwardDirection, directionToCollider.normalized) > 0)
                {
                    // Mark the enemy as detected
                    detectedEnemies.Add(enemy);
                }
            }
            else
            {
                // Remove the enemy from detectedEnemies if it's not visible
                detectedEnemies.Remove(enemy);
            }
        }

        // Remove dead enemies from detectedEnemies
        detectedEnemies.RemoveWhere(enemy => enemy.anim.GetBool("Dead"));

        // Update the list of enemies with detectedEnemies
        enemies = new List<Boss_Controller>(detectedEnemies);
    }

    #region Target 
    private void UserInterface()
    {
        if (currentTarget == null || currentTarget.anim.GetBool("Dead"))
        {
            aimSprite.color = Color.clear;
            return;
        }

        if (IsEnemyVisible(currentTarget))
        {
            aimSprite.color = Color.white;
            aimSprite.transform.position = Camera.main.WorldToScreenPoint(currentTarget.transform.position + (Vector3)uiOffset);
        }
        else
        {
            playerInput.SetLockTarget(null);
            aimSprite.color = Color.clear;
        }
    }

    private void ShowHealthBar()
    {
        foreach(var enemy in enemies)
        {
            if(enemy == currentTarget)
            {
                enemy.GetComponent<BossLifeController>().SetUICanvas(true);
            }
            else
            {
                enemy.GetComponent<BossLifeController>().SetUICanvas(false);
            }
        }
    }
    
    //check enemy is in view
    private bool IsEnemyVisible(Boss_Controller enemy)
    {
        if (enemy == null)
        {
            return false;
        }

        Vector3 forwardDirection = cam.transform.forward;
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);

        return GeometryUtility.TestPlanesAABB(planes, enemy.GetComponent<Collider>().bounds);
    }

    public Boss_Controller CurrentTarget()
    {
        return currentTarget;
    }

    public void SetCurrentTarget(Boss_Controller target)
    {
        currentTarget = target;
    }

    public float InputMagnitude()
    {
        return inputDirection.magnitude;
    }

    public Boss_Controller GetNearestEnemy()
    {
        if (enemies.Count == 0)
        {
            return null;
        }

        Vector3 playerPosition = transform.position;

        List<Boss_Controller> sortedEnemies = enemies.OrderBy(enemy => Vector3.Distance(enemy.transform.position, playerPosition)).ToList();

        return sortedEnemies[0];
    }
    #endregion
}
