using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AutomataScript : MonoBehaviour 
{
	public BotControlScript thisMotorControl;
	public AuraDetector thisAuraDetector;

	delegate IEnumerator BotAction();
	BotAction _currentAction = null;
	BotAction currentAction
	{
		get
		{
			return _currentAction;
		}
		set
		{
			_currentAction = value;
			currentActionString = value == null ? "" : value.ToString();
		}
	}
	BotAction idleAction;
	BotAction brushAction;

	public enum ActionKey 
	{
		Null = 0,
		BrushTeeth = 1,
		Idle = 2

	};
	public Dictionary<ActionKey, float> actionProbs = new Dictionary<ActionKey, float>()
	{
		{ActionKey.BrushTeeth, 0.8f},
		{ActionKey.Idle, 0.2f}
	};
	Dictionary<ActionKey, BotAction> actions = new Dictionary<ActionKey, BotAction>();
	public string currentActionString;
	Transform thisTrans;

	public BoxCollider toothArea;
	public BoxCollider switchArea;
	Transform toothTrans;
	Transform switchTrans;
	
	void Start () 
	{
		thisMotorControl = this.GetComponent<BotControlScript>();
		thisAuraDetector = this.GetComponentInChildren<AuraDetector>();
		thisTrans = this.transform;
		toothTrans = toothArea.transform;
		switchTrans = switchArea.transform;
		idleAction = new BotAction(Idle);
		brushAction = new BotAction(BrushTeeth);
		actions[ActionKey.BrushTeeth] = brushAction;
		actions[ActionKey.Idle] = idleAction;
	}
	
	// Update is called once per frame
	public Dictionary<ActionKey, float> cooldowns = new Dictionary<ActionKey, float>();

	void Update () 
	{
		if(currentAction == null)
		{
			int ran = Random.Range(0, 101);
			float perc = ran / 100f;
			float prev = 0f;
			float cur = 0f;

			ActionKey chosenKey = ActionKey.Null;
			foreach(ActionKey key in actionProbs.Keys)
			{
				chosenKey = key;
				float prob = actionProbs[key];
				cur += prob;
				if(perc >= prev && perc < cur)
				{
					break;
				}
				prev += prob;
			}

			if(cooldowns.ContainsKey(chosenKey))
			{
				if(Time.realtimeSinceStartup < cooldowns[chosenKey])
				{
					chosenKey = ActionKey.Idle;
				}
			}

			currentAction = actions[chosenKey];
			currentActionString = chosenKey.ToString();
			if(chosenKey != ActionKey.Idle)
			{
				cooldowns[chosenKey] = Time.realtimeSinceStartup + (5f * 60f);
			}
			StartCoroutine(currentAction());

		}

	}


	int idleCount = 0;
	bool standStill = false;
	IEnumerator Idle()
	{

		while(idleCount <= 10)
		{
			if(standStill)
			{
				StopMoving();
			}
			else
			{
				thisMotorControl.v = 0.2f;
				int i = 0;
				Vector3 randDir = Random.onUnitSphere;
				randDir.y = 0;
				while(i <= 15)
				{
					if(Physics.Raycast(new Ray(thisTrans.position, randDir), 5f))
					{
						randDir = Random.onUnitSphere;
						randDir.y = 0;
					}
					else
					{
						break;
					}
					i++;
				}
				randDir.y = 0f;
				thisTrans.rotation = Quaternion.LookRotation(randDir);
			}
			yield return new WaitForSeconds(2f);
			idleCount++;
			standStill = !standStill;
		}
		StopMoving();
		currentAction = null;
	}

	public bool inToothArea = false;
	public bool inSwitchArea = false;
	int cols = 0;
	Vector3 colNorm = Vector3.zero;
	


	object MoveTo(Transform trans)
	{
		Vector3 curPos = new Vector3(thisTrans.position.x, 0, thisTrans.position.z);
		Vector3 goalPos = new Vector3(trans.position.x, 0, trans.position.z);
		Vector3 moveDir = Vector3.Normalize(goalPos - curPos);
		float moveSpeed = (goalPos - curPos).magnitude / 15f;
		moveSpeed = moveSpeed > 0.5f ? 0.5f : moveSpeed;
		moveSpeed = moveSpeed < 0.2f ? 0.2f : moveSpeed;
		thisMotorControl.v = moveSpeed;
		if(cols > 0)
		{
			if(!Physics.Raycast(new Ray(thisTrans.position, moveDir), (goalPos - curPos).magnitude))
			{
				thisTrans.rotation = Quaternion.LookRotation(moveDir);
				this.thisMotorControl.h = 0;
			}
			else
			{

			
				//thisMotorControl.h += 0.02f;
				Vector3 tangent = Vector3.Cross(colNorm, thisTrans.up);
				Vector3 newDir = Vector3.Project(moveDir, tangent.normalized);
				thisTrans.rotation = Quaternion.LookRotation(newDir);
				//Debug.DrawRay(curPos, 1f * newDir.normalized, Color.blue, 2f, false);
				while(Physics.Raycast(new Ray(thisTrans.position, moveDir), 1f) && Physics.Raycast(new Ray(thisTrans.position, Vector3.RotateTowards(moveDir, Vector3.Cross(thisTrans.up, moveDir), 45f, 1f)), 2f))
				{
					//Debug.DrawRay(curPos, 1f * moveDir.normalized, Color.white, 2f, false);
					return new WaitForEndOfFrame();
				}
				return new WaitForSeconds(0.4f);

			}
			
		}
		else
		{
			thisTrans.rotation = Quaternion.LookRotation(moveDir);
			thisMotorControl.h = 0;
		}
		return new WaitForEndOfFrame();
	}



	void StopMoving()
	{
		thisMotorControl.h = 0;
		thisMotorControl.v = 0;
	}

	LightSwitch lightSwitch = null;
	public GameObject toothBrush;
	public GameObject toothPaste;
	public LightSwitch bathroomLightSwitch;

	public Transform toothPasteGrip;
	public Transform toothBrushGrip;
	public Transform toothBrushUp;
	public Transform toothBrushDown;

	Vector3 origBrushLoc;
	Vector3 origPasteLoc;
	Quaternion origBrushRot;
	Quaternion origPasteRot;

	public GameObject pasteParticles;


	IEnumerator BrushTeeth()
	{
		//move to light switch
		while(!inSwitchArea && (switchTrans.position - thisTrans.position).magnitude > 0.2f)
		{
			yield return MoveTo(switchTrans);
		}

		StopMoving();
		Debug.Log("Found Switch Area");

		if(lightSwitch == null)
		{
			foreach(Collider col in thisAuraDetector.auraObs)
			{
				if((lightSwitch = col.gameObject.GetComponentInChildren<LightSwitch>()) != null)
					break;
			}
		}
		if(lightSwitch == null)
		{
			Debug.Log("Couldn't find lightswitch");
			lightSwitch = bathroomLightSwitch;
		}
		//turn on lights if off
		if(!lightSwitch.lightsOn)
		{
			Debug.Log("Lights Off");
			thisAuraDetector.selectedObs.Add(lightSwitch.gameObject);
			yield return new WaitForSeconds(3f);
			lightSwitch.ToggleLights();
			yield return new WaitForSeconds(3f);
			thisAuraDetector.selectedObs.Remove(lightSwitch.gameObject);
		}
		lightSwitch = null;
		Debug.Log("Lights On");

		//move to sink
		while(!inToothArea && (toothTrans.position - thisTrans.position).magnitude > 0.2)
		{
			yield return MoveTo(toothTrans);
		}
		Debug.Log("Found Tooth Area");

		StopMoving();

		//brush
		origBrushLoc = toothBrush.transform.position;
		origBrushRot = toothBrush.transform.rotation;
		origPasteLoc = toothBrush.transform.position;
		origPasteRot = toothBrush.transform.rotation;
		// use toothbrush and toothpaste
		//select paste/brush
		thisAuraDetector.selectedObs.Add(toothBrush);
		thisAuraDetector.selectedObs.Add(toothPaste);

		//move paste/brush to grip positions
		toothPaste.transform.rotation = toothPasteGrip.transform.rotation;
		toothBrush.transform.rotation = toothBrushGrip.transform.rotation;

		Vector3 pasteMoveDir;
		Vector3 brushMoveDir;
		while((toothPaste.transform.position - toothPasteGrip.transform.position).magnitude > 0.05f || (toothBrush.transform.position - toothBrushGrip.transform.position).magnitude > 0.05f)
		{
			pasteMoveDir = (toothPasteGrip.transform.position - toothPaste.transform.position).normalized;
			brushMoveDir = (toothBrushGrip.transform.position - toothBrush.transform.position).normalized;
			if((toothPaste.transform.position - toothPasteGrip.transform.position).magnitude > 0.05f)
			{
				toothPaste.transform.position += 0.01f * pasteMoveDir;
			}
			if((toothBrush.transform.position - toothBrushGrip.transform.position).magnitude > 0.05f)
			{
				toothBrush.transform.position += 0.01f * brushMoveDir;
			}
			yield return new WaitForSeconds(0.01f);

		}
		CopyTransform(toothPasteGrip, toothPaste.transform);
		CopyTransform(toothBrushGrip, toothBrush.transform);

		//apply paste
		pasteParticles.SetActive(true);
		yield return new WaitForSeconds(3f);
		pasteParticles.SetActive(false);

		//move toothpaste back to origpos
		while((toothPaste.transform.position - origPasteLoc).magnitude > 0.05f )
		{
			pasteMoveDir = (origPasteLoc - toothPaste.transform.position).normalized;
			toothPaste.transform.position += 0.01f * pasteMoveDir;

			yield return new WaitForSeconds(0.01f);
			
		}
		toothPaste.transform.position = origPasteLoc;
		toothPaste.transform.rotation = origPasteRot;
		//drop toothpaste
		thisAuraDetector.selectedObs.Remove(toothPaste);

		//move brush to lower brush loc
		thisMotorControl.idling = false;
		while((toothBrush.transform.position - toothBrushDown.position).magnitude > 0.05f )
		{
			brushMoveDir = (toothBrushDown.position - toothBrush.transform.position).normalized;
			toothBrush.transform.position += 0.01f * brushMoveDir;
			
			yield return new WaitForSeconds(0.01f);
			
		}
		CopyTransform(toothBrushDown, toothBrush.transform);

		//brush up and down

		float stopBrushing = Time.realtimeSinceStartup + (20f);
		Transform movingTowards = toothBrushUp;

		while(Time.realtimeSinceStartup < stopBrushing)
		{

			brushMoveDir = (movingTowards.position - toothBrush.transform.position).normalized;
			toothBrush.transform.position += 0.01f * brushMoveDir;
			if((movingTowards.position - toothBrush.transform.position).magnitude < 0.05f)
			{
				CopyTransform(movingTowards, toothBrush.transform);
				if(movingTowards == toothBrushUp)
				{
					movingTowards = toothBrushDown;
				}
				else
				{
					movingTowards = toothBrushUp;
				}
			}

			yield return new WaitForSeconds(0.01f);

		}

		thisMotorControl.idling = true;
		//move toothbrush back to original position
		while((toothBrush.transform.position - origBrushLoc).magnitude > 0.05f )
		{
			brushMoveDir = (origBrushLoc - toothBrush.transform.position).normalized;
			toothBrush.transform.position += 0.01f * brushMoveDir;
			
			yield return new WaitForSeconds(0.01f);
			
		}
		toothBrush.transform.position = origBrushLoc;
		toothBrush.transform.rotation = origBrushRot;

		//drop toothbrush
		thisAuraDetector.selectedObs.Remove(toothBrush);

		Debug.Log("Done Brushing Teeth");
		currentAction = null;
		Debug.Log("Current action = " + (currentAction == null? "null" : "notnull"));
		yield return new WaitForSeconds(15f);
	}

	void CopyTransform(Transform from, Transform to)
	{
		to.position = from.position;
		to.rotation = from.rotation;
	}


	void OnTriggerEnter(Collider other)
	{
		if(other == toothArea)
		{
			inToothArea = true;
		}
		else if(other == switchArea)
		{
			inSwitchArea = true;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if(other == toothArea)
		{
			inToothArea = false;
		}
		else if(other == switchArea)
		{
			inSwitchArea = false;
		}
	}

	void OnCollisionEnter(Collision col)
	{
		if(col.collider.gameObject.name == "Ground")
		{
			return;
		}
		cols++;
		colNorm = col.contacts[0].normal;
		Debug.Log("col up " + cols);
	}

	void OnCollisionExit(Collision col)
	{
		if(col.collider.gameObject.name == "Ground")
		{
			return;
		}
		cols--;
		Debug.Log("col down " + cols);
	}
}
