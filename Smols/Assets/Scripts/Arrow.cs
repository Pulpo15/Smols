using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Arrow : MonoBehaviour {
    private Rigidbody rb;
    private float lifeTime = 2f;
    private float totalLifeTime = 10f;
    private bool hitSomething = false;
    private int damage;

    private const string PLAYER_TAG = "Player";
    public void SetDamage(int _damage) {
        damage = _damage;
    }

    private void Start() {
        rb = GetComponent<Rigidbody>();
        transform.rotation = Quaternion.LookRotation(rb.velocity);
        StartCoroutine(ArrowTimeOut(totalLifeTime));
    }

    private void Update() {
        if (!hitSomething) {
            transform.rotation = Quaternion.LookRotation(rb.velocity);
        }
    }

    private void PlayerShot(string _playerId, int _damage) {
        PlayerManager _player = GameManager.GetPlayer(_playerId);
        _player.RpcTakeDamage(_damage);
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
        Destroy(gameObject);
    }
}
