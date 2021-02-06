using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGenerator : MonoBehaviour
{
    //壁配置のイメージ
    //                  横壁[i,j+1]
    //
    //      縦壁[i,j]   部屋[i,j]   縦壁[i+1,j]   部屋[i+1,j]
    //
    //                  横壁[i,j]


    public GameObject wallPrefab;
    //ゴールへの経路
    public bool[,] wayToGoal = new bool[10, 10];
    //横向きの壁
    public bool[,] wallYoko = new bool[10, 11];
    //縦向きの壁
    public bool[,] wallTate = new bool[11, 10];
    //スタート位置
    private int startPos;
    //ゴール位置
    private int goalPos;
    //壁配置濃度(0～100)[%]
    private int wallDens = 40;
    //player
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        startPos = Random.Range(0, 10);
        //goalPos = Random.Range(0, 10);
        wayToGoal[startPos, 0] = true;
        int i = startPos;
        int j = 0;
        int count = 0;

        //ゴールへの道の設定
        while(j < 10)
        {   
            count += 1;

            int directNo = Random.Range(0, 4);
            switch (directNo)
            {
                case 0:
                    if (i < 9 && wayToGoal[i+1,j] == false) { i += 1; }
                    break;
                case 1:
                    if (i > 0 && wayToGoal[i-1,j] == false) { i -= 1; }
                    break;
                case 2:
                    if((j < 9 && wayToGoal[i,j+1] == false)||j == 9) {j += 1;}
                    break;
                case 3:
                    if (j > 0 && wayToGoal[i,j-1]) { j -= 1; }
                    break;
            }
            if(j < 10 )
            {
                wayToGoal[i, j] = true;
            }
            else
            {
                goalPos = i;
            }


            //やり直し
            if(count > 20)
            {
                for(int p = 0; p< 10; p++)
                {
                    for (int q = 0; q < 10; q++)
                    {
                        wayToGoal[p, q] = false;
                    }
                }
                i = startPos;
                j = 0;
                wayToGoal[startPos, 0] = true;
                count = 0;
            }

        }

        
        #region 横壁の分布決定
        for (int p = 0; p < 10; p ++)
        {
            for(int q = 0; q < 11; q++)
            {
                if(q == 0 || q == 10)
                {
                    //外周の壁
                    wallYoko[p, q] = true;
                }
                else if(wayToGoal[p,q-1] && wayToGoal[p,q])
                {
                    //経路
                    wallYoko[p, q] = false;
                }
                else
                {
                    int r = Random.Range(0, 101);
                    if(r < wallDens) { wallYoko[p, q] = true; }
                }
            }
        }
        wallYoko[startPos, 0] = false;
        wallYoko[goalPos, 10] = false;
        #endregion

        #region 縦壁の分布決定
        for (int p = 0; p < 11; p++)
        {
            for (int q = 0; q < 10; q++)
            {
                if (p == 0 || p == 10)
                {
                    //外周の壁
                    wallTate[p, q] = true;
                }
                else if (wayToGoal[p-1, q] && wayToGoal[p, q])
                {
                    //経路
                    wallTate[p, q] = false;
                }
                else
                {
                    int r = Random.Range(0, 101);
                    if (r < wallDens) { wallTate[p, q] = true; }
                }
            }
        }
        #endregion

        #region 壁の生成
        for (int p = 0; p < 10; p++)
        {
            for (int q = 0; q < 11; q++)
            {
                if (wallYoko[p, q])
                {
                    GameObject wall = Instantiate(wallPrefab);
                    wall.transform.position = new Vector3(2 * p + 1, 1, 2 * q);
                }
                if (wallTate[q, p])
                {
                    GameObject wall = Instantiate(wallPrefab);
                    wall.transform.position = new Vector3(2 * q, 1, 2 * p + 1);
                    wall.transform.rotation = Quaternion.Euler(0, 90, 0);
                }
            } 
        }
        #endregion


        player = GameObject.Find("Player");
        player.transform.position = new Vector3(startPos * 2 + 1, 0, -1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
