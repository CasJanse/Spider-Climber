using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public float MovementSpeed => movementSpeed;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float heightOffset;

    [SerializeField] private Transform[] endPoints;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Forward movement
        float translation = Input.GetAxis("Vertical") * movementSpeed;
        translation *= Time.deltaTime;
        transform.Translate(translation, 0, 0);

        // Body y position
        float averageLegHeight = GetAverageLegHeight();
        float newHeight = averageLegHeight + heightOffset;
        transform.position = new Vector3(transform.position.x, newHeight, transform.position.z);

        // Body rotation
        //float heightDifference = GetAverageLeftLegHeight() - GetAverageRightLegHeight();
        ////Debug.Log(heightDifference);
        //transform.rotation = Quaternion.Euler(heightDifference * -20, 0, 0);
    }

    float GetAverageLegHeight() 
    {
        float totalHeight = 0;
        for (int i = 0; i < endPoints.Length; i++)
        {
            totalHeight += endPoints[i].position.y;
        }
        float averageHeight =  totalHeight / endPoints.Length;
        return averageHeight;
    }

    float GetAverageLeftLegHeight() 
    {
        float totalHeight = 0;
        for (int i = 0; i < endPoints.Length / 2; i++)
        {
            totalHeight += endPoints[i].position.y;
        }
        float averageHeight = totalHeight / (endPoints.Length / 2);
        return averageHeight;
    }

    float GetAverageRightLegHeight()
    {
        float totalHeight = 0;
        for (int i = endPoints.Length / 2 - 1; i < endPoints.Length; i++)
        {
            totalHeight += endPoints[i].position.y;
        }
        float averageHeight = totalHeight / (endPoints.Length / 2);
        return averageHeight;
    }
}
