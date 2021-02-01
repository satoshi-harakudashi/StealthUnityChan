using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoofController : MonoBehaviour
{
    public bool aboveWall;

    private GameObject player;
    private Renderer renderer;
    private Vector2 thisPos;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        renderer = GetComponent<MeshRenderer>();
        thisPos = new Vector2(transform.position.x, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.z);
        if(aboveWall)
        {
            if (renderer.enabled)
            {
                
                if ((thisPos - playerPos).magnitude < 7)
                {
                    renderer.enabled = false;
                }
            }
        }
        else
        {
            if ((thisPos - playerPos).magnitude < 7)
            {
                renderer.enabled = false;
            }
            else
            {
                renderer.enabled = true;
            }
        }
    }
}
