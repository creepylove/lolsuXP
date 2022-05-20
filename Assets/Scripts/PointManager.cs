using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler {

    public static PointManager singleton;

    [HideInInspector] public Animator anim;
    public Sprite mustache;
    public GameObject clickParticles;

    public static double points = 0;

    public static float click = 1;
    public static float totalMultiplier = 1;
    public static float totalMultiplierAdditive = 1;

    public static Dictionary<Producer, int> producers = new Dictionary<Producer, int>();
    public static List<Upgrade> upgrades = new List<Upgrade>();

    public double pointsPlus = 0;

    #region Base
    void Awake() {
        anim = GetComponent<Animator>();

        singleton = this;
    }

    void Start() {

    }

    void FixedUpdate() {
        points += pointsPlus;
    }
    #endregion

    public void UpdateProductionRate() {
        // Update per second production rate
        double plus = 0;
        foreach (Producer p in producers.Keys) {
            double plusProducer = p.pointsPerSecond * producers[p];
            float multiplier = 1;
            float multiplierAdditive = 1;
            foreach (Upgrade u in upgrades) {
                if (u.affectedProducer != p) continue;
                multiplier *= u.multiplier;
                multiplierAdditive += u.multiplierAdditive;
            }
            plus += plusProducer * multiplierAdditive * multiplier;
        }
        pointsPlus = plus * Time.fixedDeltaTime * totalMultiplier * totalMultiplierAdditive;

        // Update click and total bonusses
        click = 1;
        totalMultiplier = 1; totalMultiplierAdditive = 1;
        foreach (Upgrade u in upgrades) {
            if (u.upgradeType == UpgradeType.CLICKS) {
                click *= u.multiplier;
            } else if (u.upgradeType == UpgradeType.TOTAL) {
                totalMultiplier *= u.multiplier; totalMultiplierAdditive += u.multiplierAdditive;
            }

            if (u.uid == "UpTotalMustache") GetComponent<SpriteRenderer>().sprite = mustache;
        }
    }

    #region Clickable
    public void OnPointerEnter(PointerEventData pe) {
        anim.SetBool("hover", true);
    }
    public void OnPointerExit(PointerEventData pe) {
        anim.SetBool("hover", false);
    }
    public void OnPointerDown(PointerEventData pe) {
        points += click;

        // Sound effect
        SoundManager.singleton.PlayAudio(SoundManager.Clip.CLICK);

        // Particles and animation
        ParticleSystem ps = Instantiate(clickParticles, pe.pointerCurrentRaycast.worldPosition, Quaternion.identity, transform).GetComponent<ParticleSystem>();
        ParticleSystem.Burst burst = ps.emission.GetBurst(0);
        burst.minCount = (short)(Math.Min(Math.Log10(click), 4) + 1);
        burst.maxCount = (short)(Math.Min(Math.Log10(click), 4) + 1);
        ps.emission.SetBurst(0, burst);
        anim.SetBool("click", true);
    }
    public void OnPointerUp(PointerEventData pe) {
        anim.SetBool("click", false);
    }
    #endregion

    #region Number Converter
    public string ToString(double number) {
        if (number <= 0) return "0 Lolsus";
        int mag = (int)(Math.Floor(Math.Log10(number)) / 3); // Truncates to 6, divides to 2
        double divisor = Math.Pow(10, mag * 3);

        string[] suffix = DataManager.GetStringsLocalized("Ui.Suffixes");

        double pointsShort = number / divisor;
        if (mag >= suffix.Length)
            return (pointsShort.ToString("N2") + " " + suffix[suffix.Length - 1] + "E" + (mag - (suffix.Length - 1)) * 3 + " Lolsus").Replace(".00", "");
        else return (pointsShort.ToString("N2") + " " + suffix[mag] + " Lolsus").Replace(".00", "").Replace("  ", " ");
    }
    #endregion

}