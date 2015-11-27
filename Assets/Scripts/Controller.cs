using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour {

    bool freezeMouse = false;
	
	// Update is called once per frame
	void Update () 
    {
	    if(Input.GetKeyDown(KeyCode.Escape))
        {
            freezeMouse = !freezeMouse;

            //Cursor.visible = !freezeMouse;
            //Cursor.lockState = freezeMouse ? CursorLockMode.Locked : CursorLockMode.None;
        }
        if (freezeMouse)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        
	}
}
