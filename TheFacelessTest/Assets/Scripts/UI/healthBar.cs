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
        //DAMAGE
        if (currentState > health.currentHealth)
        {
            currentState -= Time.deltaTime * rate;
        }

        //HEAL

        if (currentState < health.currentHealth)
        {
            currentState += Time.deltaTime * rate;
        }

        //BAR
        bar.fillAmount = currentState/health.maxHealth;
    }
}
