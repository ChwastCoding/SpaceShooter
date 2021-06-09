using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Health : MonoBehaviour
{
    
    public HealthBar healthBar;
    
    [SerializeField] private float maxHp = 100f;
    public float MaxHp
    {
        get => maxHp;
    }
    private float hp = 100f;
    public float Hp
    {
        get => hp;
        set => hp = Mathf.Min(value, MaxHp);
    }

    private void Start()
    {
        Hp = MaxHp;
        healthBar.SetMaxHealth(Hp);
    }

    public void ReduceHealth(float value)
    {
        hp -= value;
        healthBar.SetHealth(hp);
        if (hp <= 0)
        {
            OnHealthZero();
        }
    }

    public void OnHealthZero()
    {
    }
}
