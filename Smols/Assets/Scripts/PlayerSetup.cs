using UnityEngine;
using Mirror;

[RequireComponent(typeof(PlayerController))]
public class PlayerSetup : NetworkBehaviour {
    [SerializeField]
    private Behaviour[] componentsToDisable;
    [SerializeField]
    private string remoteLayerName = "RemotePlayer";

    private void Start() {
        if (!isLocalPlayer) {
            DisableComponents();
            AssignRemoteLayer();
        } else {
            if (GameManager.instance.mainCamera != null)
                GameManager.instance.mainCamera.gameObject.SetActive(false);
        }
        GetComponent<PlayerManager>().Setup();
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
        if (GameManager.instance.mainCamera != null)
            GameManager.instance.mainCamera.gameObject.SetActive(true);
        GameManager.UnRegisterPlayer(transform.name);
    }
}
