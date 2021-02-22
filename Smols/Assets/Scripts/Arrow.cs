using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(Rigidbody))]
public class Arrow : NetworkBehaviour, IPooledObject {
    public Collider Collision;
    public Collider Trigger;

    private Rigidbody rb;
    private bool hitSomething = false;
    private int damage;

    private const string PLAYER_TAG = "Player";

    private NetworkIdentity legacyId;
    private NetworkIdentity id;

    public void SetDamage(int _damage) {
        damage = _damage;
    }

    public void SetUp() {
        rb.constraints = RigidbodyConstraints.None;
        hitSomething = false;
        Collision.enabled = true;
        Trigger.enabled = true;
        SetLegacyAuthority();
    }

    public void SetAuthority(NetworkIdentity _id) {
        Debug.Log(_id);
        id.AssignClientAuthority(_id.connectionToClient);
    }

    private void SetPlayerAuthority() {
        GetComponent<NetworkIdentity>().AssignClientAuthority(id.connectionToClient);
    }

    private void SetLegacyAuthority() {
        GetComponent<NetworkIdentity>().AssignClientAuthority(legacyId.connectionToClient);
    }

    private void Start() {
        legacyId = new NetworkIdentity();
        id = new NetworkIdentity();
        legacyId.AssignClientAuthority(GetComponent<NetworkIdentity>().connectionToServer);
    }

    public void OnObjectSpawn() {
        rb = GetComponent<Rigidbody>();
        if (rb.velocity != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(rb.velocity);
    }

    private void Update() {
        if (rb == null)
            rb = GetComponent<Rigidbody>();
        if (!hitSomething && rb.velocity != Vector3.zero) {
            transform.rotation = Quaternion.LookRotation(rb.velocity);
        }
    }
    
    private void OnCollisionEnter(Collision collision) {
        if (collision.collider.tag != "Arrow") {
            hitSomething = true;
            Stick();
            Collision.enabled = false;
            Trigger.enabled = false;
        }
    }

    public void OnTriggerEnter(Collider collision) {
        if (collision.tag != "Arrow") {
            hitSomething = true;
            Stick();
            Collision.enabled = false;
            Trigger.enabled = false;
            if (collision.tag == PLAYER_TAG) {
                SetPlayerAuthority();
                PlayerShot(collision.gameObject.name, damage);
            }
        }
    }

    private void Stick() {
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    [Client]
    private void PlayerShot(string _playerId, int _damage) {
        PlayerManager _player = GameManager.GetPlayer(_playerId);
        _player.RpcTakeDamage(_damage);
        //PlayerShoot.instance.CmdHitShot(_playerId, _damage);
    }
}
