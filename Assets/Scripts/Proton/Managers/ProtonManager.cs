using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using TMPro;
using System.Linq;
using Proton;
using Proton.Sync;
using System.Collections;

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

    private ReceiveData receiveData;
    private PeerJSEventManager peerJSEventManager;

    private float timeToSendConnectionStats = 5;
    private float currentTimeToSendConnectionStats = 0;
    private string lastConnectionStatsData = "";
    private bool sendConnectionStats = false;

    private bool selectedMasterClient = false;

    public Dictionary<string, GameObject> playersGameObjects = new Dictionary<string, GameObject>();

    public GameObject LocalPlayer;

    public void SendToAll(string data){
        if(!_connections.Values.Any())
            return;

        foreach(var connection in _connections.Values){
            if(connection.RemoteId.Equals(peer.GetLocalPeerID()))
                continue;
            // _dataToSendAll.Append(data);
            connection.Send(data);
            // StartCoroutine(SendToAllCoroutine(connection));
        }
    }

    private Queue<string> _dataToSendAll = new Queue<string>();

    IEnumerator SendToAllCoroutine(UnityPeerJS.Peer.Connection connection){
        while(_dataToSendAll.Any())
        {
            string lastDataToSend = _dataToSendAll.Dequeue();
            yield return new WaitForSeconds(1);
            connection.Send(lastDataToSend);
        }
    }

    public GameObject SpawnObject(string peerID, string prefabPath, Vector3 pos, Quaternion rot, bool onlyLocal = false){
        GameObject spawnedObject = Instantiate(Resources.Load<GameObject>(prefabPath), pos, rot);
        spawnedObject.name = string.Format("GameObject_{0}", peerID);

        if(onlyLocal){
            return spawnedObject;
        }

        SendDataInstantiate _sendDataInstantiate = new SendDataInstantiate(peerID);
        SendData _sendData = new SendData(0f);

        _sendDataInstantiate.Add(prefabPath, pos, rot);
        _sendData.Setup(SendDataType.Instantiate, _sendDataInstantiate);
        return spawnedObject;
    }

    private Dictionary<string, TMPro.TMP_Text> playersTextGameObjects = new Dictionary<string, TMPro.TMP_Text>();

    public void FindAndSetText(string peerID, string dataKey, string val){
        string dictKey = peerID + "_" + dataKey;
        //TODO: Testar cacheamento de texts
        if(playersTextGameObjects.ContainsKey(dictKey)){
            playersTextGameObjects[dictKey].text = val;
        }else {
            GameObject textGameObject = GameObject.Find(string.Format("Text_{0}_{1}", peerID, dataKey));
            if(textGameObject == null) return;
            TMPro.TMP_Text auxText = textGameObject.GetComponent<TMPro.TMP_Text>();
            auxText.text = val;
            playersTextGameObjects.Add(dictKey, auxText);
        }
        
    }

    void CheckNewPeers(){
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
        connectionStatsText.text = "Aguardando estatísticas de conexão...";

        receiveData = new ReceiveData(this);
        peerJSEventManager = new PeerJSEventManager();

        peer = new UnityPeerJS.Peer();
        peer.OnConnection += HandleOnConnection;
        peer.OnOpen += HandleOnOpen;
        peer.OnClose += HandleOnCloseLocalPeer;
        peer.OnError += HandleOnError;
        peer.OnDisconnected += HandleOnDisconnected;
    }
    private void HandleOnDisconnected()
    {
        connectionStateText.text = "Desconectado";
        DestroyPlayerObjects(peer.GetLocalPeerID());
    }

    private void HandleOnError(string message)
    {
        connectionStateText.text = "Erro: " + message;
    }

    private void HandleOnCloseLocalPeer() {
        connectionStateText.text = "Conexão finalizada";
        DestroyPlayerObjects(peer.GetLocalPeerID());
    }

    private void HandleOnClose(string peerID)
    {
        connectionStateText.text = "Conexão finalizada";
        DestroyPlayerObjects(peerID);
    }

    //TODO: Provisória essa forma de posicao aleatoria, melhorar
    private void SpawnPlayer(string peerID, bool isLocal = false){
        //Evita de spawnar novamente pro mesmo PeerID
        if(playersGameObjects.ContainsKey(peerID))
            return;
        GameObject spawnedPlayer = Instantiate(playerPrefab, PlayerNetworkProton.GetRandomSpawnPoint(), Quaternion.identity);
        spawnedPlayer.name = "Player_" + peerID;
        spawnedPlayer.GetComponent<EntityIdentity>().SetOwner(peerID, isLocal);
        playersGameObjects.Add(peerID, spawnedPlayer);

        if(isLocal)
            LocalPlayer = spawnedPlayer;
    }

    private void DestroyPlayerObjects(string peerID){
        if(string.IsNullOrEmpty(peerID))
        {
            Debug.Log("[ProtonManager, DestroyPlayerObjects()]: PeerID is null!");
            return;
        }
        GameObject spawnedPlayer = GameObject.Find("Player_" + peerID);
        
        if(spawnedPlayer != null)
            Destroy(spawnedPlayer);
    }

    private void HandleOnOpen()
    {
        isOpen = true;

        connectionStateText.text = "Conexão aberta";

        SpawnPlayer(peer.GetLocalPeerID(), true);

        InvokeRepeating("CheckNewPeers", 0, 10);
    }

    public void Connect(string peerID, string roomName = null){
        if(string.IsNullOrEmpty(peerID))
            return;

        peer.Connect(peerID, roomName);
    }

    private void OnDestroy(){
        peer.OnConnection -= HandleOnConnection;
        peer.OnOpen -= HandleOnOpen;
        peer.OnClose -= HandleOnCloseLocalPeer;
        peer.OnError -= HandleOnError;
        peer.OnDisconnected -= HandleOnDisconnected;
        peer = null;
        peer.Destroy();
    }

    private void HandleOnConnection(UnityPeerJS.Peer.IConnection connection)
    {
        // CheckNewPeers();

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
        CheckAndSetMasterClient();
    }

    public void CheckAndSetMasterClient(){
        if(selectedMasterClient)
            return;

        string peerIDSelected = _peerList[0];
        if(!playersGameObjects.ContainsKey(peerIDSelected) || playersGameObjects[peerIDSelected] == null)
            return;

        playersGameObjects[peerIDSelected].GetComponent<EntityIdentity>().SetIsMasterClient(true);
        selectedMasterClient = true;
    }

    // void Update(){
    //     if(sendConnectionStats){
    //         currentTimeToSendConnectionStats += Time.deltaTime;
    //         if(currentTimeToSendConnectionStats >= timeToSendConnectionStats){
    //             Debug.Log(lastConnectionStatsData);
    //             currentTimeToSendConnectionStats = 0;
    //         }
    //     }
    // }
}
