using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MMBGManager : MonoBehaviour {

    public int screenWidth = 9, screenHeight = 5, maxPoints = 200;
    public float moveAmount = 1f;

    // Custom floating point simulation
    public static Dictionary<int, Vector2> pointLocs = new Dictionary<int, Vector2>();
    public static Dictionary<int, Vector2> pointDirs = new Dictionary<int, Vector2>();

    // List of lines drawn
    public static List<DictionaryEntry> linedParts = new List<DictionaryEntry>();

    public GameObject LRPrefab;

    void Start() {
        for (int i = 0; i < maxPoints; i++) {
            SpawnPoint(i);
        }
    }

    void FixedUpdate() {
        // Simulate and destroy/spawn points
        for (int i = 0; i < maxPoints; i++) {
            if (pointLocs[i].x >= screenWidth || pointLocs[i].x <= -screenWidth || pointLocs[i].y >= screenHeight || pointLocs[i].y <= -screenHeight) {
                pointLocs.Remove(i);
                pointDirs.Remove(i);
                SpawnPoint(i);
            }

            pointLocs[i] += pointDirs[i] * moveAmount;
        }
        pointLocs.Remove(maxPoints);
        pointLocs.Add(maxPoints, Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()));
    }

    void SpawnPoint(int i) {
        pointLocs.Add(i, new Vector2(Random.Range(-screenWidth * 0.75f, screenWidth * 0.75f), Random.Range(-screenHeight * 0.75f, screenHeight * 0.75f)));
        pointDirs.Add(i, new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)));
    }

    void LateUpdate() {
        for (int i = 0; i <= maxPoints; i++)
            for (int j = 0; j <= maxPoints; j++)
                if (i != j && Vector2.Distance(pointLocs[i], pointLocs[j]) <= 2)
                    if (!linedParts.Contains(new DictionaryEntry(i, j))) {
                        MMBGElement element = Instantiate(LRPrefab, transform).GetComponent<MMBGElement>();
                        element.p1 = i;
                        element.p2 = j;
                        linedParts.Add(new DictionaryEntry(i, j));
                    }
    }

}