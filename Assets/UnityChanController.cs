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
    //速度の向き
    private Vector3 runDirection;
    //走り判定
    private bool isRunning;

    // Use this for initialization
    void Start()
    {
        //アニメータコンポーネントを取得
        this.myAnimator = GetComponent<Animator>();

        

        //Rigidbodyコンポーネントを取得（追加）
        this.myRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //unitychanの方向を変える
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.rotation = Quaternion.Euler(0, 90, 0);
            runDirection = Vector3.right;
            isRunning = true;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.rotation = Quaternion.Euler(0, 270, 0);
            runDirection = Vector3.left;
            isRunning = true;
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            runDirection = Vector3.forward;
            isRunning = true;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            runDirection = Vector3.back;
            isRunning = true;
        }
        else
        {
            isRunning = false;
        }

        //Unityちゃんに速度を与える
        if(isRunning)
        {
            this.myRigidbody.velocity = runSpeed * runDirection;
            //走るアニメーションを開始
            this.myAnimator.SetFloat("Speed", 1);
        }
        else
        {
            this.myRigidbody.velocity = Vector3.zero;
            //走るアニメーションを開始
            this.myAnimator.SetFloat("Speed", 0);
        }
        
    }
}
