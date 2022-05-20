using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buyable : ScriptableObject {

    public string uid;

    public new string name;
    [TextArea(2, 4)]
    public string desc;
    public Sprite sprite;

    public double cost;

}