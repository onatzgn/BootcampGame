using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwitch : MonoBehaviour
{
    public TopDownCharacterMove playerController;   
    public TopDownCharacterMove player2Controller; 

    private bool player1Active = true;

    private void Start()
    {
        playerController.enabled = true;
        player2Controller.enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            SwitchPlayer();
        }
    }

    public void SwitchPlayer()
    {
        player1Active = !player1Active;

        if (player1Active)
        {
            playerController.enabled = true;
            player2Controller.enabled = false;
        }
        else
        {
            playerController.enabled = false;
            player2Controller.enabled = true;
        }
    }
}