using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Proton.Sync;

namespace Proton {
    public class EntityIdentity : MonoBehaviour
    {
        private bool _isMine = false;
        private string _peerID = null;
        private bool _isMasterClient = false;
        
        public void SetOwner(string peerID, bool isMine){
            _peerID = peerID;
            _isMine = isMine;
        }

        public bool IsMine() => _isMine;
        public string GetPeerID() => _peerID;

        public bool IsMasterClient() => _isMasterClient;

        public void SetIsMasterClient(bool isMasterClient) => _isMasterClient = isMasterClient;
    }
}
