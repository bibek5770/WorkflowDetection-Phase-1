using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class MirrorTrack : MonoBehaviour {

    public Vector3 mirrorNormal;
    private Transform thisTrans;
    private Transform mainTrans;
	// Use this for initialization
	void Start () 
    {
        thisTrans = this.transform;
        mainTrans = Camera.main.transform;
	}
	
	// Update is called once per frame
	void Update () 
    {
        Vector3 lookDir = mainTrans.position - thisTrans.position;
        Vector3 refDir = Quaternion.AngleAxis(180, mirrorNormal) * lookDir;
        thisTrans.rotation = Quaternion.LookRotation(refDir);

	}
}
