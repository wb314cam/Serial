using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SerialControl : MonoBehaviour
{
    public enum DataStreamType { 
    Acceleration,
    Gyroscope,
    EyeRotation,
    NumDataStreams
    };
    public DataStreamType dataStreamType;
    public SerialReader serialReader;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Quaternion q = new Quaternion();
        Matrix4x4 m = new Matrix4x4();
        switch(dataStreamType)
        {
            case DataStreamType.Acceleration:
                Vector3 acc = serialReader.acceleration.normalized;
                transform.position = Vector3.Lerp(transform.position, acc * 1.5f, Time.fixedDeltaTime * 10f);
                break;
            case DataStreamType.Gyroscope:
                Vector3 v = serialReader.angularVelocity;

                q = Quaternion.Euler( new Vector3(v.x, v.z, v.y) * Time.fixedDeltaTime * 2 * Mathf.Rad2Deg);
                transform.rotation = transform.rotation * q;
                break;
            case DataStreamType.EyeRotation:
                break;
            default:
                break;
        }
    }
}
