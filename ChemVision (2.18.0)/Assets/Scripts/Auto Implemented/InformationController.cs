
////////////////////////////////////////////////////////////////////////////////////////////////////
//
//      Information Controller
//
//      Last Updated:               2/28/2018
//      Oldest Compatible Version:  2.12.0
//      Updated By:                 Kaleb Cole
//      Project:                    Chem Vision
//
////////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.UI;

////////////////////////////////////////////////////////////////////////////////////////////////////

public class InformationController : MonoBehaviour
{

    //  Privates
    private GameObject playerObject, informationCanvasObject, exclamationCanvasObject;
    private bool isInitialized = false, inInformation = false, hasExited = false;
    private float timeOfExit;

    //  Constants
    private const float FADE_TIME = 2f, WAIT_TIME = 1f;
    private Vector3 SMALL_COLLIDER = new Vector3(5f, 7.5f, .1f), LARGE_COLLIDER = new Vector3(25f, 25f, .1f);

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    void Start()
    {
        InitializeVariables();
        EndInformation();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    void Update()
    {
        transform.LookAt(playerObject.transform);
        transform.Rotate(0, 180, 0);
        if (inInformation && hasExited)
        {
            if (Time.time - timeOfExit > WAIT_TIME)
            {
                informationCanvasObject.GetComponent<CanvasGroup>().alpha = 1 - (Time.time - timeOfExit - WAIT_TIME) / FADE_TIME;
                if (Time.time - timeOfExit > FADE_TIME + WAIT_TIME)
                {
                    EndInformation();
                    hasExited = false;
                }
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public void InitializeVariables()
    {
        if (!isInitialized)
        {
            isInitialized = true;
            playerObject = GameObject.FindGameObjectWithTag(Utility.TAG_Player);
            informationCanvasObject = transform.Find("Information Canvas").gameObject;
            exclamationCanvasObject = transform.Find("Exclamation Mark Canvas").gameObject;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public void SetString(string stringToSet)
    {
        informationCanvasObject.transform.Find("Information").transform.Find("Information Text").GetComponent<Text>().text = stringToSet;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public void OnRaycastClick()
    {
        exclamationCanvasObject.SetActive(false);
        informationCanvasObject.SetActive(true);
        transform.GetComponent<BoxCollider>().size = LARGE_COLLIDER;
        inInformation = true;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public void OnRaycastExit()
    {
        hasExited = true;
        timeOfExit = Time.time;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public void OnRaycastEnter()
    {
        informationCanvasObject.GetComponent<CanvasGroup>().alpha = 1;
        hasExited = false;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    private void EndInformation()
    {
        exclamationCanvasObject.SetActive(true);
        informationCanvasObject.SetActive(false);
        transform.GetComponent<BoxCollider>().size = SMALL_COLLIDER;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
}
