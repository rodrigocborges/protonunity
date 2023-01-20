using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Health : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private TMPro.TMP_Text healthText;
    [SerializeField] private int health;
    private int startHealth;

    void Start()
    {
        startHealth = health;
        _updateHealthText();
    }

    public void TakeDamage(int damage){
        if(!photonView.IsMine)
            return;

        health -= damage;
        
        if(health <= 0)
        {
            health = startHealth;
            transform.position = NetworkRoom.GetRandomSpawnPoint();
        }
    }

    private void _updateHealthText(){
        healthText.text = health + "%";
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting){
            stream.SendNext(health);
        }
        else {
            health = (int)stream.ReceiveNext();
        }

        _updateHealthText();
    }
}
