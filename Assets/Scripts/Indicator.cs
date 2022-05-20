using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Indicator : MonoBehaviour {

    [HideInInspector]
    public Text text;

    void Awake() {
        text = GetComponent<Text>();
    }

    void Update() {
        text.text = PointManager.singleton.ToString(PointManager.points);
    }

}
