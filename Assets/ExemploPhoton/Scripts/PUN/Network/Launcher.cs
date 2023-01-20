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

        SetRandomUsername();
        roomNameField.text = "dev";
    }

    public override void OnConnectedToMaster()
    {
        print("Conectado ao master server");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
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
            roomNameField.text = "dev";

        if(string.IsNullOrEmpty(usernameField.text))
            SetRandomUsername();

        PhotonNetwork.LocalPlayer.NickName = usernameField.text;

        PhotonNetwork.CreateRoom(roomNameField.text, new Photon.Realtime.RoomOptions { IsOpen = true, MaxPlayers = 4 });
    }

    public void JoinRoom(){
        if(string.IsNullOrEmpty(roomNameField.text))
            roomNameField.text = "dev";

        if(string.IsNullOrEmpty(usernameField.text))
            SetRandomUsername();

        PhotonNetwork.LocalPlayer.NickName = usernameField.text;

        PhotonNetwork.JoinRoom(roomNameField.text);
    }

    private void SetRandomUsername() => usernameField.text = "Usuario_" + Random.Range(1000, 9999);

}
