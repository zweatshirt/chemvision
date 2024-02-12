
////////////////////////////////////////////////////////////////////////////////////////////////////
//
//      Utility Script
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

public enum Mode { ComputerMode, VRMode, EditorMode_VR, EditorMode_NonVR };
public enum Animate { None, Forward, Reverse, Reset };

////////////////////////////////////////////////////////////////////////////////////////////////////

public class Utility : MonoBehaviour
{
    //  Publics
    public static readonly Color32 UI_Cyan = new Color32(0, 255, 255, 255);
    public static readonly Color32 UI_Orange = new Color32(255, 170, 0, 255);

    //  ReadOnlys
    public static readonly string TAG_Information = "- Information -", TAG_Question = "- Question -", TAG_Empty = "- Empty -", TAG_Player = "- Player -", TAG_VRController = "- VR Controller -";
    public static readonly string TAG_Terrain = "- Terrain -", TAG_DoNotDestroy = "- DoNotDestroy -", TAG_GvrViewerMain = "- GvrViewerMain -", TAG_EventSystem = "- EventSystem -", TAG_Generation = "- Generation -";
    public static readonly string TAG_Untagged = "Untagged";
    public static readonly string NAME_Information = " <Information>", NAME_Question = " <Question>", NAME_Empty = " <Empty>", NAME_Generation = " <Generation>";
    public static readonly char DEL_NewLine = '\n', DEL_Dash = '-', DEL_Comma = ',', DEL_Equal = '=';
    public static readonly float OFFSET_Verticle = 30f, OFFSET_Horizontal = 15f;

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public static int DestroyWithTag(string tag)
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(tag);
        for (int i = 0; i < gameObjects.Length; i++)
        {
            gameObjects[i].transform.tag = TAG_Untagged;
            Destroy(gameObjects[i]);
        }
        return gameObjects.Length;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public static void SendMessageTo(GameObject target, string message)
    {
        GameObject rootObject = target;
        if (target)
        {
            if (rootObject.transform.root.tag == TAG_Empty || rootObject.transform.root.tag == TAG_Generation)
            {
                while (rootObject.transform.parent.tag != TAG_Empty && rootObject.transform.root.tag != TAG_Generation)
                {
                    rootObject = rootObject.transform.parent.gameObject;
                }
                rootObject.SendMessage(message, target, SendMessageOptions.DontRequireReceiver);
            }
            else
            {
                target.transform.root.SendMessage(message, target, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //                                                                                                                                                                                                    //
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public class QuestionData
    {
        //  Publics
        public string name;
        public int priority, correctAnswer, playerChoice;
        public string question;
        public List<string> answerList;

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public QuestionData(string name, int priority, string question, string one, string two, string three, int correctAnswer)
        {
            this.name = name.Trim();
            this.priority = priority;
            this.question = question.Trim();
            this.correctAnswer = correctAnswer;
            answerList = new List<string> { one.Trim(), two.Trim(), three.Trim() };
            playerChoice = 0;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void DisplayQuestion(Text[] textArray)
        {
            textArray[0].text = question;
            textArray[1].text = answerList[0];
            textArray[2].text = answerList[1];
            textArray[3].text = answerList[2];
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //                                                                                                                                                                                                    //
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public class InformationData
    {
        //  Publics
        public string name, information;

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public InformationData(string name, string information)
        {
            this.name = name.Trim();
            this.information = information.Trim();
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //                                                                                                                                                                                                    //
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public class SceneQuestionSet
    {
        //  Publics
        public List<QuestionData> questionList;

        //  Privates
        private string sceneName;
        private int totalQuestions, questionsAnswered;

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public SceneQuestionSet(string sceneName, List<QuestionData> questionList)
        {
            this.sceneName = sceneName;
            this.totalQuestions = questionList.Count;
            this.questionList = questionList;
            questionsAnswered = 0;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void AnsweredQuestion()
        {
            questionsAnswered++;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public string GetSceneName()
        {
            return sceneName;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public string GetQuestionCountText()
        {
            if (totalQuestions > 0)
            {
                if (totalQuestions == questionsAnswered)
                {
                    return "All Questions Answered";
                }
                else
                {
                    return "Questions Answered: " + questionsAnswered + "/" + totalQuestions;
                }
            }
            else
            {
                return "No Questions";
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //                                                                                                                                                                                                    //
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public class ArrowData
    {
        //  Publics
        public float positiveNumber, negativeNumber;
        public bool needToTurn, turnPositive;
        public Image rightArrow, leftArrow;
        public Camera mainCamera;

        //  Privates
        private VRController vrControllerScript;

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        /*  Constructor for ArrowData */

        public ArrowData()
        {
            vrControllerScript = GameObject.FindGameObjectWithTag("- VR Controller -").GetComponent<VRController>();

            turnPositive = false;
            needToTurn = false;
            mainCamera = Camera.main;

            GameObject playerHUD = vrControllerScript.player.transform.Find("Main Camera").Find("Player HUD").gameObject;
            rightArrow = playerHUD.transform.Find("Right Arrow").GetComponent<Image>();
            leftArrow = playerHUD.transform.Find("Left Arrow").GetComponent<Image>();
            rightArrow.enabled = false;
            leftArrow.enabled = false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        /*  Calculates the data needed for the Arrows to function properly */

        public void SetArrow(GameObject objectToFace, GameObject currentWaypoint)
        {
            Vector3 temp = (objectToFace.transform.position - currentWaypoint.transform.position);
            float angle = Mathf.Atan2(temp.x, temp.z) * Mathf.Rad2Deg;
            if (angle > 0)
            {
                positiveNumber = angle;
                negativeNumber = angle - 180;
                turnPositive = true;
            }
            else
            {
                positiveNumber = angle + 180;
                negativeNumber = angle;
                turnPositive = false;
            }
            needToTurn = true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        /*  Uses the data created in SetArrow() to keep track of which arrow should be on and off and updates
            the arrows being visible */

        public void ArrowUpdate()
        {
            float currentAngle = mainCamera.transform.eulerAngles.y;
            if (currentAngle > 180)
            {
                currentAngle -= 360;
            }
            if (turnPositive)
            {
                if (currentAngle <= positiveNumber - 10 && currentAngle >= negativeNumber)       // Turn Right
                {
                    leftArrow.enabled = false;
                    rightArrow.enabled = true;
                }
                else if (currentAngle < negativeNumber || currentAngle > positiveNumber + 10)   // Turn Left
                {
                    leftArrow.enabled = true;
                    rightArrow.enabled = false;
                }
                else
                {
                    clearArrow();
                }
            }
            else
            {
                if (currentAngle <= positiveNumber && currentAngle >= negativeNumber + 10)      // Closest to 0
                {
                    leftArrow.enabled = true;
                    rightArrow.enabled = false;
                }
                else if (currentAngle < negativeNumber - 10 || currentAngle > positiveNumber)   // Closest to 180
                {
                    leftArrow.enabled = false;
                    rightArrow.enabled = true;
                }
                else
                {
                    clearArrow();
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void clearArrow()
        {
            leftArrow.enabled = false;
            rightArrow.enabled = false;
            needToTurn = false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //                                                                                                                                                                                                    //
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public class AnimationInformation
    {
        //  Privates
        private float speed;
        private Vector3 positionOne, positionTwo;
        private Quaternion rotationOne, rotationTwo;
        private bool isMovement;
        private float deltTime;

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public AnimationInformation(Vector3 positionOne, Vector3 positionTwo, float speed)
        {
            isMovement = true;
            deltTime = 0;
            this.speed = speed;
            this.positionOne = positionOne;
            this.positionTwo = positionTwo;
            rotationOne = new Quaternion();
            rotationTwo = new Quaternion();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public AnimationInformation(Quaternion rotationOne, Quaternion rotationTwo, float speed)
        {
            isMovement = false;
            deltTime = 0;
            this.speed = speed;
            positionOne = new Vector3();
            positionTwo = new Vector3();
            this.rotationOne = rotationOne;
            this.rotationTwo = rotationTwo;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void ResetTime()
        {
            deltTime = 0;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool AnimateMe(GameObject currentObject, int direction, int speedMult)
        {
            if (speed == 0)
            {
                if (isMovement && direction == 1)
                {
                    currentObject.transform.position = positionTwo;
                }
                else if (isMovement && direction == -1)
                {
                    currentObject.transform.position = positionOne;
                }
                else if (!isMovement && direction == 1)
                {
                    currentObject.transform.rotation = rotationTwo;
                }
                else if (!isMovement && direction == -1)
                {
                    currentObject.transform.rotation = rotationOne;
                }
                else
                {
                    Debug.LogError("Error With Instant Animation!");
                    return false;
                }
                return true;
            }
            else
            {
                deltTime += Time.deltaTime / (speed / ((float)speedMult/10));
                if (isMovement && direction == 1)
                {
                    currentObject.transform.position = Vector3.Lerp(positionOne, positionTwo, deltTime);
                    if (currentObject.transform.position == positionTwo)
                    {
                        deltTime = 0;
                        return true;
                    }
                }
                else if (isMovement && direction == -1)
                {
                    currentObject.transform.position = Vector3.Lerp(positionTwo, positionOne, deltTime);
                    if (currentObject.transform.position == positionOne)
                    {
                        deltTime = 0;
                        return true;
                    }
                }
                else if (!isMovement && direction == 1)
                {
                    currentObject.transform.rotation = Quaternion.Lerp(rotationOne, rotationTwo, deltTime);
                    if (currentObject.transform.rotation == rotationTwo)
                    {
                        deltTime = 0;
                        return true;
                    }
                }
                else if (!isMovement && direction == -1)
                {
                    currentObject.transform.rotation = Quaternion.Lerp(rotationTwo, rotationOne, deltTime);
                    if (currentObject.transform.rotation == rotationOne)
                    {
                        deltTime = 0;
                        return true;
                    }
                }
            }
            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////

    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //                                                                                                                                                                                                    //
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}
