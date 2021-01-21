using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    private GameObject wallGenerator;
    public GameObject enemyPrefab;
    private float spawnTime = 100;
    private float count = 99;
    private int posX;
    private int posZ;
    // Start is called before the first frame update
    void Start()
    {
        wallGenerator = GameObject.Find("WallGenerator");
    }

    // Update is called once per frame
    void Update()
    {      
        if (count < spawnTime)
        {
            count += Time.deltaTime;
        }
        else
        {
            bool isAbleToGenerate = false;
            while(!isAbleToGenerate)
            {
                int x = Random.Range(0, 100);
                int z = Random.Range(0, 100);
                if(!wallGenerator.GetComponent<WallGenerator3>().wallArray[x,z])
                {
                    isAbleToGenerate = true;
                    posX = x;
                    posZ = z;
                }
            }
            GameObject enemy = Instantiate(enemyPrefab);
            enemy.transform.position = new Vector3(posX * 2, 0, posZ * 2);
            count = 0;
        }
    }
}
