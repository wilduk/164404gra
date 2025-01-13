using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class PauseMenu : MonoBehaviour
{

    private bool pause = false;
    private Transform enemies;
    private PlayerController player;
    private Transform sounds;
    private GameObject menu;
    // Start is called before the first frame update
    void Start()
    {
        enemies = GameObject.Find("Enemies").transform;
        player = GameObject.Find("Player").transform.GetComponent<PlayerController>();
        sounds = GameObject.Find("Sounds").transform;
        menu = transform.Find("PauseMenu").gameObject;
        menu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnPause(){
        pause = !pause;
        foreach(Transform enemy in enemies){
            enemy.GetComponent<Patrol>().active = !pause;
            enemy.GetComponent<NavMeshAgent>().enabled = !pause;
        }
        player.canWalk = !pause;
        menu.SetActive(pause);
        if(pause){

        }
        else{
            
        }
    }

    public void Resume(){
        OnPause();
    }

    public void Restart(){
        string sceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName);
    }

    public void MainMenu(){
        SceneManager.LoadScene("Menu");
    }

    public void CloseGame()
    {
        Application.Quit();
    }

    public void PlaySound(string soundName){
        sounds.Find("Interact").Find(soundName).GetComponent<AudioSource>().Play();
    }
}
