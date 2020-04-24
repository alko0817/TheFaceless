using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIndicator : MonoBehaviour
{
    public GameObject indicator;
    public Transform detectPoint;
    playerController controller;
    AIBehaviour enemyController;
    public float detectRange;
    public Material level;
    internal bool detected;
    internal Color intensity;
    private void Start()
    {
        controller = GetComponentInParent<playerController>();
        intensity.r = 1f;
        intensity.g = 1f;
        level.SetColor("_EmissionColor", intensity);
    }

    private void Update()
    {
        detected = Physics.CheckSphere(detectPoint.position, detectRange, controller.enemyLayer);

        if (detected)
        {
            Collider[] enemies = Physics.OverlapSphere(detectPoint.position, detectRange, controller.enemyLayer);
            foreach (Collider enemy in enemies)
            {
                Track(enemy);
            }
        }
        else indicator.SetActive(false);
        
    }

    void Track(Collider target)
    {
        indicator.SetActive(true);
        transform.LookAt(target.transform);
        enemyController = target.GetComponent<AIBehaviour>();
        intensity.g = Mathf.Abs((enemyController.shootTimer / enemyController.fireRate) - 1);

        level.SetColor("_EmissionColor", intensity);
    }


    private void OnDrawGizmosSelected()
    {
        if (detectPoint == null) return;

        Gizmos.DrawWireSphere(detectPoint.position, detectRange);
    }
}
