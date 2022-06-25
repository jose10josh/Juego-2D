using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    [SerializeField] private Slider slider;
    [SerializeField] private Gradient gradient;
    [SerializeField] private Image fill;


    //private void Awake()
    //{
    //    if(isPlayer)
    //        objectController = gameObject.GetComponentInParent<EnemyController>();

    //}
    public void SetMaxHealth(float maxHealth)
    {
        slider.maxValue = maxHealth;
        slider.value = maxHealth;

        fill.color = gradient.Evaluate(1f);
    }

    public void SetHealthBarValue(float healt)
    {
        slider.value = healt;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

    
}
