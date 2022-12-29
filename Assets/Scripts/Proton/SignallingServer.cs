using System.Collections;
using System.Collections.Generic;
using Proyecto26;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Proton {

    [System.Serializable]
    public class SignallingServerPeer {
        [JsonProperty("connected")]
        public bool Connected { get; set; }
        [JsonProperty("peerID")]
        public string PeerID { get; set; }
        [JsonProperty("date")]
        public string Date { get; set; }

        public override string ToString()
        {
            return string.Format("PeerID: {0} | Connected: {1} | Date: {2}", PeerID, Connected, Date);
        }
    }

    public class SignallingServer
    {
        private const string URL_SERVER = "http://localhost/signalling-server/";//"https://signallingserver.xrodrigobr.repl.co/";

        public List<SignallingServerPeer> AllPeers { get; set; } = new List<SignallingServerPeer>();

        public SignallingServer() {
            CheckAndGetPeers();
        }

        public void AddNewPeer(string peerID){
            try {
                if(string.IsNullOrEmpty(peerID))
                    throw new System.Exception("Peer ID is empty!");

                RestClient.Get(string.Format("{0}?peer_id={1}", URL_SERVER, peerID)).Then(response => {
                    // log(response.Text, false);
                });
            }
            catch(System.Exception ex){
                log(ex.Message, true);
            }
        }

        public void RemovePeer(string peerID){
            try {
                if(string.IsNullOrEmpty(peerID))
                    throw new System.Exception("Peer ID is empty!");

                RestClient.Get(string.Format("{0}?delete_peer={1}", URL_SERVER, peerID)).Then(response => {
                    // log(response.Text, false);
                });
            }
            catch(System.Exception ex){
                log(ex.Message, true);
            }
        }

        public void ChangeConnectionStateOfPeer(string peerID, bool state = true){
            try {
                if(string.IsNullOrEmpty(peerID))
                    throw new System.Exception("Peer ID is empty!");

                RestClient.Get(string.Format("{0}?peer_id={1}&connect={2}", URL_SERVER, peerID, state)).Then(response => {
                    // log(response.Text, false);
                });
            }
            catch(System.Exception ex){
                log(ex.Message, true);
            }
        }

        public void CheckAndGetPeers(System.Action<List<SignallingServerPeer>> callbackGet = null){
            try {
                RestClient.Get(string.Format("{0}?get_peers", URL_SERVER)).Then(response => {
                    JObject data = JObject.Parse(response.Text);
                    AllPeers = data["data"].ToObject<List<SignallingServerPeer>>();
                    if(callbackGet != null){
                        callbackGet(AllPeers);
                    }
                    // log(response.Text, false);
                });
            }
            catch(System.Exception ex){
                log(ex.Message, true);
            }
        }

        private void log(string message, bool error){
            string text = string.Format("{0}: {1}", "[SIGNALLING SERVER]", message);
            if(error){
                Debug.LogError(text);
                return;
            }
            Debug.Log(text);        
        }
    }

}