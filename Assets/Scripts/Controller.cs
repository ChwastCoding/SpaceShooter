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
    //private Vector3 targetDirection;
    //private Vector3 targetUp;
    private Quaternion targetRotation, previousRotation;

    [SerializeField] private float speed = 10f;
    [SerializeField] private float turboSpeed = 30f;
    [SerializeField] private float normalSpeed = 20f;

    public Transform[] laserOrigins;
    public LaserController laserPrefab;
    
    [Range(0, 1)] public float t = .001f;
    [Range(0, 1)] public float acceleration = .1f;
    //private Vector3 previousDirection;

    public float Speed
    {
        get => speed;
        set => speed = value;
    }

    public float turboAgility = .07f;
    public float normalAgility = .1f;
    public float agility = 0.1f;

    public float angularSpeed = .1f;
    
    public float barrelAngularSpeed = 10f;
    private float barrelAngle = 0f;
    private bool rightwardsBarrel = false, leftwardsBarrel = false;
    public float barrelSideSpeed = 5f;
    private Vector3 barrelSideDirection = Vector3.zero;

    public UnityEvent onBarrelFinished = new UnityEvent();
    
    private bool turbo = false;

    [Range(0, 1)] public float upwardsBias = .05f;
    
    
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();

        Turbo(false);

        agility = normalAgility;
        speed = normalSpeed;

        _rigidbody.velocity = transform.forward * speed;
        // targetDirection = transform.forward;
        // previousDirection = targetDirection;
        // targetUp = Vector3.forward;

        targetRotation = previousRotation = transform.rotation;
    }


    private void FixedUpdate()
    {
        // accelerate to/from turbo
        agility = Mathf.Lerp(agility, turbo ? turboAgility : normalAgility, acceleration);
        speed = Mathf.Lerp(speed, turbo ? turboSpeed : normalSpeed, acceleration);

        /*previousDirection = Vector3.Lerp(previousDirection, targetDirection, t);

        //var rotatedUp = Quaternion.AngleAxis(transform.rotation.eulerAngles.x, transform.right) * Vector3.up;
        var rotatedUp = Vector3.up;

        var rotationAxis = Vector3.Cross(transform.forward, rotatedUp);
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


        tempUp = Quaternion.AngleAxis(180 + barrelAngle, transform.forward) * tempUp;*/

        // transform.rotation = Quaternion.LookRotation(targetDirection, tempUp);

        if (rightwardsBarrel)
        {
            transform.position += barrelSideDirection * barrelSideSpeed;
            
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
            transform.position -= barrelSideDirection * barrelSideSpeed;
            
            barrelAngle += barrelAngularSpeed;
            if (barrelAngle >= 0)
            {
                barrelAngle = 0;
                leftwardsBarrel = false;
                onBarrelFinished.Invoke();
            }
        }
        
        previousRotation = Quaternion.Slerp(previousRotation, targetRotation, t);
        
        var angle = Vector3.SignedAngle(transform.forward, previousRotation * Vector3.forward, transform.up);
        var angleToLevel = Vector3.Angle(transform.forward, Vector3.up);
        var biasBase = (1 - Mathf.Abs(angleToLevel - 90) / 90);
        var bias = Mathf.Pow(biasBase, 2f);
        var rotationalBias = Mathf.Pow(biasBase, 3f);
        
        targetRotation = Quaternion.Slerp(targetRotation, Quaternion.LookRotation(transform.forward, Vector3.up), upwardsBias * bias);
        var rotation = Quaternion.AngleAxis(angle / 2f * rotationalBias, transform.forward) * targetRotation;
        
        
        
        transform.rotation = rotation;
        // transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, agility);
        
        transform.Rotate(Vector3.forward, barrelAngle);
        
        _rigidbody.velocity = transform.forward * speed;
    }

    public void Shoot()
    {
        for (int i = 0; i < laserOrigins.Length; i++)
        {
            var origin = laserOrigins[i];
            var laser = Instantiate(laserPrefab, origin.position, origin.rotation);
        }
    }

    public void Turbo(bool state)
    {
        turbo = state;
    }

    public void RightwardsBarrel()
    {
        if (!rightwardsBarrel && !leftwardsBarrel)
        {
            barrelSideDirection = transform.right;
            rightwardsBarrel = true;
            barrelAngle = 360;
        }
    }
    
    public void LeftwardsBarrel()
    {
        if (!rightwardsBarrel && !leftwardsBarrel)
        {
            leftwardsBarrel = true;
            barrelSideDirection = transform.right;
            barrelAngle = -360;
        }
    }

    public void TurnTowardsGlobal(Vector3 direction)
    {
        // targetDirection = direction;i
        targetRotation = Quaternion.RotateTowards(targetRotation, Quaternion.LookRotation(direction), agility * angularSpeed);
    }

    public void Turn(Vector2 pitchYaw)
    {
        // targetDirection = Quaternion.AngleAxis(-pitchYaw.x * agility, transform.right) * targetDirection;
        // targetDirection = Quaternion.AngleAxis(pitchYaw.y * agility, transform.up) * targetDirection;
        
        barrelSideDirection = Quaternion.AngleAxis(-pitchYaw.x * agility, transform.right) * barrelSideDirection;
        barrelSideDirection = Quaternion.AngleAxis(pitchYaw.y * agility, transform.up) * barrelSideDirection;
        
        targetRotation = Quaternion.AngleAxis(-pitchYaw.x * agility, transform.right) * targetRotation;
        targetRotation = Quaternion.AngleAxis(pitchYaw.y * agility, transform.up) * targetRotation;
    }
}
