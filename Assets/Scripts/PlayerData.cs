using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDataInstance", menuName = "Player Data")]
public class PlayerData : ScriptableObject
{
    [SerializeField]
    private string playerName;

    [SerializeField]
    private int level;
    [SerializeField]

    private int file;
    [SerializeField]

    private int saveVersion;
    [SerializeField]

    private int[] score;

    public string Name
    {
        get => playerName;
        set => playerName = value;
    }

    public int Level
    {
        get => level;
        set => level = Mathf.Max(value, level);
    }

    public int File
    {
        get => file;
    }
    public int Score{
        get => SumScore();
    }

    private const string PlayerDataKey = "PlayerData";

    public void LoadData(int fileNum){
        if (PlayerPrefs.HasKey(PlayerDataKey + fileNum))
        {
            string jsonData = PlayerPrefs.GetString(PlayerDataKey + fileNum);
            JsonUtility.FromJsonOverwrite(jsonData, this);
            file = fileNum;
            int latestVersion = 2;
            if (saveVersion < latestVersion)
            {
                MigrateData(saveVersion, latestVersion);
            }
        }
        else
        {
            playerName = "Player "+(fileNum+1);
            level = 0;
            score = new int[3];
            file = fileNum;
            saveVersion = 2;
        }
    }
    public void SetScore(int level, int points){
        Debug.Log(points);
        Debug.Log("player "+file+" - "+" score before "+score[level]);
        score[level] = Mathf.Max(score[level],points);
        Debug.Log("player "+file+" - "+" score after  "+score[level]);
    }
    public int SumScore(){
        int sum = 0;
        foreach(int i in score){
            sum += i;
        }
        return sum;
    }

    public int GetScore(int num){
        return score[num];
    }

    public void SaveData(){
        string jsonData = JsonUtility.ToJson(this);
        PlayerPrefs.SetString(PlayerDataKey + file, jsonData);
        PlayerPrefs.Save();
        Debug.Log("saved");
    }

    public void DeleteData(int fileNum){
        PlayerPrefs.DeleteKey(PlayerDataKey + fileNum);
    }

    private void MigrateData(int currentVersion, int targetVersion){
        while (currentVersion < targetVersion)
        {
            switch (currentVersion)
            {
                case 1:
                    MigrateVersionTo2();
                    break;
                default:
                    Debug.LogError($"Unhandled migration path from version {currentVersion}");
                    return;
            }
            currentVersion = saveVersion;
        }
    }

    private void MigrateVersionTo2(){
        score = new int[3];
        saveVersion = 2;
    }
}