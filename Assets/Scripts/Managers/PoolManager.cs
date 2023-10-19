using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [TableList] public List<PoolGameObject> poolList;

    public Dictionary<GameObject, List<GameObject>> pooledObjects;
    
    private static PoolManager instance;

    public static PoolManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PoolManager>();
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(PoolManager).Name);
                    instance = singletonObject.AddComponent<PoolManager>();
                }
            }
            return instance;
        }
    }

    
    public void Initialize()
    {
        if (pooledObjects != null) {
            pooledObjects.Clear();
            return;
        }
        pooledObjects = new Dictionary<GameObject, List<GameObject>>();

        Prepare();
    }

    public void InitializePool(GameObject prefab, int count, Transform group)
    {
        if (!pooledObjects.ContainsKey(prefab))
        {
            pooledObjects[prefab] = new List<GameObject>();
        }


        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(prefab, group);
            obj.SetActive(false);
            pooledObjects[prefab].Add(obj);
        }
    }

    public GameObject GetPooledObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (pooledObjects.ContainsKey(prefab))
        {
            GameObject obj = pooledObjects[prefab].Find(o => !o.activeInHierarchy);

            if (obj == null)
            {
                PoolGameObject pool = poolList.FirstOrDefault(e => e.objPool == prefab);
                obj = Instantiate(prefab, pool.group);
                pooledObjects[prefab].Add(obj);
            }

            obj.SetActive(true);
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            return obj;
        }

        return null;
    }

    public void DisableAllObjects(GameObject prefab)
    {
        if (pooledObjects.ContainsKey(prefab))
        {
            foreach (GameObject obj in pooledObjects[prefab])
            {
                obj.SetActive(false);
            }
        }
    }

    private void Prepare()
    {
        foreach (PoolGameObject poolObject in poolList)
        {
            InitializePool(poolObject.objPool, poolObject.number, poolObject.group);
        }
    }

    public void DisableAll()
    {
        foreach(PoolGameObject poolObject in poolList)
        {
            DisableAllObjects(poolObject.objPool);
        }
    }
}

[Serializable]
public class PoolGameObject
{
    public GameObject objPool;
    public int number;
    public Transform group;
}
