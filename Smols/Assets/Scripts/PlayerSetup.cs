using UnityEngine;
using Mirror;

[RequireComponent(typeof(PlayerController))]
public class PlayerSetup : NetworkBehaviour {
    [SerializeField]
    private Behaviour[] componentsToDisable;
    [SerializeField]
    private string remoteLayerName = "RemotePlayer";
    [SerializeField]
    private string dontDrawLayerName = "DontDraw";
    [SerializeField]
    private GameObject playerGraphics;
    [SerializeField]
    private GameObject playerUIPrefab;
    private GameObject playerUIInstance;

    private void Start() {
        if (!isLocalPlayer) {
            DisableComponents();
            AssignRemoteLayer();
        } else {
            //disable main camera
            if (GameManager.instance.mainCamera != null)
                GameManager.instance.mainCamera.gameObject.SetActive(false);
            //disable player graphics for local player
            SetLayerRecursively(playerGraphics, LayerMask.NameToLayer(dontDrawLayerName));
            //create playerUi
            playerUIInstance = Instantiate(playerUIPrefab);
            playerUIInstance.name = playerUIPrefab.name;
        }
        Cursor.lockState = CursorLockMode.Locked;
        GetComponent<PlayerManager>().Setup();
    }

    private void Update() {
        //unlock--lock Cursor
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (Cursor.lockState == CursorLockMode.None)
                Cursor.lockState = CursorLockMode.Locked;
            else if (Cursor.lockState == CursorLockMode.Locked)
                Cursor.lockState = CursorLockMode.None;
        }
    }

    private void SetLayerRecursively(GameObject _obj, int _newLayer) {
        _obj.layer = _newLayer;

        foreach (Transform _child in _obj.transform) {
            SetLayerRecursively(_child.gameObject, _newLayer);
        }
    }

    public override void OnStartClient() {
        base.OnStartClient();

        //assigns the network ID
        string _netId = GetComponent<NetworkIdentity>().netId.ToString();
        PlayerManager _player = GetComponent<PlayerManager>();

        GameManager.RegisterPlayer(_netId, _player);
    }

    private void AssignRemoteLayer() {
        gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
    }

    private void DisableComponents() {
        for (int i = 0; i < componentsToDisable.Length; i++) {
            componentsToDisable[i].enabled = false;
        }
    }

    private void OnDisable() {
        Cursor.lockState = CursorLockMode.None;
        Destroy(playerUIInstance);
        if (GameManager.instance.mainCamera != null)
            GameManager.instance.mainCamera.gameObject.SetActive(true);
        GameManager.UnRegisterPlayer(transform.name);
    }
}
