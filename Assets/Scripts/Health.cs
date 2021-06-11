using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

[RequireComponent(typeof(Collider))]
public class Health : MonoBehaviour
{
    
    public HealthBar healthBar;
    public ParticleSystem explosion;

    public UnityEvent onHealthZero = new UnityEvent();
    
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
        // healthBar.SetMaxHealth(Hp);
    }

    public void ReduceHealth(float value)
    {
        hp -= value;
        // healthBar.SetHealth(hp);
        if (hp <= 0)
        {
            OnHealthZero();
            onHealthZero.Invoke();
        }
    }

    public void OnHealthZero()
    {
        ParticleSystem explosionInstance = Instantiate(explosion, transform.position, transform.rotation);
        explosionInstance.Play();
        
        //GetComponent<MeshRenderer>()
            Destroy(gameObject);
        
        Destroy(explosionInstance.gameObject, 3.0f);
    }

    private void OnCollisionEnter(Collision other)
    {
        ReduceHealth(MaxHp);
        Debug.Log("COLLISION");
    }
}
