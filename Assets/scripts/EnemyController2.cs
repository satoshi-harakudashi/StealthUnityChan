using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController2 : MonoBehaviour
{
    //待機状態、徘徊状態、追跡状態



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

    //状態変数　0:待機、1:徘徊、2:追跡
    public int stateNo = 0;
    //前回の状態変数
    private int lastStateNo = 0;

    //走行速度
    private float runSpeed = 1;
    
    //追跡中の1回の移動時間(初期)
    private float runTimeFirst = 0.6f;
    //追跡中の1回の移動時間;
    private float runTime;
    //通常の1回の移動時間
    private float walkTime = 2f;
    //待機時間
    private float waitTime = 3f;

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

    //目的地
    private Vector3 destination;
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
        else if(stateNo == 2)
        {
            Chase();
        }

        lastStateNo = stateNo;
    }

    

    private void Wait()
    {
        if(count < waitTime)
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

        //進行負荷でない場合
        if(stateNo == 2)
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
                    Debug.Log("EndChase!");
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
            //追跡終了
            stateNo = 0;
            return;
        }
        else
        {
            transform.LookAt(nextDes);
        }
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


    private void OnTriggerStay(Collider other)
    {
        if(view == null)
        {
            return;
        }

        //enemy同士でぶつかったとき、
        if (other.tag == "enemy")
        {
            //自分のほうが大きいとき、または同じ大きさで自分のx座標が小さいとき
            if (transform.localScale.y > other.transform.localScale.y
                || (Mathf.Abs(transform.localScale.y - other.transform.localScale.y) < Mathf.Epsilon && transform.position.x < other.transform.position.x)
                || (Mathf.Abs(transform.localScale.y - other.transform.localScale.y) < Mathf.Epsilon && transform.position.x == other.transform.position.x && transform.position.z < other.transform.position.z))
            {
                //巨大化
                size += other.GetComponent<EnemyController2>().size;
                float length = Mathf.Pow(size, 0.5f);


                transform.localScale = Vector3.one * length;
                view.transform.localScale = new Vector3(viewSizeVec.x/length,viewSizeVec.y/length,2);
                view.transform.localPosition = viewPos/length;

                runSpeed += 0.01f;
                runTime = runTimeFirst * 1 / runSpeed;
            }
            else if (transform.localScale.y < other.transform.localScale.y
                || (Mathf.Abs(transform.localScale.y - other.transform.localScale.y) < Mathf.Epsilon && transform.position.x > other.transform.position.x)
                || (Mathf.Abs(transform.localScale.y - other.transform.localScale.y) < Mathf.Epsilon && transform.position.x == other.transform.position.x && transform.position.z > other.transform.position.z))
            {

                //サイズをリセット
                size = firstSize;
                transform.localScale = Vector3.one * size;
                view.transform.localScale = viewSizeVec;
                view.transform.localPosition = viewPos;
                //足の速さリセット
                runSpeed = 1f;
                runTime = runTimeFirst * 1 / runSpeed;

                float distance = 0;
                while (distance < 5)
                {
                    int posX = UnityEngine.Random.Range(0, arrayInt);
                    int posZ = UnityEngine.Random.Range(0, arrayInt);
                    transform.position = new Vector3(2 * posX - arrayInt, firstY, 2 * posZ - arrayInt);
                    if (!wallGenerator.wallArray[posX, posZ])
                    {
                        distance = (transform.position - player.transform.position).magnitude;
                    }
                }

                BeInCenter();
                stateNo = 0;

            }

        }
    }




    public void OnTriggerStayCallBack(Collider other)
    {
        //playerが視界に入ったら
        if (other.tag == "player")
        {
            Vector3 myHead = transform.position + 1.5f * Vector3.up * transform.localScale.y;
            Vector3 playerHead = player.transform.position + 1.5f * Vector3.up;


            
            //Rayの作成
            Ray ray = new Ray(myHead, (playerHead - myHead).normalized);

            //Rayの飛ばせる距離
            float distance = (playerHead - myHead).magnitude;

            //Rayが当たったオブジェクトの情報を入れる箱
            RaycastHit[] hits = Physics.RaycastAll(ray, distance);

            //Rayの可視化
            Debug.DrawLine(myHead, playerHead, Color.red);

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