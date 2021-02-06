using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGenerator2 : MonoBehaviour
{
    //壁伸ばし法http://www5d.biglobe.ne.jp/stssk/maze/make.html

    public GameObject wallPrefab;
    //100×100の壁の有無の配列
    public bool[,] wallArray = new bool[100, 100];
    //生成済の壁の座標を入れるリスト
    private List<Vector2Int> wallPosList = new List<Vector2Int>(); 

    // Start is called before the first frame update
    void Start()
    {
        //最外周を壁にする
        for (int i = 0; i<100;i++)
        {

            wallArray[0, i] = true;
            wallPosList.Add(new Vector2Int(0, i));

            wallArray[99, i] = true;
            wallPosList.Add(new Vector2Int(99, i));

            wallArray[i, 0] = true;
            if(i != 0 && i != 99) { wallPosList.Add(new Vector2Int(i, 0)); }

            wallArray[i, 99] = true;
            if(i != 0 && i != 99) { wallPosList.Add(new Vector2Int(i, 99)); }

        }

        while(wallPosList.Count > 0)//※list.Countは要素数を返す
        {
            //壁から1箇所を選び、壁つくりの起点とする
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
                //選択した方向の1個先、2個先に壁がなければ壁を作り、2個先を壁つくりの起点とする
                switch (directInt)
                {
                    case 0:
                        if (!WallInArea(posX,posZ,4,-1,2,2) && posX + 2< 100)
                        {
                            wallArray[posX + 1, posZ] = true;
                            wallPosList.Add(new Vector2Int(posX + 1, posZ));
                            wallArray[posX + 2, posZ] = true;
                            wallPosList.Add(new Vector2Int(posX + 2, posZ));
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
                        if (!WallInArea(posX,posZ,-1,4,2,2) && posX -2 >=0)
                        {
                            wallArray[posX - 1, posZ] = true;
                            wallPosList.Add(new Vector2Int(posX - 1, posZ));
                            wallArray[posX - 2, posZ] = true;
                            wallPosList.Add(new Vector2Int(posX - 2, posZ));
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
                        if (!WallInArea(posX, posZ, 2, 2, 4, -1) && posZ + 2 < 100)
                        {
                            wallArray[posX, posZ + 1] = true;
                            wallPosList.Add(new Vector2Int(posX, posZ + 1));
                            wallArray[posX, posZ + 2] = true;
                            wallPosList.Add(new Vector2Int(posX, posZ + 2));
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
                            wallPosList.Add(new Vector2Int(posX, posZ - 1));
                            wallArray[posX, posZ - 2] = true;
                            wallPosList.Add(new Vector2Int(posX, posZ - 2));
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

        
        

        for (int i = 0; i < 100; i++)
        {
            for(int j = 0; j < 100; j++)
            {
                if (wallArray[i,j])
                {
                    GameObject wall = Instantiate(wallPrefab);
                    wall.transform.position = new Vector3(2*i, 1, 2*j);
                }
            }
        }



    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private bool WallInArea(int x,int z, int xPlus, int xMinus,int zPlus, int zMinus)
    {
        //チェックしない方向は-1を入れるとよい


        int checkNo = 0;

        //範囲内の起点以外の壁の個数を見る
        for(int i = x - xMinus; i <= x + xPlus; i ++)
        {
            for (int j = z - zMinus; j <= z + zPlus; j++)
            {
                if(i == x && j == z)
                {

                }
                else if (i >= 0 && i < 100 && j >= 0 && j < 100)
                {
                    if(wallArray[i,j] == true)
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
