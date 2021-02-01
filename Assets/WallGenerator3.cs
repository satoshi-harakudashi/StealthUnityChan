using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGenerator3 : MonoBehaviour
{
    //壁伸ばし法http://www5d.biglobe.ne.jp/stssk/maze/make.html

    private GameObject player;
    private GameObject roof;
    public GameObject pieceOfRoof;
    public GameObject wallPrefab;
    public GameObject stairPrefab;

    //100×100の壁の有無の配列
    public bool[,] wallArray = new bool[30, 30];
    //生成済の壁の座標を入れるリスト
    private List<Vector2Int> wallPosList = new List<Vector2Int>();

    // Start is called before the first frame update
    void Start()
    {
        //最外周を壁にする
        for (int i = 0; i < 30; i++)
        {

            wallArray[0, i] = true;

            wallArray[29, i] = true;

            wallArray[i, 0] = true;

            wallArray[i, 29] = true;

        }

        //迷路の敷地をすべてリストに加える
        for(int i = 0; i < 30; i ++)
        {
            for(int j = 0; j < 30; j ++)
            {
                wallPosList.Add(new Vector2Int(i, j));
            }
        }


        while (wallPosList.Count > 0)//※list.Countは要素数を返す
        {
            //1箇所を選び、壁つくりの起点とする
            int listNo = Random.Range(0, wallPosList.Count);
            int posX = wallPosList[listNo].x;
            int posZ = wallPosList[listNo].y;

            //壁伸ばしに失敗したらfalseになるチェックリスト
            bool[] wallCheck = { true, true, true, true };

            //すべての方向に進めなくなるまで起点から壁を伸ばす
            while (wallCheck[0] || wallCheck[1] || wallCheck[2] || wallCheck[3])
            {
                //壁を伸ばす方向をランダムに選択する
                int directInt = Random.Range(0, 4);
                //選択した方向の1個先、2個先に壁がなければ壁を作る
                switch (directInt)
                {
                    case 0:
                        if (!WallInArea(posX, posZ, 4, -1, 2, 2) && posX + 2 < 30)
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
                        if (!WallInArea(posX, posZ, 2, 2, 4, -1) && posZ + 2 < 30)
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

        for (int i = 0; i < 30; i++)
        {
            for (int j = 0; j < 30; j++)
            {
                if (wallArray[i, j])
                {
                    GameObject wall = Instantiate(wallPrefab);
                    wall.transform.position = new Vector3(2 * i, 1, 2 * j);
                }
            }
        }

        int playerPosX = 0;
        int playerPosZ = 0;

        bool isAbleToPut = false;
        while(!isAbleToPut)
        {
            playerPosX = Random.Range(0, 30);
            playerPosZ = Random.Range(0, 30);
            if (!wallArray[playerPosX, playerPosZ]) { isAbleToPut = true; }
        }

        int stairPosX = 0;
        int stairPosZ = 0;
        isAbleToPut = false;
        while(!isAbleToPut)
        {
            stairPosX = Random.Range(0, 30);
            stairPosZ = Random.Range(0, 30);
            if(!wallArray[stairPosX,stairPosZ] && (stairPosX < playerPosX -10 || stairPosX > playerPosX + 10 || stairPosZ < playerPosZ - 10 || stairPosZ > playerPosZ + 10))
            {
                isAbleToPut = true;
            }

        }

        player = GameObject.Find("Player");
        player.transform.position = new Vector3(playerPosX * 2, 0, playerPosZ * 2);

        GameObject stair = Instantiate(stairPrefab);
        stair.transform.position = new Vector3(stairPosX * 2 + 1, 0, stairPosZ * 2 -1);

        roof = GameObject.Find("Roof");

        for(int i = 0; i < 120; i ++)
        {
            for (int j = 0; j < 120; j++)
            {
                GameObject por = Instantiate(pieceOfRoof);
                por.transform.parent = roof.transform;
                por.transform.position = new Vector3((i-1.5f) * 0.5f, 5, (j-1.5f) * 0.5f);
                if(wallArray[Mathf.RoundToInt(i/4),Mathf.RoundToInt(j/4)])
                {
                    por.GetComponent<RoofController>().aboveWall = true;
                }
                else
                {
                    por.GetComponent<RoofController>().aboveWall = false;
                }

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
                else if (i >= 0 && i < 30 && j >= 0 && j < 30)
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
