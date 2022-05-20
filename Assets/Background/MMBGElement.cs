using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MMBGElement : MonoBehaviour {

    public LineRenderer lr;
    public int p1;
    public int p2;

    void Update() {
        if (Vector2.Distance(MMBGManager.pointLocs[p1], MMBGManager.pointLocs[p2]) <= 2f) {
            lr.SetPosition(0, MMBGManager.pointLocs[p1]);
            lr.SetPosition(1, MMBGManager.pointLocs[p2]);
        } else {
            MMBGManager.linedParts.Remove(new DictionaryEntry(p1, p2));
            Destroy(gameObject);
        }
    }
}
