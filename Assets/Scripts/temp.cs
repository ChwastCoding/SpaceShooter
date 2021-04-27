using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class temp : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var player = GameObject.FindGameObjectWithTag("player");
        var playerController = player.GetComponent<PlayerController>();

        var playerHealth = player.GetComponent<Health>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTargetFounnd()
    {
        
    }
}
