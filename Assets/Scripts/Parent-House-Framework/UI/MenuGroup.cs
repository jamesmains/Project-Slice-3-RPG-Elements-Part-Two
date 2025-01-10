using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ParentHouse.UI {
    /// <summary>
    /// Bugs list:
    /// Randomly a menu while set their initial state out of sync with the menu group.
    /// </summary>
    public class MenuGroup : MonoBehaviour {
        [SerializeField] [BoxGroup("Settings")]
        private string GroupName;

        [SerializeField] [BoxGroup("Settings")]
        private MenuState InitialState = MenuState.Closed;

        [SerializeField] [BoxGroup("Settings")]
        private bool AutoSetName = true;

        [SerializeField] [BoxGroup("Settings")]
        private float StateChangeDelay;
        
        [SerializeField] [FoldoutGroup("Dependencies")]
        private List<Menu> MenusInGroup;

        [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
        private List<Menu> CachedOpenMenus;

        [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
        private List<Menu> CachedClosedMenus;

        // Auto Set Hierarchy GameObject Name
#if UNITY_EDITOR
        private void OnValidate() {
            if (!AutoSetName) return;
            this.gameObject.name = $"{GroupName}_Menu Group";
        }
#endif

        private void Start() {
            if (InitialState == MenuState.Closed)
                foreach (var menu in MenusInGroup) {
                    menu.Close();
                }
            else
                foreach (var menu in MenusInGroup) {
                    menu.Open();
                }
        }

        /// <summary>
        /// This is for caching currently opened menus.
        /// The use case for this is if the player has menus open, but you need to focus
        /// on say a text box from a sign. You can call this, focus on the sign, and then
        /// when you exit the sign use LoadCachedMenus() to reopen the menus the player had
        /// open.
        /// </summary>
        public void CacheMenuStates() {
            CachedOpenMenus.Clear();
            CachedClosedMenus.Clear();
            foreach (var menu in MenusInGroup) {
                if (menu.State == MenuState.Open)
                    CachedOpenMenus.Add(menu);
                else CachedClosedMenus.Add(menu);
            }
        }

        /// <summary>
        /// Used in conjunction with CacheMenuStates() to reload whatever menus that should
        /// be open after and event or transition or whatever.
        /// </summary>
        public void LoadCachedMenusStates() {
            foreach (var menu in CachedOpenMenus) {
                menu.Open();
            }

            foreach (var menu in CachedClosedMenus) {
                menu.Close();
            }
        }

        [Button]
        public void OpenAll() {
            foreach (var menu in MenusInGroup) {
                menu.Open();
            }
        }

        public void OpenTargetMenu(Menu targetMenu, bool isExclusive = false) {
            foreach (var menu in MenusInGroup.Where(menu => menu == targetMenu)) {
                menu.Open();
            }
        }

        public void OpenExclusive(Menu targetMenu) {
            foreach (var menu in MenusInGroup) {
                if (menu == targetMenu)
                    menu.Open();
                else menu.Close();
            }
        }

        public void Toggle() {
            foreach (var menu in MenusInGroup) {
                menu.Toggle();
            }
        }

        public void ToggleExclusive(Menu targetMenu) {
            foreach (var menu in MenusInGroup) {
                if (menu == targetMenu)
                    menu.Toggle();
                else menu.Close();
            }
        }

        [Button]
        public void CloseAll() {
            foreach (var menu in MenusInGroup) {
                menu.Close();
            }
        }

        public void CloseAllReverse() {
            var group= MenusInGroup;
            group.Reverse();
            foreach (var menu in group) {
                menu.Close();
            }
        }

        public void CloseTargetMenu(Menu targetMenu, bool isExclusive = false) {
            foreach (var menu in MenusInGroup.Where(menu => menu == targetMenu)) {
                menu.Close();
            }
        }
    }
}