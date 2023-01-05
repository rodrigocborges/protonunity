using System.Collections;
using UnityEngine;

namespace Proton {
    public class ReceiveData
    {
        private ProtonManager _connectionManager;
        public ReceiveData (ProtonManager connectionManager ) {
            _connectionManager = connectionManager;
        }

        public void Handle(string data){
            if(_connectionManager == null) return;

            // Debug.Log(string.Format("[ReceiveData, Handle()]: {0}", data));

            if(data.Contains('|') && data.Contains(':') && data.Contains(';')){ //Ex: Position|0939d-80fds089f-dfs90fsdf-12921:1;2;3
                string[] allData = data.Split('|');
                SendDataType dataType = (SendDataType) System.Enum.Parse(typeof(SendDataType), allData[0], true);
                string[] dataArray = allData[1].Split(':');
                
                string peerID = dataArray[0];
                string[] infoRaw = dataArray[1].Split(';');

                // Debug.Log(string.Format("[ReceiveData, Handle()]: DataType: {0}", dataType));


                switch(dataType){
                    case SendDataType.Position:                    
                        Vector3 targetPosition = new Vector3(float.Parse(infoRaw[0]), float.Parse(infoRaw[1]), float.Parse(infoRaw[2]));
                        _connectionManager.playersGameObjects[peerID].transform.position = targetPosition;
                    break;
                    case SendDataType.Rotation:                    
                        Vector3 targetRotation = new Vector3(float.Parse(infoRaw[0]), float.Parse(infoRaw[1]), float.Parse(infoRaw[2]));
                        // _connectionManager.playersGameObjects[peerID].transform.localRotation = Quaternion.Euler(targetRotation.x, targetRotation.y, targetRotation.z);
                        _connectionManager.playersGameObjects[peerID].transform.eulerAngles = targetRotation;
                    break;
                    case SendDataType.Scale:                    
                        Vector3 targetScale = new Vector3(float.Parse(infoRaw[0]), float.Parse(infoRaw[1]), float.Parse(infoRaw[2]));
                        _connectionManager.playersGameObjects[peerID].transform.localScale = targetScale;
                    break;
                    case SendDataType.Instantiate:
                        string prefabPath = infoRaw[0];
                        Vector3 position = new Vector3(float.Parse(infoRaw[1]), float.Parse(infoRaw[2]), float.Parse(infoRaw[3]));
                        Quaternion rotation = Quaternion.Euler(float.Parse(infoRaw[4]), float.Parse(infoRaw[5]), float.Parse(infoRaw[6]));
                        
                        GameObject spawnedObject = MonoBehaviour.Instantiate(Resources.Load<GameObject>(prefabPath), position, rotation);
                        spawnedObject.name = string.Format("GameObject_{0}", peerID);
                    break;
                    case SendDataType.GenericData:
                        string receivedDataKey = infoRaw[0];
                        object receivedData = infoRaw[1];
                        Debug.Log(string.Format("[ReceiveData]: `{0}` ({1}, {2})", peerID, receivedDataKey, receivedData.ToString()));                        
                        _connectionManager.GenericDataManager.Add(peerID, receivedDataKey, receivedData);
                    break;
                }
                 
            }
        }

        public IEnumerator MoveOverSpeed (GameObject objectToMove, Vector3 end, float speed){
            // speed should be 1 unit per second
            while (objectToMove.transform.position != end)
            {
                objectToMove.transform.position = Vector3.MoveTowards(objectToMove.transform.position, end, speed * Time.deltaTime);
                yield return new WaitForEndOfFrame ();
            }
        }

    }
}


