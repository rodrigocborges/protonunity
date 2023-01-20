using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerNetwork : MonoBehaviourPun
{
    [SerializeField] private Player playerBehaviour;
    [SerializeField] private Health health;
    [SerializeField] private Score score;

    [SerializeField] private TMPro.TMP_Text nametagText;

    void Start(){        
        nametagText.text = photonView.Owner.NickName.Length < 9 ? photonView.Owner.NickName : photonView.Owner.NickName.Substring(0, 9); //Texto nao pode passar de 10 caracteres para nao sair do player
        
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
    
}
