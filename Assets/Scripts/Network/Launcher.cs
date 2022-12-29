using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class Launcher : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject loadingText;
    [SerializeField] private GameObject formConnection;
    [SerializeField] private InputField usernameField;
    [SerializeField] private InputField roomNameField;

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        loadingText.SetActive(true);
        formConnection.SetActive(false);
        PhotonNetwork.ConnectUsingSettings();   
    }

    public override void OnConnectedToMaster()
    {
        loadingText.SetActive(false);
        formConnection.SetActive(true);
    }

    public override void OnCreatedRoom()
    {
        print("Sala criada");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        print($"Erro ao entrar na sala ({returnCode}): {message}");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        print($"Erro ao criar na sala ({returnCode}): {message}");
    }

    public override void OnJoinedRoom()
    {
        print("Entrou na sala");
        PhotonNetwork.LoadLevel(1);
    }

    public void CreateRoom(){
        if(string.IsNullOrEmpty(roomNameField.text))
            return;
        PhotonNetwork.CreateRoom(roomNameField.text, new Photon.Realtime.RoomOptions { IsOpen = true, MaxPlayers = 4 });
    }

    public void JoinRoom(){
        if(string.IsNullOrEmpty(roomNameField.text))
            return;
        PhotonNetwork.JoinRoom(roomNameField.text);
    }

}
