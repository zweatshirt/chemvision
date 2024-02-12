
////////////////////////////////////////////////////////////////////////////////////////////////////
//
//      Generic Spin Script
//
//      Last Updated:               2/14/2018
//      Oldest Compatible Version:  2.10.1
//      Updated By:                 Kaleb Cole
//      Project:                    Chem Vision
//
////////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.EventSystems;

////////////////////////////////////////////////////////////////////////////////////////////////////

public class GenericSpin : MonoBehaviour
{
    //  Publics
    public float spinX = 0f, spinY = 0f, spinZ = 0f;
    public bool toggleSpin = true, slowOnLook = true;

    //  Privates
    public bool isSpinning = true;
    private int slow = 1;

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    
    /*  Start is called when the script is enabled, before any Updates are called */

    void Start()
    {
        if (toggleSpin || slowOnLook)
        {
            //  Creates an eventTrigger if one does not exist
            EventTrigger trigger = this.gameObject.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = this.gameObject.AddComponent<EventTrigger>();
            }

            if (slowOnLook)
            {
                //  Creates the enter trigger
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerEnter;
                entry.callback.AddListener((data) => { SlowSpinAll((PointerEventData)data); });
                trigger.triggers.Add(entry);

                //  Creates the exit trigger
                EventTrigger.Entry exit = new EventTrigger.Entry();
                exit.eventID = EventTriggerType.PointerExit;
                exit.callback.AddListener((data) => { SlowSpinAll((PointerEventData)data); });
                trigger.triggers.Add(exit);
            }

            if (toggleSpin)
            {
                //  Creates the click trigger
                EventTrigger.Entry click = new EventTrigger.Entry();
                click.eventID = EventTriggerType.PointerDown;
                click.callback.AddListener((data) => { ToggleSpin((PointerEventData)data); });
                trigger.triggers.Add(click);
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    /*  Update is called once per frame */

    void Update()
    {
        if (isSpinning)
        {
            transform.Rotate((spinZ / slow) * Time.deltaTime, (spinY / slow) * Time.deltaTime, (spinX / slow) * Time.deltaTime);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    /*  When called, it toggles the spin divider between 1 and 3 */

    public void SlowSpinAll(PointerEventData data)
    {
        if (slow == 1)
        {
            slow = 3;
        }
        else
        {
            slow = 1;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    /*  Toggles whether or not the object should spin */

    public void ToggleSpin(PointerEventData data)
    {
        isSpinning = !isSpinning;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public void OnRaycastEnter()
    {
        slow = 3;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public void OnRaycastExit()
    {
        slow = 1;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public void OnRaycastClick()
    {
        isSpinning = !isSpinning;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
}
