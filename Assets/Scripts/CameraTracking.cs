using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTracking : MonoBehaviour
{
    public Transform target;

    float distX, distZ;

    public float speed = 1f;

    Vector3 pos;

    void Start()
    {
        distX = transform.position.x - target.position.x;
        distZ = transform.position.z - target.position.z;
    }
    void Update()
    {
        pos.x = target.position.x + distX;
        pos.y = transform.position.y;
        pos.z = target.position.z + distZ;

        this.transform.position = Vector3.Lerp(transform.position, pos, speed * Time.deltaTime);

    }
}
