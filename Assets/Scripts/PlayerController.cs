using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;
public class PlayerController : MonoBehaviour
{
    private int score = 0;
    private int goal = 3;
    [SerializeField]
    private int level;
    [SerializeField]
    private float speed = 10.0f;
    [SerializeField]
    private new GameObject camera;
    [SerializeField]
    private GameObject exit;
    [SerializeField]
    private SceneData sceneData;
    [SerializeField]
    private SettingsData settings;
    [SerializeField]
    private GameObject canvas;
    private Transform playerTr;
    private Vector2 input;
    private SpriteRenderer sprite;
    private Animator anim;
    private Transform interact;
    private Rigidbody2D rb;
    private bool crouch;
    public bool canWalk = true;
    private bool winWalk;
    private float walkI=0;
    private AudioSource keyboardSounds;
    private Transform sounds;
    public bool Crouch{
        get => crouch;
    }
    void Start()
    {
        sprite = transform.Find("Sprite").GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        rb = transform.GetComponent<Rigidbody2D>();
        sounds = GameObject.Find("Sounds").transform;
        keyboardSounds = sounds.Find("Interact").Find("Keyboard").GetComponent<AudioSource>();
        ApplyVolume();
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

    private void PlaySound(string soundName){
        sounds.Find("Interact").Find(soundName).GetComponent<AudioSource>().Play();
    }

    void OnMove(InputValue inputValue)
    {
        if (crouch || !canWalk){
            return;
        }
        StopHack();
        input = inputValue.Get<Vector2>();
        if (input.x == 0f && input.y == 0f){
            anim.SetBool("Walking", false);
            return;
        }
        if (input.x < 0)
        {
            sprite.flipX = true;
        }
        else if (input.x > 0)
        {
            sprite.flipX = false;
        }
        if (Mathf.Abs(input.x) >= Mathf.Abs(input.y)){
            anim.SetInteger("Direction", 1);
        }
        else if(input.y > 0f){
            anim.SetInteger("Direction", 2);
        }
        else{
            anim.SetInteger("Direction", 0);
        }
        anim.SetBool("Walking", true);
    }

    void OnCrouchPressed()
    {
        if(!canWalk){
            return;
        }
        input = new Vector2(0,0);
        crouch = true;
        anim.SetBool("Crouching", true);
        anim.SetBool("Walking", false);
        StopHack();
    }

    void OnCrouchRelease()
    {
        crouch = false;
        anim.SetBool("Crouching", false);
    }

    void OnInteract()
    {
        if(input != Vector2.zero || crouch){
            return;
        }
        if(interact != null){
            interact.SendMessage("Use", SendMessageOptions.DontRequireReceiver);
        }
    }

    void OnPause(){
        canvas.SendMessage("OnPause");
    }

    public void StartHack(){
        anim.SetTrigger("HackStart");
        anim.SetBool("Hacking", true);
        keyboardSounds.time = 2f;
        keyboardSounds.Play();
    }

    public void StopHack(){
        anim.SetBool("Hacking", false);
        keyboardSounds.Stop();
        anim.ResetTrigger("HackStart");
        if(interact){
            interact.SendMessage("Stop", SendMessageOptions.DontRequireReceiver);
        }
    }

    private void FixedUpdate()
    {
        if(!canWalk){
            return;
        }
        sprite.sortingOrder = (int)(-transform.position.y * 100);
        Vector2 moveDirection = new Vector2(input.x, input.y).normalized * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + moveDirection);
    }

    private void LateUpdate()
    {
        camera.transform.position = this.transform.position + -Vector3.forward;
        if(winWalk && walkI<1){
            anim.SetBool("Walking",true);
            walkI += Time.deltaTime;
            transform.position = transform.position + new Vector3(0,Time.deltaTime,0);
        }
    }

    private void OnTriggerEnter2D(Collider2D item){
        if(!canWalk){
            return;
        }
        if(item.gameObject.tag == "Switch"){
            interact = item.transform.parent;
        }
        if(item.gameObject.tag == "Enemy"){
            Debug.Log("Lose");
            StartCoroutine(LoseSequence());
        }
        if(item.gameObject.tag == "Finish"){
            if(score >= 3){
                StartCoroutine(WinSequence());
            }
        }
    }

    private IEnumerator WinSequence(){
        sceneData.state = 1;
        sceneData.score = score;
        anim.SetBool("Walking",false);
        StartCoroutine(EndGame());
        rb.simulated = false;
        transform.position = exit.transform.Find("Trigger").position;
        anim.SetTrigger("Win");
        anim.SetInteger("Direction", 2);
        yield return new WaitForSeconds(3);
        winWalk = true;
    }

    private IEnumerator LoseSequence(){
        sceneData.state = 2;
        StartCoroutine(EndGame());
        yield return new WaitForSeconds(1);
    }

    private IEnumerator EndGame(){
        sceneData.level = level;
        anim.SetBool("Walking",false);
        canWalk = false;
        canvas.transform.Find("Progress").gameObject.SetActive(false);
        foreach(Transform enemy in GameObject.Find("Enemies").transform){
            enemy.GetComponent<Patrol>().active = false;
            enemy.GetComponent<Animator>().SetBool("isMoving", false);
            enemy.GetComponent<NavMeshAgent>().enabled = false;
        }
        yield return new WaitForSeconds(4);
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }

    private void OnTriggerExit2D(Collider2D item){
        if(item.gameObject.tag == "Switch"){
            Debug.Log("Exit");
            interact = null;
        }
    }

    public void AddScore()
    {
        score += 1;
        canvas.transform.Find("Progress").GetComponent<TextMeshProUGUI>().text = "Hacked Computers: "+score+"/3";
        if(score >= goal){
            exit.GetComponent<Animator>().SetBool("Open",true);
        }
    }
}