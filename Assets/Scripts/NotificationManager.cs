using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationManager : MonoBehaviour {

    public static NotificationManager singleton;

    public GameObject notificationPrefab;

    void Awake() {
        singleton = this;
    }

    public void Notify(string message, string color = NotificationColor.WHITE) {
        StartCoroutine(DisplayMessage("<color=" + color + ">" + message + "</color>"));
    }

    public IEnumerator DisplayMessage(string message) {
        GameObject notification = Instantiate(notificationPrefab, transform);
        notification.GetComponentInChildren<Text>().text = message;
        yield return new WaitForSecondsRealtime(10f);
        Destroy(notification);
    }

}

// Enum with string values but not an enum because c# enums suck ass
public static class NotificationColor {
    public const string WHITE = "white", RED = "red", GREEN = "green", BLUE = "blue";
}