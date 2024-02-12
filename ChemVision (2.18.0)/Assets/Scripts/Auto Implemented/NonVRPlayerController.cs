
////////////////////////////////////////////////////////////////////////////////////////////////////
//
//      Non-VR Player Controller
//
//      Last Updated:               3/29/2018
//      Oldest Compatible Version:  2.15.3
//      Updated By:                 Kaleb Cole
//      Project:                    Chem Vision
//
////////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

////////////////////////////////////////////////////////////////////////////////////////////////////

[RequireComponent(typeof(Collider))]
public class NonVRPlayerController : MonoBehaviour
{
    //  Publics
    public GameObject pausedHUD, gvrReticle;
    public Text currentSceneText, questionsLeftText, sessionIDText;

    //  Privates
    private VRController vrControllerScript;
    private Utility.ArrowData arrowData;
    private bool move = false, paused = false;
    private float[] clamp = new float[6] { 0, 0, 0, 0, 0, 0 };
    private float mouseX = 0, mouseY = 0;
    private GameObject previousRaycastObject = null;
    private Ray ray;

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

        arrowData = new Utility.ArrowData();
        arrowData.clearArrow();

        pausedHUD = vrControllerScript.mainCamera.transform.Find("Player HUD").Find("Paused HUD").gameObject;
        gvrReticle = vrControllerScript.mainCamera.transform.Find("GvrReticle").gameObject;
        currentSceneText = transform.Find("Main Camera").Find("Player HUD").Find("Current Scene Text").GetComponent<Text>();
        questionsLeftText = transform.Find("Main Camera").Find("Player HUD").Find("Questions Left Text").GetComponent<Text>();
        sessionIDText = transform.Find("Main Camera").Find("Player HUD").Find("Session ID Text").GetComponent<Text>();

        currentSceneText.text = SceneManager.GetActiveScene().name.Trim();
        sessionIDText.text = vrControllerScript.persistantDataScript.sessionID;
        vrControllerScript.UpdateQuestionsLeft();
        Pause(false);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    /*  Update is called once per frame */

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Pause(true);
        }
        if (Input.GetKeyDown(KeyCode.R) && vrControllerScript.persistantDataScript.Peek() != null)
        {
            arrowData.clearArrow();
            vrControllerScript.persistantDataScript.fadeDirection = 1;
            vrControllerScript.PreviousScene();
        }

        if (!paused)
        {
            UpdatePlayer();
            CheckRaycast();
            if (vrControllerScript.letPlayerMove && !vrControllerScript.persistantDataScript.sicknessMode)
            {
                Movement();
            }
            if (arrowData.needToTurn)
            {
                arrowData.ArrowUpdate();
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    private void CheckRaycast()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Camera.main.transform.forward, out hit, 1000))
        {
            GameObject current = hit.collider.gameObject;
            if (Input.GetMouseButtonDown(0))
            {
                Utility.SendMessageTo(current, "OnRaycastClick");
            }
            if (previousRaycastObject != current)
            {
                if (hit.transform.tag != "- Terrain -")
                {
                    vrControllerScript.gvrReticleScript.OnGazeStart(Camera.main, hit.transform.gameObject, hit.transform.position, true);
                }
                else
                {
                    vrControllerScript.gvrReticleScript.OnGazeExit(Camera.main, previousRaycastObject);
                }
                Utility.SendMessageTo(previousRaycastObject, "OnRaycastExit");
                Utility.SendMessageTo(current, "OnRaycastEnter");
                previousRaycastObject = current;
            }
        }
        else
        {
            vrControllerScript.gvrReticleScript.OnGazeExit(Camera.main, previousRaycastObject);
            Utility.SendMessageTo(previousRaycastObject, "OnRaycastExit");
            previousRaycastObject = null;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    private void UpdatePlayer()
    {
        mouseX += MOUSE_SPEED * Input.GetAxis("Mouse X");
        mouseY -= MOUSE_SPEED * Input.GetAxis("Mouse Y");
        transform.eulerAngles = new Vector3(mouseY, mouseX, 0f);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    private void Movement()
    {
        if (Input.GetKey(KeyCode.W))
        {
            arrowData.clearArrow();
            transform.Translate(new Vector3(0, 0, 1) * Time.deltaTime * vrControllerScript.persistantDataScript.speed, Space.Self);
        }
        if (Input.GetKey(KeyCode.A))
        {
            arrowData.clearArrow();
            transform.Translate(new Vector3(-1,0,0) * Time.deltaTime * vrControllerScript.persistantDataScript.speed ,Space.Self);
        }
        if (Input.GetKey(KeyCode.S))
        {
            arrowData.clearArrow();
            transform.Translate(new Vector3(0, 0, -1) * Time.deltaTime * vrControllerScript.persistantDataScript.speed, Space.Self);
        }
        if (Input.GetKey(KeyCode.D))
        {
            arrowData.clearArrow();
            transform.Translate(new Vector3(1, 0, 0) * Time.deltaTime * vrControllerScript.persistantDataScript.speed, Space.Self);
        }
        if (Input.GetKey(KeyCode.Space))
        {
            arrowData.clearArrow();
            transform.Translate(new Vector3(0, 1, 0) * Time.deltaTime * vrControllerScript.persistantDataScript.speed, Space.Self);
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            arrowData.clearArrow();
            transform.Translate(new Vector3(0, -1, 0) * Time.deltaTime * vrControllerScript.persistantDataScript.speed, Space.Self);
        }
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, clamp[0], clamp[1]), Mathf.Clamp(transform.position.y, clamp[2], clamp[3]), Mathf.Clamp(transform.position.z, clamp[4], clamp[5]));
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public void Pause(bool update)
    {
        if (update)
        {
            paused = !paused;
        }
        pausedHUD.SetActive(paused);
        gvrReticle.SetActive(!paused);
        if (paused)
        {
            Cursor.lockState = CursorLockMode.None;
            if (vrControllerScript.persistantDataScript.Peek() == null)
            {
                pausedHUD.transform.Find("Return Button").Find("Return Text").GetComponent<Text>().color = Color.black;
                pausedHUD.transform.Find("Return Button").GetComponent<Button>().interactable = false;
            }
            else
            {
                pausedHUD.transform.Find("Return Button").Find("Return Text").GetComponent<Text>().color = Utility.UI_Cyan;
                pausedHUD.transform.Find("Return Button").GetComponent<Button>().interactable = true;
            }

            if (SceneManager.GetActiveScene().name == "Options Scene")
            {
                pausedHUD.transform.Find("Options Menu Button").Find("Options Menu Text").GetComponent<Text>().color = Color.black;
                pausedHUD.transform.Find("Options Menu Button").GetComponent<Button>().interactable = false;
            }
            else
            {
                pausedHUD.transform.Find("Options Menu Button").Find("Options Menu Text").GetComponent<Text>().color = Utility.UI_Cyan;
                pausedHUD.transform.Find("Options Menu Button").GetComponent<Button>().interactable = true;
            }
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public void Return()
    {
        arrowData.clearArrow();
        vrControllerScript.persistantDataScript.fadeDirection = 1;
        vrControllerScript.PreviousScene();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public void OptionsMenu()
    {
        if (SceneManager.GetActiveScene().name != "Options Menu")
        {
            arrowData.clearArrow();
            vrControllerScript.persistantDataScript.fadeDirection = 1;
            vrControllerScript.NextScene("Options Scene");
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public void Quit()
    {
        vrControllerScript.persistantDataScript.newSession();
        Application.Quit();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public void NextSession()
    {
        vrControllerScript.persistantDataScript.newSession();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public Utility.ArrowData GetArrowData()
    {
        return arrowData;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

}