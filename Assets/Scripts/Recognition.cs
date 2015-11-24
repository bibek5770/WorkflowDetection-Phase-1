using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Recognition : MonoBehaviour {
	
	// Use this for initialization
	AuraDetector detector;

	public string workflowName = "";
	void Start () 
	{
		detector = this.transform.parent.GetComponentInChildren<AuraDetector>();
		foreach(Transform child in this.transform)
		{
			Debug.Log("I am " + child.ToString() + " and a child of " + this.ToString());
		}
	}
	
	// Update is called once per frame
	void Update () {
		GameObject selectedOb = detector.selectedGo;// delete


		// objects in vicinity
		List<Collider> auraObs = detector.auraObs;
		// objects selected by workflow
		List<GameObject> selectedObs = detector.selectedObs;


		Properties props = null;
		//AutomataScript aut = selectedOb.GetComponent<AutomataScript>();
		if(selectedOb != null)
			props = selectedOb.GetComponent<Properties>();
		if(props != null)
		{
			string shape = props.shape.ToString();
			string color = props.color.ToString();
			string texture = props.texture.ToString();
			
			Debug.Log("Observer is considering the object:");
			Debug.Log("With Properties:\nShape = " + shape + " and Color = " + color + " and Texture = " + texture);
		}

	}
}
