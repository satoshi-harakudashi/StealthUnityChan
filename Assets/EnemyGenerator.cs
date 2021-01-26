using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    private GameObject wallGenerator;
    public GameObject enemyPrefab;

    private int posX;
    private int posZ;
    // Start is called before the first frame update
    void Start()
    {
        wallGenerator = GameObject.Find("WallGenerator");

        for(int i = 0; i < 10; i++)
        {
            bool isAbleToGenerate = false;
            while (!isAbleToGenerate)
            {
                int x = Random.Range(0, 30);
                int z = Random.Range(0, 30);
                if (!wallGenerator.GetComponent<WallGenerator3>().wallArray[x, z])
                {
                    isAbleToGenerate = true;
                    posX = x;
                    posZ = z;
                }
            }
            GameObject enemy = Instantiate(enemyPrefab);
            enemy.transform.position = new Vector3(posX * 2, 0, posZ * 2);
        }

    }

    // Update is called once per frame
    void Update()
    {      
        
    }
}
