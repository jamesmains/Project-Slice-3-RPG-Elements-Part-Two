using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class Debugger : MonoBehaviour {
    [SerializeField, BoxGroup("Dependencies")]
    private TextMeshProUGUI DebugText;
    
    [SerializeField]
    private List<DebugMsgGroup> MessageGroups = new();

    public static Debugger Singleton;

    private void Awake() {
        Singleton = this;
    }

    void Update() {
        DebugText.text = "";
        foreach (var group in MessageGroups.ToList()) {
            DebugText.text += group.tag + "\n";
            DebugText.text += group.GetFormattedText();
        }
        
        MessageGroups.Clear();
    }

    public static void AddField(string msg, string tag = "default") {
        foreach (var group in Singleton.MessageGroups.ToList()) {
            if (group.tag == tag) {
                group.msgs.Add(msg);
                return;
            }
        }
        Singleton.MessageGroups.Add(new DebugMsgGroup(tag, msg));
    }
}

[Serializable]
public class DebugMsgGroup {
    public DebugMsgGroup(string tag, string initialMessage = "") {
        this.tag = tag;
        if(!string.IsNullOrEmpty(initialMessage))
            msgs.Add(initialMessage);
    }
    public string tag;
    public List<string> msgs = new();

    public string GetFormattedText() {
        var msg = "";
        foreach (var m in msgs) {
            msg += m + "\n";
        }

        return msg;
    }
}
