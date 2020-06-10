using System.Collections;
using UnityEngine;

public class WildFire : DischargeAttack
{
    [Header("Effects")]
    public ParticleSystem lingeringFlame;
    public ParticleSystem swordFire;
    public GameObject explosion;
    [Header("Effects")]
    public float shakeDuration;
    public float shakeMagnitude;
    public float slowDuration;
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

    float timer;
    bool shot = false;
    bool startFlame = false;

    protected override void Start()
    {
        base.Start();
        timer = travelTime;
        vel = new Vector3(speed, 0, 0);
        orPos = transform.localPosition;
        maxPos = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + maxDistance);
    }
    protected override void Update()
    {
        base.Update();
        if (AllowAction() && AllowDischarge() && skillIndex == 3)
        {
            if (Input.GetButtonDown("discharge") && !shot)
            {
                shot = true;
                StartCoroutine(Attack(cooldown, anim, delay, initialDamage, controller.heavyPoint, 0));
                StartCoroutine(Flame());

            }
        }
        if (startFlame)
        {
            timer -= Time.deltaTime;
            controller.UIDischarge(timer, travelTime);
            Guide();
            ClampDistance();
        }
    }

    IEnumerator Flame()
    {
        controller.health.Immortality(true);
        controller.canDischarge = false;
        controller.discharging = true;
        BlockMovement();
        yield return new WaitForSeconds(.8f);
        controller.timeManager.slowmoDuration = slowDuration;
        controller.timeManager.Slowmo();
        yield return new WaitForSeconds(.6f);
        controller.health.Immortality(true);
        AllowMovement();
        StartCoroutine(controller.camShake.Shake(shakeDuration, shakeMagnitude));
        Instantiate(explosion, controller.burstPoint.position, Quaternion.Euler(90, 0, 0));
        lingeringFlame.Play();
        swordFire.Play();
        startFlame = true;
        controller.cameraView.ZoomOut();
        yield return new WaitForSeconds(travelTime);
        controller.cameraView.ZoomIn();
        controller.discharging = false;
        startFlame = false;
        shot = false;
        transform.localPosition = orPos;
        lingeringFlame.Stop();
        swordFire.Stop();
        timer = travelTime;
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
