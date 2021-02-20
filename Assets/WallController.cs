using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallController : MonoBehaviour
{
    static UnityChanController uniCon;



    // Start is called before the first frame update
    void Start()
    {
        if (uniCon == null)
        {
            uniCon = GameObject.Find("Player").GetComponent<UnityChanController>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (uniCon.isClear || uniCon.isNext)
        //{
        //    Destroy(this.gameObject);
        //}
    }
}
