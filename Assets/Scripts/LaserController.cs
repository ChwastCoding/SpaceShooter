using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LaserController : MonoBehaviour
{
    public float speed = 100f;
    public float damage = 20f;
    public float lifeTime = 1f;
    private float currentTime = 0f;
    
    private int layerMask;
    void Start()
    {
        layerMask = 1 << LayerMask.NameToLayer("Ships"); // TODO potentially needs a bit shift
        GetComponent<Rigidbody>().velocity = transform.up * speed;
    }

    

    private void FixedUpdate()
    {
        // var distance = speed * Time.fixedTime;
        // transform.position = transform.position + transform.up * distance;
        currentTime += Time.fixedTime;

        if (currentTime >= lifeTime)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        var health = other.gameObject.GetComponent<Health>();
        if (health != null)
        {
            health.ReduceHealth(damage);
            Debug.Log("Hit");
        }
        Destroy(gameObject);
    }
}
