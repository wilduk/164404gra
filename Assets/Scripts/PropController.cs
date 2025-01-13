using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropController : MonoBehaviour
{
    public float offset;
    private float X;
    private float Y;
    // Start is called before the first frame update
    private SpriteRenderer sprite;
    void Start()
    {
        X = (float)((int)Mathf.RoundToInt(transform.position.x * 2))/2;
        Y = (float)((int)Mathf.RoundToInt(transform.position.y * 2))/2;
        transform.position = new Vector3(X,Y,0);
        sprite = transform.GetComponent<SpriteRenderer>();
        if(sprite){
            transform.GetComponent<SpriteRenderer>().sortingOrder = (int) (-(transform.position.y+offset)*100);
        }
        ProcessChild(transform, transform.position);
    }

    void ProcessChild(Transform processed, Vector3 position)
    {
        foreach (Transform child in processed)
        {
            sprite = child.GetComponent<SpriteRenderer>();
            if (sprite){
                sprite.sortingOrder = (int) (-(position.y+offset)*100);
            }
            if(child.name == "ProgBar"){
                sprite.sortingOrder -= 2;
            }
            else if(child.name == "Progress"){
                sprite.sortingOrder -= 1;
            }
            ProcessChild(child, position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
