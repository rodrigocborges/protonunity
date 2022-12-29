using UnityEngine;
using Proton;

public class PlayerPeerJS : MonoBehaviour
{
    [SerializeField] private float speed;

    private SendData _sendDataRaycast;
    private SendDataInstantiate _sendDataInstantiate;

    private RaycastHit hit;

    private EntityIdentity _identity;

    void Awake(){
        _identity = GetComponent<EntityIdentity>();
    }

    void Start(){
        GetComponent<MeshRenderer>().material.color = _identity.IsMine() ? Color.red : Color.blue;

        // _sendData = new SendData(0.05f); //1 -> LENTO, 0.5 -> MAIS OU MENOS; 0.2 -> relativamente bom
        _sendDataInstantiate = new SendDataInstantiate(_identity.GetPeerID());
        _sendDataRaycast = new SendData(0f);
    }

    void SpawnObject(bool onlyLocal){
        GameObject spawnedObject = Instantiate(Resources.Load<GameObject>("TestCube"), new Vector3(hit.point.x, 0, hit.point.z), Quaternion.identity);
        spawnedObject.name = string.Format("GameObject_{0}", _identity.GetPeerID());

        if(onlyLocal){
            return;
        }

        _sendDataInstantiate.Add("TestCube", new Vector3(hit.point.x, 0, hit.point.z), Quaternion.identity);
        _sendDataRaycast.Setup(SendDataType.Instantiate, _sendDataInstantiate);
    }

    void Update()
    {
        if(!_identity.IsMine() || string.IsNullOrEmpty(_identity.GetPeerID()))
            return;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        transform.Translate(new Vector3(horizontal * Time.deltaTime * speed, 0, vertical * Time.deltaTime * speed));

        if(Input.GetMouseButtonDown(1)){
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hit))
                SpawnObject(false);
        }

    }
}
