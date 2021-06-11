using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SImpleController : MonoBehaviour
{
    private Controller _controller;
    private CameraController _cameraController;
    
    [SerializeField] float sensibility = 0.01f;

    [SerializeField] private bool invertX = false;
    [SerializeField] private bool invertY = false;

    private bool isTurboPressed = false;
    
    void Start()
    {
        GetComponent<Health>().onHealthZero.AddListener(OnPlayerDeath);
        
        _controller = GetComponent<Controller>();
        _cameraController = Camera.main.GetComponent<CameraController>();
        _controller.onBarrelFinished.AddListener(delegate
        {
            _cameraController.cameraRotates = true; 
            _cameraController.SetReducedResponsiveness(false);
        });
    }

    // Update is called once per frame
    void Update()
    {
        // _controller.Turn(new Vector2(sensibility * Input.GetAxis("Vertical"), sensibility * Input.GetAxis("Horizontal")));

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _controller.Turbo(true);
            _cameraController.SetReducedResponsiveness(true);
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _controller.Turbo(false);
            _cameraController.SetReducedResponsiveness(false);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            _controller.RightwardsBarrel();
            _cameraController.cameraRotates = false;
            _cameraController.SetReducedResponsiveness(true);
        }
        else if(Input.GetKeyDown(KeyCode.Q))
        {
            _controller.LeftwardsBarrel();
            _cameraController.cameraRotates = false;
            _cameraController.SetReducedResponsiveness(true);
        }
        
        if (Input.GetKeyDown(KeyCode.Space)){
            _controller.Shoot();
        }

        var mousePos = Input.mousePosition;
        var input = new Vector3(
            (invertY ? -1 : 1) * (Screen.height / 2.0f - mousePos.y), 
            (invertX ? -1 : 1) * (mousePos.x - Screen.width / 2.0f)) * sensibility;
        _controller.Turn(input);
        
    }

    private void OnPlayerDeath()
    {
        _cameraController.enabled = false;
        _controller.enabled = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
}
