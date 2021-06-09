using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Controller))]
public class AIController : MonoBehaviour
{
    public Transform target = null;
    
    private Controller _controller;
    private Rigidbody _targetRigidbody;
    
    public float minDistanceToTarget = 10f;
    public float turningDistance = 10f;
    public float lookAngle = 20f;

    public float maxTimeToLooseTarget = 3f;
    private float timeTillLastSeen = 0f;
    
    void Start()
    {
        _controller = GetComponent<Controller>();
        _targetRigidbody = target.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        var pitchYaw = Vector2.zero;
        LookForCollision(pitchYaw);
        
        if (pitchYaw == Vector2.zero)
        {
            Vector3 targetDirection = target.position - transform.position;
            if (targetDirection.magnitude < minDistanceToTarget)
            {
                targetDirection -= target.forward * (minDistanceToTarget - targetDirection.magnitude);
            }
            _controller.TurnTowardsGlobal(targetDirection);
        }
        else
        {
            _controller.Turn(pitchYaw);
        }
    }

    private void LookForCollision(Vector2 pitchYawResult)
    {
        var ray = new Ray(transform.position, Quaternion.AngleAxis(lookAngle, transform.right) * transform.forward);
        var hitInfo = new RaycastHit();
        if (Physics.Raycast(ray, out hitInfo, turningDistance, LayerMask.NameToLayer("Environment")))
        {
            pitchYawResult.x += hitInfo.distance;
        }
        ray = new Ray(transform.position, Quaternion.AngleAxis(-lookAngle, transform.right) * transform.forward);
        hitInfo = new RaycastHit();
        if (Physics.Raycast(ray, out hitInfo, turningDistance, LayerMask.NameToLayer("Environment")))
        {
            pitchYawResult.x += hitInfo.distance;
        }
        ray = new Ray(transform.position, Quaternion.AngleAxis(lookAngle, transform.right) * transform.up);
        hitInfo = new RaycastHit();
        if (Physics.Raycast(ray, out hitInfo, turningDistance, LayerMask.NameToLayer("Environment")))
        {
            pitchYawResult.y += hitInfo.distance;
        }
        ray = new Ray(transform.position, Quaternion.AngleAxis(-lookAngle, transform.right) * transform.up);
        hitInfo = new RaycastHit();
        if (Physics.Raycast(ray, out hitInfo, turningDistance, LayerMask.NameToLayer("Environment")))
        {
            pitchYawResult.y += hitInfo.distance;
        }
    }

    private bool CheckLineOfSite()
    {
        var direction = target.position - transform.position;
        var ray = new Ray(transform.position, direction);
        var hitInfo = new RaycastHit();

        if (Physics.Raycast(ray, out hitInfo) && (hitInfo.transform == target))
        {
            timeTillLastSeen = 0f;
            return true;
        }
        else
        {
            timeTillLastSeen += Time.fixedDeltaTime;
            return false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
