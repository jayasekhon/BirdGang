using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MayorFollow : MonoBehaviour
{
    private CinemachineVirtualCamera cam;
    private MayorScript[] mayorInGame;

    // Start is called before the first frame update
    void Awake()
    {
        cam = GetComponent<CinemachineVirtualCamera>();
        mayorInGame = GameObject.FindObjectsOfType<MayorScript>();
        // cam.Follow = mayorInGame[0].transform;
        // cam.LookAt = mayorInGame[0].transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
