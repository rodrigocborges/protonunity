using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using TMPro;
using System.Linq;
using Proton;
using Proton.Sync;

[System.Serializable]
public class EventManagerData {
    public int Code { get; set; }
    public int ConnectionIndex { get; set; }
    public string Data { get; set; }
    public string Error { get; set; }
    public string PeerID { get; set; }
}

[System.Serializable]
public class ConnectionStatsData {
    public string Type { get; set; }
    public int PacketsSent { get; set; }
    public int PacketsReceived { get; set; }
    public int BytesSent { get; set; }
    public int BytesReceived { get; set; }
    public float TotalRoundTripTime { get; set; }
    public float CurrentRoundTripTime { get; set; }
}

public class ProtonManager : MonoBehaviour
{
    public static ProtonManager Instance;
    [Header("Prefabs")]
    [SerializeField] private GameObject playerPrefab;
    [Header("HUD")]
    [SerializeField] private TMP_InputField messageField;

    [SerializeField] private TMP_InputField myPeerIDField;
    [SerializeField] private TMP_InputField connectPeerIDField;
    [SerializeField] private TMP_Text connectionStateText;
    [SerializeField] private TMP_Text messageContent;
    [SerializeField] private TMP_Text connectionStatsText;


    private UnityPeerJS.Peer peer;
    private readonly Dictionary<int, UnityPeerJS.Peer.Connection> _connections = new Dictionary<int, UnityPeerJS.Peer.Connection>();
    private int _numberOfPlayers = 0;

    private bool isOpen = false;

    private List<string> connectedPeersIDs = new List<string>();

    private SignallingServer signallingServer;
    private ReceiveData receiveData;
    public GenericDataManager GenericDataManager;

    public Dictionary<string, GameObject> playersGameObjects = new Dictionary<string, GameObject>();

    public void SendToAll(string data){
        if(!_connections.Values.Any())
            return;

        foreach(var connection in _connections.Values){
            if(connection.RemoteId.Equals(peer.GetLocalPeerID()))
                continue;
            connection.Send(data);
        }
    }

    public SyncText FindText(string peerID, string dataKey){
        GameObject textGameObject = GameObject.Find(string.Format("Text_{0}_{1}", peerID, dataKey));
        if(textGameObject == null) return null;
        return textGameObject.GetComponent<SyncText>();
    }

    void CheckNewPeers(){
        signallingServer.CheckAndGetPeers((peers) => {
            var filteredPeers = peers.Where(x => !x.PeerID.Equals(peer.GetLocalPeerID()));
            foreach(var p in filteredPeers){
                //Lista auxiliar para identificar quais peers já foram conectados com o local
                if(!connectedPeersIDs.Contains(p.PeerID)){
                    peer.Connect(p.PeerID);
                    connectedPeersIDs.Add(p.PeerID);
                }
            }
        });
    }

    void Awake(){
        Instance = this;
        
        connectionStateText.text = "Aguardando conexão";

        signallingServer = new SignallingServer();
        receiveData = new ReceiveData(this);

        // messageField.interactable = false;  

        // messageField.onSubmit.AddListener((text) => {
        //     SendToAll(text);
        //     messageField.text = "";
        // });
        
        peer = new UnityPeerJS.Peer();
        peer.OnConnection += HandleOnConnection;
        peer.OnOpen += HandleOnOpen;
        peer.OnClose += HandleOnClose;
        peer.OnError += HandleOnError;
        peer.OnDisconnected += HandleOnDisconnected;
    }
    private void HandleOnDisconnected()
    {
        connectionStateText.text = "Desconectado";

        signallingServer.RemovePeer(peer.GetLocalPeerID());
    }

    private void HandleOnError(string message)
    {
        connectionStateText.text = "Erro: " + message;
    }

    private void HandleOnClose()
    {
        connectionStateText.text = "Conexão finalizada";

        signallingServer.ChangeConnectionStateOfPeer(peer.GetLocalPeerID(), false);
        signallingServer.RemovePeer(peer.GetLocalPeerID());
    }

    private void SpawnPlayer(string peerID, bool isLocal = false){
        //Evita de spawnar novamente pro mesmo PeerID
        if(playersGameObjects.ContainsKey(peerID))
            return;
        GameObject spawnedPlayer = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        spawnedPlayer.name = "Player_" + peerID;
        spawnedPlayer.GetComponent<EntityIdentity>().SetOwner(peerID, isLocal);
        playersGameObjects.Add(peerID, spawnedPlayer);
        GenericDataManager.Setup(peerID, spawnedPlayer);
    }

    private void HandleOnOpen()
    {
        isOpen = true;

        signallingServer.AddNewPeer(peer.GetLocalPeerID());

        connectionStateText.text = "Conexão aberta";

        SpawnPlayer(peer.GetLocalPeerID(), true);

        InvokeRepeating("CheckNewPeers", 2, 10);
    }

    public void Connect(){
        if(string.IsNullOrEmpty(connectPeerIDField.text))
            return;

        peer.Connect(connectPeerIDField.text);
    }

    private void OnDestroy(){
        signallingServer.RemovePeer(peer.GetLocalPeerID());
        peer.OnConnection -= HandleOnConnection;
        peer.OnOpen -= HandleOnOpen;
        peer.OnClose -= HandleOnClose;
        peer.OnError -= HandleOnError;
        peer.OnDisconnected -= HandleOnDisconnected;
        peer = null;
        peer.Destroy();
    }

    private void HandleOnConnection(UnityPeerJS.Peer.IConnection connection)
    {
        
        CheckNewPeers();

        signallingServer.ChangeConnectionStateOfPeer(connection.RemoteId, true);

        SpawnPlayer(connection.RemoteId);

        //Numero de conexões: 2^n ('n' numero de players)
        //3 JOGADORES, 8   - >  Será?
        _numberOfPlayers = MathUtil.Pow2Reverse(_connections.Count);
        connectionStateText.text = "Conectado ("+ _numberOfPlayers +" jogadores)";

        connection.OnData += HandleOnData;
        connection.OnClose += HandleOnClose;
    }

    private void HandleOnData(string data) => receiveData.Handle(data);

    //Método responsável por sinalizar callbacks do PeerJS
    public void EventManager(string data){
        if(string.IsNullOrEmpty(data)){
            Debug.Log("Call EventManager() -> data is empty");
            return;
        }
        // Debug.Log("Call EventManager() -> data: " + data);

        EventManagerData eventManagerData = JsonConvert.DeserializeObject<EventManagerData>(data);
        if(eventManagerData == null)
            return;

        UnityPeerJS.PeerEventType peerEventType = (UnityPeerJS.PeerEventType)eventManagerData.Code;
        switch (peerEventType)
        {
            case UnityPeerJS.PeerEventType.Initialized:
            {
                HandleOnOpen();
                break;
            }
            case UnityPeerJS.PeerEventType.Connected:
            {
                int connectionIndex = eventManagerData.ConnectionIndex;
                string remoteId = eventManagerData.PeerID;
                _connections[connectionIndex] = new UnityPeerJS.Peer.Connection(peer, connectionIndex, remoteId);
                HandleOnConnection(_connections[connectionIndex]);
                Debug.Log(string.Format("[ProtonManager] `{0}` connected with `{1}`", peer.GetLocalPeerID(), remoteId));
                break;
            }
            case UnityPeerJS.PeerEventType.Received:
            {
                _connections[eventManagerData.ConnectionIndex].EmitOnData(eventManagerData.Data);
                break;
            }

            case UnityPeerJS.PeerEventType.ConnClosed:
            {
                _connections[eventManagerData.ConnectionIndex].EmitOnClose();
                break;
            }

            case UnityPeerJS.PeerEventType.PeerDisconnected:
            {
                HandleOnDisconnected();
                break;
            }

            case UnityPeerJS.PeerEventType.PeerClosed:
            {
                HandleOnClose();
                break;
            }

            case UnityPeerJS.PeerEventType.Error:
            {
                HandleOnError(eventManagerData.Data);
                break;
            }

            default:
            {
                break;
            }
        }
    }

    public void ConnectionStatsManager(string data){
        if(string.IsNullOrEmpty(data)){
            Debug.Log("Call ConnectionStatsManager() -> data is empty");
            return;
        }

        ConnectionStatsData connectionStatsData = JsonConvert.DeserializeObject<ConnectionStatsData>(data);
        if(connectionStatsData == null)
            return;

        connectionStatsText.text = string.Format("Pacotes enviados: {0}\n", connectionStatsData.PacketsSent);
        connectionStatsText.text += string.Format("Pacotes recebidos: {0}\n", connectionStatsData.PacketsReceived);
        connectionStatsText.text += string.Format("Bytes enviados: {0}\n", connectionStatsData.BytesSent);
        connectionStatsText.text += string.Format("Bytes recebidos: {0}\n", connectionStatsData.BytesReceived);
        connectionStatsText.text += string.Format("Ping: {0}ms\n", connectionStatsData.CurrentRoundTripTime);
        connectionStatsText.text += string.Format("TotalRoundTripTime: {0}ms\n", connectionStatsData.TotalRoundTripTime);
        connectionStatsText.text += string.Format("Número de conexões: {0}\n", _connections.Count);
    }
}
