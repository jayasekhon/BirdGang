using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MiniBossController : MonoBehaviour
{
    private Animator _animator;
    // private bool _isDistanceCheck = false;

    private GameObject[] playersInGame;
    private bool playerInRange = false;
    private int counter = 0;

    private float distance;

    private PhotonView PV;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
        _animator = GetComponent<Animator>();
    }
    // Start is called before the first frame update
    void Start()
    {

    } 

    // Update is called once per frame
    void Update()
    {   
        playersInGame = GameObject.FindGameObjectsWithTag("Player"); //this line shouldn't really be here but also doesn't work in start atm

        if (!PV.IsMine)
        {
            return;
        }

        // Loop through the distance of each player to the miniboss until a player is within range
        for (int p = 0; p < playersInGame.Length; p++)
        {
            Vector3 playerPosition = playersInGame[p].transform.position;
            distance = Vector3.Distance(playerPosition, transform.position);
            if (distance < 8.0f)
            {
                playerInRange = true;
                break;
            } 
            else
            {
                counter += 1;
            }
        }

        // If no player in the game is within range of miniboss
        if (counter == playersInGame.Length)
        {
            Debug.Log("Reset playerInRange bool");
            playerInRange = false;
            counter = 0;
        } 

        if (playerInRange)
        {
            Debug.Log("A player is within range");
            _animator.SetBool("Attack", true);
        }

        // If there are no players within range and the miniboss has been attacking then reset
        if ((!playerInRange) && _animator.GetBool("Attack"))
        {
            Debug.Log("No players within range");
            _animator.SetBool("Attack", false);
        }
    }
}
