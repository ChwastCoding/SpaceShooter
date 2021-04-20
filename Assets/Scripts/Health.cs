using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Health : MonoBehaviour
{
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
    }

    public void ReduceHealth(float value)
    {
        hp -= value;
        if (hp <= 0)
        {
            OnHealthZero();
        }
    }

    public void OnHealthZero()
    {
    }
}
