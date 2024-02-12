
////////////////////////////////////////////////////////////////////////////////////////////////////
//
//      Question Controller
//
//      Last Updated:               3/23/2018
//      Oldest Compatible Version:  2.14.1
//      Updated By:                 Kaleb Cole
//      Project:                    Chem Vision
//
////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

////////////////////////////////////////////////////////////////////////////////////////////////////

public class QuestionController : MonoBehaviour
{

    //  Publics
    public List<Utility.QuestionData> questionDataList;

    //  Privates
    private bool isInitialized = false, inQuestion = false, hasExited = false;
    private GameObject playerObject, questionMarkCanvasObject, questionCanvasObject, leftArrowObject, rightArrowObject;
    private GameObject[] answerHighlightArray;
    private Text[] questionTextArray;
    private float timeOfExit = 0;
    private int currentQuestionPosition = 0;
    private VRController vrControllerScript;

    //  Constants
    public const float FADE_TIME = 2f, GROW_SHRINK = 50f, WAIT_TIME = 1f;
    private Vector2 ARROW_ORIGINAL_SIZE = new Vector2(100f, 400f);
    private Vector3 SMALL_COLLIDER = new Vector3(5f, 7.5f, .1f), LARGE_COLLIDER = new Vector3(40f, 32.5f, .1f);
    private Color32 UI_CYAN = new Color32(0, 255, 255, 255);

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    /*  Start is called when the script is enabled, before any Updates are called */

    void Start()
    {
        InitializeVariables();
        questionDataList.Sort((x, y) => x.priority.CompareTo(y.priority));
        EndQuestion();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    /*  Update is called once per frame */

    void Update()
    {
        transform.LookAt(playerObject.transform);
        transform.Rotate(0, 180, 0);
        if (inQuestion && hasExited)
        {
            if (Time.time - timeOfExit > WAIT_TIME)
            {
                questionCanvasObject.GetComponent<CanvasGroup>().alpha = 1 - (Time.time - timeOfExit - WAIT_TIME) / FADE_TIME;
                if (Time.time - timeOfExit > FADE_TIME + WAIT_TIME)
                {
                    EndQuestion();
                    hasExited = false;
                }
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    /*  Used to initialize the variables for this script, allowing them to be called from anywhere that
     *  needs to access them */

    public void InitializeVariables()
    {
        if (!isInitialized)
        {
            isInitialized = true;

            playerObject = GameObject.FindGameObjectWithTag("- Player -");
            questionMarkCanvasObject = transform.Find("Question Mark Canvas").gameObject;
            questionCanvasObject = transform.Find("Question Canvas").gameObject;

            questionDataList = new List<Utility.QuestionData>();

            answerHighlightArray = new GameObject[3];
            answerHighlightArray[0] = questionCanvasObject.transform.Find("Answer 1").transform.Find("Answer 1 Highlight").gameObject;
            answerHighlightArray[1] = questionCanvasObject.transform.Find("Answer 2").transform.Find("Answer 2 Highlight").gameObject;
            answerHighlightArray[2] = questionCanvasObject.transform.Find("Answer 3").transform.Find("Answer 3 Highlight").gameObject;

            questionTextArray = new Text[4];
            questionTextArray[0] = questionCanvasObject.transform.Find("Question").Find("Question Text").GetComponent<Text>();
            questionTextArray[1] = questionCanvasObject.transform.Find("Answer 1").Find("Answer 1 Text").GetComponent<Text>();
            questionTextArray[2] = questionCanvasObject.transform.Find("Answer 2").Find("Answer 2 Text").GetComponent<Text>();
            questionTextArray[3] = questionCanvasObject.transform.Find("Answer 3").Find("Answer 3 Text").GetComponent<Text>();

            leftArrowObject = questionCanvasObject.transform.Find("Left Arrow").gameObject;
            rightArrowObject = questionCanvasObject.transform.Find("Right Arrow").gameObject;

            vrControllerScript = GameObject.FindGameObjectWithTag("- VR Controller -").GetComponent<VRController>();

            questionCanvasObject.SetActive(false);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    /*  This runs whenever a new question is started. This sets the proper colors and if the answer has
     *  already been given, shows the answer selected and the correct answer. */

    public void StartQuestion()
    {
        if (questionDataList[currentQuestionPosition].playerChoice != 0)
        {
            CheckAnswer(0);
        }
        else
        {
            answerHighlightArray[0].SetActive(false);
            answerHighlightArray[1].SetActive(false);
            answerHighlightArray[2].SetActive(false);
            questionTextArray[1].color = UI_CYAN;
            questionTextArray[2].color = UI_CYAN;
            questionTextArray[3].color = UI_CYAN;
        }

        leftArrowObject.SetActive(currentQuestionPosition != 0);
        rightArrowObject.SetActive(currentQuestionPosition != questionDataList.Count - 1);

        if (currentQuestionPosition == 0 && vrControllerScript.persistantDataScript.gameMode != Mode.ComputerMode)
        {
            leftArrowObject.GetComponent<Image>().rectTransform.sizeDelta = ARROW_ORIGINAL_SIZE;
        }
        else if (currentQuestionPosition == (questionDataList.Count - 1) && vrControllerScript.persistantDataScript.gameMode != Mode.ComputerMode)
        {
            rightArrowObject.GetComponent<Image>().rectTransform.sizeDelta = ARROW_ORIGINAL_SIZE;
        }

        questionDataList[currentQuestionPosition].DisplayQuestion(questionTextArray);

        questionMarkCanvasObject.SetActive(false);
        questionCanvasObject.SetActive(true);
        inQuestion = true;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    /*  Called to check the answer and set the correct one */

    public void CheckAnswer(int choice)
    {
        questionTextArray[1].color = Color.red;
        questionTextArray[2].color = Color.red;
        questionTextArray[3].color = Color.red;
        questionTextArray[questionDataList[currentQuestionPosition].correctAnswer].color = Color.green;

        if (questionDataList[currentQuestionPosition].playerChoice == 0)
        {
            questionDataList[currentQuestionPosition].playerChoice = choice;
            vrControllerScript.currentSceneQuestionSet.AnsweredQuestion();
            vrControllerScript.UpdateQuestionsLeft();
        }

        answerHighlightArray[0].SetActive(false);
        answerHighlightArray[1].SetActive(false);
        answerHighlightArray[2].SetActive(false);
        answerHighlightArray[questionDataList[currentQuestionPosition].playerChoice - 1].SetActive(true);
        answerHighlightArray[questionDataList[currentQuestionPosition].playerChoice - 1].GetComponent<Image>().color = questionTextArray[questionDataList[currentQuestionPosition].playerChoice].color;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    /*  Called when the person stops looking at the question */

    public void ExitQuestion()
    {
        hasExited = true;
        timeOfExit = Time.time;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    /*  Called when the player looks at the question */

    public void EnterQuestion()
    {
        questionCanvasObject.GetComponent<CanvasGroup>().alpha = 1;
        hasExited = false;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    /*  Called whenever the player activates an arrow to switch questions */

    public void TraverseQuestion(int movement)
    {
        currentQuestionPosition += movement;
        StartQuestion();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    /*  Enlarges the highlighted object */

    public void OnRaycastEnter(GameObject currentObject)
    {
        EnterQuestion();
        if (currentObject.GetComponent<Image>() && !currentObject.name.Contains("Question"))
        {
            currentObject.GetComponent<Image>().rectTransform.sizeDelta += new Vector2(GROW_SHRINK, GROW_SHRINK);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    /*  Shrinks the object that was highlighted but is no longer being looked at */

    public void OnRaycastExit(GameObject currentObject)
    {
        ExitQuestion();
        if (currentObject.GetComponent<Image>() && !currentObject.name.Contains("Question"))
        {
            currentObject.GetComponent<Image>().rectTransform.sizeDelta -= new Vector2(GROW_SHRINK, GROW_SHRINK);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    /*  Called to turn off the questions */

    private void EndQuestion()
    {
        questionCanvasObject.SetActive(false);
        questionMarkCanvasObject.SetActive(true);
        inQuestion = false;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public void OnRaycastClick(GameObject current)
    {
        switch (current.transform.name)
        {
            case "Question Mark":
                StartQuestion();
                break;
            case "Right Arrow":
                currentQuestionPosition++;
                StartQuestion();
                break;
            case "Left Arrow":
                currentQuestionPosition--;
                StartQuestion();
                break;
            case "Answer 1":
                CheckAnswer(1);
                break;
            case "Answer 2":
                CheckAnswer(2);
                break;
            case "Answer 3":
                CheckAnswer(3);
                break;
            default:
                break;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
}
