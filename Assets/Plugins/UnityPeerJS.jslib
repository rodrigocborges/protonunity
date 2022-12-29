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
            peer: new Peer(null, { debug: 2 }),
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

                peer.events.push({ ev: 2, conn: connInstance, id: conn.peer });
                window.myGameInstance.SendMessage('PeerJSManager', 'EventManager', JSON.stringify({ Code: 2, ConnectionIndex: connInstance, PeerID: conn.peer }));
                console.log({ method: 'Callback Connection Opened', conn });

                _getStatsOfConnection();

                conn.on('data', function (data) {

                    _getStatsOfConnection();

                    peer.events.push({ ev: 3, conn: connInstance, data: data });
                    window.myGameInstance.SendMessage('PeerJSManager', 'EventManager', JSON.stringify({ Code: 3, ConnectionIndex: connInstance, Data: data }));

                });
            });

            conn.on('close', function () {

                peer.events.push({ ev: 4, conn: connInstance });
                window.myGameInstance.SendMessage('PeerJSManager', 'EventManager', JSON.stringify({ Code: 4, ConnectionIndex: connInstance }));

            });
        };

        peer.popEvent = function (eventType) {
            
            if (peer.events.length == 0) {
                console.log("error: popEvent: event queue is empty");
                return null;
            }

            if (eventType != 0 && peer.events[0].ev != eventType) {
                console.log("error: popEvent: event type mismatch", eventType, peer.events[0].ev);
                return null;
            }

            var result = peer.events.shift();
            return result;
        };

        peer.peer.on('open', function (id) {
            // console.log('callback open peer: ' + id);
            peer.localId = id;
            peer.initialized = true;
            peer.events.push({ ev: 1 });
            window.myGameInstance.SendMessage('PeerJSManager', 'EventManager', JSON.stringify({ Code: 1 }));
        });

        peer.peer.on('connection', peer.newConnection);
        peer.peer.on('disconnected', function () { 
            peer.events.push({ ev: 5 });
            window.myGameInstance.SendMessage('PeerJSManager', 'EventManager', JSON.stringify({ Code: 5 }));
        });
        peer.peer.on('close', function () { 
            peer.events.push({ ev: 6 });
            window.myGameInstance.SendMessage('PeerJSManager', 'EventManager', JSON.stringify({ Code: 6 }));
        });
        peer.peer.on('error', function (err) { 
            peer.events.push({ ev: 7, err: err.type });
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

    Connect: function (peerInstance, id) {
        
        var idstr = UTF8ToString(id);
        var peer = UnityPeerJS.peers[peerInstance];
        
        // console.log({ method: 'Connect', idstr, peer });
        
        peer.newConnection(peer.peer.connect(idstr));
    },

    Send: function (peerInstance, connInstance, data, length) {

        var peer = UnityPeerJS.peers[peerInstance];
        var conn = peer.conns[connInstance];
        var datastr = UTF8ToString(data);

        // console.log({ method: 'Send', peer, conn, datastr, length });

        // conn.send(datastr);
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
    },

    NextEventType: function (peerInstance) {

        var peer = UnityPeerJS.peers[peerInstance];

        if (peer.events.length == 0)
            return 0;//"";

        return peer.events[0].ev;
    },

    PopAnyEvent: function (peerInstance) {

        var peer = UnityPeerJS.peers[peerInstance];

        peer.popEvent(0);
    },

    PopInitializedEvent: function (peerInstance) {

        var peer = UnityPeerJS.peers[peerInstance];
        var ev = peer.popEvent(1);
    },

    PopConnectedEvent: function (peerInstance, remoteIdPtr, remoteIdMaxLength) {

        var peer = UnityPeerJS.peers[peerInstance];
        var ev = peer.popEvent(2);

        console.log({ method: 'PopConnectedEvent', peer, ev });

        //OLD EXAMPLE
        // var returnStr = "bla";
        // var buffer = _malloc(lengthBytesUTF8(returnStr) + 1);
        // writeStringToMemory(returnStr, buffer);
        // return buffer;
        
        // writeStringToMemory(ev.id, remoteIdPtr);

        var bufferSize = lengthBytesUTF8(ev.id) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(ev.id, buffer, bufferSize); 

        return ev.conn;
    },

    PeekReceivedEventSize: function (peerInstance) {
        
        var peer = UnityPeerJS.peers[peerInstance];
        
        if (peer.events.length == 0) {
            console.error("error: PeekReceivedEventSize: no event to peek at");
            return 0;
        }

        var ev = peer.events[0];
        
        if (ev.ev == 3) {
            return lengthBytesUTF8(ev.data);
        }
        
        if (ev.ev == 2) {
            return lengthBytesUTF8(ev.id);
        }
        
        console.error("error: PeekReceivedEventSize: next event is of wrong type", ev.ev);
        return 0;
    },

    PopReceivedEvent: function (peerInstance, dataPtr, dataMaxLength) {

        var peer = UnityPeerJS.peers[peerInstance];
        var ev = peer.popEvent(3);
        
        console.log({ method: 'PopReceivedEvent', peer, data: ev.data });

        if (ArrayBuffer.isView(ev.data)) {
            console.log("Array buffers not supported");
            return ev.conn;
        }
        
        // writeStringToMemory(ev.data, dataPtr);

        var bufferSize = lengthBytesUTF8(ev.data) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(ev.data, buffer, bufferSize); 

        return ev.conn;
    },

    PopConnClosedEvent: function (peerInstance) {
        
        var peer = UnityPeerJS.peers[peerInstance];
        var ev = peer.popEvent(4);

        return ev.conn;
    },

    PopPeerDisconnectedEvent: function (peerInstance) {
        
        var peer = UnityPeerJS.peers[peerInstance];
        var ev = peer.popEvent(5);
    },

    PopPeerClosedEvent: function (peerInstance) {
        
        var peer = UnityPeerJS.peers[peerInstance];
        var ev = peer.popEvent(6);
    },

    PopErrorEvent: function (peerInstance, errorPtr, errorMaxLength) {
        
        var peer = UnityPeerJS.peers[peerInstance];
        var ev = peer.popEvent(7);

        var str = ev.err.slice(0, Math.max(0, errorMaxLength / 2 - 1));

        // writeStringToMemory(str, errorPtr);
        
        var bufferSize = lengthBytesUTF8(str) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(str, buffer, bufferSize); 
    },
};

autoAddDeps(LibraryUnityPeerJS, '$UnityPeerJS');
mergeInto(LibraryManager.library, LibraryUnityPeerJS);