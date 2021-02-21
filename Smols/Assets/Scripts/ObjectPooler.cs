using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ObjectPooler : NetworkBehaviour {

    [System.Serializable]
    public class Pool {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public static ObjectPooler instance;

    private void Awake() {
        instance = this;
    }

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    private List<GameObject> parents;
    private const string PARENT_NAME = "Parent";

    //handles requests to spawn GameObjects on the client
    public delegate GameObject SpawnDelegate(Vector3 _position, System.Guid _assetId);

    //handles requests to unspawn GameObjects on the client
    public delegate void UnSpawnGameObject(GameObject _spawned);

    private void Start() {
        if (isClientOnly)
            return;
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        parents = new List<GameObject>();

        foreach (Pool pool in pools) {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            GameObject parent = new GameObject(pool.tag + " Parent");
            parents.Add(parent);
            for (int i = 0; i < pool.size; i++) {
                GameObject obj = Instantiate(pool.prefab);
                obj.transform.parent = parents[pools.IndexOf(pool)].transform;
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string _tag, Vector3 _position, Quaternion _rotation) {
        if (!poolDictionary.ContainsKey(_tag)) {
            Debug.LogError("Pool doesn't contain " + _tag);
            return null;
        }
        
        GameObject objToSpawn = poolDictionary[_tag].Dequeue();

        objToSpawn.SetActive(true);
        objToSpawn.transform.position = _position;
        objToSpawn.transform.rotation = _rotation;

        IPooledObject pooledObj = objToSpawn.GetComponent<IPooledObject>();

        if (pooledObj != null)
            pooledObj.OnObjectSpawn();

        poolDictionary[_tag].Enqueue(objToSpawn);

        return objToSpawn;
    }

    public GameObject SpawnObject(Vector3 _position, System.Guid _id) {
        return SpawnFromPool("2Arrow", _position, Quaternion.identity);
    }

    public void UnSpawnObject(GameObject _objToUnSpawn) {
        _objToUnSpawn.SetActive(false);
    }
}
