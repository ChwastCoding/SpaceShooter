using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    // Start is called before the first frame update

    // _enemyHealthBar.transform.localScale = new Vector3(1, 1, 1); to enable
    GameObject _enemyHealthBar ;
    GameObject _playerHealthBar ;
    private GameObject _player;
    private PlayerController _playerController;
    private Health _playerHealth;
    private Health currentTarget;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("player");
        _playerController = _player.GetComponent<PlayerController>();
        _playerHealth = _player.GetComponent<Health>();
        _enemyHealthBar = GameObject.Find("Enemy Health Bar");
        _enemyHealthBar.transform.localScale = new Vector3(0, 0, 0);
        currentTarget = _playerController.currentTarget;

    }

    // Update is called once per frame
    void Update()
    {
        if (currentTarget != null)
        {
            _enemyHealthBar.transform.localScale = new Vector3(1, 1, 1);
        }
    }
}
