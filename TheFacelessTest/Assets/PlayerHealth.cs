using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    Image healthOverlay;
    public float maxHealth = 100f;
    public float healingRate = 2f;
    public float healingDelay = 1f;
    float currentHealth;
    public int testDamage = 10;
    bool canRegen = true;
    Color tempAlpha;

    private void Start()
    {
        currentHealth = maxHealth;
        healthOverlay = GetComponent<Image>();
        tempAlpha = healthOverlay.color;


    }

    private void Update()
    {
        if (canRegen)
        {
            currentHealth += Time.deltaTime * healingRate;
            currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

            tempAlpha.a = (maxHealth - currentHealth) / maxHealth;
            
            healthOverlay.color = tempAlpha;
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            Damage(testDamage);
        }

        if (currentHealth <= 0)
        {
            Death();
        }
    }

    public void Damage(int damage)
    {
        StopCoroutine("HealingDelay");
        canRegen = false;
        currentHealth -= damage;
        tempAlpha.a = (maxHealth - currentHealth) / maxHealth;
        healthOverlay.color = tempAlpha;
        StartCoroutine("HealingDelay");
    }

    IEnumerator HealingDelay ()
    {
        yield return new WaitForSeconds(healingDelay);
        canRegen = true;
    }

    void Death ()
    {
        canRegen = false;
        //death animation
    }
}
