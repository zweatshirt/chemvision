
////////////////////////////////////////////////////////////////////////////////////////////////////
//
//      Persistant Data
//
//      Last Updated:               3/23/2018
//      Oldest Compatible Version:  2.14.1
//      Updated By:                 Kaleb Cole
//      Project:                    Chem Vision
//
////////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.IO;

////////////////////////////////////////////////////////////////////////////////////////////////////

public class PersistantData : MonoBehaviour
{
    //  Publics
    public float speed;
    public int fadeDirection = 0;
    public bool vrMode, sicknessMode, inEditor;
    public Texture2D fadeOutTexture;
    public float fadeAlpha = 1f;
    public List<Utility.SceneQuestionSet> sceneQuestionSetList;
    public Mode gameMode;
    public string sessionID;

    //  Privates
    private bool isInitialized = false;

    //  Statics
    private static Stack<string> sceneStack = new Stack<string>();

    //  Constants
    public readonly float FADE_SPEED = 1.5f;
    private static readonly string ERROR = "CVE [PersistantData] | ";

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    /*  Awake is called when all objects are enabled, before any Starts are called */

    private void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
        InitializeVariables();
        sessionID = Guid.NewGuid().ToString().Substring(0, 8);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    /*  Used to initialize the variables for this script, allowing them to be called from anywhere that
     *  needs to access them */

    public void InitializeVariables()
    {
        if (!isInitialized)
        {
            sceneQuestionSetList = new List<Utility.SceneQuestionSet>();
            isInitialized = true;
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.LinuxEditor)
            {
                gameMode = Mode.EditorMode_NonVR;
                vrMode = false;
            }
            else if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.LinuxPlayer)
            {
                gameMode = Mode.ComputerMode;
                vrMode = false;
            }
            else
            {
                gameMode = Mode.VRMode;
                vrMode = true;
            }
            speed = 50;
            sicknessMode = false;
            sceneStack.Push(null);
            if (SceneManager.GetActiveScene().name != "Main Menu Scene")
            {
                sceneStack.Push("Main Menu Scene");
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    /*  Returns the next item from the sceneStack without removing it */

    public string Peek()
    {
        return sceneStack.Peek();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    /*  Returns the next item from the sceneStack and removes it */

    public string Pop()
    {
        return sceneStack.Pop();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    private void OnGUI()
    {
        fadeAlpha += fadeDirection * Time.deltaTime / FADE_SPEED;
        fadeAlpha = Mathf.Clamp01(fadeAlpha);
        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, fadeAlpha);
        GUI.depth = -1000;
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeOutTexture);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    /*  Adds an item to the top of the sceneStack */

    public void Push(string stringToPush)
    {
        sceneStack.Push(stringToPush);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public void SaveQuestions()
    {
        int index = sceneQuestionSetList.FindIndex(x => x.GetSceneName().Equals(SceneManager.GetActiveScene().name));
        if (index != -1)
        {
            sceneQuestionSetList[index].questionList.Clear();
            List<GameObject> tempGameObjectList = new List<GameObject>();
            tempGameObjectList.AddRange(GameObject.FindGameObjectsWithTag(Utility.TAG_Question));
            foreach (GameObject currentObject in tempGameObjectList)
            {
                sceneQuestionSetList[index].questionList.AddRange(currentObject.GetComponent<QuestionController>().questionDataList);
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public int GetSceneQuestionSetIndex()
    {
        return sceneQuestionSetList.FindIndex(x => x.GetSceneName().Equals(SceneManager.GetActiveScene().name));
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public string newSession()
    {
        string path;

        if(gameMode == Mode.ComputerMode || gameMode == Mode.EditorMode_NonVR || gameMode == Mode.EditorMode_VR)
        {
            path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Answers_Folder\\";
        } else
        {
            path = "/mnt/sdcard/Answers_Folder/";
        }

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        FileInfo fout = new FileInfo(path + sessionID + "_Answers.txt");
        StreamWriter wout;
        Debug.Log(ERROR + "Saved Answers: " + path);

        wout = fout.CreateText();

        wout.WriteLine("Session ID: " + sessionID);
        wout.WriteLine("Time of Save: " + DateTime.Now);

        bool deleteFile = true;
        foreach (Utility.SceneQuestionSet currentSceneQuestionData in sceneQuestionSetList)
        {
            if (currentSceneQuestionData.questionList.Count != 0)
            {
                deleteFile = false;
                int numCorrect = 0, questionNum = 1;

                wout.WriteLine();
                wout.WriteLine("--------------------------------------------------");
                wout.WriteLine();
                wout.WriteLine("Scene Name: " + currentSceneQuestionData.GetSceneName());
                wout.WriteLine();

                currentSceneQuestionData.questionList.Sort(delegate (Utility.QuestionData x, Utility.QuestionData y) { return x.question.CompareTo(y.question); });
                foreach (Utility.QuestionData currentQuestionData in currentSceneQuestionData.questionList)
                {
                    if (currentQuestionData.question.Length > 50)
                    {
                        wout.Write(questionNum + ".) " + string.Format("{0, -50}", currentQuestionData.question.Substring(0, 47) + "..."));
                    }
                    else
                    {
                        wout.Write(questionNum + ".) " + string.Format("{0, -50}", currentQuestionData.question));
                    }

                    for (int i = 1; i < 4; i++)
                    {
                        if (i == currentQuestionData.playerChoice)
                        {
                            wout.Write(" [X] ");
                        }
                        else if (i == currentQuestionData.correctAnswer)
                        {
                            wout.Write(" [C] ");
                        }
                        else
                        {
                            wout.Write(" [ ] ");
                        }
                        wout.Write(string.Format("{0, -15}", currentQuestionData.answerList[i - 1]));
                    }

                    if (currentQuestionData.playerChoice == 0)
                    {
                        wout.Write(" (Unanswered)");
                    }
                    else if (currentQuestionData.playerChoice == currentQuestionData.correctAnswer)
                    {
                        numCorrect++;
                        wout.Write(" (Correct)");
                    }
                    else
                    {
                        wout.Write(" (Wrong)");
                    }
                    wout.WriteLine();
                    questionNum++;
                }
                wout.WriteLine();
                wout.WriteLine("Score: " + numCorrect + "/" + (questionNum - 1));
            }
        }

        wout.WriteLine();
        wout.WriteLine("--------------------------------------------------");
        wout.WriteLine();
        wout.Close();

        if (deleteFile)
        {
            fout.Delete();
        }

        sceneQuestionSetList.Clear();
        sessionID = Guid.NewGuid().ToString().Substring(0, 8);
        GameObject.FindGameObjectWithTag(Utility.TAG_Player).transform.Find("Main Camera").Find("Player HUD").Find("Session ID Text").GetComponent<Text>().text = sessionID;
        return sessionID;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
}
