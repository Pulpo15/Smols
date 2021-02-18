using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(Rigidbody))]
public class Arrow : NetworkBehaviour, IPooledObject {  
    private Rigidbody rb;
    private float lifeTime = 2f;
    private float totalLifeTime = 10f;
    private bool hitSomething = false;
    private int damage;

    private const string PLAYER_TAG = "Player";

    public void SetDamage(int _damage) {
        damage = _damage;
    }

    public void OnObjectSpawn() {
        rb = GetComponent<Rigidbody>();
        if (rb.velocity != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(rb.velocity);
        StartCoroutine(ArrowTimeOut(totalLifeTime));
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
            StartCoroutine(ArrowTimeOut(lifeTime));
        }
    }

    private void OnTriggerEnter(Collider collision) {
        if (collision.tag != "Arrow") {
            hitSomething = true;
            Stick();
            StartCoroutine(ArrowTimeOut(lifeTime));
            if (collision.tag == PLAYER_TAG)
                PlayerShot(collision.gameObject.name, damage);
        }
    }

    private void Stick() {
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    private IEnumerator ArrowTimeOut(float _time) {
        yield return new WaitForSeconds(_time);
        rb.constraints = RigidbodyConstraints.None;
        hitSomething = false;
        gameObject.SetActive(false);
    }

    [Command]
    private void PlayerShot(string _playerId, int _damage) {
        PlayerManager _player = GameManager.GetPlayer(_playerId);
        _player.RpcTakeDamage(_damage);
    }
}
