using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerNetwork : MonoBehaviourPun
{
    [SerializeField] private Player playerBehaviour;
    [SerializeField] private Health health;
    [SerializeField] private Score score;

    void Start(){        
        if(!photonView.IsMine)
            return;
    }

    void Update()
    {
        if(!photonView.IsMine)
            return;

        playerBehaviour.LookAtMouse();

        playerBehaviour.Fire(this.gameObject); 
    }

    void FixedUpdate(){
        
        if(!photonView.IsMine)
            return;

        playerBehaviour.Move();
    }

    // [PunRPC]
    // public void RpcTakeDamage(int damage, int photonViewIDPlayer){
    //     health.TakeDamage(damage, photonViewIDPlayer);
    // }

    
}
