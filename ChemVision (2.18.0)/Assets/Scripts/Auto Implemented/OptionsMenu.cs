
////////////////////////////////////////////////////////////////////////////////////////////////////
//
//      Options Menu
//
//      Last Updated:               3/16/2018
//      Oldest Compatible Version:  2.13.0
//      Updated By:                 Kaleb Cole
//      Project:                    Chem Vision
//
////////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.UI;

////////////////////////////////////////////////////////////////////////////////////////////////////

public class OptionsMenu : MonoBehaviour
{
    //  Privates
    private VRController vrControllerScript;
    private Text sicknessModeText, vrModeText, questionText;

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    void Start()
    {
        vrControllerScript = GameObject.FindGameObjectWithTag("- VR Controller -").GetComponent<VRController>();
        sicknessModeText = GameObject.Find("Sickness Mode Text").GetComponent<Text>();
        vrModeText = GameObject.Find("VR Mode Text").GetComponent<Text>();
        questionText = GameObject.Find("Question Controller Text").GetComponent<Text>();
        SicknessToggle(false);
        VRModeToggle(false);
        questionText.text = "Session ID: " + vrControllerScript.persistantDataScript.sessionID;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public void SicknessToggle(bool update)
    {
        if (update)
        {
            vrControllerScript.persistantDataScript.sicknessMode = !vrControllerScript.persistantDataScript.sicknessMode;
        }
        if (vrControllerScript.persistantDataScript.sicknessMode)
        {
            sicknessModeText.text = "Sickness Mode: Enabled";
        }
        else
        {
            sicknessModeText.text = "Sickness Mode: Disabled";
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public void VRModeToggle(bool update)
    {
        if (vrControllerScript.persistantDataScript.gameMode == Mode.ComputerMode)
        {

            vrModeText.text = "VR Mode: Not Available";
        }
        else
        {
            if (update)
            {
                vrControllerScript.persistantDataScript.vrMode = !vrControllerScript.persistantDataScript.vrMode;
                vrControllerScript.vrPlayerControllerScript.UpdateVRMode();
            }
            if (vrControllerScript.persistantDataScript.vrMode)
            {
                vrModeText.text = "VR Mode: Enabled";
            }
            else
            {
                vrModeText.text = "VR Mode: Disabled";
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public void NewSession()
    {
        questionText.text = "Session ID: " + vrControllerScript.persistantDataScript.newSession();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public void OnRaycastClick(GameObject current)
    {
        switch (current.transform.name)
        {
            case "Sickness Mode":
                SicknessToggle(true);
                break;
            case "VR Mode":
                VRModeToggle(true);
                break;
            case "Question Controller":
                NewSession();
                break;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
}
