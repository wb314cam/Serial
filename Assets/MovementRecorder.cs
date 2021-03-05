using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DitzelGames.FastIK;
public class MovementRecorder : MonoBehaviour
{

    public string filename;
    public bool recording = false;
    public int chainLength;
    public int recordingWindow = 100;
    public int recordingIdx = 0;
    public int fileNo = 0;
    public Transform target;
    public bool recordIKchain = false;
    public GameObject model;
    protected List<Transform> transforms;
    //
    private void Init()
    {
        transforms = new List<Transform>();
        for(int i = 0; i <= chainLength; i++)
        { 
            Transform current;
            if (i == 0) current = transform;
            else current = transforms[i - 1].parent;
            transforms.Add(current);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }
    private void Update()
    {
        if(recording)
        {
            model.GetComponent<Renderer>().material.SetColor("_Color",Color.red);
        }
        else
        {
            model.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        string p = "";
        string r = "";
        if(recordingIdx>=recordingWindow)
        {
            recording = false;
            recordingIdx = 0;
        }
        if (Input.GetButtonDown("Jump") && (!recording))
        {
            recording = true;
            fileNo++;
        }
        if (recording)
        {
            recordingIdx++;
            if (recordIKchain)
            {
                for (int i = 0; i < chainLength; i++)
                {
                    Vector3 pv = transforms[i].position;
                    Vector3 rv = transforms[i].rotation.eulerAngles;
                    p += (pv.x.ToString() + "," + pv.y.ToString() + "," + pv.z.ToString());
                    if (i != (chainLength - 1)) p += ",";
                    else p += "\n";
                    r += (rv.x.ToString() + "," + rv.y.ToString() + "," + rv.z.ToString());
                    if (i != (chainLength - 1)) r += ",";
                    else r += "\n";
                }
                File.AppendAllText("./MovementData/" + filename + fileNo.ToString() + "Pos.csv", p);
                File.AppendAllText("./MovementData/" + filename + fileNo.ToString() + "Rot.csv", r);
            }
            else
            {
                p = target.position.x.ToString()+","+ target.position.y.ToString() + "," + target.position.z.ToString() + "\n";
                Vector3 lr = GetComponent<FastIKFabric>().lookRotation;
                r = lr.x + "," + lr.y + "," + lr.z + "\n";
                File.AppendAllText("./MovementData/" + filename + fileNo.ToString() + "Pos.csv", p);
                File.AppendAllText("./MovementData/" + filename + fileNo.ToString() + "Rot.csv", r);
            }
            
        }
    }
}
