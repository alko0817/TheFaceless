using System.Collections;
using UnityEngine;

public class WildFire : DischargeAttack
{
    [Header("Effects")]
    public ParticleSystem lingeringFlame;
    public ParticleSystem swordFire;
    public GameObject explosion;
    [Header("Wild Fire")]
    public float speed;
    public float travelTime;
    public LayerMask enemyLayer;
    public int wildFireDamage;
    public float dmgRadius;
    [Range(3f, 30f)]
    public float maxDistance;
    [Header("Initial Burst")]
    public float cooldown = 1f;
    public float delay = 1f;
    public string anim;
    public int initialDamage = 5;

    Vector3 maxPos;
    Vector3 orPos;
    Vector3 vel;

    bool shot = false;
    bool startFlame = false;

    protected override void Start()
    {
        base.Start();
        vel = new Vector3(speed, 0, 0);
        orPos = transform.localPosition;
        maxPos = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + maxDistance);
    }
    protected override void Update()
    {
        base.Update();
        if (!AllowAction() || !AllowDischarge() || skillIndex != 3) return;
        if (Input.GetButtonDown("discharge") && !shot)
        {
            shot = true;
            StartCoroutine(Attack(cooldown, anim, delay, initialDamage, controller.heavyPoint, 0));
            StartCoroutine(Flame());
            
        }
        if (startFlame)
        {
            Guide();
            ClampDistance();
        }
    }

    IEnumerator Flame()
    {
        controller.health.Immortality(true);
        BlockMovement();
        yield return new WaitForSeconds(.8f);
        controller.timeManager.slowmoDuration = 1f;
        controller.timeManager.Slowmo();
        yield return new WaitForSeconds(.6f);
        controller.health.Immortality(true);
        AllowMovement();
        StartCoroutine(controller.camShake.Shake(controller.shakeDuration, controller.shakeMagnitude));
        Instantiate(explosion, controller.burstPoint.position, Quaternion.Euler(90, 0, 0));
        lingeringFlame.Play();
        swordFire.Play();
        startFlame = true;
        controller.cameraView.ZoomOut();
        yield return new WaitForSeconds(travelTime);
        controller.cameraView.ZoomIn();
        startFlame = false;
        shot = false;
        transform.localPosition = orPos;
        lingeringFlame.Stop();
        swordFire.Stop();
    }

    void Guide()
    {
        if (Input.GetKey(KeyCode.Mouse3))
        {
            transform.Translate(vel);
        }

        if (Input.GetKey(KeyCode.Mouse4))
        {
            transform.Translate(-vel);
        }

        Collider[] enemies = Physics.OverlapSphere(transform.position, dmgRadius, enemyLayer);

        foreach (Collider enemy in enemies)
        {
            enemy.GetComponent<EnemyBase>().TakeDamage(wildFireDamage);
        }
    }

    void ClampDistance ()
    {
        float distance = Vector3.Distance(orPos, transform.localPosition);
        if (distance > maxDistance) transform.localPosition = maxPos;
        if (transform.localPosition.magnitude < orPos.magnitude) transform.localPosition = orPos;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, dmgRadius);
    }
}
