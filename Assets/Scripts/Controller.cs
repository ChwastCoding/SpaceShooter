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

    [Range(0, 1)] public float t = .001f;
    private Vector3 previousDirection;
    public float Speed
    {
        get => speed;
        set => speed = value;
    }

    public float agility = 1f;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody >();

        _rigidbody.velocity = transform.forward * speed;
        targetDirection = transform.forward;
        previousDirection = targetDirection;
    }

    private void FixedUpdate()
    {
        var maxStep = Time.fixedDeltaTime * speed;

        /*var newDirection = Vector3.RotateTowards(transform.forward, targetDirection, maxStep, 0f);
        transform.rotation = Quaternion.LookRotation(newDirection, Vector3.up);
        */

        
        /*var angle = Vector3.SignedAngle(previousDirection, transform.forward, ) / 2.0f;
        var localUp = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle));*/

        previousDirection = Vector3.Lerp(previousDirection, targetDirection, t);

        Debug.DrawRay(Vector3.zero, previousDirection);

        var rotationAxis = Vector3.Cross(transform.forward, Vector3.up);

        var tempUp = Quaternion.AngleAxis(90, rotationAxis) * previousDirection;
        tempUp.y = -tempUp.y;
        tempUp = Quaternion.AngleAxis(180, transform.forward) * tempUp;
        
        Debug.DrawRay(Vector3.zero, tempUp);
        
/*
        var worldAngle = Vector3.Angle(transform.forward, Vector3.forward);
        localUp = Quaternion.Euler(0.0f, worldAngle, 0.0f) * localUp;
        
        */
        transform.rotation = Quaternion.LookRotation(targetDirection, tempUp);
        
        //Debug.DrawRay(Vector3.zero, localUp);
        
        _rigidbody.velocity = transform.forward * speed;
    }

    public void Shoot()
    {
        // TODO implement shooting
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
        targetDirection = Quaternion.AngleAxis(pitchYaw.y * agility, Vector3.up) * targetDirection;
    }
}
