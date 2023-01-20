var LibraryUnityPeerJS = {
    $UnityPeerJS: {
        peers: []
    },
    
    Init: function () {
        console.log('Init');
    },

    // enum EventType
    // {
    //     Initialized = 1
    //     Connected = 2,
    //     Received = 3,
    //     ConnClosed = 4,
    //     PeerDisconnected = 5,
    //     PeerClosed = 6,
    //     Error = 7,
    // }

    OpenPeer: function () {

        var peer = {
            peer: new Peer(null, { host: 'proton-server.squareweb.app', debug: 2 }),
            initialized: false,
            conns: [],
            events: [],
            localId: ''
        };

        var peerInstance = UnityPeerJS.peers.push(peer) - 1;

        peer.newConnection = function (conn) {

            var connInstance = peer.conns.push(conn) - 1;

            conn.on('open', function () {

                function _getStatsOfConnection(){
                    conn.peerConnection.getStats(null).then((stats) => {
                        stats.forEach((report) => {
                            if(report.type === "candidate-pair"){
                                let connectionStatsData = {};
                                Object.keys(report).forEach((statName) => {
                                    connectionStatsData[statName] = report[statName];
                                });
                                window.myGameInstance.SendMessage('PeerJSManager', 'ConnectionStatsManager', JSON.stringify(connectionStatsData));
                            }                 
                        });
                    });
                }

                window.myGameInstance.SendMessage('PeerJSManager', 'EventManager', JSON.stringify({ Code: 2, ConnectionIndex: connInstance, PeerID: conn.peer }));
                // console.log({ method: 'Callback Connection Opened', conn });

                _getStatsOfConnection();

                conn.on('data', function (data) {

                    _getStatsOfConnection();

                    window.myGameInstance.SendMessage('PeerJSManager', 'EventManager', JSON.stringify({ Code: 3, ConnectionIndex: connInstance, Data: data }));

                });
            });

            conn.on('close', function () {
                window.myGameInstance.SendMessage('PeerJSManager', 'EventManager', JSON.stringify({ Code: 4, ConnectionIndex: connInstance }));
            });
        };

        peer.peer.on('open', function (id) {
            peer.localId = id;
            peer.initialized = true;
            window.myGameInstance.SendMessage('PeerJSManager', 'EventManager', JSON.stringify({ Code: 1 }));

            setInterval(function() {
                peer.peer.listAllPeers(function(peerList) {
                    window.myGameInstance.SendMessage('PeerJSManager', 'EventManager', JSON.stringify({ Code: 8, Data: JSON.stringify(peerList) }));
                });
            }, 5000);
        });

        peer.peer.on('connection', peer.newConnection);
        
        peer.peer.on('disconnected', function () { 
            window.myGameInstance.SendMessage('PeerJSManager', 'EventManager', JSON.stringify({ Code: 5 }));
        });
        
        peer.peer.on('close', function () { 
            window.myGameInstance.SendMessage('PeerJSManager', 'EventManager', JSON.stringify({ Code: 6 }));
        });

        peer.peer.on('error', function (err) { 
            window.myGameInstance.SendMessage('PeerJSManager', 'EventManager', JSON.stringify({ Code: 7, Data: err.type }));
        });

        return peerInstance;
    },

    GetLocalPeerID: function (peerInstance){
        var peer = UnityPeerJS.peers[peerInstance];
        var peerId = peer.localId;
        
        //PROVISORIO
        if(peerId == undefined || peerId == null){
            console.log('`peer.localId` empty');
            peerId = localStorage.getItem('webrtc_peer_id');
            if(peerId == undefined || peerId == null){
                console.log('`localStorage peerId` empty');
                peerId = '';
            }
        }

        // console.log({ method: 'GetLocalPeerID', peer });

        var bufferSize = lengthBytesUTF8(peerId) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(peerId, buffer, bufferSize);

        return buffer;
    },

    Connect: function (peerInstance, id, roomName) {
        var idstr = UTF8ToString(id);
        var peer = UnityPeerJS.peers[peerInstance];
        var roomNameStr = UTF8ToString(roomName);
        //serialization: binary (default)
        peer.newConnection(peer.peer.connect(idstr, { label: roomNameStr, serialization: 'json' }));
    },

    Send: function (peerInstance, connInstance, data, length) {
        var peer = UnityPeerJS.peers[peerInstance];
        var conn = peer.conns[connInstance];
        var datastr = UTF8ToString(data);
        // console.log({ method: 'Send', peer, conn, datastr, length });
        conn.send(datastr);
    },

    ConnClose: function (peerInstance, connInstance) {
        var peer = UnityPeerJS.peers[peerInstance];
        var conn = peer.conns[connInstance];
        conn.close();
    },

    PeerDisconnect: function (peerInstance) {
        var peer = UnityPeerJS.peers[peerInstance];
        peer.disconnect();
    },

    PeerDestroy: function (peerInstance) {
        var peer = UnityPeerJS.peers[peerInstance];
        peer.destroy();
    }
};

autoAddDeps(LibraryUnityPeerJS, '$UnityPeerJS');
mergeInto(LibraryManager.library, LibraryUnityPeerJS);