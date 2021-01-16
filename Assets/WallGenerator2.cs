using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGenerator2 : MonoBehaviour
{
    public GameObject wallPrefab;


    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < 21; i++)
        {
            for(int j = 0; j < 21; j++)
            {
                if (i == 0 || i == 20 || j == 0 || j == 20 ||(i%2 == 0 && j%2 == 0))
                {
                    GameObject wall = Instantiate(wallPrefab);
                    wall.transform.position = new Vector3(i, 1, j);
                }
            }
        }



    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
