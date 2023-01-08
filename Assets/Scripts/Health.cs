using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Health : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private GameObject healthBar;
    [SerializeField] private int health;
    private int startHealth;

    void Start()
    {
        startHealth = health;
        _updateHealthBar();
    }

    public void TakeDamage(int damage, GameObject localPlayer, GameObject remotePlayer){
        health -= damage;
        
        if(health <= 0)
        {
            health = startHealth;
            remotePlayer.transform.position = NetworkRoom.GetRandomSpawnPoint();
            
            localPlayer.GetComponent<Score>().AddScore(5);
            localPlayer.GetComponent<PhotonView>().RPC("RpcUpdateScoreText", RpcTarget.Others);
        }

        _updateHealthBar();
    }

    private void _updateHealthBar(){
        healthBar.transform.localScale = new Vector3(health / 100.0f, healthBar.transform.localScale.y, healthBar.transform.localScale.z);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting){
            stream.SendNext(health);
        }
        else {
            health = (int)stream.ReceiveNext();
        }
    }
}
