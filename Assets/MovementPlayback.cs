using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEditorInternal;
using DitzelGames.FastIK;
public class MovementPlayback : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = Vector3.up;
    public string[] posfiles;
    public string[] rotfiles;
    public int chainLength;
    public float playbackSpeed = 1;
    public float damping = 1;
    public float exaggeration = 1;
    List<Transform> transforms;
    Vector3[] position = new Vector3[2];
    Vector3[] rotation = new Vector3[2];
    private StreamReader posfile;
    private StreamReader rotfile;
    public string folder = "./MovementData/";
    public bool playing = false;
    public List<Vector3> chpos = new List<Vector3>();
    public List<Vector3> chrot = new List<Vector3>();
    public int f;
    private void Init()
    { 
        posfiles = Directory.GetFiles(folder, "*Pos.csv");
        rotfiles = Directory.GetFiles(folder, "*Rot.csv");
        transforms = new List<Transform>();
        for (int i = 0; i <= chainLength; i++)
        {
            Transform current;
            if (i == 0) current = transform;
            else current = transforms[i - 1].parent;
            transforms.Add(current);
        }

    }

    private void LoadFile(string f, string p, string r)
    {
        posfile = new StreamReader(p); 
        rotfile = new StreamReader(r);
    }
    // Start is called before the first frame update
    void Start()
    {
        Init();
       
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Input.GetButtonDown("Jump") && !playing)
        {
            LoadFile(folder, posfiles[(int)Mathf.Clamp(f, 0, (float)posfiles.Length-1)], rotfiles[(int)Mathf.Clamp(f, 0, (float)rotfiles.Length - 1)]);
            playing = true;
        }

        if(playing)
        {
            string p = posfile.ReadLine();
            string r = rotfile.ReadLine();
            if (!rotfile.EndOfStream || !posfile.EndOfStream)
            {
                char delim = ',';
                string[] pos = p.Split(delim);
                string[] rot = r.Split(delim);
                chpos = new List<Vector3>();
                chrot = new List<Vector3>();
                for (int i = 0; i < (chainLength * 3); i += 3)
                {
                    chpos.Add(new Vector3(float.Parse(pos[i]),
                                          float.Parse(pos[i + 1]),
                                          float.Parse(pos[i + 2])));
                    chrot.Add(new Vector3(float.Parse(rot[i]),
                                          float.Parse(rot[i + 1]),
                                          float.Parse(rot[i + 2])));
                }
                //  for (int i = 0; i < chainLength; i++)
                // {
                //    transforms[i].position = chpos[i];
                //     transforms[i].rotation = Quaternion.Euler(chrot[i]);
                // }
                //if(chpos[0]!=null && chrot[0] != null){
                
                target.position = Vector3.Lerp(target.position, exaggeration*chpos[0] + offset, Time.fixedDeltaTime*damping);
                GetComponent<FastIKFabric>().lookRotation = Quaternion.Slerp(Quaternion.Euler(GetComponent<FastIKFabric>().lookRotation), Quaternion.Euler(chrot[0]), Time.fixedDeltaTime*damping).eulerAngles;
              //  Quaternion q = new Quaternion(-1f, 0f, -1f, 0);
              //      target.rotation = Quaternion.Euler(chrot[0]+new Vector3(0f,-180f,0f));
               // }
            }
            if (rotfile.EndOfStream || posfile.EndOfStream)
            {
                playing = false;
            }
        }
    }
}
