using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EventManagerData {
    public int Code { get; set; }
    public int ConnectionIndex { get; set; }
    public string Data { get; set; }
    public string Error { get; set; }
    public string PeerID { get; set; }
}

public class PeerJSEventManager
{
    public PeerJSEventManager() {}

    public void Handle(EventManagerData data, 
        UnityPeerJS.Peer localPeer,
        Dictionary<int, UnityPeerJS.Peer.Connection> connections, 
        System.Action HandleOnOpen,
        System.Action<UnityPeerJS.Peer.IConnection> HandleOnConnection, 
        System.Action HandleOnDisconnected,
        System.Action HandleOnClose,
        System.Action<string> HandleOnError
    ){
        UnityPeerJS.PeerEventType localPeerEventType = (UnityPeerJS.PeerEventType)data.Code;
        switch (localPeerEventType)
        {
            case UnityPeerJS.PeerEventType.Initialized:
            {
                HandleOnOpen();
                break;
            }
            case UnityPeerJS.PeerEventType.Connected:
            {
                int connectionIndex = data.ConnectionIndex;
                string remoteId = data.PeerID;
                connections[connectionIndex] = new UnityPeerJS.Peer.Connection(localPeer, connectionIndex, remoteId);
                HandleOnConnection(connections[connectionIndex]);
                Debug.Log(string.Format("[ProtonManager] `{0}` connected with `{1}`", localPeer.GetLocalPeerID(), remoteId));
                break;
            }
            case UnityPeerJS.PeerEventType.Received:
            {
                connections[data.ConnectionIndex].EmitOnData(data.Data);
                break;
            }

            case UnityPeerJS.PeerEventType.ConnClosed:
            {
                connections[data.ConnectionIndex].EmitOnClose();
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
                HandleOnError(data.Data);
                break;
            }

            case UnityPeerJS.PeerEventType.PeerList:
            {
                ProtonManager.Instance.SetPeerList(Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(data.Data));
                break;
            }

            default:
            {
                break;
            }
        }
    }
}
