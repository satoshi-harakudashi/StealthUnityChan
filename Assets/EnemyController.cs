using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    //player
    private GameObject player;
    //wallgenerator
    private GameObject wallGererator;
    //アニメーションするためのコンポーネントを入れる
    private Animator myAnimator;
    //追跡中か否か
    public bool isChasing = false;
    //追跡中の1回の移動量
    private float runSpeed = 2;
    //追跡の走りの向き
    private Vector3 direction;
    //追跡中の1回の移動時間
    private float runTime = 0.35f;
    //歩きの1回の移動量
    private float walkSpeed = 2;




    //カウント
    private float count;
    
    //目的地
    private Vector3 destination;
    //プレイヤーの向き
    private Quaternion playerRotation;
    
    // Start is called before the first frame update
    void Start()
    {
        wallGererator = GameObject.Find("WallGenerator");
        //アニメータコンポーネントを取得
        this.myAnimator = GetComponent<Animator>();

        //player取得
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        //追跡
        if(isChasing)
        {
            if(count < runTime)
            {
                //走るアニメーションを開始
                this.myAnimator.SetFloat("Speed", 1);
            
                transform.position += runSpeed * Time.deltaTime / runTime * direction;
                count += Time.deltaTime;
            }
            else
            {
                //playerとの相対位置
                Vector3 posRelative = destination - transform.position;

                //相対位置各成分の絶対値
                float xAbs = Mathf.Abs(posRelative.x);
                float zAbs = Mathf.Abs(posRelative.z);

                #region 進行方向に壁があったら進まない

                int arrayX = Mathf.RoundToInt(transform.position.x / 2);
                int arrayZ = Mathf.RoundToInt(transform.position.z / 2);

                if (posRelative.x > 0 && posRelative.z > 0 && wallGererator.GetComponent<WallGenerator3>().wallArray[arrayX + 1, arrayZ])
                {
                    direction = Vector3.forward;
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                else if (posRelative.x > 0 && posRelative.z > 0 && wallGererator.GetComponent<WallGenerator3>().wallArray[arrayX, arrayZ + 1])
                {
                    direction = Vector3.right;
                    transform.rotation = Quaternion.Euler(0, 90, 0);
                }
                else if (posRelative.x > 0 && posRelative.z < 0 && wallGererator.GetComponent<WallGenerator3>().wallArray[arrayX + 1, arrayZ])
                {
                    direction = Vector3.back;
                    transform.rotation = Quaternion.Euler(0, 180, 0);
                }
                else if (posRelative.x > 0 && posRelative.z < 0 && wallGererator.GetComponent<WallGenerator3>().wallArray[arrayX, arrayZ - 1])
                {
                    direction = Vector3.right;
                    transform.rotation = Quaternion.Euler(0, 90, 0);
                }
                else if (posRelative.x < 0 && posRelative.z < 0 && wallGererator.GetComponent<WallGenerator3>().wallArray[arrayX - 1, arrayZ])
                {
                    direction = Vector3.back;
                    transform.rotation = Quaternion.Euler(0, 180, 0);
                }
                else if (posRelative.x < 0 && posRelative.z < 0 && wallGererator.GetComponent<WallGenerator3>().wallArray[arrayX, arrayZ - 1])
                {
                    direction = Vector3.left;
                    transform.rotation = Quaternion.Euler(0, 270, 0);
                }

                else if (posRelative.x < 0 && posRelative.z > 0 && wallGererator.GetComponent<WallGenerator3>().wallArray[arrayX - 1, arrayZ])
                {
                    direction = Vector3.forward;
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                else if (posRelative.x < 0 && posRelative.z > 0 && wallGererator.GetComponent<WallGenerator3>().wallArray[arrayX, arrayZ + 1])
                {
                    direction = Vector3.left;
                    transform.rotation = Quaternion.Euler(0, 270, 0);
                }
                #endregion
                #region 壁がなければ直線的に進む
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
                #endregion
                count = 0;

                BeInCenter();
            }

            //追跡終了
            if((transform.position - destination).magnitude < 2)
            {
                isChasing = false;
            }
        }


        
        else
        {
            //走るアニメーションを終了
            this.myAnimator.SetFloat("Speed", 0);
            transform.rotation = playerRotation; 
        }
    }

    public void BeInCenter()
    {
        int newX = Mathf.RoundToInt(transform.position.x / 2);
        int newZ = Mathf.RoundToInt(transform.position.z / 2);
        newX *= 2;
        newZ *= 2;

        int halfX = newX / 2;
        int halfZ = newZ / 2;

        transform.position = new Vector3(newX, transform.position.y, newZ);
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

            if ((transform.position - player.transform.position).magnitude >= 2)
            {
                isChasing = true;
                destination = player.transform.position;
                playerRotation = player.transform.rotation;
            }
            
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
