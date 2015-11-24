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
		{ActionKey.BrushTeeth, 0.2f},
		{ActionKey.Idle, 0.8f}
	};
	Dictionary<ActionKey, BotAction> actions = new Dictionary<ActionKey, BotAction>();
	public string currentActionString;
	Transform thisTrans;

	public BoxCollider toothArea;
	Transform toothTrans;

	void Start () 
	{
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

	IEnumerator Idle()
	{
		yield return new WaitForSeconds(30);
		currentAction = null;
	}

	public bool inToothArea = false;
	IEnumerator BrushTeeth()
	{
		while(!inToothArea)
		{
			Vector3 curPos = thisTrans.position;
			Vector3 goalPos = toothTrans.position;
			Vector3 moveDir = goalPos - curPos;

			//pickup toothbrush


			yield return new WaitForEndOfFrame();
		}
	
		currentAction = null;
	}

	void OnTriggerEnter()
	{

	}

	void OnTriggerExit()
	{

	}
}
