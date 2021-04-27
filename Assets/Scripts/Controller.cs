using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody))]
public class Controller : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private Vector3 targetDirection;
    
    [SerializeField] private float speed = 10f;
    [SerializeField] private float turboSpeed = 30f;
    [SerializeField] private float normalSpeed = 20f;
    
    [Range(0, 1)] public float t = .001f;
    [Range(0, 1)] public float acceleration = .1f;
    private Vector3 previousDirection;
    public float Speed
    {
        get => speed;
        set => speed = value;
    }

    public float turboAgility = .07f;
    public float normalAgility = .1f;
    public float agility = 0.1f;
    
    private bool turbo = false;
    
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody >();

        Turbo(false);

        agility = normalAgility;
        speed = normalSpeed;
        
        _rigidbody.velocity = transform.forward * speed;
        targetDirection = transform.forward;
        previousDirection = targetDirection;
    }


    private void FixedUpdate()
    {
        agility = Mathf.Lerp(agility, turbo ? turboAgility : normalAgility, acceleration);
        speed = Mathf.Lerp(speed, turbo ? turboSpeed : normalSpeed, acceleration);
        
        previousDirection = Vector3.Lerp(previousDirection, targetDirection, t);

        var rotationAxis = Vector3.Cross(transform.forward, Vector3.up);
        var tempUp = Quaternion.AngleAxis(90, rotationAxis) * previousDirection;
        tempUp.y = -tempUp.y;
        tempUp = Quaternion.AngleAxis(180, transform.forward) * tempUp;
        
        transform.rotation = Quaternion.LookRotation(targetDirection, tempUp);
        
        _rigidbody.velocity = transform.forward * speed;
    }

    public void Shoot()
    {
        // TODO implement shooting
    }

    public void Turbo(bool state)
    {
        turbo = state;
    }

    private void Update()
    {
        Debug.DrawRay(Vector3.zero, targetDirection);
    }

    public void TurnTowardsGlobal(Vector3 direction)
    {
        targetDirection = direction;
    }

    public void Turn(Vector2 pitchYaw)
    {
        targetDirection = Quaternion.AngleAxis(-pitchYaw.x * agility, transform.right) * targetDirection;
        targetDirection = Quaternion.AngleAxis(pitchYaw.y * agility, transform.up) * targetDirection;
    }
}
