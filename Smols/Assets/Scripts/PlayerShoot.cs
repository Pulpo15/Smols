using UnityEngine;
using Mirror;
using System;

public class PlayerShoot : NetworkBehaviour {

    public PlayerWeapon weapon;

    [SerializeField]
    private Camera cam;

    [SerializeField]
    private LayerMask mask;

    private const string PLAYER_TAG = "Player";

    private void Start() {
        if (cam == null) {
            Debug.LogError("PlayerShoot: No camera referenced");
            this.enabled = false;
        }
    }

    private void Update() {
        if (Input.GetButtonDown("Fire1")) {
            Shoot();
        }
    }

    [Client]
    private void Shoot() {
        RaycastHit _hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit, weapon.range, mask)) {
            if (_hit.collider.tag == PLAYER_TAG)
                CmdPlayerShot(_hit.collider.name, weapon.damage);
        }
    }

    [Command]
    private void CmdPlayerShot(string _playerId, int _damage) {
        //Debug.Log(_playerId + " has been shot.");

        PlayerManager _player = GameManager.GetPlayer(_playerId);
        _player.RpcTakeDamage(_damage);
    }
}
