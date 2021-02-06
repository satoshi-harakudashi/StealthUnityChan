using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoofController : MonoBehaviour
{
    public bool aboveWall;
    //player
    static GameObject player;
    //unitychancontroller
    static UnityChanController unityChanController;
    //
    static WallGenerator3 wallGenerator;
    
    private Renderer renderer;
    private Vector3 thisPos3;



    // Start is called before the first frame update
    void Start()
    {
        if (player == null)
        {
            player = GameObject.Find("Player");
            unityChanController = player.GetComponent<UnityChanController>();
        }
        if (wallGenerator == null)
        {
            wallGenerator = GameObject.Find("WallGenerator").GetComponent<WallGenerator3>();
        }
        
        renderer = GetComponent<MeshRenderer>();
        thisPos3 = transform.position - player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = thisPos3 + player.transform.position;

        if (unityChanController.isClear || unityChanController.isNext)
        {
            Destroy(this.gameObject);
        }

        int x = Mathf.RoundToInt((transform.position.x + wallGenerator.arrayInt) / 2);
        int z = Mathf.RoundToInt((transform.position.z + wallGenerator.arrayInt) / 2);

        //if (wallGenerator.wallArray[x,z])
        //{
        //    aboveWall = true;
        //}
        //else
        //{
        //    aboveWall = false;
        //}





        Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.z);
        if(aboveWall)
        {
            if (renderer.enabled)
            {
                renderer.enabled = false;
            }
        }
        else
        {

        }
    }
}
