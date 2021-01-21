using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    private GameObject wallGenerator;
    public GameObject enemyPrefab;
    private GameObject player;
    private float spawnTime = 10;
    private float count = 0;
    private int posX;
    private int posZ;
    // Start is called before the first frame update
    void Start()
    {
        wallGenerator = GameObject.Find("WallGenerator");
        player = GameObject.Find("Player");
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
                if(!wallGenerator.GetComponent<WallGenerator3>().wallArray[x,z] 
                    && Mathf.Abs(x - (int)player.transform.position.x) < 20 
                    && Mathf.Abs(z - (int)player.transform.position.z) < 20)
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
