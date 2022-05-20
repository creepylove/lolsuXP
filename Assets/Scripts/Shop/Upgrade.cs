using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade", menuName = "Upgrade", order = 0)]
public class Upgrade : Buyable {

    public UpgradeType upgradeType;
    public Producer affectedProducer;

    public float multiplier = 1;
    public float multiplierAdditive = 0;

}

public enum UpgradeType {
    CLICKS, PRODUCER, TOTAL
}