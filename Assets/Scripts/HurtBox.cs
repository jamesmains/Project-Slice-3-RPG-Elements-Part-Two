using Sirenix.OdinInspector;
using UnityEngine;

public class HurtBox : MonoBehaviour
{
    [SerializeField] [BoxGroup("Dependencies")] 
    private Entity ParentEntity;
}
