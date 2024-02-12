
////////////////////////////////////////////////////////////////////////////////////////////////////
//
//      Focus Controller Script
//
//      Last Updated:               10/25/2017
//      Oldest Compatible Version:  2.3.0
//      Updated By:                 Kaleb Cole
//      Project:                    Chem Vision
//
////////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.EventSystems;

////////////////////////////////////////////////////////////////////////////////////////////////////

public class FocusController : MonoBehaviour {

    //  Publics
    public float distance = 10;

    //  Privates
    private GameObject playerObject;
    private Camera mainCamera;
    private bool focusLock = false;

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    /*  Start is called when the script is enabled, before any Updates are called */

    void Start () {
        playerObject = GameObject.FindGameObjectWithTag("- Player -");
        mainCamera = Camera.main;
        EventTrigger trigger = this.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = this.gameObject.AddComponent<EventTrigger>();
        }
        EventTrigger.Entry focus = new EventTrigger.Entry();
        focus.eventID = EventTriggerType.PointerDown;
        focus.callback.AddListener((data) => { ToggleLock((PointerEventData)data); });
        trigger.triggers.Add(focus);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    /*  Update is called once per frame */

    void Update () {
        if (focusLock)
        {
            playerObject.transform.position = transform.position + mainCamera.transform.forward * -distance;
        }
	}

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    /*  Toggles in and out of being Focused on an object */

    public void ToggleLock(PointerEventData data)
    {
        focusLock = !focusLock;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public void OnRaycastClick()
    {
        focusLock = !focusLock;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
}
