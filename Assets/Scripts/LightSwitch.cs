using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightSwitch : MonoBehaviour {

	// Use this for initialization
    public Vector3 rotationAxis;
    public ToggleLamp[] lightArray;
    public Transform offTransform;

    Transform onTransform;
    Camera mainCamera;
    MeshCollider thisCol;
    Transform thisTrans;

    Vector3 onPos;
    Vector3 offPos;
    Quaternion onRot;
    Quaternion offRot;
	void Start () 
    {
        mainCamera = Camera.main;
        thisCol = this.GetComponent<MeshCollider>();
        thisTrans = this.transform;

        onPos = this.transform.position;
        onRot = this.transform.rotation;
        offPos = offTransform.position;
        offRot = offTransform.rotation;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hit))
            {
                if(hit.collider == thisCol)
                {
                    ToggleLights();
                }
            }
        }
	}

    bool lightsOn = true;
    void ToggleLights()
    {
        lightsOn = !lightsOn;
        if (lightsOn)
        {
            thisTrans.position = onPos;
            thisTrans.rotation = onRot;
            foreach(ToggleLamp light in lightArray)
            {
                light.TurnOn();
            }
        }
        else
        {
            thisTrans.position = offPos;
            thisTrans.rotation = offRot;
            foreach (ToggleLamp light in lightArray)
            {
                light.TurnOff();
            }
        }
    }
}
