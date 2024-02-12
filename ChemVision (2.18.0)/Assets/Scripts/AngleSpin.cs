
////////////////////////////////////////////////////////////////////////////////////////////////////
//
//      Angle Spin Script
//
//      Last Updated:               2/14/2018
//      Oldest Compatible Version:  2.10.1
//      Updated By:                 Kaleb Cole
//      Project:                    Chem Vision
//
////////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;

////////////////////////////////////////////////////////////////////////////////////////////////////

public enum AXIS
{
    X = 0,
    Y = 1,
    Z = 2
}

////////////////////////////////////////////////////////////////////////////////////////////////////

public class AngleSpin : MonoBehaviour
{

    //  Publics
    public GameObject objectToSpin;
    public int numPositions = 1;
    public float spinTime = 1f;
    public AXIS axisOfRotation;

    //  Privates
    private bool rotate = false;
    private float angle, currentRotation = 0;

    //  Constants
    private const string ERROR = "CVE [Angle Spin Script] | ";

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    /*  Start is called when the script is enabled, before any Updates are called */

    void Start()
    {
        numPositions = Mathf.Max(1, numPositions);
        angle = 360 / numPositions;

        ErrorCheck();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    /*  Update is called once per frame */

    void Update()
    {
        if (rotate && currentRotation < angle)
        {
            float tempFloat = (angle / spinTime) * Time.deltaTime;
            switch (axisOfRotation)
            {
                case AXIS.X:
                    objectToSpin.transform.Rotate(Mathf.Clamp(tempFloat, 0, angle - currentRotation), 0, 0);
                    break;
                case AXIS.Y:
                    objectToSpin.transform.Rotate(0, Mathf.Clamp(tempFloat, 0, angle - currentRotation), 0);
                    break;
                case AXIS.Z:
                    objectToSpin.transform.Rotate(0, 0, Mathf.Clamp(tempFloat, 0, angle - currentRotation));
                    break;
            }
            currentRotation += tempFloat;
        }
        else
        {
            rotate = false;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    /*  ErrorCheck checks developer input to make sure everything is set properly */

    private void ErrorCheck()
    {
        if (objectToSpin == null)
        {
            Debug.LogError(ERROR + "Object not set for " + transform.name);
            objectToSpin = transform.gameObject;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    /*  Called to initiate a rotation */

    public void OnRaycastClick()
    {
        if (!rotate)
        {
            rotate = true;
            currentRotation = 0;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
}
