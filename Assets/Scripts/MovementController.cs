using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public float MovementSpeed => movementSpeed;
    [SerializeField] private float movementSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float translation = Input.GetAxis("Vertical") * movementSpeed;
        translation *= Time.deltaTime;
        transform.Translate(translation, 0, 0);
    }
}
