using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderController : MonoBehaviour
{
    GameObject enemy;
    EnemyController2 eneCon;


    // Start is called before the first frame update
    void Start()
    {
        
    }



    public void Initialize(GameObject other)
    {
        enemy = other;
        eneCon = enemy.GetComponentInParent<EnemyController2>();


    }


    // Update is called once per frame
    private void OnTriggerStay(Collider other)
    {
        if(eneCon == null) { return; }
        eneCon.OnTriggerStayCallBack(other);
    }

    //private void OnTriggerExit(Collider other)
    //{
    //    eneCon.OnTriggerExitCallBack(other);
    //}


}
