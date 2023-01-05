using UnityEngine;
using Proton;

public class PlayerPeerJS : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float rotateSpeed;

    private SendData _sendData;
    private SendDataInstantiate _sendDataInstantiate;
    private RaycastHit hit;
    private EntityIdentity _identity;
    
    void Awake(){
        _identity = GetComponent<EntityIdentity>();
    }

    void Start(){
        GetComponent<MeshRenderer>().material.color = _identity.IsMine() ? Color.red : Color.blue;

        if(!_identity.IsMine())
            return;

        _sendDataInstantiate = new SendDataInstantiate(_identity.GetPeerID());
        _sendData = new SendData(0f);
    }

    void SpawnObject(bool onlyLocal){
        GameObject spawnedObject = Instantiate(Resources.Load<GameObject>("TestCube"), new Vector3(hit.point.x, 0, hit.point.z), Quaternion.identity);
        spawnedObject.name = string.Format("GameObject_{0}", _identity.GetPeerID());

        if(onlyLocal){
            return;
        }

        _sendDataInstantiate.Add("TestCube", new Vector3(hit.point.x, 0, hit.point.z), Quaternion.identity);
        _sendData.Setup(SendDataType.Instantiate, _sendDataInstantiate);
    }

    void Update()
    {
        if(!_identity.IsMine() || string.IsNullOrEmpty(_identity.GetPeerID()))
            return;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        transform.Translate(new Vector3(0, 0, vertical * Time.deltaTime * speed));
        transform.Rotate(new Vector3(0, horizontal * Time.deltaTime * rotateSpeed, 0));

        if(Input.GetKeyDown(KeyCode.X)){
            transform.localScale = new Vector3(Random.Range(1, 3), Random.Range(1, 3), Random.Range(1, 3));
        }

        if(Input.GetKeyDown(KeyCode.Z)){
            transform.localScale = new Vector3(1, 1, 1);
        }

        if(Input.GetMouseButtonDown(1)){
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hit))
                SpawnObject(false);
        }
    }
}
