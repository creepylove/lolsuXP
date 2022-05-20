using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Newtonsoft.Json.Linq;

public class DataManager : MonoBehaviour {

    public static JObject json;
    public static string currentLang = "de";

    void Start() {
        LoadData();
        StartCoroutine(AutoSave());
    }

    #region Localization
    public void ChangeLanguage(string lang) {
        if (currentLang == lang) return;
        currentLang = lang;
        SaveData();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadLanguage() {
        // Load Language Files
        Debug.Log("lang: " + currentLang);
        TextAsset file = Resources.Load<TextAsset>("Lang/lang_" + currentLang);
        json = JObject.Parse(file.text);
    }

    public static string UidToPath(string uid) {
        return string.Join(".", Regex.Split(uid, @"(?<!^)(?=[A-Z])"));
    }

    public static string GetStringLocalized(string path) {
        return json.SelectToken("$." + path).ToString();
    }

    public static string[] GetStringsLocalized(string path) {
        return json.SelectToken("$." + path).Select(s => (string)s).ToArray();
    }
    #endregion

    #region Save, Load & Reset
    public IEnumerator AutoSave() {
        while (true) {
            yield return new WaitForSecondsRealtime(60f);
            SaveData("automatic");
        }
    }

    public void SaveData(string mode = "manual") {
        string prodList = "";
        foreach (Producer p in PointManager.producers.Keys) {
            prodList += p.uid + ":" + PointManager.producers[p] + "!";
        }

        string upList = "";
        foreach (Upgrade u in PointManager.upgrades) {
            upList += u.uid + "!";
        }

        // Gameplay stuff
        PlayerPrefs.SetString("producers", prodList);
        PlayerPrefs.SetString("upgrades", upList);
        PlayerPrefs.SetString("points", PointManager.points.ToString("R"));

        // Settings
        PlayerPrefs.SetFloat("volume", SoundManager.singleton.isMuted ? 0f : SoundManager.singleton.volume);
        PlayerPrefs.SetString("lang", currentLang);

        PlayerPrefs.Save();

        SoundManager.singleton.PlayAudio(SoundManager.Clip.CONFIRM);

        if (mode == "manual") NotificationManager.singleton.Notify(GetStringLocalized("Ui.Data.SaveManual"));
        else NotificationManager.singleton.Notify(GetStringLocalized("Ui.Data.SaveAutomatic"));
    }

    public void LoadData() {
        // Make dictionaries for converting the uids to the actual classes
        Dictionary<string, Producer> uidToProd = new Dictionary<string, Producer>();
        Dictionary<string, Upgrade> uidToUp = new Dictionary<string, Upgrade>();
        foreach (Producer p in ShopManager.singleton.producers) { uidToProd.Add(p.uid, p); }
        foreach (Upgrade u in ShopManager.singleton.upgrades) { uidToUp.Add(u.uid, u); }

        // Get lists, split by the character, convert the uids back into classes and put them into the real lists after clearing them
        PointManager.producers.Clear(); PointManager.upgrades.Clear();
        string[] prodList = PlayerPrefs.GetString("producers").Split('!');
        string[] upList = PlayerPrefs.GetString("upgrades").Split('!');
        foreach (string s in prodList) { try { PointManager.producers.Add(uidToProd[s.Split(':')[0]], int.Parse(s.Split(':')[1])); } catch (Exception) { continue; } }
        foreach (string s in upList) { try { PointManager.upgrades.Add(uidToUp[s]); } catch (Exception) { continue; } }

        // Get points and parse them and now we got our points again yay!
        PointManager.points = double.Parse(PlayerPrefs.GetString("points", 0d.ToString("R")));

        PointManager.singleton.UpdateProductionRate();

        // Load Settings
        float volume = PlayerPrefs.GetFloat("volume", 0.5f);
        SoundManager.singleton.UpdateVolume(volume);
        SoundManager.singleton.slider.value = volume;
        currentLang = PlayerPrefs.GetString("lang", "de");

        LoadLanguage();

        NotificationManager.singleton.Notify(GetStringLocalized("Ui.Data.Loaded"));
        ShopManager.singleton.UpdateInfoBox(null, PointerEvent.EXIT);
    }

    public void ResetData(bool confirmed) {

        if (!confirmed) {
            StartCoroutine(ResetDataConfirm());
        } else {
            PlayerPrefs.SetString("producers", "");
            PlayerPrefs.SetString("upgrades", "");
            PlayerPrefs.SetString("points", 0d.ToString("R"));
            PlayerPrefs.SetFloat("volume", 0.5f);
            PlayerPrefs.Save();

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

    }

    public IEnumerator ResetDataConfirm() {
        NotificationManager.singleton.Notify(GetStringLocalized("Ui.Data.ResetConfirm"), NotificationColor.RED);
        GetComponentsInChildren<Button>()[1].onClick.RemoveAllListeners();
        GetComponentsInChildren<Button>()[1].onClick.AddListener(() => ResetData(true));
        yield return new WaitForSecondsRealtime(10f);
        GetComponentsInChildren<Button>()[1].onClick.RemoveAllListeners();
        GetComponentsInChildren<Button>()[1].onClick.AddListener(() => ResetData(false));
    }
    #endregion

}