
////////////////////////////////////////////////////////////////////////////////////////////////////
//
//      Movement Controller
//
//      Last Updated:               4/20/2018
//      Oldest Compatible Version:  2.18.0
//      Updated By:                 Kaleb Cole
//      Project:                    Chem Vision
//
////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

////////////////////////////////////////////////////////////////////////////////////////////////////

public class MovementController : MonoBehaviour
{

    //  Publics
    public TextAsset animationText;

    //  Privates
    private List<GameObject> animationObjectList;
    private GameObject forwards, blankForwards, stepForwards, reverse, blankReverse, stepReverse, play, pause, speed;
    private Text speedText;
    private int currentPhase, maxPhases, currentSpeedMultiplier;
    private Animate currentAnimation;

    //  Constants
    public const float GROW_SHRINK = 10f;
    private static readonly string ERROR = "CVE [Animation Controller] | ";

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    private void Awake()
    {
        animationObjectList = new List<GameObject>();

        forwards = transform.Find("Animation Canvas").Find("Forwards").gameObject;
        blankForwards = transform.Find("Animation Canvas").Find("Blank Forwards").gameObject;
        stepForwards = transform.Find("Animation Canvas").Find("Step Forwards").gameObject;
        reverse = transform.Find("Animation Canvas").Find("Reverse").gameObject;
        blankReverse = transform.Find("Animation Canvas").Find("Blank Reverse").gameObject;
        stepReverse = transform.Find("Animation Canvas").Find("Step Reverse").gameObject;
        play = transform.Find("Animation Canvas").Find("Play").gameObject;
        pause = transform.Find("Animation Canvas").Find("Pause").gameObject;
        speed = transform.Find("Animation Canvas").Find("Speed").gameObject;

        speedText = speed.transform.Find("Speed Text").GetComponent<Text>();
        currentAnimation = Animate.None;
        currentSpeedMultiplier = 10;
        currentPhase = 0;
        maxPhases = 0;

        speedText.text = "x " + (currentSpeedMultiplier / 10) + "." + (currentSpeedMultiplier % 10);

        SetPaused(false);
        SetButtons(true, true, false);

        if (animationText)
        {
            AnimationParser();
        }
        else
        {
            Debug.LogError(ERROR + "Question text file not set!");
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    private void LateUpdate()
    {
        bool flag = true;
        foreach (GameObject currentObject in animationObjectList)
        {
            if (currentObject.GetComponent<Movement>().animate != Animate.None)
            {
                flag = false;
            }
        }
        if (flag)
        {
            currentAnimation = Animate.None;
            SetPaused(false);
            SetButtons((currentPhase != maxPhases), (currentPhase != 0), false);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    private void AnimationParser()
    {
        List<string> lineParsed = new List<string>();
        lineParsed.AddRange(animationText.text.Split(Utility.DEL_NewLine));
        List<string> equalParsed = new List<string>();

        foreach (string currentLine in lineParsed)
        {
            equalParsed.AddRange(currentLine.Split(Utility.DEL_Equal));
        }

        List<Utility.AnimationInformation> animationList = new List<Utility.AnimationInformation>();
        Vector3 lastPosition = new Vector3();
        Quaternion lastRotation = new Quaternion();
        int currentObject = -1, tempCurrentPhase = 0;
        List<string> tempList;

        int lineCount = 1;
        for (int i = 0; i < equalParsed.Count; i++)
        {
            switch (equalParsed[i].ToUpper().Trim())
            {
                case "GAMEOBJECT":
                    i++;
                    GameObject newObject = GameObject.Find(equalParsed[i].Trim());
                    if (newObject)
                    {
                        SaveList(currentObject, ref tempCurrentPhase, ref animationList);
                        maxPhases = Mathf.Max(maxPhases, tempCurrentPhase);
                        tempCurrentPhase = 0;

                        newObject.AddComponent<Movement>();
                        newObject.GetComponent<Movement>().InitializeVariables();
                        lastPosition = newObject.transform.position;
                        lastRotation = newObject.transform.rotation;
                        animationObjectList.Add(newObject);
                        animationList = new List<Utility.AnimationInformation>();
                        currentObject = animationObjectList.Count - 1;
                    }
                    else
                    {
                        Debug.LogError(ERROR + equalParsed[i].Trim() + " could not be found in scene!");
                    }
                    break;
                case "MOVE":
                    i++;
                    tempList = new List<string>();
                    tempList.AddRange(equalParsed[i].Split(Utility.DEL_Comma));
                    if (tempList.Count == 4)
                    {
                        Vector3 tempVector3 = new Vector3(float.Parse(tempList[0]), float.Parse(tempList[1]), float.Parse(tempList[2]));
                        animationList.Add(new Utility.AnimationInformation(lastPosition, tempVector3, Mathf.Max(0, float.Parse(tempList[3]))));
                        lastPosition = tempVector3;
                    }
                    else
                    {
                        Debug.LogError(ERROR + "Incomplete 'Move' on line: " + lineCount);
                    }
                    break;
                case "ROTATE":
                    i++;
                    tempList = new List<string>();
                    tempList.AddRange(equalParsed[i].Split(Utility.DEL_Comma));
                    if (tempList.Count == 4)
                    {
                        Quaternion tempQuaternion = Quaternion.Euler(float.Parse(tempList[0]), float.Parse(tempList[1]), float.Parse(tempList[2]));
                        animationList.Add(new Utility.AnimationInformation(lastRotation, tempQuaternion, Mathf.Max(0, float.Parse(tempList[3]))));
                        lastRotation = tempQuaternion;
                    }
                    else
                    {
                        Debug.LogError(ERROR + "Incomplete 'Rotate' on line: " + lineCount);
                    }
                    break;
                case "PAUSE":
                    i++;
                    int tempNum = Mathf.Max(1, int.Parse(equalParsed[i]));
                    for (int j = 0; j < tempNum; j++)
                    {
                        SaveList(currentObject, ref tempCurrentPhase, ref animationList);
                    }
                    break;
                case "":
                    break;
                default:
                    Debug.LogError(ERROR + "Unrecognized keyword on line: " + lineCount);
                    break;
            }
            lineCount++;
        }
        SaveList(currentObject, ref tempCurrentPhase, ref animationList);
        maxPhases = Mathf.Max(maxPhases, tempCurrentPhase);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    private void SaveList(int currentObject, ref int currentPhase, ref List<Utility.AnimationInformation> animationList)
    {
        if (currentObject != -1)
        {
            animationObjectList[currentObject].GetComponent<Movement>().animationDictionary.Add(currentPhase, animationList);
            animationList = new List<Utility.AnimationInformation>();
            currentPhase++;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    /*  Enlarges the highlighted object */

    public void OnRaycastEnter(GameObject currentObject)
    {
        if (currentObject != speed)
        {
            if (currentObject.GetComponent<Image>())
            {
                currentObject.GetComponent<Image>().rectTransform.sizeDelta += new Vector2(GROW_SHRINK, GROW_SHRINK);
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    /*  Shrinks the object that was highlighted but is no longer being looked at */

    public void OnRaycastExit(GameObject currentObject)
    {
        if (currentObject != speed)
        {
            if (currentObject.GetComponent<Image>())
            {
                currentObject.GetComponent<Image>().rectTransform.sizeDelta -= new Vector2(GROW_SHRINK, GROW_SHRINK);
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public void OnRaycastClick(GameObject callingObject)
    {
        switch (callingObject.transform.name)
        {
            case "Forwards":
                currentAnimation = Animate.Forward;
                SetButtons(false, false, false);
                foreach (GameObject currentObject in animationObjectList)
                {
                    Movement tempScript = currentObject.GetComponent<Movement>();
                    if (currentPhase < tempScript.animationDictionary.Count)
                    {
                        tempScript.SetDefaults(currentPhase, 0, Animate.Forward);
                    }
                }
                currentPhase = Mathf.Clamp(++currentPhase, 0, maxPhases);
                break;
            case "Reverse":
                currentAnimation = Animate.Reverse;
                SetButtons(false, false, false);
                foreach (GameObject currentObject in animationObjectList)
                {
                    Movement tempScript = currentObject.GetComponent<Movement>();
                    if ((currentPhase - 1) < tempScript.animationDictionary.Count && currentPhase > 0)
                    {
                        tempScript.SetDefaults((currentPhase - 1), (tempScript.animationDictionary[currentPhase - 1].Count - 1), Animate.Reverse);
                    }
                }
                currentPhase = Mathf.Clamp(--currentPhase, 0, maxPhases);
                break;
            case "Reset":
                currentAnimation = Animate.None;
                foreach (GameObject currentObject in animationObjectList)
                {
                    Movement tempScript = currentObject.GetComponent<Movement>();
                    tempScript.SetDefaults(0, 0, Animate.Reset);
                }
                currentPhase = 0;
                SetPaused(false);
                SetButtons(true, true, false);
                break;
            case "Pause":
                SetPaused(true);
                break;
            case "Play":
                SetPaused(false);
                break;
            case "Step Forwards":
            case "Step Reverse":
                foreach (GameObject currentObject in animationObjectList)
                {
                    currentObject.GetComponent<Movement>().oneRun = true;
                }
                break;
            case "Speed Up":
                currentSpeedMultiplier = Mathf.Clamp(currentSpeedMultiplier + 1, 1, 20);
                speedText.text = "x " + (currentSpeedMultiplier / 10) + "." + (currentSpeedMultiplier % 10);
                foreach (GameObject currentObject in animationObjectList)
                {
                    currentObject.GetComponent<Movement>().currentSpeedMultiplier = currentSpeedMultiplier;
                }
                break;
            case "Speed Down":
                currentSpeedMultiplier = Mathf.Clamp(currentSpeedMultiplier - 1, 1, 20);
                speedText.text = "x " + (currentSpeedMultiplier / 10) + "." + (currentSpeedMultiplier % 10);
                foreach (GameObject currentObject in animationObjectList)
                {
                    currentObject.GetComponent<Movement>().currentSpeedMultiplier = currentSpeedMultiplier;
                }
                break;
            case "Blank Forwards":
            case "Blank Reverse":
                break;
            default:
                Debug.LogError(ERROR + "Error with Generation!");
                break;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    private void SetPaused(bool pauseBool)
    {
        play.SetActive(pauseBool);
        pause.SetActive(!pauseBool);
        SetButtons((currentPhase != maxPhases), (currentPhase != 0), pauseBool);

        foreach (GameObject currentObject in animationObjectList)
        {
            currentObject.GetComponent<Movement>().isPaused = pauseBool;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    private void SetButtons(bool forwardBool, bool reverseBool, bool stepBool)
    {
        forwards.SetActive(forwardBool && !stepBool);
        reverse.SetActive(reverseBool && !stepBool);
        blankForwards.SetActive(!forwardBool && !stepBool);
        blankReverse.SetActive(!reverseBool && !stepBool);
        stepForwards.SetActive(stepBool && (currentAnimation == Animate.Forward));
        stepReverse.SetActive(stepBool && (currentAnimation == Animate.Reverse));
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
}
