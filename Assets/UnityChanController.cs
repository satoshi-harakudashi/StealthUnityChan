using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityChanController : MonoBehaviour
{
    //アニメーションするためのコンポーネントを入れる
    private Animator myAnimator;
    //1秒の移動量
    private float runSpeedPerSec = 8;
    //速度の向き
    private Vector3 runDirection;
    //カウント
    private float count;

    // Use this for initialization
    void Start()
    {
        //アニメータコンポーネントを取得
        this.myAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        GoToNextRoom();
    }
    private void GoToNextRoom()
    {

        if(count < 0.25f)
        {
            //走るアニメーションを開始
            this.myAnimator.SetFloat("Speed", 1);

            transform.position += runSpeedPerSec * Time.deltaTime * runDirection;
            count += Time.deltaTime;
        }
        else
        {
            //unitychanの方向を変える
            if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.rotation = Quaternion.Euler(0, 90, 0);
                runDirection = Vector3.right;
                count = 0;
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.rotation = Quaternion.Euler(0, 270, 0);
                runDirection = Vector3.left;
                count = 0;
            }
            else if (Input.GetKey(KeyCode.UpArrow))
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                runDirection = Vector3.forward;
                count = 0;
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
                runDirection = Vector3.back;
                count = 0;
            }
            else
            {
                float newX = Mathf.RoundToInt(transform.position.x/2);
                float newZ = Mathf.RoundToInt(transform.position.z/2);
                newX *= 2;
                newZ *= 2;

                transform.position = new Vector3(newX,transform.position.y,newZ);
                //走るアニメーションを終了
                this.myAnimator.SetFloat("Speed", 0);
            }
        }
    }


}
