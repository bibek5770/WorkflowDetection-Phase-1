using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class AuraDetector : MonoBehaviour {

	SphereCollider thisCol;
    Transform colTrans;
	Transform thisTrans;
	public Canvas HoverCanvas;
	public Text HoverText;
	public Canvas AuraCanvas;
	public Text AuraText;

	public enum AuraType
	{
		PC = 1,
		NPC = 2
	};

	public AuraType auraType = AuraType.NPC;

	void Start () 
	{
		thisCol = GetComponent<SphereCollider>();
		//thisTrans = this.gameObject.transform;
		//HoverText = HoverCanvas.GetComponentInChildren<Text>();
		//AuraText = AuraCanvas.GetComponentInChildren<Text>();
        thisTrans = Camera.main.transform;
        colTrans = thisCol.transform;
	}

	public GameObject selectedGo = null;
	public List<GameObject> selectedObs = new List<GameObject>();
	void Update()
	{
		// show current viewed
		if(auraType == AuraType.PC)
		{
			RaycastHit hit;
			Ray ray = new Ray(thisTrans.position, thisTrans.forward);
			if(Physics.Raycast(ray, out hit))
			{
	            Collider hitCol = hit.collider;
	            GameObject hitGo = hitCol.gameObject;
				selectedGo = hitGo;
	            float distance = (colTrans.position - hitCol.ClosestPointOnBounds(colTrans.position)).magnitude;
	            if (hitGo.CompareTag("viewable") &&  distance <= thisCol.radius)
	            {
	                HoverCanvas.enabled = true;
	                HoverText.text = "Name: " + hitGo.name + "\n" +
	                    "Distance: " + distance;
	            }
	            else
	            {
	                HoverCanvas.enabled = false;
	            }
				
			}
			else
			{
				HoverCanvas.enabled = false;
			}
		}
		else
		{

			if(selectedObs.Count <= 0)
			{
				HoverCanvas.enabled = false;
			}
			else
			{
				AuraCanvas.enabled = true;
				AuraText.text = "Selected:\n";
				foreach(GameObject go in selectedObs)
				{
					AuraText.text += go.name + "\n";
				}
			}
		}

		// show list of obs in aura
		if(auraDirty)
		{
			auraDirty = false;
		
			if(auraObs.Count <= 0)
			{
				AuraCanvas.enabled = false;
			}
			else
			{
				AuraCanvas.enabled = true;
				AuraText.text = "Objects:\n";
     //           Debug.Log(auraObs[0].gameObject == auraObs[1].gameObject ? "true" : "false");
       //         Debug.Log(auraObs[0] == auraObs[1] ? "2true" : "2false");
				foreach(Collider col in auraObs)
				{
                    GameObject go = col.gameObject;
                    Vector3 distVec = col.ClosestPointOnBounds(colTrans.position);
                    float dist = (distVec - colTrans.position).magnitude;
                    AuraText.text += go.name + "\n";
                    
				}
			}
		}
	}

	public List<Collider> auraObs = new List<Collider>();
	bool auraDirty = false;
	void OnTriggerEnter(Collider other)
	{
        
        if(!other.gameObject.CompareTag("viewable"))
        {
            return;
        }
		if(!auraObs.FirstOrDefault(auob => auob.gameObject == other.gameObject)) 
		{
			auraObs.Add(other);
		}
		auraDirty = true;
	}

	void OnTriggerExit(Collider other)
	{
        if (!other.gameObject.CompareTag("viewable")) 
        {
            return;
        }
        if (auraObs.FirstOrDefault(auob => auob.gameObject == other.gameObject))
		{
			auraObs.Remove(other);
		}
		auraDirty = true;
	}
}


public class GoDist
{
    public GameObject go;
    public float dist;

    public GoDist(GameObject go, float dist)
    {
        this.go = go;
        this.dist = dist;
    }
}