using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UnityChanController : MonoBehaviour
{
    private GameObject wallGererator;
    //アニメーションするためのコンポーネントを入れる
    private Animator myAnimator;
    //1秒の移動量
    private float runSpeedPerSec = 8;
    //速度の向き
    private Vector3 runDirection;
    //カウント
    private float count = 0.25f;

    public int arrayInt;
    public int floorNo;
    //クリア判定
    public bool isClear = false;
    //ゲームオーバー判定
    public bool isDead = false;
    //クリアせずに次の階へ行く
    public bool isNext = false;
    //ポーズ画面
    public bool isPose = false;
    //
    private float alpha = 0;
    private float red = 1;

    //ゲームオーバーテキスト
    private Text gameOverText;
    //インフォメーションテキスト
    private Text informationText;
    //ポーズテキスト
    private Text poseText;
    //ゲームオーバーパネル
    private GameObject gameOverPanel;

    private Text resetText;

    // Use this for initialization
    void Start()
    {
        wallGererator = GameObject.Find("WallGenerator");
        
        //アニメータコンポーネントを取得
        myAnimator = GetComponentInChildren<Animator>();
        gameOverText = GameObject.Find("GameOverText").GetComponent<Text>();
        informationText = GameObject.Find("InformationText").GetComponent<Text>();
        poseText = GameObject.Find("PoseText").GetComponent<Text>();
        gameOverPanel = GameObject.Find("GameOverPanel");
        resetText = GameObject.Find("ResetText").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        informationText.text = floorNo + "階";

        if(isPose)
        {
            poseText.text = "Pose!";
            myAnimator.SetFloat("AnimationSpeed", 0);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isPose = false;
            }
            return;

        }
        else
        {
            poseText.text = "";
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isPose = true;
            }

        }




        if (isClear)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                //次の階に上がる処理
                GoToNextFloor();

            }
            return;
        }
        else if (isDead) 
        {
            myAnimator.SetFloat("AnimationSpeed", 0);

            if(alpha < 1)
            {
                alpha += 0.0005f;
                gameOverPanel.GetComponent<Image>().color = new Color(1,0,0,alpha);
            }
            else
            {
                if(red > 0)
                {
                    red -= 0.0005f;
                    resetText.color = new Color(red, 0, 0, 1);
                }
            }
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SceneManager.LoadScene("OpeningScene");
            }

            return; 
        }
        else if (isNext)
        {
            //次の階に上がる処理
            GoToNextFloor();
        }


        if (Input.GetKeyDown(KeyCode.Return))
        {
            //isNext = true;            
        }
        else
        {
            GoToNextRoom();
        }

        
    }

    private void GoToNextFloor()
    {
        wallGererator.GetComponent<WallGenerator3>().floorNo += 1;
        wallGererator.GetComponent<WallGenerator3>().arrayInt += 1;
        wallGererator.GetComponent<WallGenerator3>().PrepareThisFloor();
        isClear = false;
        isDead = false;
        isNext = false;
        gameOverText.GetComponent<Text>().text = "";

        count = 0.25f;
        int newX = Mathf.RoundToInt((transform.position.x + arrayInt) / 2);
        int newZ = Mathf.RoundToInt((transform.position.z + arrayInt) / 2);
        newX = 2 * newX - arrayInt;
        newZ = 2 * newZ - arrayInt;
        transform.position = new Vector3(newX, transform.position.y, newZ);

        myAnimator.SetInteger("State", 0);
    }

    private void GoToNextRoom()
    {
        
        if(count < 0.25f)
        {
            //走るアニメーションを開始
            myAnimator.SetInteger("State", 2);
            myAnimator.SetFloat("AnimationSpeed", 2);

            transform.position += runSpeedPerSec * Time.deltaTime * runDirection;
            count += Time.deltaTime;
        }
        else
        {
            int newX = Mathf.RoundToInt((transform.position.x + arrayInt)/2);
            int newZ = Mathf.RoundToInt((transform.position.z + arrayInt)/2);
            
            //unitychanの方向を変える
            if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.rotation = Quaternion.Euler(0, 90, 0);
                runDirection = Vector3.right;
                if (!wallGererator.GetComponent<WallGenerator3>().wallArray[newX + 1, newZ]) {count = 0;}                
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.rotation = Quaternion.Euler(0, 270, 0);
                runDirection = Vector3.left;
                if (!wallGererator.GetComponent<WallGenerator3>().wallArray[newX - 1, newZ]) {count = 0; }
            }
            else if (Input.GetKey(KeyCode.UpArrow))
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                runDirection = Vector3.forward;
                if(!wallGererator.GetComponent<WallGenerator3>().wallArray[newX,newZ + 1]) {count = 0; }
                
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
                runDirection = Vector3.back;
                if (!wallGererator.GetComponent<WallGenerator3>().wallArray[newX, newZ - 1]) {count = 0; }
            }
            else
            {
                //走るアニメーションを終了
                myAnimator.SetInteger("State", 0);
                myAnimator.SetFloat("AnimationSpeed", 1);
            }
            newX = 2 * newX - arrayInt;
            newZ = 2 * newZ - arrayInt;
            transform.position = new Vector3(newX,transform.position.y,newZ);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!isClear && other.tag == "enemy")
        {
            //ゲームオーバー
            string ordinalNumberText = "th";
            if(floorNo%10 == 1 && floorNo%100 != 11)
            {
                ordinalNumberText = "st";
            }
            else if(floorNo%10 == 2)
            {
                ordinalNumberText = "nd";
            }
            else if(floorNo%10 == 3)
            {
                ordinalNumberText = "rd";
            }

            gameOverText.GetComponent<Text>().text = "You were killed on " + floorNo + ordinalNumberText +" floor";
            isDead = true;

            
        }
        else if(!isDead && other.tag == "goal")
        {
            //次の階層へ
            gameOverText.GetComponent<Text>().text = "CLEAR";
            isClear = true;


        }
    }
}
