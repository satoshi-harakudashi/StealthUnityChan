﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class markController : MonoBehaviour
{
    Material material;
    public float count;
    static UnityChanController uniCon;

    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<MeshRenderer>().material;
        if(uniCon == null)
        {
            uniCon = GameObject.Find("Player").GetComponent<UnityChanController>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(uniCon.isClear || uniCon.isNext)
        {
            Destroy(this.gameObject);
        }
        else if(!uniCon.isDead)
        {
            count += Time.deltaTime;
            material.SetFloat("_Count",count);
        }
    }
}
