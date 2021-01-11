using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPointMovement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 2f, Vector3.down, out hit, Mathf.Infinity))
        {
            transform.position = hit.point;
        }
    }
}
