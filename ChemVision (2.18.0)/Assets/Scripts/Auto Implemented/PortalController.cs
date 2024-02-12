
////////////////////////////////////////////////////////////////////////////////////////////////////
//
//      Focus Controller Script
//
//      Last Updated:               3/16/2018
//      Oldest Compatible Version:  2.13.0
//      Updated By:                 Kaleb Cole
//      Project:                    Chem Vision
//
////////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

////////////////////////////////////////////////////////////////////////////////////////////////////

public class PortalController : MonoBehaviour
{
    //  Privates
    private bool goToScene = false, fading = false;
    private VRController vrControllerScript;

    //  Publics
    public string nextScene;

    //  Constants
    private const string ERROR = "CVE [PortalController] | ";

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    private void Start()
    {
        nextScene = nextScene.Trim();
        if(string.IsNullOrEmpty(nextScene))
        {
            Debug.LogError(ERROR + "Portal Scene not set!");
            nextScene = "BROKEN";
        }
        vrControllerScript = GameObject.FindGameObjectWithTag("- VR Controller -").GetComponent<VRController>();
        transform.Find("Portal Canvas").Find("Portal Text").GetComponent<Text>().text = nextScene;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    private void Update()
    {
        if (goToScene)
        {
            vrControllerScript.persistantDataScript.fadeDirection = 1;
            StartCoroutine(Wait());
            if (fading)
            {
                vrControllerScript.NextScene(nextScene);
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public void OnRaycastEnter()
    {
        transform.localScale = transform.localScale + new Vector3(.1f, .1f, 0);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public void OnRaycastExit()
    {
        transform.localScale = transform.localScale - new Vector3(.1f, .1f, 0);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public void OnRaycastClick()
    {
        if (nextScene != "BROKEN")
        {
            goToScene = true;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(vrControllerScript.persistantDataScript.FADE_SPEED);
        fading = true;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
}
