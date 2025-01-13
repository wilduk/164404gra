using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveFile : MonoBehaviour
{
    [SerializeField]
    private Sprite[] unlocked;
    [SerializeField]
    private Sprite[] locked;
    public int fileNum;
    public void InsertData(string name, int progress, int score){
        for(int i = 1; i<=3 ; i++){
            if(progress >= i){
                transform.Find("Image").Find(i.ToString()).GetComponent<Image>().sprite = unlocked[i-1];
            }
            else{
                transform.Find("Image").Find(i.ToString()).GetComponent<Image>().sprite = locked[i-1];
            }
        }
        if(score >= 15){
            transform.Find("Image").GetComponent<Image>().color = new Color(137f/255,255f/255,106f/255);
        }
        else{
            transform.Find("Image").GetComponent<Image>().color = new Color(105f/255,255f/255,246f/255);
        }
        transform.Find("Play").Find("Text").GetComponent<TextMeshProUGUI>().text = name;
        transform.Find("Image").Find("Score").GetComponent<TextMeshProUGUI>().text = "Score: "+score;
    }
}
