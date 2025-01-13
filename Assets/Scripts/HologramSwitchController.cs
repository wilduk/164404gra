using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HologramSwitchController : MonoBehaviour
{
    // Start is called before the first frame update
    private int state = 0;
    private Transform sounds;
    [SerializeField]
    private HologramController[] holograms;
    [SerializeField]
    private float ChargeTime = 5f;
    private float startTime;
    private Animator anim;

    void Start()
    {
        sounds = GameObject.Find("Sounds").transform;
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(state == 1 && !CheckHolograms()){
            anim.SetInteger("State", 2);
            state = 2;
            startTime = Time.time;
        }
        if(state == 2 && Time.time > startTime + ChargeTime){
            anim.SetInteger("State", 0);
            state = 0;
        }
    }

    private void Use(){
        if(state == 0){
            anim.SetInteger("State", 1);
            state = 1;
            PlaySound("Sequence_03");
            foreach(HologramController hologram in holograms){
                hologram.State = true;
            }
        }
        else if(state == 1){
            anim.SetInteger("State", 2);
            state = 2;
            PlaySound("Sequence_03");
            foreach(HologramController hologram in holograms){
                hologram.State = false;
            }
        }
        else{
            PlaySound("Denied_02");
        }
    }

    private void PlaySound(string soundName){
        sounds.Find("Interact").Find(soundName).GetComponent<AudioSource>().Play();
    }

    private bool CheckHolograms(){
        foreach(HologramController hologram in holograms){
            if(hologram.State == true){
                return true;
            }
            return false;
        }
        return false;
    }
}
