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
        int layerMask = 1 << 6;
        layerMask = ~layerMask;

        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 30f, Vector3.down, out hit, Mathf.Infinity, layerMask))
        {
            transform.position = hit.point;
        }
    }
}
