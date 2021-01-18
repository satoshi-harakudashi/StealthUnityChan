using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderController : MonoBehaviour
{
    GameObject enemy;
    EnemyController eneCon;


    // Start is called before the first frame update
    void Start()
    {
        enemy = transform.parent.gameObject;
        eneCon = enemy.GetComponent<EnemyController>();
    }

    // Update is called once per frame
    private void OnTriggerStay(Collider other)
    {
        eneCon.OnTriggerStayCallBack(other);
    }

    private void OnTriggerExit(Collider other)
    {
        eneCon.OnTriggerExitCallBack(other);
    }


}
