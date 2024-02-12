
////////////////////////////////////////////////////////////////////////////////////////////////////
//
//      Waypoint Controller
//
//      Last Updated:               3/23/2018
//      Oldest Compatible Version:  2.14.1
//      Updated By:                 Kaleb Cole
//      Project:                    Chem Vision
//
////////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.UI;

////////////////////////////////////////////////////////////////////////////////////////////////////

public class WaypointController : MonoBehaviour
{
    //  Publics
    public GameObject objectToFace;

    //  Privates
    private VRController vrControllerScript;
    private Image arrowImage;

    //  Constants
    private static float SHORT = 50f, LONG = 100f;

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    /*  Start is called when the script is enabled, before any Updates are called */

    void Start()
    {
        vrControllerScript = GameObject.FindGameObjectWithTag("- VR Controller -").GetComponent<VRController>();
        arrowImage = transform.Find("Waypoint Canvas").Find("Waypoint Arrow").GetComponent<Image>();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    /*  Update is called once per frame */

    void Update()
    {

        Vector3 distanceV3 = vrControllerScript.player.transform.position - transform.position;
        float distance = Mathf.Clamp(Vector3.Distance(vrControllerScript.player.transform.position, transform.position), SHORT, LONG);
        transform.GetComponent<BoxCollider>().size = new Vector3(distance / 10, distance / 10);
        arrowImage.rectTransform.sizeDelta = new Vector2(distance / 10, distance / 10);
        distanceV3.x = distanceV3.z = 0.0f;
        transform.LookAt(vrControllerScript.player.transform.position - distanceV3);
        transform.Rotate(0, 180, 0);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public void OnRaycastClick()
    {
        vrControllerScript.player.transform.position = transform.position;
        if (vrControllerScript.persistantDataScript.gameMode == Mode.ComputerMode || vrControllerScript.persistantDataScript.gameMode == Mode.EditorMode_NonVR)
        {
            vrControllerScript.nonVRPlayerControllerScript.GetArrowData().clearArrow();
        }
        else
        {
            vrControllerScript.vrPlayerControllerScript.GetArrowData().clearArrow();
        }
        if (objectToFace)
        {
            if (vrControllerScript.persistantDataScript.gameMode == Mode.ComputerMode || vrControllerScript.persistantDataScript.gameMode == Mode.EditorMode_NonVR)
            {
                vrControllerScript.nonVRPlayerControllerScript.GetArrowData().SetArrow(objectToFace, this.gameObject);
            }
            else
            {
                vrControllerScript.vrPlayerControllerScript.GetArrowData().SetArrow(objectToFace, this.gameObject);
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
}
