using System;
using System.Collections.Generic;
using UnityEngine;

public interface IStat {
    // List<StatDetails> StartingStats { get; set; }
    // List<Stat> CurrentStats { get; set; }
    public List<Stat> GetStats();
}
public interface IViewableEquipment {
    public GameObject EquipModel { get; set; }
    public List<Mesh> EquipMeshList { get; set; }
}