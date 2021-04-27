using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SImpleController : MonoBehaviour
{
    private Controller _controller;
    [SerializeField] float sensibility = 0.01f;
    void Start()
    {
        _controller = GetComponent<Controller>();
    }

    // Update is called once per frame
    void Update()
    {
        _controller.Turn(new Vector2(sensibility * Input.GetAxis("Vertical"), sensibility * Input.GetAxis("Horizontal")));
    }
}
