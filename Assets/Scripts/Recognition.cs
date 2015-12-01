using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class Recognition : MonoBehaviour
{
    // Use this for initialization
    AuraDetector detector;
    public List<string> objectsRecognizedByBot;
    public Dictionary<string, List<string>> predefinedWorkflow;
    //Hey Sean
    //display foundworkflow at top corner if possible, I am currently logging to console
    public Dictionary<string, List<string>> foundWorkflow;
    int counter = 0;


    void Start()
    {
        detector = this.transform.parent.GetComponentInChildren<AuraDetector>();
        objectsRecognizedByBot = new List<string>();
        predefinedWorkflow = new Dictionary<string, List<string>>();
        foundWorkflow = new Dictionary<string, List<string>>();
        initializeWorkflow();
    }

	public GameObject recCanvas;
	public Text recText;
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
                recText.text = ("1. Possible Workflow  :=>\t" + item.Key +
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
            workflowObjects = item.Value;
            workflowName = item.Key.ToLower();
            objectsCorrespondingToWorkflow = new List<string>();
            if (!(foundWorkflow.ContainsKey(workflowName)))
            {
                foreach (var botObjects in objectsRecognizedByBot)
                {
                    if (workflowObjects.Contains(botObjects))
                    {
                        itemsFoundInWorkflow++;
                        objectsCorrespondingToWorkflow.Add(botObjects);
                    }
                }
                probabilityRate = 100.0 * itemsFoundInWorkflow / workflowObjects.Count;
                if (probabilityRate >= 40)
                {
                    foundWorkflow.Add(workflowName, objectsCorrespondingToWorkflow);
                }
                itemsFoundInWorkflow = 0;
            }
        }
    }
    private void initializeWorkflow()
    {
        //to be edited
        List<string> brushingTeethObjects = new List<string>()
        {
            "toothbrush", "toothpaste", "mirror","towel","sink2"
        };
        List<string> washingCLothesObjects = new List<string>()
        {
            "ground", "leftwall", "sink2", "wall","jacuzzi"
        };
        List<string> test = new List<string>()
        {
            "ground", "leftwall", "sink2", "wall","jacuzzi"
        };
        predefinedWorkflow.Add("Brushing Teeth", brushingTeethObjects);
        predefinedWorkflow.Add("Washing Clothes", washingCLothesObjects);
        predefinedWorkflow.Add("test", test);
    }
}
