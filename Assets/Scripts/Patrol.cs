using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;

public class Patrol : MonoBehaviour{
    [SerializeField]
    private Vector3[] patrolPoints;
    private int currPoint = 0;
    public bool loopedPatrol = true;
    private bool forward = true;
    private SpriteRenderer sprite;
    private float patrolSpeed = 3f;
    private float moveSpeed = 6f;
    private float stayLength = 1f;
    [SerializeField]
    private float seeRange = 7f;
    private float stayStart;
    private bool waiting = false;
    private Animator anim;
    private NavMeshAgent agent;
    private Vector2 LookDir;
    private NavMeshPath path;
    private GameObject player;
    private LayerMask wallMask;
    private LayerMask crouchMask;
    private Vector3 lastKnownLocation;
    public bool active = true;
    private Transform holograms;
    // Start is called before the first frame update
    void Start(){
        sprite = transform.Find("Sprite").GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
		agent.updateRotation = false;
		agent.updateUpAxis = false;
        agent.SetDestination((Vector3)(Vector2)patrolPoints[currPoint]);
        transform.position = (Vector3)patrolPoints[0];
        path = new NavMeshPath();
        player = GameObject.Find("Player");
        wallMask = LayerMask.GetMask("Wall");
        crouchMask = LayerMask.GetMask("CrouchBlock");
        lastKnownLocation = Vector3.zero;
        holograms = GameObject.Find("Holograms").transform;
        transform.Find("Light").GetComponent<Light2D>().pointLightInnerRadius = seeRange - 2;
        transform.Find("Light").GetComponent<Light2D>().pointLightOuterRadius = seeRange + 2;
    }

    // Update is called once per frame
    void Update(){
        if(!active){
            return;
        }
        foreach(Transform hologram in holograms){
            if(LookAtHologram(hologram)){
                lastKnownLocation = hologram.position;
                agent.speed = moveSpeed;
                agent.SetDestination(lastKnownLocation);
            }
        }
        if(LookAtPlayer()){
            lastKnownLocation = player.transform.position;
            agent.speed = moveSpeed;
            agent.SetDestination(lastKnownLocation);
        }
        if(lastKnownLocation != Vector3.zero && Vector2.Distance(transform.position,lastKnownLocation) <= 0.1){
            waiting = true;
            Debug.Log("Reached "+transform.name+" next location "+currPoint);
            lastKnownLocation = Vector3.zero;
            agent.speed = patrolSpeed;
            agent.CalculatePath((Vector3)(Vector2)patrolPoints[currPoint], path);
            stayLength = 0f;
        }
        if(Time.time - stayStart < stayLength && waiting == true){
            anim.SetBool("isMoving", false);
            return;
        }
        waiting = false;
        anim.SetBool("isMoving", true);
        if(Vector2.Distance(transform.position, patrolPoints[currPoint]) <= 0.1){
            waiting = true;
            stayStart = Time.time;
            stayLength = patrolPoints[currPoint].z;
            currPoint = forward ? currPoint + 1 : currPoint - 1;
            if(currPoint >= patrolPoints.Length){ 
                if(loopedPatrol){
                    currPoint = 0;
                }
                else{
                    forward = false;
                    currPoint = patrolPoints.Length - 2;
                }
            }
            else if(currPoint < 0){
                forward = true;
                currPoint = 1;
            }
            agent.CalculatePath((Vector3)(Vector2)patrolPoints[currPoint], path);
        }
        else if(path.status == NavMeshPathStatus.PathComplete && lastKnownLocation == Vector3.zero){
            anim.SetBool("isMoving", true);
            agent.SetPath(path);
        }
    }

    public void InsertLastKnownLocation(Vector3 location){
        lastKnownLocation = location;
        agent.speed = moveSpeed;
        agent.SetDestination(location);
    }

    bool LookAtPlayer(){
        Vector3 dir = (player.transform.position - transform.position).normalized;
        bool crouching = player.GetComponent<PlayerController>().Crouch;
        float dist = Vector2.Distance(transform.position, player.transform.position);
        RaycastHit2D hitCrouch = Physics2D.Raycast(transform.position, dir, dist, crouchMask);
        RaycastHit2D hitWall = Physics2D.Raycast(transform.position, dir, dist, wallMask);
        if(Vector2.Angle(LookDir, dir)> 45){
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
        if(Vector2.Angle(LookDir, dir)> 45){
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

    void FixedUpdate(){
        if(!active){
            transform.Find("Light").eulerAngles = new Vector3(0,0,180);
            return;
        }
        if(!waiting || lastKnownLocation != Vector3.zero){
            LookDir = (Vector2)agent.velocity.normalized;
        }
        else if (path.status == NavMeshPathStatus.PathComplete){
            LookDir = (Vector2)(path.corners[1] - path.corners[0]).normalized;
        }
        if (LookDir.x < 0){
            sprite.flipX = false;
        }
        else if (LookDir.x > 0){
            sprite.flipX = true;
        }
        Debug.DrawLine(transform.position, transform.position+(Vector3)LookDir*3, Color.yellow, 0f);
        //transform.Find("MaskRotationPoint").eulerAngles = new Vector3(0,0,Vector2.SignedAngle(new Vector2(-1,0),LookDir));
        transform.Find("Light").eulerAngles = new Vector3(0,0,Vector2.SignedAngle(new Vector2(0,1),LookDir));
        sprite.sortingOrder = (int)(-transform.position.y * 100);
        // walkDirection = (patrolPoints[currPoint] - new Vector2(transform.position.x,transform.position.y)).normalized;
        // if (walkDirection.x < 0)
        // {
        //     sprite.flipX = false;
        // }
        // else if (walkDirection.x > 0)
        // {
        //     sprite.flipX = true;
        // }
        // if(!waiting)transform.Translate(walkDirection * patrolSpeed * Time.deltaTime);
    }

    void LateUpdate(){
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }
}
