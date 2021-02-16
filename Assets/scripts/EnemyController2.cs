using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController2 : MonoBehaviour
{
    //待機状態、徘徊状態、追跡状態


    //
    public GameObject destinationMarkPrefab;
    //wallgenerator
    static WallGenerator3 wallGenerator;
    //player
    static GameObject player;
    //view
    public GameObject view;
    //unitychancontroller
    static UnityChanController unityChanController;
    //アニメーションするためのコンポーネントを入れる
    private Animator myAnimator;
    public List<Material> material = new List<Material>();

    //状態変数　0:待機、1:徘徊、2:追跡
    public int stateNo = 0;
    //前回の状態変数
    private int lastStateNo = 0;
    //stateNo == 5の時に使う状態
    private int stateNo5 = 0;
    //markを置いた個数をカウント
    private int markCo = 0;
    //
    private bool isPutting;
    //走行速度
    private float runSpeed = 1;
    
    //追跡中の1回の移動時間(初期)
    public float runTimeFirst = 0.6f;
    //追跡中の1回の移動時間;
    public float runTime;
    //通常の1回の移動時間
    private float walkTime = 2f;
    //待機時間
    private float waitTime = 3f;

    //階層内の敵の総数
    public int enemyNo = 1;
    //初期サイズ
    public float firstSize = 1;
    //サイズ
    public float size = 1;

    //時間のカウント
    public float count;
    //歩いたマスのカウント
    int walkCount;
    //待機までに歩くマスの数
    private int walkBeforeWait = 5;

    //
    private int state3From = 10;
    private int state3To = 50;

    //目的地
    private Vector3 destination;
    //stae5のみ使う目的地リスト
    private List<Vector3> destinationList = new List<Vector3>();
    private List<GameObject> markList = new List<GameObject>();
    
    //プレイヤーの向き
    private Quaternion playerRotation;
    //方向の箱
    private Vector3[] directArray = new Vector3[2];

    private Vector3 viewSizeVec;
    private Vector3 viewPos;
    private int arrayInt;
    public int firstY;

    // Start is called before the first frame update
    void Start()
    {
        if (wallGenerator == null)
        {
            wallGenerator = GameObject.Find("WallGenerator").GetComponent<WallGenerator3>();

        }
        arrayInt = wallGenerator.arrayInt;
        //player取得
        if (player == null)
        {
            player = GameObject.Find("Player");
            unityChanController = player.GetComponent<UnityChanController>();
        }

        //すべてのenemyの体のmaterial取得
        List<GameObject> allChildren = new List<GameObject>();
        GetChildren(gameObject, ref allChildren);
        for(int i = 0; i < allChildren.Count; i++)
        {
            if(allChildren[i].tag != "view")
            {
                material.Add(allChildren[i].GetComponent<MeshRenderer>().material);
            }
        }


        //アニメータコンポーネントを取得
        myAnimator = GetComponentInChildren<Animator>();

        view = transform.GetChild(0).gameObject;
        view.GetComponent<ColliderController>().Initialize(gameObject);
        viewSizeVec = view.transform.localScale;
        viewPos = view.transform.localPosition;

        runTime = runTimeFirst;
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < material.Count; i ++)
        {
            material[i].SetFloat("_Size",size);
            material[i].SetInt("_StateNo", stateNo);
        }

 


        if(unityChanController.isDead || unityChanController.isPose)
        {
            myAnimator.SetFloat("AnimationSpeed", 0);
            return;
        }
        else
        {
            myAnimator.SetFloat("AnimationSpeed", 1);
        }


        if(unityChanController.isClear || unityChanController.isNext)
        {
            Destroy(this.gameObject);
        }

        //待機処理
        if(stateNo == 0)
        {
            Wait();
        }
        else if(stateNo == 1)
        {
            Walk();
        }
        else if(stateNo == 2 )//|| stateNo == 4)
        {
            Chase();
        }
        else if(stateNo == 3)
        {
            Posing();
        }
        else if(stateNo == 5)
        {
            Attack();
        }



        lastStateNo = stateNo;
    }

    private void Attack()
    {
        //目的地を配置する時間
        float putTime = 0.1f;

        if (stateNo5 == 0)
        {
            myAnimator.SetInteger("State", 3);
            // 目的地リストを作る
            //最初はplayerの位置を加える

            //float px = 2 * Mathf.RoundToInt((player.transform.position.x + arrayInt) / 2) - arrayInt;
            //float py = player.transform.position.y;
            //float pz = 2 * Mathf.RoundToInt((player.transform.position.z + arrayInt) / 2) - arrayInt;

            //destinationList.Add(new Vector3(px,py,pz));

            //リストが空または最後の要素とplayerとの距離が遠い限り、
            if (markList.Count == 0 || (markList[markList.Count - 1].transform.position - player.transform.position).magnitude > 0.5f)
            {
                isPutting = true;
            }
            else
            {
                isPutting = false;
            }

            if(isPutting)
            {
                if(count > putTime)
                {

                    #region 目的地決める
                    Vector3 pos = transform.position;
                    if (markList.Count != 0)
                    {
                        pos = markList[markList.Count - 1].transform.position;
                    }

                    //Xの距離
                    float X = (player.transform.position - pos).x;
                    //Zの距離
                    float Z = (player.transform.position - pos).z;

                    Vector3 dir = Vector3.zero;
                    if (Mathf.Abs(X) > Mathf.Abs(Z) && X > 0)
                    {
                        dir = Vector3.right;
                    }
                    else if (Mathf.Abs(X) > Mathf.Abs(Z) && X <= 0)
                    {
                        dir = Vector3.left;
                    }
                    else if (Mathf.Abs(X) <= Mathf.Abs(Z) && Z > 0)
                    {
                        dir = Vector3.forward;
                    }
                    else if (Mathf.Abs(X) <= Mathf.Abs(Z) && Z <= 0)
                    {
                        dir = Vector3.back;
                    }

                    pos += 2 * dir;

                    #endregion

                    #region 置く
                    //markを作る
                    GameObject mark = Instantiate(destinationMarkPrefab);
                    //markをposの位置に置く
                    mark.transform.position = pos;
                    //marklistにmark追加
                    markList.Add(mark);
                    //countをリセット
                    count = 0;
                    #endregion
                    Debug.Log((pos - player.transform.position).magnitude);
                }
                else
                {
                    //0.1s待つ
                    count += Time.deltaTime;
                }
            }
            else
            {
                //向きを変える(リストの最初が直近の目的地)
                transform.LookAt(markList[0].transform);
                //
                count = 0;
                //置き終わったので次へ
                stateNo5 += 1;
            }
        }
        if(stateNo5 == 1)
        {
            myAnimator.SetInteger("State", 2);
            #region
            if(count < runTime)
            {
                //前進（直近の目的地へ）
                transform.position += 2 * Time.deltaTime / runTime * transform.forward;
                count += Time.deltaTime;
            }
            else
            {
                count = 0;
                Debug.Log("state5:" + stateNo5 + " List:" + destinationList.Count);
                
                BeInCenter();
                //リストの最後のmarkを削除
                Destroy(markList[0].gameObject);

                //直近の目的地をリストから破棄（リストの最後）
                markList.Remove(markList[0]);

                
                if(markList.Count > 0)
                {
                    transform.LookAt(markList[0].transform);
                }
                else
                {
                    stateNo5 = 0;
                }

            }
            #endregion
        }
    }

    private void GetChildren(GameObject obj, ref List<GameObject> allChildren)
    {
        Transform children = obj.GetComponentInChildren<Transform>();
        //子要素がいなければ終了
        if (children.childCount == 0)
        {
            return;
        }
        foreach (Transform ob in children)
        {
            allChildren.Add(ob.gameObject);
            GetChildren(ob.gameObject, ref allChildren);
        }
    }


    private void Posing()
    {
        if(size >= state3To)
        {
            stateNo = 0;
            count = 0;
        }
        // アニメーションを開始
            this.myAnimator.SetInteger("State", 3);
        count += Time.deltaTime;
        return;
    }

    private void Wait()
    {
        //if(size > state3From && size < state3To)
        //{
        //    stateNo = 3;
        //    return;
        //}

        if((transform.position - player.transform.position).magnitude > 25)
        {
            Warp();
        }


        if (count < waitTime)
        {
            //アニメーションを開始
            this.myAnimator.SetInteger("State", 0);
            count += Time.deltaTime;
            return;
        }
        else
        {
            //徘徊モードに移行
            stateNo = 1;
            //歩行マスカウントリセット
            walkCount = 0;

            DirectRand();

            count = 0;
        }


    }

    private void Walk()
    {
        if(walkCount < walkBeforeWait)
        {
            if (count < walkTime)
            {
                //アニメーションを開始
                this.myAnimator.SetInteger("State", 1);
                //前進
                transform.position += 2 * Time.deltaTime / walkTime * transform.forward;
                count += Time.deltaTime;
            }
            else
            {

                //位置合わせ、向き変える

                BeInCenter();

                DirectRand();

                count = 0;
                walkCount += 1;

            }
        }
        else
        {
            walkCount = 0;
            //待機に戻す
            stateNo = 0;
        }




    }
    private void Chase()
    {
        if(lastStateNo != stateNo)
        {
            DirectChase();
        }

        //進行不可でない場合
        if(stateNo == 2 )//|| stateNo == 4)
        {
            if(count < runTime)
            {
                //アニメーションを開始
                this.myAnimator.SetInteger("State", 2);

                transform.position += 2 * Time.deltaTime / runTime * transform.forward;
                count += Time.deltaTime;
            }
            else
            {
                //位置合わせ、向き変える

                BeInCenter();

                DirectChase();
            

                //目的地に近づいたら追跡をやめて、最後にプレイヤーが向いていた方向を見る
                if((transform.position - destination).magnitude < 1)
                {
                    //Debug.Log("EndChase!");
                    stateNo = 0;
                    transform.rotation = playerRotation;
                }
                else
                {
                
                }

                count = 0;
            
            }
        }

        
    }

    private void DirectChase()
    {
        #region destinationによって進行方向の優先度決める

        Vector3 direction = destination - transform.position;

        float dirX = direction.x;
        float dirZ = direction.z;
        float absX = Mathf.Abs(dirX);
        float absZ = Mathf.Abs(dirZ);

        //縦ラインを合わせる
        if (dirZ > 0 && absZ >= absX)
        {
            directArray[0] = Vector3.forward;
        }
        else if (dirZ < 0 && absZ >= absX)
        {
            directArray[0] = Vector3.back;
        }
        else if (dirX > 0 && absX >= absZ)
        {
            directArray[0] = Vector3.right;
        }
        else if (dirX < 0 && absX >= absZ)
        {
            directArray[0] = Vector3.left;
        }
        //横ラインを合わせる
        if (directArray[0] == Vector3.forward || directArray[0] == Vector3.back)
        {
            if (dirX > 0)
            {
                directArray[1] = Vector3.right;
            }
            else
            {
                directArray[1] = Vector3.left;
            }
        }
        else
        {
            if (dirZ > 0)
            {
                directArray[1] = Vector3.forward;
            }
            else
            {
                directArray[1] = Vector3.back;
            }
        }
        #endregion

        Vector3 nextDes = Vector3.one;

        int wallCo = 0;
        //壁があれば優先度低いやつ選ぶ
        for (int i = 0; i < 2; i++)
        {
            nextDes = transform.position + 2 * directArray[i];

            int X = Mathf.RoundToInt((nextDes.x + arrayInt) / 2);
            int Z = Mathf.RoundToInt((nextDes.z + arrayInt) / 2);

            if (!wallGenerator.wallArray[X, Z])
            {
                break;
            }
            else
            {
                wallCo += 1;
            }
        }

        if (wallCo == 2)
        {
            stateNo = 0;
        }
        else
        {
            transform.LookAt(nextDes);
        }


        //if (size > state3From && size < state3To)
        //{
        //    stateNo = 3;
        //}
    }

    private void DirectRand()
    {
        bool isAbleToMove = false;
        Vector3 nextDes = Vector3.one;

        while(!isAbleToMove)
        {
            //ランダムな方向を向く
            int rnd = UnityEngine.Random.Range(0, 4);

            switch (rnd)
            {
                case 0:
                    nextDes = transform.position + 2 * Vector3.forward;
                    break;
                case 1:
                    nextDes = transform.position + 2 * Vector3.right;
                    break;
                case 2:
                    nextDes = transform.position + 2 * Vector3.back;
                    break;
                case 3:
                    nextDes = transform.position + 2 * Vector3.left;
                    break;
            }

            int X = Mathf.RoundToInt((nextDes.x + arrayInt) / 2);
            int Z = Mathf.RoundToInt((nextDes.z + arrayInt) / 2);

            if(!wallGenerator.wallArray[X,Z])
            {
                isAbleToMove = true;
            }
        }

        transform.LookAt(nextDes);

    }

    private void BeInCenter()
    {
        //位置合わせ
        int newX = 2 * Mathf.RoundToInt((transform.position.x + arrayInt)/ 2) - arrayInt;
        int newZ = 2 * Mathf.RoundToInt((transform.position.z + arrayInt)/ 2) - arrayInt;
        transform.position = new Vector3(newX, transform.position.y, newZ);
    }

    private void Warp()
    {
        float distance = 0;
        while (distance < 10 || distance > 15 + enemyNo / 3)
        {
            int posX = UnityEngine.Random.Range(0, arrayInt);
            int posZ = UnityEngine.Random.Range(0, arrayInt);
            transform.position = new Vector3(2 * posX - arrayInt, firstY, 2 * posZ - arrayInt);
            if (!wallGenerator.wallArray[posX, posZ])
            {
                distance = (transform.position - player.transform.position).magnitude;
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if(view == null)
        {
            return;
        }

        if(stateNo == 5 && other.tag == "wall" )
        {
            

            int X = Mathf.RoundToInt((other.transform.position.x + arrayInt) / 2);
            int Z = Mathf.RoundToInt((other.transform.position.z + arrayInt) / 2);

            if(X > 0 && X < arrayInt-1 && Z > 0 && Z < arrayInt -1)
            {
                Destroy(other.gameObject);
                wallGenerator.wallArray[X, Z] = false;
            }

        }



        //enemy同士でぶつかったとき、
        if (other.tag == "enemy")
        {
            if(stateNo == 5 && other.GetComponent<EnemyController2>().stateNo ==5)
            {
                return;
            }

            //相手がstateNo5でないならば、または、自分のほうが大きいとき、または同じ大きさで自分のx座標が小さいとき
            if ((stateNo == 5 && other.GetComponent<EnemyController2>().stateNo !=5) ||
                (transform.localScale.y > other.transform.localScale.y
                || (Mathf.Abs(transform.localScale.y - other.transform.localScale.y) < Mathf.Epsilon && transform.position.x < other.transform.position.x)
                || (Mathf.Abs(transform.localScale.y - other.transform.localScale.y) < Mathf.Epsilon && transform.position.x == other.transform.position.x && transform.position.z < other.transform.position.z)))
            {
                //巨大化
                size += other.GetComponent<EnemyController2>().size;
                float length = Mathf.Pow(size, 0.5f);


                transform.localScale = Vector3.one * length;
                view.transform.localScale = new Vector3(viewSizeVec.x/length,viewSizeVec.y/length,2);
                view.transform.localPosition = viewPos/length;

                runSpeed = 1 + (size-1)* 0.03f;
                runTime = runTimeFirst * 1 / runSpeed;
            }
            else if ((stateNo5 != 5 && other.GetComponent<EnemyController2>().stateNo ==5) ||
                (transform.localScale.y < other.transform.localScale.y
                || (Mathf.Abs(transform.localScale.y - other.transform.localScale.y) < Mathf.Epsilon && transform.position.x > other.transform.position.x)
                || (Mathf.Abs(transform.localScale.y - other.transform.localScale.y) < Mathf.Epsilon && transform.position.x == other.transform.position.x && transform.position.z > other.transform.position.z)))
            {

                //サイズをリセット
                size = firstSize;
                float length = Mathf.Pow(size, 0.5f);
                transform.localScale = Vector3.one * length;
                view.transform.localScale = new Vector3(viewSizeVec.x / length, viewSizeVec.y / length, 2);
                view.transform.localPosition = viewPos / length;
                //足の速さリセット
                runSpeed = 1f;
                runTime = runTimeFirst * 1 / runSpeed;

                Warp();
                stateNo = 0;
                BeInCenter();
                

            }

        }
    }

    

    public void OnTriggerStayCallBack(Collider other)
    {
        if(stateNo == 5)
        {
            return;
        }
        //enemyが視界に入ったら
        //if (other.tag == "enemy" && other.GetComponent<EnemyController2>().stateNo == 3)
        //{
        //    Vector3 rayFrom = transform.position + 1.5f * Vector3.up;
        //    Vector3 rayTo = other.transform.position + 1.5f * Vector3.up;

        //    //Rayの作成
        //    Ray ray = new Ray(rayFrom, -(rayFrom - rayTo).normalized);

        //    //Rayの飛ばせる距離
        //    float distance = (rayFrom - rayTo).magnitude;

        //    //Rayが当たったオブジェクトの情報を入れる箱
        //    RaycastHit[] hits = Physics.RaycastAll(ray, distance);

        //    //Rayの可視化
        //    Debug.DrawLine(rayFrom, rayTo, Color.red);

        //    bool isAbleToDetect = true;

        //    foreach (var obj in hits)
        //    {
        //        switch (obj.collider.tag)
        //        {
        //            case "wall":
        //                isAbleToDetect = false;
        //                break;
        //            case "enemy":
        //                //isAbleToDetect = false;
        //                break;
        //        }
        //    }

        //    if (isAbleToDetect)
        //    {
        //        stateNo = 4;
        //        destination = other.transform.position;
        //        //count = runTime;
        //        playerRotation = other.transform.rotation;
        //        return;
        //    }
        //}

        if (other.tag == "player" && stateNo !=3 && stateNo != 4)
        {
            Vector3 myHead = transform.position + Vector3.up;//1.5f * Vector3.up * transform.localScale.y;
            Vector3 playerHead = player.transform.position + 1.5f * Vector3.up;
            
            //Rayの作成
            Ray ray = new Ray(myHead, (playerHead - myHead).normalized);

            //Rayの飛ばせる距離
            float distance = (playerHead - myHead).magnitude;

            //Rayが当たったオブジェクトの情報を入れる箱
            RaycastHit[] hits = Physics.RaycastAll(ray, distance);

            //Rayの可視化
            //Debug.DrawLine(myHead, playerHead, Color.red);

            bool isAbleToDetect = true;

            foreach (var obj in hits)
            {
                switch (obj.collider.tag)
                {
                    case "wall":
                        isAbleToDetect = false;
                        break;
                    case "enemy":
                        //isAbleToDetect = false;
                        break;
                }
            }

            if (isAbleToDetect)
            {
                //Debug.Log("player!");
                stateNo = 2;
                destination = player.transform.position;
                //count = runTime;
                playerRotation = player.transform.rotation;

            }
        }
    }
}