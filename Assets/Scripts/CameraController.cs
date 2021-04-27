using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [NotNull] [SerializeField] private Transform target;

    private Vector3 offsetPosition;
    private Quaternion offsetRotation;

    [Range(0, 1)] public float rotationalResponsiveness = .1f; 
    [Range(0, 1)] public float positionalResponsiveness = .5f;

    [Range(0, 1)] public float comebackSpeed = .5f;
    
    private float loweredPositionalResponsiveness;
    private float actuallPositionalResponsiveness;
    private float targetPositionalResponsiveness;
    
    
    public bool cameraRotates = true;
    
    void Start()
    {
        offsetPosition = transform.position - target.position;
        offsetRotation = transform.rotation * Quaternion.Inverse(target.rotation);
        actuallPositionalResponsiveness = positionalResponsiveness;
        loweredPositionalResponsiveness = positionalResponsiveness / 4.0f;
        targetPositionalResponsiveness = positionalResponsiveness;
    }
    
    void FixedUpdate()
    {
        actuallPositionalResponsiveness =
            Mathf.Lerp(actuallPositionalResponsiveness, targetPositionalResponsiveness, comebackSpeed);
        
        transform.position = Vector3.Lerp(transform.position, target.position + target.rotation * offsetPosition, actuallPositionalResponsiveness);
        if(cameraRotates)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation * offsetRotation, rotationalResponsiveness);
        }
    }

    public void SetReducedResponsiveness(bool b)
    {
        if (b)
        {
            actuallPositionalResponsiveness = loweredPositionalResponsiveness;
            targetPositionalResponsiveness = loweredPositionalResponsiveness;
        }
        else
        {
            targetPositionalResponsiveness = positionalResponsiveness;
        }
    }
}
