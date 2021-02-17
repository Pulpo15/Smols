using UnityEngine;
using Mirror;
using System.Collections;

public class PlayerManager : NetworkBehaviour {

    [SyncVar]
    private bool _isDead = false;
    public bool isDead {
        get { return _isDead; }
        protected set { _isDead = value; }
    }

    [SerializeField]
    private int maxHealth = 100;

    [SyncVar]
    private int currentHealth;

    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;
    [SerializeField]
    private Behaviour[] enableOnDeath;
    private bool[] wasDisabled;

    public void Setup() {
        wasEnabled = new bool[disableOnDeath.Length];
        for (int i = 0; i < wasEnabled.Length; i++) {
            wasEnabled[i] = disableOnDeath[i].enabled;
        }
        wasDisabled = new bool[enableOnDeath.Length];
        for (int i = 0; i < wasDisabled.Length; i++) {
            wasDisabled[i] = enableOnDeath[i].enabled;
        }
        
        SetDefaults();
    }

    [ClientRpc]
    public void RpcTakeDamage(int _damage) {
        if (isDead)
            return;

        currentHealth -= _damage;

        Debug.Log(transform.name + " now has " + currentHealth + " health");

        if (currentHealth <= 0) {
            Die();
        }
    }

    private void Die() {
        isDead = true;

        //deactivates objects when die
        for (int i = 0; i < disableOnDeath.Length; i++) {
            disableOnDeath[i].enabled = false;
        }
        //activates objects whe die
        for (int i = 0; i < enableOnDeath.Length; i++) {
            enableOnDeath[i].enabled = true;
        }
        if (GameManager.instance.mainCamera != null)
            GameManager.instance.mainCamera.gameObject.SetActive(true);

        Collider _col = GetComponent<Collider>();
        if (_col != null)
            _col.enabled = false;

        Debug.Log(transform.name + " is Dead");

        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn() {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime);

        SetDefaults();
        Transform _spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = _spawnPoint.position;
        transform.rotation = _spawnPoint.rotation;
    }

    public void SetDefaults() {
        isDead = false;
        currentHealth = maxHealth;

        for (int i = 0; i < disableOnDeath.Length; i++) {
            disableOnDeath[i].enabled = wasEnabled[i];
        }
        for (int i = 0; i < enableOnDeath.Length; i++) {
            enableOnDeath[i].enabled = wasDisabled[i];
        }
        if (GameManager.instance.mainCamera != null)
            GameManager.instance.mainCamera.gameObject.SetActive(false);

        Collider _col = GetComponent<Collider>();
        if (_col != null)
            _col.enabled = true;
    }

}
