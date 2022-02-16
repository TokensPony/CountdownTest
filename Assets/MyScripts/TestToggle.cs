
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using System.Collections;
using TMPro;

[UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
public class TestToggle : UdonSharpBehaviour
{
    [Tooltip("List of objects to toggle on and off")]
    public GameObject[] toggleObjects;
    public TextMeshPro wallText;
    private bool textState = true;

    void Start()
    {
        Debug.Log("This is a test");    
    }

    private void Update()
    {
        transform.Rotate(Vector3.up, 1f);
    }

    public override void Interact()
    {
        foreach (GameObject toggleObject in toggleObjects)
        {
            toggleObject.SetActive(!toggleObject.activeSelf);
        }

        textState = !textState;
        if (textState)
        {
            //wallText.Setext("I am true");
        }
        else
        {
            //wallText.SetText("I am a lie");
        }
    }
}
