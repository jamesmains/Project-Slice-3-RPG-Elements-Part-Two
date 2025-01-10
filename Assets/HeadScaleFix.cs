using Sirenix.OdinInspector;
using UnityEngine;

public class HeadScaleFix : MonoBehaviour {
    [SerializeField, BoxGroup("Settings")] 
    private Vector3 TargetScale = new Vector3(0.75f, 0.75f, 0.75f);
    void Awake()
    {
        transform.localScale = TargetScale;
    }
}
