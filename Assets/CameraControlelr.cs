using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControlelr : MonoBehaviour
{
    //Unityちゃんのオブジェクト
    private GameObject unitychan;
    //Unityちゃんとカメラの距離
    private float difference;

    // Use this for initialization
    void Start()
    {
        //Unityちゃんのオブジェクトを取得
        unitychan = GameObject.Find("Player");

        //Unityちゃんとカメラの位置（y座標）の差を求める
        difference =  this.transform.position.y - unitychan.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        //Unityちゃんの位置に合わせてカメラの位置を移動
        transform.position = new Vector3(this.unitychan.transform.position.x, difference, this.unitychan.transform.position.z);


    }
}
