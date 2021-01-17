using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    //アニメーションするためのコンポーネントを入れる
    private Animator myAnimator;
    //Unityちゃんを移動させるコンポーネントを入れる（追加）
    private Rigidbody myRigidbody;
    //速度の大きさ
    private float runSpeed = 5;
    //速度の向き
    private Vector3 runDirection;
    //追跡中のブール
    public bool isChasing = false;
    //player
    private GameObject player;
    //目的地
    private Vector3 destination;
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
        if(isChasing)
        {
            transform.LookAt(player.transform);
            runDirection = player.transform.position - transform.position;
            runDirection = runDirection.normalized;
            this.myRigidbody.velocity = runSpeed * runDirection;
            //走るアニメーションを開始
            this.myAnimator.SetFloat("Speed", 1);
        }
        else
        {
            this.myRigidbody.velocity = Vector3.zero;
            //走るアニメーションを終了
            this.myAnimator.SetFloat("Speed", 0);
        }
    }

    public void OnTriggerEnterCallBack(Collider other)
    {
        //playerが視界に入ったら
        if (other.tag == "player")
        {
            isChasing = true;
            //目的地決定
            destination = other.transform.position;
            destination -= 2 * (Vector3.forward + Vector3.right);

        }
    }

    public void OnTriggerExitCallBack(Collider other)
    {
        //playerが視界から出たら
        if (other.tag == "player")
        {
            isChasing = false;
        }
    }
}
