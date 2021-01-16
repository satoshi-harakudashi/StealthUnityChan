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

    // Start is called before the first frame update
    void Start()
    {
        //アニメータコンポーネントを取得
        this.myAnimator = GetComponent<Animator>();
        //Rigidbodyコンポーネントを取得（追加）
        this.myRigidbody = GetComponent<Rigidbody>();
        //player取得
        player = GameObject.Find("player");
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
}
