using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    [SerializeField] float speed = 360f;
    [SerializeField] Vector3 angle;


    void Update()
    {
        transform.Rotate(angle * speed * Time.deltaTime);
    }
}
