using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour {

    public static ShopManager singleton;

    public GameObject upgradeEffect;
    public GameObject producerEffect;

    public Upgrade[] upgrades;
    public Producer[] producers;
    public GameObject upgradePrefab;
    public GameObject producerPrefab;
    public Transform upgradeLocation;
    public Transform producerLocation;

    public Text infoBox;

    private Dictionary<GameObject, Producer> prodObjects = new Dictionary<GameObject, Producer>();
    private Dictionary<GameObject, Upgrade> upObjects = new Dictionary<GameObject, Upgrade>();

    void Awake() {
        singleton = this;
    }

    IEnumerator Start() {
        // Waits one frame to spawn everything in to let the data finish loading
        yield return new WaitForEndOfFrame();

        GetComponentsInChildren<Text>()[0].text = DataManager.GetStringLocalized("Ui.Shop.Label");

        // Spawn producer shop objects
        foreach (Producer p in producers) {
            GameObject obj = Instantiate(producerPrefab, producerLocation);
            Text[] texts = obj.GetComponentsInChildren<Text>();
            string path = DataManager.UidToPath(p.uid);
            texts[0].text = DataManager.GetStringLocalized(path + ".Name");
            obj.GetComponentsInChildren<Image>()[1].sprite = p.sprite;
            obj.GetComponent<Button>().onClick.AddListener(() => BuyProducer(obj));
            p.desc = DataManager.GetStringLocalized(path + ".Desc");
            obj.GetComponent<ShopHoverable>().buyable = p;
            prodObjects.Add(obj, p);
        }

        // Sort upgrades after cost and spawn the upgrade shop objects
        Array.Sort(upgrades, (a, b) => a.cost.CompareTo(b.cost));
        foreach (Upgrade u in upgrades) {
            if (PointManager.upgrades.Contains(u)) continue;
            GameObject obj = Instantiate(upgradePrefab, upgradeLocation);
            obj.GetComponentsInChildren<Image>()[1].sprite = u.sprite;
            obj.GetComponent<Button>().onClick.AddListener(() => BuyUpgrade(obj));
            u.desc = DataManager.GetStringLocalized(DataManager.UidToPath(u.uid) + ".Desc");
            obj.GetComponent<ShopHoverable>().buyable = u;
            upObjects.Add(obj, u);
        }

        UpdateInfoBox(null, PointerEvent.EXIT);
    }

    public void UpdateInfoBox(GameObject obj, PointerEvent pe) {
        if (pe == PointerEvent.ENTER)
            if (prodObjects.ContainsKey(obj)) infoBox.text = prodObjects[obj].desc;
            else infoBox.text = String.Format(DataManager.GetStringLocalized("Ui.Shop.PriceUp"), upObjects[obj].desc, upObjects[obj].cost > PointManager.points ? "red" : "lime", PointManager.singleton.ToString(upObjects[obj].cost));
        else infoBox.text = PointManager.singleton.ToString(PointManager.singleton.pointsPlus / Time.fixedDeltaTime).Replace("Lolsus", "Lps") + "\nBonus: " + ((PointManager.totalMultiplierAdditive - 1) * 100).ToString("N0") + "% (x" + PointManager.totalMultiplier + ")";
    }

    public void BuyProducer(GameObject obj) {
        Producer p = prodObjects[obj];
        bool contains = PointManager.producers.ContainsKey(p);
        double cost = contains ? p.cost * Math.Pow(p.costMultiplier, PointManager.producers[p]) : p.cost;

        if (PointManager.points < cost) { SoundManager.singleton.PlayAudio(SoundManager.Clip.DENY); return; }

        if (contains) PointManager.producers[p] += 1;
        else PointManager.producers.Add(p, 1);

        PointManager.points -= cost;

        Text[] texts = obj.GetComponentsInChildren<Text>();
        ShopProducer sp = obj.GetComponent<ShopProducer>();
        sp.currentCost *= p.costMultiplier;
        sp.UpdateInfoText();
        texts[2].text = (int.Parse(texts[2].text) + 1).ToString();

        SoundManager.singleton.PlayAudio(SoundManager.Clip.PURCHASE_PROD);

        Instantiate(producerEffect, obj.transform.position, Quaternion.identity, transform);

        PointManager.singleton.UpdateProductionRate();
    }

    public void BuyUpgrade(GameObject obj) {
        Upgrade u = upObjects[obj];
        if (PointManager.upgrades.Contains(u)) { Destroy(obj); return; }
        if (PointManager.points < u.cost) { SoundManager.singleton.PlayAudio(SoundManager.Clip.DENY); return; }

        PointManager.upgrades.Add(u);
        PointManager.points -= u.cost;

        SoundManager.singleton.PlayAudio(SoundManager.Clip.PURCHASE_UP);

        Instantiate(upgradeEffect, obj.transform.position, Quaternion.identity, transform);

        Destroy(obj);

        PointManager.singleton.UpdateProductionRate();
    }

}