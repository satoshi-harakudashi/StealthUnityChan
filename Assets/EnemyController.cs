using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    
    //wallgenerator
    static WallGenerator3 wallGenerator;
    //player
    static GameObject player;
    //unitychancontroller
    static UnityChanController unityChanController;
    //アニメーションするためのコンポーネントを入れる
    private Animator myAnimator;
    //追跡中か否か
    public bool isChasing = false;
    //待機中か否か
    private bool isWaiting = true;
    //1回の移動量
    private float runSpeed = 2;
    //移動の向き
    private Vector3 direction;
    //追跡中の1回の移動時間
    private float runTime = 0.3f;
    //通常の1回の移動時間
    private float walkTime = 0.8f;
    //待機時間
    private float waitTime = 3f;



    //移動用のカウント
    private float count;
    //歩いたマスのカウント
    int walkCount;
    //待機までに歩くマスの数
    private int walkBeforeWait = 5;
    
    //目的地
    private Vector3 destination;
    //プレイヤーの向き
    private Quaternion playerRotation;
    
    // Start is called before the first frame update
    void Start()
    {
        if(wallGenerator == null)
        {
            wallGenerator = GameObject.Find("WallGenerator").GetComponent<WallGenerator3>();
        }
        //player取得
        if(player == null)
        {
            player = GameObject.Find("Player");
            unityChanController = player.GetComponent<UnityChanController>();
        }
        
        

        //アニメータコンポーネントを取得
        this.myAnimator = GetComponent<Animator>();

        
    }

    // Update is called once per frame
    void Update()
    {
        if (unityChanController.isDead) { return; }

        if(isWaiting && count < waitTime) 
        {
            //アニメーションを開始
            this.myAnimator.SetInteger("State", 0);
            count += Time.deltaTime;
            return; 
        }
        else if(isWaiting && count >= waitTime)
        {
            isWaiting = false;
            walkCount = 0;
            //目的地設定するためのカウント代入
            count = walkTime;
        }

        EnemyMove(isChasing);
    }

    //1マス移動完了の処理
    private void BeInCenter()
    {   
        int newX = Mathf.RoundToInt(transform.position.x / 2);
        int newZ = Mathf.RoundToInt(transform.position.z / 2);
        newX *= 2;
        newZ *= 2;

        transform.position = new Vector3(newX, transform.position.y, newZ);

        if(isChasing && (destination-transform.position).magnitude < 1)
        {
            isChasing = false;
            isWaiting = true;
            transform.rotation = playerRotation;
        }

        if(!isChasing)
        {
            if (walkCount < walkBeforeWait)
            {
                walkCount += 1;
            }
            else
            {
                isWaiting = true;
            }
            
        }
    }

    private void EnemyMove(bool isChase)
    {
        int state;
        float moveTime;

        #region isChase応じて変数設定
        if (isChase)
        {
            moveTime = runTime;
            state = 2;
        }
        else
        {
            moveTime = walkTime;
            state = 1;


        }
        #endregion



        if (count < moveTime)
        {
            //アニメーションを開始
            this.myAnimator.SetInteger("State", state);

            transform.position += 2 * Time.deltaTime / moveTime * direction;
            count += Time.deltaTime;
        }
        else
        {
            BeInCenter();
            //歩行中の目的地設定
            if (!isChasing && !isWaiting)
            {
                bool isAbleToMove = false;
                while (!isAbleToMove)
                {
                    int rnd = Random.Range(0, 4);
                    switch (rnd)
                    {
                        case 0:
                            destination = transform.position + 2 * Vector3.forward;
                            break;
                        case 1:
                            destination = transform.position + 2 * Vector3.back;
                            break;
                        case 2:
                            destination = transform.position + 2 * Vector3.right;
                            break;
                        case 3:
                            destination = transform.position + 2 * Vector3.left;
                            break;
                    }
                    //進行方向に壁がなければ進行可能
                    int x = Mathf.RoundToInt(destination.x / 2);
                    int z = Mathf.RoundToInt(destination.z / 2);
                    if (!wallGenerator.wallArray[x, z]) { isAbleToMove = true; }

                    //自分の位置に壁があればwhileから出る
                    x = Mathf.RoundToInt(transform.position.x / 2);
                    z = Mathf.RoundToInt(transform.position.z / 2);
                    if (wallGenerator.wallArray[x, z]) { isAbleToMove = true; }
                }


            }
            
            //待機中でなければ目的地の再設定と向きの変更
            if (!isWaiting)
            {
                //目的地との相対位置
                Vector3 posRelative = destination - transform.position;

                //相対位置各成分の絶対値
                float xAbs = Mathf.Abs(posRelative.x);
                float zAbs = Mathf.Abs(posRelative.z);


                int arrayX = Mathf.RoundToInt(transform.position.x / 2);
                int arrayZ = Mathf.RoundToInt(transform.position.z / 2);

                if (posRelative.x > 0 && posRelative.z > 0 && wallGenerator.wallArray[arrayX + 1, arrayZ])
                {
                    direction = Vector3.forward;
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                else if (posRelative.x > 0 && posRelative.z > 0 && wallGenerator.wallArray[arrayX, arrayZ + 1])
                {
                    direction = Vector3.right;
                    transform.rotation = Quaternion.Euler(0, 90, 0);
                }
                else if (posRelative.x > 0 && posRelative.z < 0 && wallGenerator.wallArray[arrayX + 1, arrayZ])
                {
                    direction = Vector3.back;
                    transform.rotation = Quaternion.Euler(0, 180, 0);
                }
                else if (posRelative.x > 0 && posRelative.z < 0 && wallGenerator.wallArray[arrayX, arrayZ - 1])
                {
                    direction = Vector3.right;
                    transform.rotation = Quaternion.Euler(0, 90, 0);
                }
                else if (posRelative.x < 0 && posRelative.z < 0 && wallGenerator.wallArray[arrayX - 1, arrayZ])
                {
                    direction = Vector3.back;
                    transform.rotation = Quaternion.Euler(0, 180, 0);
                }
                else if (posRelative.x < 0 && posRelative.z < 0 && wallGenerator.wallArray[arrayX, arrayZ - 1])
                {
                    direction = Vector3.left;
                    transform.rotation = Quaternion.Euler(0, 270, 0);
                }

                else if (posRelative.x < 0 && posRelative.z > 0 && wallGenerator.wallArray[arrayX - 1, arrayZ])
                {
                    direction = Vector3.forward;
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                else if (posRelative.x < 0 && posRelative.z > 0 && wallGenerator.wallArray[arrayX, arrayZ + 1])
                {
                    direction = Vector3.left;
                    transform.rotation = Quaternion.Euler(0, 270, 0);
                }


                else
                if (posRelative.x > 0 && xAbs > zAbs)
                {
                    direction = Vector3.right;
                    transform.rotation = Quaternion.Euler(0, 90, 0);
                }
                else if (posRelative.x < 0 && xAbs > zAbs)
                {
                    direction = Vector3.left;
                    transform.rotation = Quaternion.Euler(0, 270, 0);
                }
                else if (posRelative.z > 0 && zAbs >= xAbs)
                {
                    direction = Vector3.forward;
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                else if (posRelative.z < 0 && zAbs >= xAbs)
                {
                    direction = Vector3.back;
                    transform.rotation = Quaternion.Euler(0, 180, 0);
                }

            }



            count = 0;

            
        }




    }




    public void OnTriggerStayCallBack(Collider other)
    {
        //playerが視界に入ったら
        if (other.tag == "player")
        {
            //Rayの作成
            Ray ray = new Ray(transform.position, (player.transform.position - transform.position).normalized);

            //Rayが当たったオブジェクトの情報を入れる箱
            RaycastHit hit;

            //Rayの飛ばせる距離
            float distance = (player.transform.position - transform.position).magnitude;

            //Rayの可視化
            Debug.DrawLine(transform.position, player.transform.position - transform.position, Color.red);

            //もしRayにオブジェクトが衝突したら
            if (Physics.Raycast(ray, out hit, distance))
            {
                //Rayが当たったオブジェクトのtagがwallだったら終了
                if (hit.collider.tag == "wall") { return; }
            }

            isChasing = true;
            isWaiting = false;
            destination = player.transform.position;
            playerRotation = player.transform.rotation;

            
        }
    }

    public void OnTriggerExitCallBack(Collider other)
    {
        //playerが視界から出たら
        if (other.tag == "player")
        {
            //isChasing = false;
        }
    }
}
