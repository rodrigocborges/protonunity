using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Score : MonoBehaviour, IPunObservable
{
    [SerializeField] private TMPro.TMP_Text scoreText;

    private int score = 0;

    void Start(){
        _updateScoreText();
    }

    public void AddScore(int val){
        score += val;
        _updateScoreText();
    }

    public void DecreaseScore(int val){
        score -= val;
        if(score <= 0)
            score = 0;
        _updateScoreText();
    }

    public void _updateScoreText(){
        scoreText.text = "Pontos: " + score;
    }

    [PunRPC]
    public void RpcUpdateScoreText(){
        _updateScoreText();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting){
            stream.SendNext(score);
        }
        else {
            score = (int)stream.ReceiveNext();
        }
    }
}
