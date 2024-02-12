
////////////////////////////////////////////////////////////////////////////////////////////////////
//
//      VR Player Controller
//
//      Last Updated:               3/23/2018
//      Oldest Compatible Version:  2.14.1
//      Updated By:                 Kaleb Cole
//      Project:                    Chem Vision
//
////////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

////////////////////////////////////////////////////////////////////////////////////////////////////

[RequireComponent(typeof(Collider))]
public class VRPlayerController : MonoBehaviour
{
    //  Publics
    public bool exists = true;
    public Text currentSceneText, questionsLeftText, sessionIDText;

    //  Privates
    private VRController vrControllerScript;
    private Utility.ArrowData arrowData;
    private bool move = false;
    private float[] clamp = new float[6] { 0, 0, 0, 0, 0, 0 };

    //  Constants
    private const float CLAMP_OFFSET = 10;
    private const float MOUSE_SPEED = 2f;

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    /*  Start is called when the script is enabled, before any Updates are called */

    void Start()
    {
        vrControllerScript = GameObject.FindGameObjectWithTag("- VR Controller -").GetComponent<VRController>();

        clamp[0] = vrControllerScript.terrain.transform.position.x + CLAMP_OFFSET;                                                                          //  Sets the X clamp
        clamp[1] = vrControllerScript.terrain.transform.position.x + vrControllerScript.terrain.GetComponent<Terrain>().terrainData.size.x - CLAMP_OFFSET;
        clamp[2] = vrControllerScript.terrain.transform.position.y;                                                                                         //  Sets the Y clamp
        clamp[3] = vrControllerScript.terrain.transform.position.y + vrControllerScript.terrain.GetComponent<Terrain>().terrainData.size.y;
        clamp[4] = vrControllerScript.terrain.transform.position.z + CLAMP_OFFSET;                                                                          //  Sets the Z clamp
        clamp[5] = vrControllerScript.terrain.transform.position.z + vrControllerScript.terrain.GetComponent<Terrain>().terrainData.size.z - CLAMP_OFFSET;

        currentSceneText = transform.Find("Main Camera").Find("Player HUD").Find("Current Scene Text").GetComponent<Text>();
        questionsLeftText = transform.Find("Main Camera").Find("Player HUD").Find("Questions Left Text").GetComponent<Text>();
        sessionIDText = transform.Find("Main Camera").Find("Player HUD").Find("Session ID Text").GetComponent<Text>();
        currentSceneText.text = SceneManager.GetActiveScene().name.Trim();
        sessionIDText.text = vrControllerScript.persistantDataScript.sessionID;
        vrControllerScript.UpdateQuestionsLeft();

        arrowData = new Utility.ArrowData();
        arrowData.clearArrow();
        UpdateVRMode();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    /*  Update is called once per frame */

    void Update()
    {
            if (move)
            {
                arrowData.clearArrow();
                transform.Translate(vrControllerScript.mainCamera.transform.forward * Time.deltaTime * vrControllerScript.persistantDataScript.speed, Space.World);
                transform.position = new Vector3(Mathf.Clamp(transform.position.x, clamp[0], clamp[1]), Mathf.Clamp(transform.position.y, clamp[2], clamp[3]), Mathf.Clamp(transform.position.z, clamp[4], clamp[5]));
            }
            if (arrowData.needToTurn)
            {
                arrowData.ArrowUpdate();
            }
            CheckMove();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    private void LateUpdate()
    {
        GvrViewer.Instance.UpdateState();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    /*  This checks if the player has pressed the button, and if they have then it determines whether they
        want to stop or start moving. Stopping always happens, while starting only happens if the player is
        not looking at anything or is looking at terrain */

    private void CheckMove()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = vrControllerScript.mainCamera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
            RaycastHit hit;
            if (!Physics.Raycast(ray, out hit) || hit.transform.root.tag == "- Terrain -")
            {
                move = !vrControllerScript.persistantDataScript.sicknessMode && vrControllerScript.letPlayerMove;
            }
        }
        else if (!Input.GetMouseButton(0))
        {
            move = false;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    /*  Swaps in and out of VR Mode */

    public void UpdateVRMode()
    {
        GvrViewer.Instance.VRModeEnabled = vrControllerScript.persistantDataScript.vrMode;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public Utility.ArrowData GetArrowData()
    {
        return arrowData;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
}

