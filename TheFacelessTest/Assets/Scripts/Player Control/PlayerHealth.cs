using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    Image healthOverlay;
    Color tempAlpha;
    playerController player;
    Animator anim;
    public GameObject deathScreen;
    internal float currentHealth;
    public float maxHealth = 100f;
    public float healingRate = 2f;
    public float healingDelay = 1f;
    [Tooltip("The smaller the number, the higher the migigation")]
    [Range(0f, .9f)]
    public float blockMitigation = .5f;

    

    public int testDamage = 10;
    public float intensity = 1f;
    internal bool regening = false;
    bool canRegen = true;
    internal bool dead = false;
    internal bool immortal = false;
    private void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
        healthOverlay = GetComponent<Image>();
        tempAlpha = healthOverlay.color;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<playerController>();
        //deathScreen = GameObject.FindGameObjectWithTag("Death Screen");
    }

    private void Update()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (Input.GetKeyDown(KeyCode.K)) Damage(testDamage);
        if (currentHealth <= 0 && !dead) Death();
    }

    public void Damage(int damage)
    {
        if (immortal || player.blocking) return;

        anim.SetTrigger("hit");

        currentHealth -= damage;
        int rand = Random.Range(0, player.ReceiveDmgSound.Length);
        player.SwordSounds.PlayOneShot(player.ReceiveDmgSound[rand]);

        StopCoroutine("HealingDelay");
        canRegen = false;
        tempAlpha.a = ((maxHealth - currentHealth) / maxHealth) * intensity;
        healthOverlay.color = tempAlpha;
        StartCoroutine("HealingDelay");
    }
    public void PassiveRegen(float healRate)
    {
        if (canRegen)
        {
            regening = true;
            currentHealth += Time.deltaTime * healRate;
            tempAlpha.a = (maxHealth - currentHealth) / maxHealth * intensity;
            healthOverlay.color = tempAlpha;
        }
        else regening = false;
    }
    public void Heal (int heal)
    {
        currentHealth += heal;
        tempAlpha.a = ((maxHealth - currentHealth) / maxHealth) * intensity;
        healthOverlay.color = tempAlpha;

    }

    public void ResetHealth ()
    {
        currentHealth = maxHealth;
        tempAlpha.a = 0;
        healthOverlay.color = tempAlpha;
        canRegen = true;
    }

    public void Immortality (bool value)
    {
        immortal = value;
    }

    void Death ()
    {
        if (!player.canDie) return;
        canRegen = false;
        dead = true;
        player.anim.SetTrigger("die");
        player.anim.SetBool("dead", true);
        player.SwordSounds.PlayOneShot(player.DeathSound);
        deathScreen.SetActive(true);

        
    }
    

    IEnumerator HealingDelay()
    {
        yield return new WaitForSeconds(healingDelay);
        canRegen = true;
    }
}
