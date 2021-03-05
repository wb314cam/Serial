using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Track : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public bool invertX;
    public bool invertY;
    public bool invertZ;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 v = (target.localPosition) + offset;
        if (invertX) v.x = v.x * -1;
        if (invertY) v.y = v.y * -1;
        if (invertZ) v.z = v.z * -1;
        transform.localPosition = v;
    }
}
