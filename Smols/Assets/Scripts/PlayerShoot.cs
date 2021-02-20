using UnityEngine;
using System.Collections;
using Mirror;
using System;

public class PlayerShoot : NetworkBehaviour {

    public PlayerWeapon weapon;

    [SerializeField]
    private Camera cam;
    [SerializeField]
    private LayerMask mask;
    [SerializeField]
    private GameObject graphicsArrowPrefab;
    [SerializeField]
    private GameObject arrowPrefab;
    [SerializeField]
    private Transform arrowSpawn;

    private ObjectPooler objectPooler;
    private GameObject graphicsArrow;
    private bool canShoot = true;
    private string id;

    private void Start() {
        if (cam == null) {
            Debug.LogError("PlayerShoot: No camera referenced");
            this.enabled = false;
        }
        objectPooler = ObjectPooler.instance;
        weapon.curRange = weapon.initRange;
        graphicsArrow = Instantiate(graphicsArrowPrefab, arrowSpawn.position, Quaternion.identity);
        id = GetComponent<NetworkIdentity>().netId.ToString();
        Debug.Log(id);
        graphicsArrow.SetActive(false);
    }

    private void Update() {
        if (!canShoot)
            return;
        if (Input.GetButton("Fire1")) {
            if (weapon.curRange <= weapon.maxRange)
                weapon.curRange += weapon.addRange * Time.deltaTime;
            if (weapon.curRange > weapon.maxRange)
                weapon.curRange = weapon.maxRange;
            graphicsArrow.SetActive(true);
        } else if (Input.GetButtonUp("Fire1")) {
            StartCoroutine(Shoot(weapon.curRange));
            graphicsArrow.SetActive(false);
            canShoot = false;
        }
        if (graphicsArrow.activeSelf) {
            graphicsArrow.transform.position = arrowSpawn.position;
            graphicsArrow.transform.rotation = arrowSpawn.rotation;
        }

    }

    [Client]
    private IEnumerator Shoot(float _range) {
        CmdPlayerShot(_range);

        yield return new WaitForSeconds(weapon.reloadTime);

        canShoot = true;
    }

    [Command]
    private void CmdPlayerShot(float _range) {
        //Debug.Log(_playerId + " has been shot.");
        if (objectPooler == null)
            objectPooler = ObjectPooler.instance;
        GameObject go = objectPooler.SpawnFromPool(id + "Arrow", arrowSpawn.position, arrowSpawn.rotation);
        Rigidbody rb = go.GetComponent<Rigidbody>();

        //NetworkServer.Spawn(go);

        rb.velocity = cam.transform.forward * _range;
        go.GetComponent<Arrow>().SetDamage(weapon.damage);

        weapon.curRange = weapon.initRange;
    }

}
