using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Buttons : MonoBehaviour
{
    [SerializeField]
    private GameObject[] menus;
    [SerializeField]
    private PlayerData playerData;
    [SerializeField]
    private SceneData sceneData;
    [SerializeField]
    private SettingsData settings;
    [SerializeField]
    private GameObject settingsCanvas;
    [SerializeField]
    private Transform sounds;
    [SerializeField]
    private GameObject[] saves;
    [SerializeField]
    private GameObject[] levels;
    [SerializeField]
    private Transform resultsScreen;
    private Color green = new Color(137f/255,255f/255,106f/255);
    private string[] tips = {
        "Hide by crouching behind lower objects that cast shadow",
        "Standing guard faces direction he will go",
        "Guards can only see what their flashlight shines on",
        "Cameras will alert guards",
        "Holograms will lure guards away",
        "Cameras can see holograms, use it for your advantage"
    };

    void Start()
    {
        int lastPlayer = -1;
        if(sceneData.state == 1){
            lastPlayer = sceneData.playerFile;
            gameObject.SetActive(false);
            resultsScreen.gameObject.SetActive(true);
            sceneData.state = 0;
            resultsScreen.Find("Retry").gameObject.SetActive(false);
            if(sceneData.score >= 5){
                resultsScreen.Find("Image").GetComponent<Image>().color = green;
            }
            resultsScreen.Find("Image").Find("Result").GetComponent<TextMeshProUGUI>().text = "YOU WON";
            resultsScreen.Find("Image").Find("Score").GetComponent<TextMeshProUGUI>().text = "Score: "+sceneData.score;
            playerData.LoadData(sceneData.playerFile);
            playerData.Level = sceneData.level;
            playerData.SetScore(sceneData.level-1,sceneData.score);
            playerData.SaveData();
        }
        else if(sceneData.state == 2){
            lastPlayer = sceneData.playerFile;
            sceneData.state = 0;
            if(settings.AutoReplay){
                PlayAgain();
            }
            gameObject.SetActive(false);
            resultsScreen.gameObject.SetActive(true);
            resultsScreen.Find("NextLevel").gameObject.SetActive(false);
            resultsScreen.Find("Image").GetComponent<Image>().color = new Color(1,38f/255,48f/255);
            resultsScreen.Find("Image").Find("Result").GetComponent<TextMeshProUGUI>().text = "YOU LOST";
            string tip = tips[Random.Range(0,tips.Length)];
            resultsScreen.Find("Image").Find("Score").GetComponent<TextMeshProUGUI>().text = "Tip: "+tip;
        }
        ReloadData();
        LoadSettings();
        if(lastPlayer >= 0){
            playerData.LoadData(lastPlayer);
        }
    }

    public void OpenMenu(GameObject menu){
        foreach(GameObject close in menus){
            close.SetActive(false);
        }
        menu.SetActive(true);
    }

    private void ReloadData(){
        foreach(GameObject save in saves){
            int saveNum = save.GetComponent<SaveFile>().fileNum;
            playerData.LoadData(saveNum);
            save.GetComponent<SaveFile>().InsertData(playerData.Name, playerData.Level, playerData.Score);
        }
    }

    public void SetPlayerSceneData(int num){
        sceneData.playerFile = num;
    }

    private void SaveSettings(){
        settings.SaveData();
    }

    private void LoadSettings(){
        settings.LoadData();
        SetSetting("Master", "Slider", settings.MasterVolume);
        SetSetting("Music", "Slider", settings.MusicVolume);
        SetSetting("Interact", "Slider", settings.InteractVolume);
        SetSetting("Replay", "Toggle", settings.AutoReplay);
        ApplyVolume();
        //to jest głupie ale unity nie lubi ref ¯\_(ツ)_/¯
        Slider slider = settingsCanvas.transform.Find("Master").Find("Slider").GetComponent<Slider>();
        slider.onValueChanged.AddListener(value =>{
            settings.MasterVolume = (int)value;
            ApplyVolume();
            SaveSettings();
        });
        slider = settingsCanvas.transform.Find("Music").Find("Slider").GetComponent<Slider>();
        slider.onValueChanged.AddListener(value =>{
            settings.MusicVolume = (int)value;
            ApplyVolume();
            SaveSettings();
        });
        slider = settingsCanvas.transform.Find("Interact").Find("Slider").GetComponent<Slider>();
        slider.onValueChanged.AddListener(value =>{
            settings.InteractVolume = (int)value;
            ApplyVolume();
            SaveSettings();
        });
        Toggle toggle = settingsCanvas.transform.Find("Replay").Find("Toggle").GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(value =>{
            settings.AutoReplay = value;
            SaveSettings();
        });
    }

    public void OpenUrl(string URL){
        Application.OpenURL(URL);
    }

    public void PlaySound(AudioSource audio){
        audio.Play();
    }

    public void PlayLevel(string levelName){
        SceneManager.LoadScene(levelName, LoadSceneMode.Single);
    }

    public void PlayNext(){
        PlayLevel("Map"+sceneData.level);
    }
    public void PlayAgain(){
        Debug.Log("Map"+(sceneData.level-1));
        PlayLevel("Map"+(sceneData.level-1));
    }

    public void LoadData(int num){
        playerData.LoadData(num);
        for(int i = 0; i < levels.Length;i++){
            if(playerData.Level >= i){
                levels[i].transform.Find("PlayDisabled").gameObject.SetActive(false);
                levels[i].transform.Find("Play").gameObject.SetActive(true);
                int score = playerData.GetScore(i);
                if(score == 5){
                    levels[i].transform.Find("Image").GetComponent<Image>().color = green;
                }
                else{
                    levels[i].transform.Find("Image").GetComponent<Image>().color = new Color(1,1,1);
                }
                levels[i].transform.Find("Image").Find("Score").gameObject.SetActive(true);
                levels[i].transform.Find("Image").Find("Score").GetComponent<TextMeshProUGUI>().text = "Score: "+score;
                levels[i].transform.Find("Image").Find("Thumbnail").gameObject.GetComponent<Image>().color = new Color(1,1,1);
            }
            else{
                levels[i].transform.Find("PlayDisabled").gameObject.SetActive(true);
                levels[i].transform.Find("Play").gameObject.SetActive(false);
                levels[i].transform.Find("Image").GetComponent<Image>().color = new Color(0.5f,0.5f,0.5f);
                levels[i].transform.Find("Image").Find("Score").gameObject.SetActive(false);
                levels[i].transform.Find("Image").Find("Thumbnail").GetComponent<Image>().color = new Color(0.5f,0.5f,0.5f);
            }
        }
    }

    public void SaveData(){
        playerData.SaveData();
        ReloadData();
    }

    public void CloseGame()
    {
        Application.Quit();
    }

    public void DeleteFile(int num){
        playerData.DeleteData(num);
        ReloadData();
    }

    private void ApplyVolume(){
        float interact = ((float)settings.MasterVolume/100)*((float)settings.InteractVolume/100);
        float music = ((float)settings.MasterVolume/100)*((float)settings.MusicVolume/100);
        foreach(Transform sound in sounds.Find("Interact")){
            sound.GetComponent<AudioSource>().volume = interact;
        }
        foreach(Transform sound in sounds.Find("Music")){
            sound.GetComponent<AudioSource>().volume = music;
        }
    }

    private void SetSetting<T>(string name, string where, T value){
        if(where == "Slider" && value is int intval){
            settingsCanvas.transform.Find(name).Find("Slider").GetComponent<Slider>().value = intval;
        }
        else if(where == "Toggle" && value is bool boolval){
            settingsCanvas.transform.Find(name).Find("Toggle").GetComponent<Toggle>().isOn = boolval;
        }
    }
}
