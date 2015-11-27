using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AutomataScript : MonoBehaviour 
{
	public BotControlScript thisMotorControl;

	delegate IEnumerator BotAction();
	BotAction currentAction = null;
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
	Transform toothTrans;

	void Start () 
	{
		thisMotorControl = this.GetComponent<BotControlScript>();
		thisTrans = this.transform;
		toothTrans = toothArea.transform;
		idleAction = new BotAction(Idle);
		brushAction = new BotAction(BrushTeeth);
		actions[ActionKey.BrushTeeth] = brushAction;
		actions[ActionKey.Idle] = idleAction;
	}
	
	// Update is called once per frame

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

			currentActionString = chosenKey.ToString(); // for display
			currentAction = actions[chosenKey];
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
				thisMotorControl.h = 0;
				thisMotorControl.v = 0;
			}
			else
			{
				thisMotorControl.v = 0.2f;
				Vector3 randDir = Random.onUnitSphere;
				randDir.y = 0f;
				thisTrans.rotation = Quaternion.LookRotation(randDir);
			}
			yield return new WaitForSeconds(2);
			idleCount++;
			standStill = !standStill;
		}

		currentAction = null;
	}

	public bool inToothArea = false;
	int cols = 0;
	Vector3 colNorm = Vector3.zero;
	IEnumerator BrushTeeth()
	{
		while(!inToothArea)
		{
			Vector3 curPos = new Vector3(thisTrans.position.x, 0, thisTrans.position.z);
			Vector3 goalPos = new Vector3(toothTrans.position.x, 0, toothTrans.position.z);
			Vector3 moveDir = Vector3.Normalize(goalPos - curPos);
			float moveSpeed = (goalPos - curPos).magnitude / 15f;
			moveSpeed = moveSpeed > 0.5f ? 0.5f : moveSpeed;
			thisMotorControl.v = moveSpeed;
			if(cols > 0)
			{
				//thisMotorControl.h += 0.02f;
				Vector3 tangent = Vector3.Cross(colNorm, thisTrans.up);
				Vector3 newDir = Vector3.Project(moveDir, tangent.normalized);
				thisTrans.rotation = Quaternion.LookRotation(newDir);
				//Debug.DrawRay(curPos, 1f * newDir.normalized, Color.blue, 2f, false);
				while(Physics.Raycast(new Ray(thisTrans.position, moveDir), 1f))
				{
					//Debug.DrawRay(curPos, 1f * moveDir.normalized, Color.white, 2f, false);
					yield return new WaitForEndOfFrame();
				}
				yield return new WaitForSeconds(0.4f);

			}
			else
			{
				thisTrans.rotation = Quaternion.LookRotation(moveDir);
				thisMotorControl.h = 0;
			}


			//pickup toothbrush


			yield return new WaitForEndOfFrame();
		}
		thisMotorControl.h = 0;
		thisMotorControl.v = 0;
	
		currentAction = null;
	}

	void OnTriggerEnter(Collider other)
	{
		if(other == toothArea)
		{
			inToothArea = true;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if(other == toothArea)
		{
			inToothArea = false;
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
		cols = 0;
		Debug.Log("col down " + cols);
	}
}
