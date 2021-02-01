using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoofController : MonoBehaviour
{
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
        if(renderer.enabled)
        {
            Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.z);
            if((thisPos - playerPos).magnitude < 7)
            {
                renderer.enabled = false;
            }
        }
    }
}
