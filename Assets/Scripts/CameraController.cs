using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CameraController : MonoBehaviour
{
    private GameObject player;
    private Transform enemies;
    private Vector2 angle;
    private int dir = 1;
    [SerializeField]
    private float Speed = 1;
    private LayerMask wallMask;
    private LayerMask crouchMask;
    private Animator anim;
    private bool playerSee = false;
    private bool holoSee = false;
    private Transform holograms;
    [SerializeField]
    private float seeRange = 9f;
    void Start(){
        wallMask = LayerMask.GetMask("UpperWall");
        crouchMask = LayerMask.GetMask("CrouchBlock");
        anim = GetComponent<Animator>();
        player = GameObject.Find("Player");
        angle = new Vector2(Mathf.Sin(60f * Mathf.Deg2Rad), Mathf.Cos(60f * Mathf.Deg2Rad));
        holograms = GameObject.Find("Holograms").transform;
        transform.Find("Light").GetComponent<Light2D>().pointLightInnerRadius = seeRange - 2;
        transform.Find("Light").GetComponent<Light2D>().pointLightOuterRadius = seeRange + 2;
        enemies = GameObject.Find("Enemies").transform;
    }

    void Update(){
        float direction = Vector2.SignedAngle(new Vector2(0,1),angle);
        if(direction > 60f){
            dir = -1;
        }
        else if(direction < -60f){
            dir = 1;
        }
        angle = RotateVector2(angle,dir*Time.deltaTime*Speed);
        anim.SetFloat("Direction",direction+dir*Time.deltaTime*Speed);
        if(LookAtPlayer() && !playerSee){
            playerSee = true;
            SendInfo(player.transform.position);
        }
        else if(!LookAtPlayer() && !playerSee){
            playerSee = false;
        }
        bool iterSee = false;
        foreach(Transform hologram in holograms){
            if(LookAtHologram(hologram) && !holoSee && !iterSee){
                holoSee = true;
                iterSee = true;
                SendInfo(hologram.position);
                Debug.Log(hologram.position);
            }
        }
        if(iterSee){
            holoSee = false;
        }
    }
    void LateUpdate(){
        Debug.DrawLine(transform.position, transform.position+(Vector3)MirrorVector2(angle)*3, Color.yellow, 0f);
        transform.Find("Light").eulerAngles = new Vector3(0,0,Vector2.SignedAngle(new Vector2(0,1),MirrorVector2(angle)));
    }

    bool LookAtPlayer(){
        Vector3 dir = (player.transform.position - transform.position).normalized;
        bool crouching = player.GetComponent<PlayerController>().Crouch;
        float dist = Vector2.Distance(transform.position, player.transform.position);
        RaycastHit2D hitCrouch = Physics2D.Raycast(transform.position, dir, dist, crouchMask);
        RaycastHit2D hitWall = Physics2D.Raycast(transform.position, dir, dist, wallMask);
        if(Vector2.Angle(MirrorVector2(angle), dir)> 45){
            return false;
        }
        if(Vector2.Distance(player.transform.position, transform.position) >= seeRange){
            return false;
        }
        if(hitWall){
            return false;
        }
        if(hitCrouch && crouching){
            return false;
        }
        
        return true;
    }

    bool LookAtHologram(Transform hologram){
        if(!hologram.GetComponent<HologramController>().State){
            return false;
        }
        Vector3 dir = (hologram.position - transform.position).normalized;
        float dist = Vector2.Distance(transform.position, hologram.position);
        RaycastHit2D hitWall = Physics2D.Raycast(transform.position, dir, dist, wallMask);
        if(Vector2.Angle(MirrorVector2(angle), dir)> 45){
            return false;
        }
        if(Vector2.Distance(hologram.position, transform.position) >= seeRange){
            return false;
        }
        if(hitWall){
            return false;
        }
        return true;
    }

    private void SendInfo(Vector2 location){
        foreach (Transform enemy in enemies)
        {
            enemy.GetComponent<Patrol>().InsertLastKnownLocation(location);
        }
    }

    Vector2 RotateVector2(Vector2 v, float degrees){
        float radians = degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);
        return new Vector2(
            v.x * cos - v.y * sin,
            v.x * sin + v.y * cos
        );
    }

    Vector2 MirrorVector2(Vector2 v){
        return new Vector2(v.x,-v.y);
    }
}
