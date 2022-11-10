using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class Spawner : MonoBehaviour
{
    private ObjectPool<GameObject> _pool = new ObjectPool<GameObject>(createFunc: () => new GameObject("PooledObject"), actionOnGet: (obj) => obj.SetActive(true), actionOnRelease: (obj) => obj.SetActive(false), actionOnDestroy: (obj) => Destroy(obj), collectionCheck: false, defaultCapacity: 10, maxSize: 10);
    private ObjectPool<GameObject> pool = new ObjectPool<GameObject>(OnCreateSound, OnTakeSound, OnReleaseSound, OnDestroySound);

    private static GameObject OnCreateSound()
    {
        GameObject soundGameObject = new GameObject("Audio");
        soundGameObject.AddComponent<AudioSource>();

        return soundGameObject;
    }

    private static void OnTakeSound(GameObject obj)
    {
        obj.SetActive(true);
        Debug.Log("Taken");
    }

    private static void OnReleaseSound(GameObject obj)
    {
        obj.SetActive(false);
        Debug.Log("Released");
    }

    private static void OnDestroySound(GameObject obj)
    {
        Destroy(obj);
    }

    private void Start()
    {
        GameObject soundObject1 = pool.Get();
        GameObject soundObject2 = pool.Get();
        GameObject soundObject3 = pool.Get();
        GameObject soundObject4 = pool.Get();
        GameObject soundObject5 = pool.Get();
        soundObject1.transform.SetParent(transform);
        soundObject2.transform.SetParent(transform);
        soundObject3.transform.SetParent(transform);
        soundObject4.transform.SetParent(transform);
        soundObject5.transform.SetParent(transform);
        pool.Release(soundObject5);
    }
}
