using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityChanController : MonoBehaviour
{
    //アニメーションするためのコンポーネントを入れる
    private Animator myAnimator;
    //Unityちゃんを移動させるコンポーネントを入れる（追加）
    private Rigidbody myRigidbody;
    //速度の大きさ
    private float runSpeed = 5;
    //updateごとの移動量
    private float runSpeedByUpdate = 0.1f;
    //速度の向き
    private Vector3 runDirection;
    //移動カウント
    private int runCount;

    // Use this for initialization
    void Start()
    {
        //アニメータコンポーネントを取得
        this.myAnimator = GetComponent<Animator>();
        //Rigidbodyコンポーネントを取得（追加）
        this.myRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        GoToNextRoom();



    }
    private void GoToNextRoom()
    {

        if(runCount < 20)
        {
            //走るアニメーションを開始
            this.myAnimator.SetFloat("Speed", 1);

            transform.position += runSpeedByUpdate * runDirection;
            runCount += 1;
        }
        else
        {
            

            //unitychanの方向を変える
            if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.rotation = Quaternion.Euler(0, 90, 0);
                runDirection = Vector3.right;
                runCount = 0;
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.rotation = Quaternion.Euler(0, 270, 0);
                runDirection = Vector3.left;
                runCount = 0;
            }
            else if (Input.GetKey(KeyCode.UpArrow))
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                runDirection = Vector3.forward;
                runCount = 0;
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
                runDirection = Vector3.back;
                runCount = 0;
            }
            else
            {
                //走るアニメーションを終了
                this.myAnimator.SetFloat("Speed", 0);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //enemyの視界に入ったら
        if(other.tag == "view")
        {
            //enemyのisChasingをtrueにする
            GameObject oya = other.transform.parent.gameObject;
            oya.GetComponent<EnemyController>().isChasing = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        //enemyの視界から出たら
        if (other.tag == "view")
        {
            //enemyのisChasingをfalseにする
            GameObject oya = other.transform.parent.gameObject;
            oya.GetComponent<EnemyController>().isChasing = false;
        }
    }
}
