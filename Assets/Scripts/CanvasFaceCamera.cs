using UnityEngine;
using System.Collections;

public class CanvasFaceCamera : MonoBehaviour {

	public Transform mainCam;
	Transform thisTrans;
	// Use this for initialization
	void Start () {
		thisTrans = this.transform;
	}
	
	// Update is called once per frame
	void Update () {
		thisTrans.rotation = Quaternion.LookRotation(thisTrans.position - mainCam.position );
	}
}
