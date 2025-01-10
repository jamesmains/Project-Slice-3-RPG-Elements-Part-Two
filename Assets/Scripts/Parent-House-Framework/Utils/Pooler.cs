using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ParentHouse.Utils {
    
    public struct PoolSpawnOptions {
        public GameObject Obj;
        public bool SetPosition;
        public Vector3 Position;
        public bool UseWorldSpace;
        public Transform Parent;
    }

    public class Pooler : MonoBehaviour {
        private static Pooler Singleton;
        
        [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "pooledObject")]
        [SerializeField]
        [ReadOnly]
        private List<Pool> ActivePools;
        
        /// <summary>
        /// This PoolContentObj instantiated in Awake because without a prefab it would create two pool content objects
        /// per pool, while it doesn't affect functionality it creates ugly new empty unused objects in the scene hierarchy. This
        /// hotfix isn't ideal but prevents any dependencies to set in inspector. If you know a proper fix for this please
        /// e-mail me: support@tyler-sims.com
        /// </summary>
        private static GameObject PoolContentObj;

        private void Awake() {
            if (Singleton == null) {
                Singleton = this;
                DontDestroyOnLoad(this.gameObject);
                PoolContentObj = Instantiate(new GameObject("ContentPrefab"),this.transform);
            }
            else {
                Destroy(this.gameObject);
            }
        }

        public static GameObject Spawn(GameObject prefab, Transform parent = null, bool useWorldSpace = false) {
            PoolSpawnOptions options = new() {
                Obj = prefab,
                SetPosition = false,
                UseWorldSpace = useWorldSpace,
                Parent = parent
            };
            return Singleton.GetPooledObject(options);
        }

        public static GameObject SpawnAt(GameObject prefab, Vector3 spawnLocation, Transform parent = null,
            bool useWorldSpace = false) {
            PoolSpawnOptions options = new() {
                Obj = prefab,
                SetPosition = true,
                Position = spawnLocation,
                UseWorldSpace = useWorldSpace,
                Parent = parent
            };
            return Singleton.GetPooledObject(options);
        }

        private GameObject GetPooledObject(PoolSpawnOptions options) {
            if (options.Obj == null) {
                return null;
            }

            var pool = ActivePools.FirstOrDefault(o => o.pooledObject.name == options.Obj.name);
            pool ??= CreateNewPool(options);
            if (options.Parent == null)
                options.Parent = pool.poolContent.transform;

            var obj = pool?.spawnedObjects.FirstOrDefault(o => o.activeSelf == false);

            if (obj == null) {
                var amountToSpawn = pool?.increaseBoundsAmount;
                while (amountToSpawn > 0) {
                    var newPooledObject =
                        Instantiate(pool.GetPooledObject(), options.Parent, options.UseWorldSpace);
                    newPooledObject.name = pool.GetPooledObject().name;
                    pool.spawnedObjects.Add(newPooledObject);
                    newPooledObject.SetActive(false);
                    amountToSpawn--;
                }

                obj = pool?.spawnedObjects.FirstOrDefault(o => o.activeSelf == false);
                StartCoroutine(IncreasePoolVolume());

                IEnumerator IncreasePoolVolume() {
                    var volumeIncreaseAmount = pool?.increaseBoundsAmount;
                    while (volumeIncreaseAmount > 0) {
                        var newPooledObject =
                            Instantiate(pool.GetPooledObject(), options.Parent, options.UseWorldSpace);
                        newPooledObject.name = pool.GetPooledObject().name;
                        pool.spawnedObjects.Add(newPooledObject);
                        newPooledObject.SetActive(false);
                        volumeIncreaseAmount--;
                    }

                    obj = pool?.spawnedObjects.FirstOrDefault(o => o.activeSelf == false);
                    yield return new WaitForEndOfFrame();
                }
            }

            if (options.SetPosition)
                obj.transform.position = options.Position;
            if(options.Parent != null)
                obj.transform.parent = options.Parent;
            obj.SetActive(true);
            return obj;
        }

        private Pool CreateNewPool(PoolSpawnOptions Options) {
            var newPool = new Pool {
                pooledObject = Options.Obj,
                initialAmount = 2,
                increaseBoundsAmount = 2,
                poolContent = Instantiate(PoolContentObj, this.transform)
            };
            newPool.poolContent.gameObject.name = Options.Obj.name;
            ActivePools.Add(newPool);
            return newPool;
        }
    }

    [Serializable]
    public class Pool {
        ~Pool() {
            if(pooledObject!= null)
                pooledObject.SetActive(true);
        }
        
        public GameObject poolContent;
        public GameObject pooledObject;
        public int initialAmount;
        public int increaseBoundsAmount;
        public List<GameObject> spawnedObjects = new();

        public GameObject GetPooledObject() {
            pooledObject.SetActive(false);
            return pooledObject;
        }
    }
}