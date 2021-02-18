using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerWeapon {

    public string name = "Arrow";

    public int damage = 10;
    public float reloadTime = 2f;
    public float maxRange = 200f;
    public float initRange = 100f;
    public float addRange = 10f;
    public float curRange = 0f;

}
