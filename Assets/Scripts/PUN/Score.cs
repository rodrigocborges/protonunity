using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Score : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private TMPro.TMP_Text scoreText;

    private int score = 0;

    void Start(){
        _updateScoreText();
    }

    public void AddScore(int val){
        if(!photonView.IsMine)
            return;

        score += val;
    }

    public void _updateScoreText(){
        scoreText.text = "Pontos: " + score;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting){
            stream.SendNext(score);
        }
        else {
            score = (int)stream.ReceiveNext();
        }

        _updateScoreText();
    }
}
