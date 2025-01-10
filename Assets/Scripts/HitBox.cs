using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    [SerializeField] [FoldoutGroup("Dependencies")] 
    private Entity ParentEntity;
    
    [SerializeField] [FoldoutGroup("Dependencies")]
    private Collider2D DamageCollider;

    private void Awake() {
        DamageCollider.enabled = false;
    }

    public void FlashHitBox(float duration = 0.1f, float delay = 0.15f) {
        StartCoroutine(Flash());
        IEnumerator Flash() {
            yield return new WaitForSeconds(delay);
            DamageCollider.enabled = true;
            yield return new WaitForSeconds(duration);
            DamageCollider.enabled = false;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.TryGetComponent(out Entity t)) {
            t.TakeDamage(ParentEntity.HitEntity(t));
        }
    }
}
