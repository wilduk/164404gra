using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class HologramController : MonoBehaviour
{
    private bool state = false;
    private Animator anim;
    private new Light2D light;
    public bool State{
        set {
            anim.SetBool("Active",value);
            state = value;
            light.intensity = value ? 1 : 0;
        }
        get => state;
    }
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        light = GetComponent<Light2D>();
        light.intensity = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D item){
        if(item.gameObject.tag == "Player" || item.gameObject.tag == "Enemy"){
            State = false;
        }
    }
}
