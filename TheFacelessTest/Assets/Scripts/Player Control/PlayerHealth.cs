using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    Image healthOverlay;
    Color tempAlpha;
    playerController player;
    public GameObject deathScreen;

    [HideInInspector]
    public float currentHealth;

    public float maxHealth = 100f;
    public float healingRate = 2f;
    public float healingDelay = 1f;
    [Tooltip("The smaller the number, the higher the migigation")]
    [Range(0f, .9f)]
    public float blockMitigation = .5f;

    

    public int testDamage = 10;
    public float intensity = 1f;
    bool canRegen = true;
    internal bool dead = false;
    internal bool immortal = false;

    

    private void Start()
    {
        currentHealth = maxHealth;
        healthOverlay = GetComponent<Image>();
        tempAlpha = healthOverlay.color;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<playerController>();
        //deathScreen = GameObject.FindGameObjectWithTag("Death Screen");


    }

    private void Update()
    {
        #region PassiveRegen
        //if (canRegen)
        //{
        //    currentHealth += Time.deltaTime * healingRate;
        //    currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        //    tempAlpha.a = (maxHealth - currentHealth) / maxHealth;

        //    healthOverlay.color = tempAlpha;
        //}
        #endregion

        if (Input.GetKeyDown(KeyCode.K))
        {
            Damage(testDamage);
        }

        if (currentHealth <= 0 && !dead)
        {
            Death();
        }

        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    public void Damage(int damage)
    {
        if (immortal || player.blocking) return;

        currentHealth -= damage;
        player.SwordSounds.PlayOneShot(player.ReceiveDmgSound);

        //if (!player.blocking)
        //{
        //    currentHealth -= damage;
        //    player.SwordSounds.PlayOneShot(player.ReceiveDmgSound);
        //}

        //else
        //{
        //    currentHealth -= damage * blockMitigation;
        //    player.SwordSounds.PlayOneShot(player.BlockSound);
        //}

        StopCoroutine("HealingDelay");
        canRegen = false;
        tempAlpha.a = ((maxHealth - currentHealth) / maxHealth) * intensity;
        healthOverlay.color = tempAlpha;
        StartCoroutine("HealingDelay");
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
        player.SwordSounds.PlayOneShot(player.DeathSound);
        deathScreen.SetActive(true);

        
    }
    

    IEnumerator HealingDelay()
    {
        yield return new WaitForSeconds(healingDelay);
        canRegen = true;
    }
}
