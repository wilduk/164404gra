using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SettingsDataInstance", menuName = "Settings Data")]
public class SettingsData : ScriptableObject
{
    [SerializeField, Range(0,100)]
    private int masterVolume = 100;
    [SerializeField, Range(0,100)]
    private int musicVolume = 50;
    [SerializeField, Range(0,100)]
    private int interactVolume = 50;
    [SerializeField]
    private bool autoReplay = false;
    private int saveVersion;

    public int MasterVolume
    {
        get => masterVolume;
        set => masterVolume = Mathf.Clamp(value, 0, 100);
    }
    public int MusicVolume
    {
        get => musicVolume;
        set => musicVolume = Mathf.Clamp(value, 0, 100);
    }
    public int InteractVolume
    {
        get => interactVolume;
        set => interactVolume = Mathf.Clamp(value, 0, 100);
    }
    public bool AutoReplay
    {
        get => autoReplay;
        set => autoReplay = value;
    }

    public void SaveData(){
        string jsonData = JsonUtility.ToJson(this);
        PlayerPrefs.SetString("Settings", jsonData);
        PlayerPrefs.Save();
    }

    public void LoadData(){
        if (PlayerPrefs.HasKey("Settings"))
        {
            string jsonData = PlayerPrefs.GetString("Settings");
            JsonUtility.FromJsonOverwrite(jsonData, this);
            int latestVersion = 1;
            if (saveVersion < latestVersion)
            {
                MigrateData(saveVersion, latestVersion);
            }
        }
        else
        {
            masterVolume = 100;
            musicVolume = 50;
            interactVolume = 50;
            autoReplay = false;
            saveVersion = 1;
        }
    }

    private void MigrateData(int currentVersion, int targetVersion){
        while (currentVersion < targetVersion)
        {
            switch (currentVersion)
            {
                case 0:
                    saveVersion = 1;
                    break;
                case 1:
                    break;
                default:
                    Debug.LogError($"Unhandled migration path from version {currentVersion}");
                    return;
            }
            currentVersion = saveVersion;
        }
    }
}
