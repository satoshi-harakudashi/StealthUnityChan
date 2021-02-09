using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGenerator3 : MonoBehaviour
{
    //壁伸ばし法http://www5d.biglobe.ne.jp/stssk/maze/make.html
    public GameObject enemyPrefab;
    private GameObject player;
    private GameObject roof;
    public GameObject pieceOfRoof;
    public GameObject wallPrefab;
    public GameObject stairPrefab;
    public GameObject floorPrefab;

    public int arrayInt = 10;
    public int floorNo = 1;

    //100×100の壁の有無の配列
    public bool[,] wallArray;// = new bool[30, 30];
    //生成済の壁の座標を入れるリスト
    private List<Vector2Int> wallPosList = new List<Vector2Int>();

    // Start is called before the first frame update
    void Start()
    {
        
        PrepareThisFloor();

    }

    public void PrepareThisFloor()
    {
        GameObject floor = Instantiate(floorPrefab);
        floor.transform.position = new Vector3(-1, 4 * (floorNo - 1) - 1, -1);
        floor.transform.localScale = new Vector3(2 * arrayInt, 2, 2 * arrayInt);
        floor.GetComponent<Renderer>().material.mainTextureScale = new Vector2(arrayInt*0.4f, arrayInt*0.4f);

        wallArray = new bool[arrayInt, arrayInt];

        //最外周を壁にする
        for (int i = 0; i < arrayInt; i++)
        {

            wallArray[0, i] = true;

            wallArray[arrayInt - 1, i] = true;

            wallArray[i, 0] = true;

            wallArray[i, arrayInt - 1] = true;

        }

        //迷路の敷地をすべてリストに加える
        for (int i = 0; i < arrayInt; i++)
        {
            for (int j = 0; j < arrayInt; j++)
            {
                wallPosList.Add(new Vector2Int(i, j));
            }
        }


        while (wallPosList.Count > 0)//※list.Countは要素数を返す
        {
            //1箇所を選び、壁つくりの起点とする
            int listNo = UnityEngine.Random.Range(0, wallPosList.Count);
            int posX = wallPosList[listNo].x;
            int posZ = wallPosList[listNo].y;

            //壁伸ばしに失敗したらfalseになるチェックリスト
            bool[] wallCheck = { true, true, true, true };

            //すべての方向に進めなくなるまで起点から壁を伸ばす
            while (wallCheck[0] || wallCheck[1] || wallCheck[2] || wallCheck[3])
            {
                //壁を伸ばす方向をランダムに選択する
                int directInt = UnityEngine.Random.Range(0, 4);
                //選択した方向の1個先、2個先に壁がなければ壁を作る
                switch (directInt)
                {
                    case 0:
                        if (!WallInArea(posX, posZ, 4, -1, 2, 2) && posX + 2 < arrayInt)
                        {
                            wallArray[posX + 1, posZ] = true;
                            //wallPosList.Add(new Vector2Int(posX + 1, posZ));
                            wallArray[posX + 2, posZ] = true;
                            //wallPosList.Add(new Vector2Int(posX + 2, posZ));
                            posX = posX + 2;
                            wallCheck[0] = true;
                            wallCheck[1] = true;
                            wallCheck[2] = true;
                            wallCheck[3] = true;
                        }
                        else
                        {
                            wallCheck[0] = false;
                        }
                        break;
                    case 1:
                        if (!WallInArea(posX, posZ, -1, 4, 2, 2) && posX - 2 >= 0)
                        {
                            wallArray[posX - 1, posZ] = true;
                            //wallPosList.Add(new Vector2Int(posX - 1, posZ));
                            wallArray[posX - 2, posZ] = true;
                            //wallPosList.Add(new Vector2Int(posX - 2, posZ));
                            posX = posX - 2;
                            wallCheck[0] = true;
                            wallCheck[1] = true;
                            wallCheck[2] = true;
                            wallCheck[3] = true;
                        }
                        else
                        {
                            wallCheck[1] = false;
                        }
                        break;
                    case 2:
                        if (!WallInArea(posX, posZ, 2, 2, 4, -1) && posZ + 2 < arrayInt)
                        {
                            wallArray[posX, posZ + 1] = true;
                            //wallPosList.Add(new Vector2Int(posX, posZ + 1));
                            wallArray[posX, posZ + 2] = true;
                            //wallPosList.Add(new Vector2Int(posX, posZ + 2));
                            posZ = posZ + 2;
                            wallCheck[0] = true;
                            wallCheck[1] = true;
                            wallCheck[2] = true;
                            wallCheck[3] = true;
                        }
                        else
                        {
                            wallCheck[2] = false;
                        }
                        break;
                    case 3:
                        if (!WallInArea(posX, posZ, 2, 2, -1, 4) && posZ - 2 >= 0)
                        {
                            wallArray[posX, posZ - 1] = true;
                            //wallPosList.Add(new Vector2Int(posX, posZ - 1));
                            wallArray[posX, posZ - 2] = true;
                            //wallPosList.Add(new Vector2Int(posX, posZ - 2));
                            posZ = posZ - 2;
                            wallCheck[0] = true;
                            wallCheck[1] = true;
                            wallCheck[2] = true;
                            wallCheck[3] = true;
                        }
                        else
                        {
                            wallCheck[3] = false;
                        }
                        break;
                }
            }
            //whileを抜けたのでこれ以上壁を作れない→Listから削除
            wallPosList.Remove(new Vector2Int(posX, posZ));
        }

        for (int i = 0; i < arrayInt; i++)
        {
            for (int j = 0; j < arrayInt; j++)
            {
                if (wallArray[i, j])
                {
                    GameObject wall = Instantiate(wallPrefab);
                    wall.transform.position = new Vector3(2 * i - arrayInt, 4 * floorNo - 3, 2 * j - arrayInt);
                }
            }
        }

        int playerPosX = 0;
        int playerPosZ = 0;

        bool isAbleToPut = false;
        while (!isAbleToPut)
        {
            playerPosX = UnityEngine.Random.Range(0, arrayInt);
            playerPosZ = UnityEngine.Random.Range(0, arrayInt);
            if (!wallArray[playerPosX, playerPosZ]) { isAbleToPut = true; }
        }

        player = GameObject.Find("Player");
        player.transform.position = new Vector3(2 * playerPosX - arrayInt, 4 * (floorNo - 1), 2 * playerPosZ - arrayInt);
        player.GetComponent<UnityChanController>().arrayInt = arrayInt;
        player.GetComponent<UnityChanController>().floorNo = floorNo;

        for (int i = 0; i < arrayInt - 9; i++)
        {
            int enemyPosX = 0;
            int enemyPosZ = 0;
            isAbleToPut = false;
            GameObject enemy = Instantiate(enemyPrefab);
            enemy.GetComponent<EnemyController2>().firstSize = 1 + UnityEngine.Random.Range(0,2) + 0.001f * (i + 1);
            enemy.GetComponent<EnemyController2>().firstY = 4 * (floorNo - 1);
            enemy.GetComponent<EnemyController2>().size = enemy.GetComponent<EnemyController2>().firstSize;
            float distance = 0;
            while (distance < 8 || distance > 10 + arrayInt/2)
            {
                int posX = UnityEngine.Random.Range(0, arrayInt);
                int posZ = UnityEngine.Random.Range(0, arrayInt);
                enemy.transform.position = new Vector3(2 * posX - arrayInt, 4 * (floorNo - 1), 2 * posZ - arrayInt);
                if (!wallArray[posX, posZ])
                {
                    distance = (enemy.transform.position - player.transform.position).magnitude;
                }
            }


            //while (!isAbleToPut)
            //{
            //    enemyPosX = UnityEngine.Random.Range(0, arrayInt);
            //    enemyPosZ = UnityEngine.Random.Range(0, arrayInt);
            //    if (!wallArray[enemyPosX, enemyPosZ] && (enemyPosX < playerPosX - 3 || enemyPosX > playerPosX + 3 || enemyPosZ < playerPosZ - 3 || enemyPosZ > playerPosZ + 3))
            //    {
            //        isAbleToPut = true;
            //    }
            //}
            
            enemy.transform.localScale = Vector3.one * Mathf.Pow(enemy.GetComponent<EnemyController2>().size, 0.5f);
        }

        int stairPosX = 0;
        int stairPosZ = 0;
        isAbleToPut = false;
        while (!isAbleToPut)
        {
            stairPosX = UnityEngine.Random.Range(0, arrayInt);
            stairPosZ = UnityEngine.Random.Range(0, arrayInt);
            if (!wallArray[stairPosX, stairPosZ] && (stairPosX < playerPosX - 3 || stairPosX > playerPosX + 3 || stairPosZ < playerPosZ - 3 || stairPosZ > playerPosZ + 3))
            {
                isAbleToPut = true;
            }

        }

        
        GameObject stair = Instantiate(stairPrefab);
        stair.transform.position = new Vector3(2 * stairPosX - arrayInt + 1, 4 * (floorNo - 1), 2 * stairPosZ - arrayInt - 1);

        roof = GameObject.Find("Roof");

        for (int i = 0; i < 20* 4; i++)
        {
            for (int j = 0; j < 20 * 4; j++)
            {
                //GameObject por = Instantiate(pieceOfRoof);
                //por.transform.parent = roof.transform;
                //por.transform.position = new Vector3((i - 3.75f) * 0.25f - arrayInt, 3 + 4 * (floorNo - 1), (j - 3.75f) * 0.25f - arrayInt) + player.transform.position;


                //if (wallArray[Mathf.RoundToInt(i / 4), Mathf.RoundToInt(j / 4)])
                //{
                //    por.GetComponent<RoofController>().aboveWall = true;
                //}
                //else
                //{
                //    por.GetComponent<RoofController>().aboveWall = false;
                //}

            }
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    private bool WallInArea(int x, int z, int xPlus, int xMinus, int zPlus, int zMinus)
    {
        //チェックしない方向は-1を入れるとよい


        int checkNo = 0;

        //範囲内の起点以外の壁の個数を見る
        for (int i = x - xMinus; i <= x + xPlus; i++)
        {
            for (int j = z - zMinus; j <= z + zPlus; j++)
            {
                if (i == x && j == z)
                {

                }
                else if (i >= 0 && i < arrayInt && j >= 0 && j < arrayInt)
                {
                    if (wallArray[i, j] == true)
                    {
                        checkNo += 1;
                    }
                }
            }
        }

        if (checkNo > 0)
        {
            //trueは範囲内の起点以外に壁があることをあらわす
            return true;
        }
        else
        {
            return false;
        }
    }

}
