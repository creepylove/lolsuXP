using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopUpgrade : ShopHoverable {

    public Text nameText;

    public override void Start() {
        base.Start();
        nameText.text = DataManager.GetStringLocalized(DataManager.UidToPath(buyable.uid) + ".Name");
    }

    protected override void Enable() {
        nameText.gameObject.SetActive(false);
    }

    protected override void Disable() {
        nameText.gameObject.SetActive(true);
    }

    public override void OnPointerEnter(PointerEventData data) {
        base.OnPointerEnter(data);
        nameText.gameObject.SetActive(true);
    }

    public override void OnPointerExit(PointerEventData data) {
        base.OnPointerExit(data);
        if (PointManager.points >= buyable.cost)
            nameText.gameObject.SetActive(false);
    }

}