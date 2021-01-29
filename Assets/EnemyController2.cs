using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    //wallgenerator
    static WallGenerator3 wallGenerator;
    //player
    static GameObject player;
    //viewprefab
    public GameObject viewPrefab;
    //unitychancontroller
    static UnityChanController unityChanController;
    //アニメーションするためのコンポーネントを入れる
    private Animator myAnimator;
    //追跡中か否か
    public bool isChasing = false;
    //待機中か否か
    private bool isWaiting = true;
    //速度
    private float runSpeed = 1;
    //移動の向き
    private Vector3 direction;
    //追跡中の1回の移動時間(初期)
    private float runTimeFirst = 0.5f;
    //追跡中の1回の移動時間;
    private float runTime;
    //通常の1回の移動時間
    private float walkTime = 0.8f;
    //待機時間
    private float waitTime = 3f;
    //サイズ
    private int size = 1;


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
        if (wallGenerator == null)
        {
            wallGenerator = GameObject.Find("WallGenerator").GetComponent<WallGenerator3>();
        }
        //player取得
        if (player == null)
        {
            player = GameObject.Find("Player");
            unityChanController = player.GetComponent<UnityChanController>();
        }

        //アニメータコンポーネントを取得
        this.myAnimator = GetComponent<Animator>();

        GameObject view = Instantiate(viewPrefab);

        view.transform.parent = transform;


    }
    // Update is called once per frame
    void Update()
    {

    }

}