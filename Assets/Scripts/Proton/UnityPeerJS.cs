using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public static class UnityPeerJS
{
    public enum PeerEventType
    {
        None = 0,
        Initialized = 1,
        Connected = 2,
        Received = 3,
        ConnClosed = 4,
        PeerDisconnected = 5,
        PeerClosed = 6,
        Error = 7
    }

    public class Peer
    {
        private readonly Dictionary<int, Connection> _connections = new Dictionary<int, Connection>();

        private readonly int _peerIndex;

        public int GetPeerIndex() => _peerIndex;

        public Peer()
        {
            
            Init();

            _peerIndex = OpenPeer();
        }

        public event Action OnOpen;
        public event Action<IConnection> OnConnection;
        public event Action OnDisconnected;
        public event Action OnClose;
        public event Action<string> OnError;

        public string GetLocalPeerID(){
            return UnityPeerJS.GetLocalPeerID(_peerIndex);
        }

        public void Connect(string remoteId)
        {
            UnityPeerJS.Connect(_peerIndex, remoteId);
        }

        public void Disconnect()
        {
            PeerDisconnect(_peerIndex);
        }

        public void Destroy()
        {
            PeerDestroy(_peerIndex);
        }

        public interface IConnection
        {
            Peer Peer { get; }

            string RemoteId { get; }

            event Action<string> OnData;
            event Action OnClose;

            void Send(string str);
            void Close();
        }

        public class Connection : IConnection
        {
            private readonly int _connIndex;

            public Connection(Peer peer, int connIndex, string remoteId)
            {
                Peer = peer;
                _connIndex = connIndex;
                RemoteId = remoteId;
            }

            public event Action<string> OnData;
            public event Action OnClose;

            public Peer Peer { get; set;  }
            public string RemoteId { get; set; }

            public void Send(string str)
            {
                UnityPeerJS.Send(Peer._peerIndex, _connIndex, str, str.Length);
            }

            public void Close()
            {
                ConnClose(Peer._peerIndex, _connIndex);
            }

            public void EmitOnData(string str)
            {
                if (OnData != null)
                    OnData(str);
            }

            public void EmitOnClose()
            {
                if (OnClose != null)
                    OnClose();
            }
        }
    }

#if UNITY_WEBGL && !UNITY_EDITOR

    [DllImport("__Internal")]
    public static extern void Init();

    [DllImport("__Internal")]
    public static extern int OpenPeer();
    [DllImport("__Internal")]
    public static extern string GetLocalPeerID(int peerInstance);

    [DllImport("__Internal")]
    public static extern void Connect(int peerInstance, string remoteId);

    [DllImport("__Internal")]
    public static extern void Send(int peerInstance, int connInstance, string ptr, int length);

    [DllImport("__Internal")]
    public static extern void ConnClose(int peerInstance, int connInstance);

    [DllImport("__Internal")]
    public static extern void PeerDisconnect(int peerInstance);

    [DllImport("__Internal")]
    public static extern void PeerDestroy(int peerInstance);
#else

    public static void Init()
    {
        throw new NotImplementedException();
    }

    public static string GetLocalPeerID(int peerInstance){
        throw new NotImplementedException();
    }

    public static int OpenPeer()
    {
        throw new NotImplementedException();
    }

    public static void Connect(int peerInstance, string remoteIdStr)
    {
        throw new NotImplementedException();
    }

    public static void Send(int peerInstance, int connInstance, string ptr, int length)
    {
        throw new NotImplementedException();
    }

    public static void ConnClose(int peerInstance, int connInstance)
    {
        throw new NotImplementedException();
    }

    public static void PeerDisconnect(int peerInstance)
    {
        throw new NotImplementedException();
    }

    public static void PeerDestroy(int peerInstance)
    {
        throw new NotImplementedException();
    }
#endif
}