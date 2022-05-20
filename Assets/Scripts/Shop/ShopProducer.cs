using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopProducer : ShopHoverable {

    private Text[] texts;

    public override void Start() {
        texts = GetComponentsInChildren<Text>();
        if (PointManager.producers.ContainsKey((Producer)buyable)) {
            currentCost = buyable.cost * Math.Pow(((Producer)buyable).costMultiplier, PointManager.producers[(Producer)buyable]);
            texts[2].text = PointManager.producers[(Producer)buyable].ToString();
        } else {
            currentCost = buyable.cost;
        }

        UpdateInfoText(false);
    }

    protected override void Enable() {
        UpdateInfoText(true);
    }

    protected override void Disable() {
        UpdateInfoText(false);
    }

    public void UpdateInfoText() {
        UpdateInfoText(PointManager.points >= currentCost);
    }

    public void UpdateInfoText(bool sufficientFunds) {
        texts[1].text = String.Format(
            DataManager.GetStringLocalized("Ui.Shop.PriceProd"),
            sufficientFunds ? "lime" : "red",
            PointManager.singleton.ToString(currentCost),
            PointManager.singleton.ToString(((Producer)buyable).pointsPerSecond).Replace("Lolsus", "Lps")
        );
    }

}