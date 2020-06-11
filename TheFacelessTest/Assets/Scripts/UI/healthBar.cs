using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class healthBar : MonoBehaviour
{
    public Image bar;
    public PlayerHealth health;
    public float rate;
    float currentState;

    private void Start()
    {
        bar.fillAmount = 1f;
        currentState = health.maxHealth;
    }

    private void Update()
    {
        Reduce();
        if (health.regening) Increment();
        else BurstIncrease();

        bar.fillAmount = currentState/health.maxHealth;
    }

    void Reduce()
    {
        if (currentState > health.currentHealth)
        {
            currentState -= Time.deltaTime * rate;
        }
    }
    
    void BurstIncrease()
    {
        if (currentState < health.currentHealth)
        {
            currentState += Time.deltaTime * rate;
        }
    }

    void Increment()
    {
        currentState = health.currentHealth;
    }
}
