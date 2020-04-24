using UnityEngine;
using UnityEngine.UI;
public class Dummy : MonoBehaviour
{
    public float maxHealth;
    private float currentHealth;
    public Image healthBar;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (currentHealth <= 0)
        {
            ResetHealth();
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;            
        healthBar.fillAmount = currentHealth / maxHealth;
      
    }

    void ResetHealth()
    {
        currentHealth = maxHealth;
    }
}
