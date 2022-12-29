using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerNetwork : MonoBehaviourPun
{
    [SerializeField] private Player playerBehaviour;
    void Start()
    {
        
    }

    void Update()
    {
        if(!photonView.IsMine)
            return;

        playerBehaviour.LookAtMouse();
        playerBehaviour.Move();
        playerBehaviour.Fire(); 
    }
}
