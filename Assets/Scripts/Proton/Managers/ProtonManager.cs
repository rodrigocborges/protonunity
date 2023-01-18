using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using TMPro;
using System.Linq;
using Proton;
using Proton.Sync;

[System.Serializable]
public class ConnectionStatsData {
    public string Type { get; set; }
    public int PacketsSent { get; set; }
    public int PacketsReceived { get; set; }
    public int BytesSent { get; set; }
    public int BytesReceived { get; set; }
    public float TotalRoundTripTime { get; set; }
    public float CurrentRoundTripTime { get; set; }
    public long Timestamp { get; set; }
}

public class ProtonManager : MonoBehaviour
{
    public static ProtonManager Instance;
    [Header("Prefabs")]
    [SerializeField] private GameObject playerPrefab;
    [Header("HUD")]

    [SerializeField] private TMP_Text connectionStateText;
    [SerializeField] private TMP_Text connectionStatsText;

    private UnityPeerJS.Peer peer;
    private readonly Dictionary<int, UnityPeerJS.Peer.Connection> _connections = new Dictionary<int, UnityPeerJS.Peer.Connection>();
    private int _numberOfPeers = 0;

    private bool isOpen = false;

    private List<string> connectedPeersIDs = new List<string>();
    private List<string> _peerList = new List<string>();

    private SignallingServer signallingServer;
    private ReceiveData receiveData;
    public GenericDataManager GenericDataManager;

    private PeerJSEventManager peerJSEventManager;

    private float timeToSendConnectionStats = 10;
    private float currentTimeToSendConnectionStats = 0;
    private string lastConnectionStatsData = "";
    private bool sendConnectionStats = false;

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
        /*signallingServer.CheckAndGetPeers((peers) => {
            var filteredPeers = peers.Where(x => !x.PeerID.Equals(peer.GetLocalPeerID()));
            foreach(var p in filteredPeers){
                //Lista auxiliar para identificar quais peers já foram conectados com o local
                if(!connectedPeersIDs.Contains(p.PeerID)){
                    peer.Connect(p.PeerID);
                    connectedPeersIDs.Add(p.PeerID);
                }
            }
        });*/

        if(_peerList == null || !_peerList.Any())
            return;

        var filteredPeers = _peerList.Where(x => !x.Equals(peer.GetLocalPeerID()));
        foreach(var p in filteredPeers){
            //Lista auxiliar para identificar quais peers já foram conectados com o local
            if(!connectedPeersIDs.Contains(p)){
                peer.Connect(p, null);
                connectedPeersIDs.Add(p);
            }
        }
    }

    void Awake(){
        Instance = this;
        
        connectionStateText.text = "Aguardando conexão";

        signallingServer = new SignallingServer();
        receiveData = new ReceiveData(this);
        peerJSEventManager = new PeerJSEventManager();

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
    }

    private void HandleOnError(string message)
    {
        connectionStateText.text = "Erro: " + message;
    }

    private void HandleOnClose()
    {
        connectionStateText.text = "Conexão finalizada";
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

        connectionStateText.text = "Conexão aberta";

        SpawnPlayer(peer.GetLocalPeerID(), true);

        InvokeRepeating("CheckNewPeers", 2, 10);
    }

    public void Connect(string peerID, string roomName = null){
        if(string.IsNullOrEmpty(peerID))
            return;

        peer.Connect(peerID, roomName);
    }

    private void OnDestroy(){
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

        SpawnPlayer(connection.RemoteId);

        connectionStateText.text = "Conectado";

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

        EventManagerData eventManagerData = JsonConvert.DeserializeObject<EventManagerData>(data);
        if(eventManagerData == null)
            return;

        peerJSEventManager.Handle(
            eventManagerData, 
            peer, 
            _connections, 
            HandleOnOpen, 
            HandleOnConnection, 
            HandleOnDisconnected, 
            HandleOnClose, 
            HandleOnError
        );
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

        // if(!sendConnectionStats)
        //     sendConnectionStats = true;

        // lastConnectionStatsData = JsonConvert.SerializeObject(connectionStatsData);
    }

    public void SetPeerList(List<string> peerList){
        _peerList = peerList;
        _numberOfPeers = peerList.Count;
        connectionStateText.text = "Conectado ("+ _numberOfPeers +" jogadores)";
    }

    // void Update(){
    //     if(sendConnectionStats){
    //         currentTimeToSendConnectionStats += Time.deltaTime;
    //         if(currentTimeToSendConnectionStats >= timeToSendConnectionStats){
    //             signallingServer.SendConnectionStats(lastConnectionStatsData);
    //             currentTimeToSendConnectionStats = 0;
    //         }
    //     }
    // }
}
