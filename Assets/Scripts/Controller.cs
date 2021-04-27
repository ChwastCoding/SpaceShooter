using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
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

    public float barrelAngularSpeed = 10f;
    private float barrelAngle = 0f;
    private bool rightwardsBarrel = false;
    private bool leftwardsBarrel = false;
    public float barrelSideSpeed = 5f;

    public UnityEvent onBarrelFinished = new UnityEvent();
    
    private bool turbo = false;
    
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();

        Turbo(false);

        agility = normalAgility;
        speed = normalSpeed;

        _rigidbody.velocity = transform.forward * speed;
        targetDirection = transform.forward;
        previousDirection = targetDirection;

        //barrelSideSpeed *= Time.fixedDeltaTime;
    }


    private void FixedUpdate()
    {
        // accelerate to/from turbo
        agility = Mathf.Lerp(agility, turbo ? turboAgility : normalAgility, acceleration);
        speed = Mathf.Lerp(speed, turbo ? turboSpeed : normalSpeed, acceleration);

        previousDirection = Vector3.Lerp(previousDirection, targetDirection, t);

        var rotationAxis = Vector3.Cross(transform.forward, Vector3.up);
        var tempUp = Quaternion.AngleAxis(90, rotationAxis) * previousDirection;
        tempUp.y = -tempUp.y;

        if (rightwardsBarrel)
        {
            var direction = Vector3.Cross(transform.forward, Vector3.up).normalized;
            transform.position -= direction * barrelSideSpeed;
            
            barrelAngle -= barrelAngularSpeed;
            if (barrelAngle <= 0)
            {
                barrelAngle = 0;
                rightwardsBarrel = false;
                onBarrelFinished.Invoke();
            }
        }
        
        if (leftwardsBarrel)
        {
            var direction = Vector3.Cross(transform.forward, Vector3.up).normalized;
            transform.position += direction * barrelSideSpeed;
            
            barrelAngle += barrelAngularSpeed;
            if (barrelAngle >= 0)
            {
                barrelAngle = 0;
                leftwardsBarrel = false;
                onBarrelFinished.Invoke();
            }
        }


        tempUp = Quaternion.AngleAxis(180 + barrelAngle, transform.forward) * tempUp;

        transform.rotation = Quaternion.LookRotation(targetDirection, tempUp);

        _rigidbody.velocity = transform.forward * speed;
    }

    public void Shoot()
    {
        Debug.DrawRay(transform.position, transform.position + transform.forward * 50f, Color.red, .2f);
    }

    public void Turbo(bool state)
    {
        turbo = state;
    }

    private void Update()
    {
        Debug.DrawRay(Vector3.zero, targetDirection);
    }

    public void RightwardsBarrel()
    {
        if (!rightwardsBarrel && !leftwardsBarrel)
        {
            rightwardsBarrel = true;
            barrelAngle = 360;
        }
    }
    
    public void LeftwardsBarrel()
    {
        if (!rightwardsBarrel && !leftwardsBarrel)
        {
            leftwardsBarrel = true;
            barrelAngle = -360;
        }
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
