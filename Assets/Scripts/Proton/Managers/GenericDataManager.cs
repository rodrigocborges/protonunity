using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Proton {

    public class GenericDataInfo {
        public string PeerID { get; set; }
        public string DataKey { get; set; }
        public object DataValue { get; set; }

        public override string ToString()
        {
            return string.Format("{0}:{1};{2}", PeerID, DataKey, DataValue.ToString());
        }
    }


    public class GenericDataManager : MonoBehaviour
    {
        // private Dictionary<string, List<KeyValuePair<string, object>>> _data = new Dictionary<string, List<KeyValuePair<string, object>>>();

        private List<GenericDataInfo> _allData = new List<GenericDataInfo>();
    
        private const int DATA_CAPACITY_BY_PEER = 200;
        private string _peerID = null;

        private GameObject _playerGameObject;

        private List<EntityIdentity> _listeners = null;

        public void Setup(string peerID, GameObject playerGameObject){
            _peerID = peerID;
            _listeners = new List<EntityIdentity>();
            _allData.Add(new GenericDataInfo { PeerID = _peerID });
            _playerGameObject = playerGameObject;

            Debug.Log(string.Format("[GenericDataManager, Setup()] `{0}`", peerID));
        }

        public void AddListener(EntityIdentity entity){
            if(_listeners.Contains(entity))
                return;

            _listeners.Add(entity);
        }

        public void RemoveListener(EntityIdentity entity){
            if(!_listeners.Contains(entity))
                return;

            _listeners.Remove(entity);
        }

        public void Add(string peerID, string key, object val){
            try {
                if(string.IsNullOrEmpty(peerID) || val == null)
                    return;

                /*
                    - Chaves para buscar na lista: PeerID e DataKey.
                    - Só é permitido adicionar caso a DataKey for diferente, pois
                        se for igual, apenas altera o valor.
                */

                GenericDataInfo _data = _allData.FirstOrDefault(x => x.PeerID.Equals(peerID));

                if(_data == null){
                    GenericDataInfo _newData = new GenericDataInfo { PeerID = peerID, DataKey = key, DataValue = val };
                    _allData.Add(_newData);
                    _data = _newData;
                }else {
                    _data.DataKey = key;
                    _data.DataValue = val;
                }

                Debug.Log(string.Format("[GenericDataManager, Add()] `{0}` -> ", peerID, _data.ToString()));
            }
            catch(System.Exception ex){
                Debug.Log(string.Format("[GenericDataManager, Add()] `{0}` exception -> {1}", peerID, ex.Message));
            }
        }

        public object Get(string peerID, string key){
            try {
                if(string.IsNullOrEmpty(peerID) || string.IsNullOrEmpty(key))
                    return "";

                GenericDataInfo _data = _allData.FirstOrDefault(x => x.PeerID.Equals(peerID) && x.DataKey.Equals(key));
                return _data.DataValue;
            }
            catch(System.Exception ex){
                Debug.Log(string.Format("[GenericDataManager, Get()] `{0}` exception -> {1}", peerID, ex.Message));
                return "";
            }
        }
    }
}
