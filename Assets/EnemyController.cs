using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    //アニメーションするためのコンポーネントを入れる
    private Animator myAnimator;
    //Unityちゃんを移動させるコンポーネントを入れる（追加）
    private Rigidbody myRigidbody;
    //1秒の移動量
    private float runSpeedPerSec = 5;
    //速度の向き
    private Vector3 runDirection;
    //追跡中か否か
    public bool isChasing = false;
    //player
    private GameObject player;
    //目的地
    private Vector3 destination;
    //プレイヤーの向き
    private Quaternion playerRotation;
    //カウント
    private float count = 0.25f;
    // Start is called before the first frame update
    void Start()
    {
        //アニメータコンポーネントを取得
        this.myAnimator = GetComponent<Animator>();
        //Rigidbodyコンポーネントを取得（追加）
        this.myRigidbody = GetComponent<Rigidbody>();
        //player取得
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if(count < 0.4f)
        {
            //走るアニメーションを開始
            this.myAnimator.SetFloat("Speed", 1);
            
            transform.position += runSpeedPerSec * Time.deltaTime * runDirection;
            count += Time.deltaTime;
        }
        else
        {
            if(isChasing)
            {
                Vector3 posRelative = destination - transform.position;

                float xAbs = Mathf.Abs(posRelative.x);
                float zAbs = Mathf.Abs(posRelative.z);

                if (posRelative.x > 0 && xAbs > zAbs)
                {
                    runDirection = Vector3.right;
                    transform.rotation = Quaternion.Euler(0, 90, 0);
                }
                else if (posRelative.x < 0 && xAbs > zAbs)
                {
                    runDirection = Vector3.left;
                    transform.rotation = Quaternion.Euler(0, 270, 0);
                }
                else if (posRelative.z > 0 && zAbs >= xAbs)
                {
                    runDirection = Vector3.forward;
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                else if (posRelative.z < 0 && zAbs >= xAbs)
                {
                    runDirection = Vector3.back;
                    transform.rotation = Quaternion.Euler(0, 180, 0);
                }
                count = 0;
                
            }
            else
            {
                //走るアニメーションを終了
                this.myAnimator.SetFloat("Speed", 0);
                transform.rotation = playerRotation;
            }
            
            float newX = Mathf.RoundToInt(transform.position.x / 2);
            float newZ = Mathf.RoundToInt(transform.position.z / 2);
            newX *= 2;
            newZ *= 2;

            transform.position = new Vector3(newX, transform.position.y, newZ);
        }
        
        if((transform.position - destination).magnitude < 2)
        {
            isChasing = false;
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
