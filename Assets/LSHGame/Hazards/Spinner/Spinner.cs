using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 1f;
   

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(new Vector3(0, 0, rotationSpeed));
    }

    
}
