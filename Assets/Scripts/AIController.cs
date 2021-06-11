using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum Team
{
    Blue,
    Red
}

[RequireComponent(typeof(Controller))]
public class AIController : MonoBehaviour
{
    public Transform target = null;
    public Team team;

    private Controller _controller;
    
    public float minDistanceToTarget = 10f;
    public float turningDistance = 10f;
    public float lookAngle = 20f;
    public float fieldOfView = 60f;
    public float shootingAngle = 5;

    public float maxTimeToLooseTarget = 3f;
    private float timeSinceLastSeen = 0f;
    private float turningStrength = 10f;

    public float shootTime = 200f;
    private float timeSinceLastShoot;
    
    private int environmentMaks;

    public UnityEvent onLooseTarget = new UnityEvent();
    
    void Start()
    {
        _controller = GetComponent<Controller>();
        
        GetComponent<Health>().onHealthZero.AddListener(OnAIDeath);
        environmentMaks = 1 << LayerMask.NameToLayer("Environment")
            | 1 << LayerMask.NameToLayer("Ships");
    }

    private void FixedUpdate()
    {
        var pitchYaw = Vector2.zero;
        //pitchYaw = LookForCollision(pitchYaw);
        UpdateTimeSinceSeen(Time.fixedTime);
        timeSinceLastShoot += Time.fixedTime;
        
        if (!LookForCollision(ref pitchYaw))
        {
            Vector3 targetDirection;
            
            if (target == null)
            {
                targetDirection = transform.forward;
            }
            else
            {
                targetDirection = target.position - transform.position;
                if (targetDirection.magnitude < minDistanceToTarget)
                {
                    targetDirection -= target.forward * (minDistanceToTarget - targetDirection.magnitude);
                }
            }

            _controller.TurnTowardsGlobal(targetDirection);
        }
        else
        {
            _controller.Turn(pitchYaw);
        }
    }

    private bool LookForCollision(ref Vector2 pitchYawResult)
    {
        bool result = false;
        
        var ray = new Ray(transform.position, Quaternion.AngleAxis(lookAngle, transform.right) * transform.forward);
        var hitInfo = new RaycastHit();
        if (Physics.Raycast(ray, out hitInfo, turningDistance, environmentMaks))
        {
            pitchYawResult.x += hitInfo.distance * turningStrength;
            result = true;
        }
        ray = new Ray(transform.position, Quaternion.AngleAxis(-lookAngle, transform.right) * transform.forward);
        hitInfo = new RaycastHit();
        if (Physics.Raycast(ray, out hitInfo, turningDistance, environmentMaks))
        {
            pitchYawResult.x += hitInfo.distance * turningStrength;
            result = true;
        }
        
        ray = new Ray(transform.position,  transform.forward);
        hitInfo = new RaycastHit();
        if (Physics.Raycast(ray, out hitInfo, turningDistance, environmentMaks))
        {
            pitchYawResult.x += hitInfo.distance * turningStrength;
            result = true;
        }
        
        ray = new Ray(transform.position, Quaternion.AngleAxis(lookAngle, transform.right) * transform.up);
        hitInfo = new RaycastHit();
        if (Physics.Raycast(ray, out hitInfo, turningDistance, environmentMaks))
        {
            pitchYawResult.y += hitInfo.distance * turningStrength;
            result = true;
        }
        ray = new Ray(transform.position, Quaternion.AngleAxis(-lookAngle, transform.right) * transform.up);
        hitInfo = new RaycastHit();
        if (Physics.Raycast(ray, out hitInfo, turningDistance, environmentMaks))
        {
            pitchYawResult.y += hitInfo.distance * turningStrength;
            result = true;
        }

        return result;
    }

    private void UpdateTimeSinceSeen(float time)
    {
        if (target != null)
        {
            var toTarget = target.position - transform.position;
            var angle = Vector3.Angle(transform.forward, toTarget);
            if (angle > fieldOfView)
            {
                timeSinceLastSeen += time;
                if (timeSinceLastSeen > maxTimeToLooseTarget)
                {
                    target = null;
                }
            }
            else
            {
                var ray = new Ray(transform.position, toTarget);
                var hitInfo = new RaycastHit();

                if (Physics.Raycast(ray, out hitInfo) && (hitInfo.transform == target))
                {
                    timeSinceLastSeen = 0f;
                    if(angle < shootingAngle && timeSinceLastShoot > shootTime)
                    {
                        _controller.Shoot();
                        timeSinceLastShoot = 0;
                    }
                }
            }
        }
        else
        {
            onLooseTarget.Invoke();
        }
    }


    private bool CheckLineOfSite()
    {
        var direction = target.position - transform.position;
        var ray = new Ray(transform.position, direction);
        var hitInfo = new RaycastHit();

        if (Physics.Raycast(ray, out hitInfo) && (hitInfo.transform == target))
        {
            timeSinceLastSeen = 0f;
            return true;
        }
        else
        {
            timeSinceLastSeen += Time.fixedDeltaTime;
            return false;
        }
    }

    
    private void OnAIDeath()
    {
        _controller.enabled = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    public void SetNewTarget(Transform newTarget)
    {
        target = newTarget;
        timeSinceLastSeen = 0;
    }
}
