using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PcController : MonoBehaviour
{
    private PlayerController player;
    private Animator anim;
    private int state = 0;
    private float prog = 0;
    private Transform sounds;
    // Start is called before the first frame update
    [SerializeField]
    private float Speed = 1f;
    public float Prog{
        get => prog;
    }
    void Start()
    {
        anim = GetComponent<Animator>();
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        sounds = GameObject.Find("Sounds").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (state == 1){
            prog += Time.deltaTime*Speed;
            if (prog >= 100){
                prog = 100;
                player.AddScore();
                player.StopHack();
                state = 3;
                anim.SetInteger("State", 2);
                PlaySound("Complete_01");
            }
            transform.Find("ProgBar").Find("Hook").Find("Progress").localScale = new Vector3(((float)(int)((prog/100)*14))/14,1,0);
        }
    }

    private void PlaySound(string soundName){
        sounds.Find("Interact").Find(soundName).GetComponent<AudioSource>().Play();
    }

    private void Use(){
        if(state == 0 || state == 2){
            anim.SetInteger("State", 1);
            state = 1;
            player.StartHack();
        }
        else if(state == 3){
            PlaySound("Denied_02");
        }
    }

    public void Stop(){
        if(prog < 100){
            state = 2;
        }
    }
}
