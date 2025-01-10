using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ParentHouse.Utils {
    public class MouseOverUserInterfaceUtil : MonoBehaviour {
        public static bool IsMouseOverUI;
        [SerializeField] private LayerMask UILayer;
        private int layer;

        private void Awake() {
            layer = 5;
        }

        private void Update() {
            IsMouseOverUI = IsPointerOverUIElement();
        }


        //Returns 'true' if we touched or hovering on Unity UI element.
        public bool IsPointerOverUIElement() {
            return IsPointerOverUIElement(GetEventSystemRaycastResults());
        }


        //Returns 'true' if we touched or hovering on Unity UI element.
        private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults) {
            for (int index = 0; index < eventSystemRaysastResults.Count; index++) {
                RaycastResult curRaysastResult = eventSystemRaysastResults[index];
                if (curRaysastResult.gameObject.layer == layer) {
                    return true;
                }
            }

            return false;
        }


        //Gets all event system raycast results of current mouse or touch position.
        static List<RaycastResult> GetEventSystemRaycastResults() {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;
            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raycastResults);
            return raycastResults;
        }
    }

}