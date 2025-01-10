using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ParentHouse.Utils {
    public class SelfDestruct : MonoBehaviour {
        [SerializeField] [BoxGroup("Settings")]
        private float SelfDestructTime = 20f;
        [SerializeField] [BoxGroup("Settings")]
        private bool DestroyOnDestruct = false;

        private void OnEnable() {
            StopAllCoroutines();
            StartCoroutine(DestructSelf());
        }

        private IEnumerator DestructSelf() {
            yield return new WaitForSeconds(SelfDestructTime);
            if (DestroyOnDestruct)
                Destroy(gameObject);
            else gameObject.SetActive(false);
        }
    }
}