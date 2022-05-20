using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopHoverable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public Buyable buyable;

    public GameObject cover;

    public double currentCost;

    public virtual void Start() {
        currentCost = buyable.cost;
    }

    public void Update() {
        if (PointManager.points >= currentCost) {
            if (cover.activeInHierarchy) {
                cover.SetActive(false);
                Enable();
            }
        } else {
            if (!cover.activeInHierarchy) {
                cover.SetActive(true);
                Disable();
            }
        }
    }

    protected virtual void Enable() {

    }
    protected virtual void Disable() {

    }

    public virtual void OnPointerEnter(PointerEventData data) {
        ShopManager.singleton.UpdateInfoBox(gameObject, PointerEvent.ENTER);
    }

    public virtual void OnPointerExit(PointerEventData data) {
        ShopManager.singleton.UpdateInfoBox(gameObject, PointerEvent.EXIT);
    }

}

public enum PointerEvent {
    ENTER, EXIT
}