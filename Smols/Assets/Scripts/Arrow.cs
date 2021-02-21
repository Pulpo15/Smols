using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(Rigidbody))]
public class Arrow : NetworkBehaviour, IPooledObject {
    private PlayerShoot playerShoot;
    private Rigidbody rb;
    private bool hitSomething = false;
    private int damage;

    private const string PLAYER_TAG = "Player";

    public void SetDamage(int _damage) {
        damage = _damage;
    }

    private void Start() {
        playerShoot = FindObjectOfType<PlayerShoot>();
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
        }
    }

    private void OnTriggerEnter(Collider collision) {
        if (collision.tag != "Arrow") {
            hitSomething = true;
            Stick();
            if (collision.tag == PLAYER_TAG)
                PlayerShot(collision.gameObject.name, damage);
        }
    }

    private void Stick() {
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    public void SetUp() {
        rb.constraints = RigidbodyConstraints.None;
        hitSomething = false;
    }

    private void PlayerShot(string _playerId, int _damage) {
        //playerShoot.CmdHitShot(_playerId, _damage);
    }
}
