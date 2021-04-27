using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SImpleController : MonoBehaviour
{
    private Controller _controller;
    [SerializeField] float sensibility = 0.01f;

    [SerializeField] private bool invertX = false;
    [SerializeField] private bool invertY = false;

    private bool isTurboPressed = false;
    
    void Start()
    {
        _controller = GetComponent<Controller>();
    }

    // Update is called once per frame
    void Update()
    {
        // _controller.Turn(new Vector2(sensibility * Input.GetAxis("Vertical"), sensibility * Input.GetAxis("Horizontal")));

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _controller.Turbo(true);
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _controller.Turbo(false);
        }

        var mousePos = Input.mousePosition;
        var input = new Vector3(
            (invertY ? -1 : 1) * (Screen.height / 2.0f - mousePos.y), 
            (invertX ? -1 : 1) * (mousePos.x - Screen.width / 2.0f)) * sensibility;
        _controller.Turn(input);
        
    }
}
