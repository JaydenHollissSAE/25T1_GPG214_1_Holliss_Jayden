using Gamekit2D;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class SaveFile : MonoBehaviour
{
    public static SaveFile saveManager;
    public List<string> saveFiles = new List<string>();
    private bool savesLoaded = false;


    private void Awake()
    {
        if (saveManager == null)
        {
            saveManager = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine(FetchSaves());
    }
    IEnumerator FetchSaves()
    {
        for (int i = 0; i < 4; i++)
        {
            string savePath = Path.Combine(Application.persistentDataPath, "gameSave" + i.ToString() + ".json");
            if (!File.Exists(savePath))
            {
                SaveToFile(i, "{}");
            }
            if (!saveFiles.Contains(savePath))
            {
                if (saveFiles.Count < i)
                {
                    saveFiles.Add(savePath);
                }
                else
                {
                    string[] lines = File.ReadAllLines(savePath);
                    string jsonText = "";
                    foreach (string line in lines)
                    {
                        jsonText += line;
                    }
                    saveFiles[i] = jsonText;
                }
            }

        }
        savesLoaded = true;
        yield return null;

    }

    public void StartSave(GameObject checkpoint, bool hasWeapon, int health, int saveSlot, string scene)
    {
        if (saveSlot == null)
        {
            saveSlot = 0;
        }
        JsonDataStorage saveToJson = new JsonDataStorage();
        saveToJson.checkpoint = checkpoint;
        saveToJson.hasWeapon = hasWeapon;
        saveToJson.health = health;
        saveToJson.scene = scene;
        string jsonData = JsonUtility.ToJson(saveToJson);
        SaveToFile(saveSlot, jsonData);
    }


    public void SaveToFile(int saveSlot, string data)
    {
        if (savesLoaded)
        {
            string savePath = Path.Combine(Application.persistentDataPath, "gameSave" + saveSlot.ToString() + ".json");
            if (!File.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            File.WriteAllText(savePath, data);
            saveFiles[saveSlot] = data;
        }
        else
        {
            Debug.LogWarning("Saves are still being fetched");
            SaveToFile(saveSlot, data);
        }
    }

    public void LoadSave(int saveSlot)
    {
        if (savesLoaded)
        {
            string data = saveFiles[saveSlot];
            if (data != "{}")
            {
                JsonDataStorage jsonData = new JsonDataStorage();
                jsonData = JsonUtility.FromJson<JsonDataStorage>(data);
                SceneManager.LoadScene(jsonData.scene);
                PlayerCharacter player = FindFirstObjectByType<PlayerCharacter>();
                player.SetChekpoint(jsonData.checkpoint.GetComponent<Checkpoint>());
                Damager damager = player.gameObject.GetComponent<Damager>();
                if (jsonData.hasWeapon)
                {
                    damager.EnableDamage();
                }
                else
                {
                    damager.DisableDamage();
                }
                Damageable damageable = player.gameObject.GetComponent<Damageable>();
                damageable.SetHealth(jsonData.health);
            }
            else
            {
                SceneManager.LoadScene("Zone1");
            }
        }
        else
        {
            Debug.LogWarning("Saves are still being fetched");
            LoadSave(saveSlot);
        }
    }

}

[Serializable]
public class JsonDataStorage
{
    public GameObject checkpoint;
    public bool hasWeapon;
    public int health;
    public string scene;
}
