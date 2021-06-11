using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BattleManager : MonoBehaviour
{
    public int teamMembersOnABattleField = 10;

    public Transform[] blueSpawnPoints;
    public Transform[] redSpawnPoints;

    private List<Transform> _redTeam = new List<Transform>();
    private List<Transform> _blueTeam = new List<Transform>();

    public Transform redPrefab, bluePrefab;
    private int redRequest = 0, blueRequest = 0;
    private float blueTimer = 0, redTimer = 0;

    public Transform cameraPosition, playerPosition;
    public Transform playerPrefab;

    private CameraController cameraController;

    public Transform player;
    
    IEnumerator Start()
    {
        cameraController = Camera.main.GetComponent<CameraController>();
        
        player.GetComponent<Health>().onHealthZero.AddListener(OnPlayerDeath);
        
        _blueTeam.Add(player);
        
        for (int i = 1; i < teamMembersOnABattleField; i++)
        {
            AddBlue();
            AddRed();
            yield return new WaitForSeconds(1f);
        }
        AddRed();
    }

    private void FixedUpdate()
    {
        SafeAddBlue(Time.fixedTime);
        SafeAddRed(Time.fixedTime);
    }

    private void AddBlue()
    {
        var position = blueSpawnPoints[Random.Range(0, blueSpawnPoints.Length)];
        Transform blue = Instantiate(bluePrefab, position.position, position.rotation);
        
        _blueTeam.Add(blue);
        blue.GetComponent<Health>().onHealthZero.AddListener(delegate
        {
            _blueTeam.Remove(blue);
            // AddBlue();
            blueRequest++;
        });
        var blueAI = blue.GetComponent<AIController>();
        blueAI.onLooseTarget.AddListener(delegate
        {
            blueAI.target = FindTarget(_redTeam);
        });
    }

    void OnPlayerDeath()
    {
        _blueTeam.Remove(player);

        cameraController.transform.position = cameraPosition.position;
        cameraController.transform.rotation = cameraPosition.rotation;

        var newPlayer = Instantiate(playerPrefab, playerPosition.position, playerPosition.rotation, transform.parent);

        _blueTeam.Add(newPlayer);
        player = newPlayer;
        newPlayer.GetComponent<Health>().onHealthZero.AddListener(OnPlayerDeath);
        
        cameraController.target = newPlayer;
        cameraController.enabled = true;
        Debug.Log(cameraController.isActiveAndEnabled);
        Camera.main.GetComponent<CameraController>().enabled = true;
    }


    private void AddRed()
    {   var position = redSpawnPoints[Random.Range(0, redSpawnPoints.Length)];
        Transform red = Instantiate(redPrefab, position.position, position.rotation);
        
        _redTeam.Add(red);
        red.GetComponent<Health>().onHealthZero.AddListener(delegate
        {
            _redTeam.Remove(red);
            // AddRed();
            redRequest++;
        });
        var redAI = red.GetComponent<AIController>();
        redAI.onLooseTarget.AddListener(delegate
        {
            redAI.target = FindTarget(_blueTeam);
        });
    }

    private Transform FindTarget(List<Transform> targetList)
    {
        return targetList[Random.Range(0, targetList.Count)];
    }

    private void SafeAddBlue(float dt)
    {
        blueTimer += dt;
        if (blueTimer > 1000f && blueRequest > 0)
        {
            blueTimer = 0;
            blueRequest--;
            AddBlue();
        }
    }
    
    private void SafeAddRed(float dt)
    {
        redTimer += dt;
        if (redTimer > 1000f && redRequest > 0)
        {
            redTimer = 0;
            redRequest--;
            AddRed();
        }
    }
}
