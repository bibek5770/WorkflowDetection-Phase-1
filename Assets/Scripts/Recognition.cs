using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Recognition : MonoBehaviour
{
    // Use this for initialization
    AuraDetector detector;
    public List<string> objectsRecognizedByBot;
	public Dictionary<string, Dictionary<string,int>> predefinedWorkflow;

    //Hey Sean
    //display foundworkflow at top corner if possible, I am currently logging to console
    public Dictionary<string, List<string>> foundWorkflow;
    int counter = 0;


    void Start()
    {
        detector = this.transform.parent.GetComponentInChildren<AuraDetector>();
        objectsRecognizedByBot = new List<string>();
		predefinedWorkflow = new Dictionary<string, Dictionary<string,int>> ();

        foundWorkflow = new Dictionary<string, List<string>>();
        initializeWorkflow();
    }

    // Update is called once per frame
    void Update()
    {
        GameObject selectedOb = detector.selectedGo;// delete
        // objects in vicinity
        List<Collider> auraObs = detector.auraObs;
        // objects selected by workflow
        List<GameObject> selectedObs = detector.selectedObs;
        //add objects interacted by workflow bot to the list of recognized objects(currently the ones in vicinity)
        string currentItem;
		foreach (var item in selectedObs)
        {
            currentItem = item.name.ToLower();
            if (!(objectsRecognizedByBot.Contains(currentItem)))
            {
                objectsRecognizedByBot.Add(currentItem);
            }
		}
		foreach (var item in auraObs)
		{
			currentItem = item.name.ToLower();
			if (!(objectsRecognizedByBot.Contains(currentItem)))
			{
				objectsRecognizedByBot.Add(currentItem);
			}
		}

        getWorkflow(objectsRecognizedByBot);
        if (counter <= foundWorkflow.Count)
        {
            for (int index = counter; index < foundWorkflow.Count; index++)
            {
                var item = foundWorkflow.ElementAt(index);
                Debug.Log("1. Possible Workflow  :=>\t" + item.Key +
                    "\n2. Recognized Objects:=>\t" + string.Join(" , ", item.Value.ToArray()));
            }
            counter = foundWorkflow.Count;
        }
    }

    public void getWorkflow(List<string> objectsRecognizedByBot)
    {

        List<string> possibleWorkflows = new List<string>();

        List<string> workflowObjects;

        int itemsFoundInWorkflow = 0;
        string workflowName;

        double probabilityRate = 0;

        List<string> objectsCorrespondingToWorkflow;
        foreach (var item in predefinedWorkflow)
        {
            workflowObjects = item.Value.Keys.ToList<string>();
			objectsCorrespondingToWorkflow = new List<string>();

				workflowName = item.Key.ToLower();
            	
	            if (!(foundWorkflow.ContainsKey(workflowName)))
	            {
	                foreach (var botObjects in objectsRecognizedByBot)
	                {
	                    if (workflowObjects.Contains(botObjects))
	                    {
							itemsFoundInWorkflow+=item.Value[botObjects];
	                        objectsCorrespondingToWorkflow.Add(botObjects);
	                    }
	                }
	                probabilityRate = 100.0 * itemsFoundInWorkflow / item.Value["sum"];
	                if (probabilityRate >= 40)
	                {
					objectsCorrespondingToWorkflow.Add(" [ Probability Rate = "+probabilityRate+ " ] ");
	                    foundWorkflow.Add(workflowName, objectsCorrespondingToWorkflow);
	                }
	                itemsFoundInWorkflow = 0;

            }
        }
    }
    private void initializeWorkflow()
    {
       
		var test = new Dictionary<string, int>
        {
			{"ground",1}, {"floor",4}, {"table",2}, {"wall",2},{"jacuzzi",2}, {"sum",11}
		};
		predefinedWorkflow.Add("test", test);
        
        
		var x = new Dictionary<string, int>
		{
			{"toothbrush",3},{"toothpaste",3},{"mirror",1},{"sink",1},{"sum",8}
		};
		predefinedWorkflow.Add("Brushing Teeth", x);

    }
}
