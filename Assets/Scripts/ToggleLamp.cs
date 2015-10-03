using UnityEngine;
using System.Collections;

public class ToggleLamp : MonoBehaviour {

    public Material onMaterial;
    public Material offMaterial;
    public Light light;

    MeshRenderer mr;
    public void Start()
    {
        mr = this.GetComponent<MeshRenderer>();
    }

    public void TurnOn()
    {
        Material[] newMats = new Material[2];
        newMats[0] = mr.materials[0];
        newMats[1] = onMaterial;
        mr.materials = newMats;
        light.enabled = true;
    }

    public void TurnOff()
    {
        Material[] newMats = new Material[2];
        newMats[0] = mr.materials[0];
        newMats[1] = offMaterial;
        mr.materials = newMats;
        light.enabled = false;
    }
}
