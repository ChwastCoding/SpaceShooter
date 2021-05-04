using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Controller _controller;
    private CameraController _cameraController;

    [SerializeField] float sensibility = 0.01f;

    [SerializeField] private bool invertX = false;
    [SerializeField] private bool invertY = false;

    private float zatstart = 0f;
    private float xatstart = 0f;

    private Health _currentTarget = null;
    public Health currentTarget => _currentTarget;
    void Start()
    {
        zatstart = Input.acceleration.z;
        xatstart = Input.acceleration.x;
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
        foreach (Touch touch in Input.touches)
        {
            if ((touch.position.x < Screen.width / 2) && (touch.position.y < Screen.height / 2) && (touch.phase == TouchPhase.Began))
            {
                _controller.Turbo(true);
                _cameraController.SetReducedResponsiveness(true);
            }

            if ((touch.position.x < Screen.width / 2) && (touch.position.y < Screen.height / 2) && (touch.phase == TouchPhase.Ended))
            {
                _controller.Turbo(false);
                _cameraController.SetReducedResponsiveness(false);
            }

            if ((touch.position.x > Screen.width / 2) && (touch.position.y > Screen.height / 2) && (touch.phase == TouchPhase.Began))
            {
                _controller.RightwardsBarrel();
                _cameraController.cameraRotates = false;
                _cameraController.SetReducedResponsiveness(true);
            }

            if ((touch.position.x < Screen.width / 2) && (touch.position.y > Screen.height / 2) && (touch.phase == TouchPhase.Began))
            {
                _controller.LeftwardsBarrel();
                _cameraController.cameraRotates = false;
                _cameraController.SetReducedResponsiveness(true);
            }

            if ((touch.position.x > Screen.width / 2) && (touch.position.y < Screen.width / 2))
            {
                _controller.Shoot();
            }
        }

        _controller.Turn(sensibility * new Vector2(
                    (invertX ? -1 : 1) * (Input.acceleration.z - zatstart + 0.2f),
                    (invertY ? -1 : 1) * (Input.acceleration.x - xatstart)));
    }
}
