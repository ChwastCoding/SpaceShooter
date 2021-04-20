using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody))]
public class Controller : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private Vector3 targetDirection;
    
    [SerializeField] private float speed = 10f;
    public float Speed
    {
        get => speed;
        set => speed = value;
    }

    public float agility = 1f;
    
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();

        _rigidbody.velocity = transform.forward * speed;
        targetDirection = transform.forward;
    }

    private void FixedUpdate()
    {
        var maxStep = Time.fixedDeltaTime * speed;

        var newDirection = Vector3.RotateTowards(transform.forward, targetDirection, maxStep, 0f);
        transform.rotation = Quaternion.LookRotation(newDirection);
        
        _rigidbody.velocity = transform.forward * speed;
    }

    public void Shoot()
    {
        // TODO implement shooting
    }

    public void TurnTowardsGlobal(Vector3 direction)
    {
        targetDirection = direction;
    }

    public void Turn(Vector2 pitchYaw)
    {
        targetDirection = Quaternion.Euler(-pitchYaw.x, pitchYaw.y, 0) * targetDirection;
    }
}
