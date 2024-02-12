
////////////////////////////////////////////////////////////////////////////////////////////////////
//
//      VR Controller
//
//      Last Updated:               3/29/2018
//      Oldest Compatible Version:  2.15.3
//      Updated By:                 Kaleb Cole
//      Project:                    Chem Vision
//
////////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

////////////////////////////////////////////////////////////////////////////////////////////////////

public class VRController : MonoBehaviour
{
    //  Publics
    public GameObject gvrViewerMain, eventSystem, persistantObject, INFO_PREFAB, EMPTY_PREFAB, QUESTION_PREFAB;
    public GameObject playerVR_Prefab, playerNon_Prefab;
    public TextAsset informationTextFile, questionTextFile;
    public Vector3 playerPosition;
    public float playerRotationY;
    public bool letPlayerMove;

    public GameObject player, terrain;
    public PersistantData persistantDataScript;
    public VRPlayerController vrPlayerControllerScript;
    public NonVRPlayerController nonVRPlayerControllerScript;
    public GvrReticle gvrReticleScript;
    public Camera mainCamera;

    public Utility.SceneQuestionSet currentSceneQuestionSet;

    //  Privates
    private string sceneChange = "None";
    private bool changingScenes = true, startSceneChange = false, isInitialized = false;
    private List<Utility.InformationData> informationDataList = new List<Utility.InformationData>();

    //  Constants
    private static readonly string ERROR = "CVE [VR Controller] | ";

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    /*  Awake is called when all objects are enabled, before any Starts are called */

    private void Awake()
    {
        SetupPersistantData();
        SetupPlayer();
        SetupGVRViewer();
        SetupEventSystem();
        SetGlobalReferences();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public void SetGlobalReferences()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError(ERROR + "Camera Not Set");
        }

        terrain = GameObject.FindGameObjectWithTag(Utility.TAG_Terrain);
        if (terrain == null)
        {
            Debug.LogError(ERROR + "Terrain does not exist with '- Terrain -' tag");
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    /*  Start is called when the script is enabled, before any Updates are called */

    private void Start()
    {
        changingScenes = false;
        persistantDataScript.fadeDirection = -1;
        ParseTextFiles();
        InformationObjectConstructor();
        QuestionObjectConstructor();
        InformationAndQuestionCleanup();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    /*  LateUpdate is called once per frame, after all of the Updates */

    private void LateUpdate()
    {
        if (!changingScenes)
        {
            if ((Input.deviceOrientation == DeviceOrientation.Portrait || persistantDataScript.gameMode == Mode.EditorMode_VR) && (mainCamera.transform.eulerAngles.z < 290 && mainCamera.transform.eulerAngles.z > 280))
            {
                startSceneChange = true;
                if (persistantObject.GetComponent<PersistantData>().Peek() != null)
                {
                    persistantDataScript.fadeDirection = 1;
                }
                sceneChange = "Previous";
            }
            if ((Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown || persistantDataScript.gameMode == Mode.EditorMode_VR) && (mainCamera.transform.eulerAngles.z < 90 && mainCamera.transform.eulerAngles.z > 80))
            {
                if (SceneManager.GetActiveScene().name != "Options Scene")
                {
                    startSceneChange = true;
                    persistantDataScript.fadeDirection = 1;
                    sceneChange = "Options";
                }
            }
        }
        if (startSceneChange)
        {
            if ((Input.deviceOrientation == DeviceOrientation.LandscapeLeft || persistantDataScript.gameMode == Mode.EditorMode_VR) && (mainCamera.transform.eulerAngles.z < 30 && mainCamera.transform.eulerAngles.z > -30))
            {
                startSceneChange = false;
                if (sceneChange == "Previous")
                {
                    PreviousScene();
                }
                else if (sceneChange == "Options")
                {
                    NextScene("Options Scene");
                }
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    /*  Changes to the next scene and adds the current scene to the sceneStack */

    public void NextScene(string nextScene)
    {
        if (!changingScenes)
        {
            persistantDataScript.SaveQuestions();
            changingScenes = true;
            persistantObject.GetComponent<PersistantData>().Push(SceneManager.GetActiveScene().name);
            StartCoroutine(AsynchronousLoad(nextScene));
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    /*  Changes scenes to the top of the sceneStack and removes the scene from the stack */

    public void PreviousScene()
    {
        if (!changingScenes && persistantObject.GetComponent<PersistantData>().Peek() != null)
        {
            persistantDataScript.SaveQuestions();
            changingScenes = true;
            StartCoroutine(AsynchronousLoad(persistantObject.GetComponent<PersistantData>().Pop()));
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    /*  The Asynchronous Load for allowing the new items to be loaded in before the old ones are removed */

    IEnumerator AsynchronousLoad(string newScene)
    {
        while (persistantDataScript.fadeAlpha < 1)
        {
            yield return null;
        }

        AsyncOperation load = SceneManager.LoadSceneAsync(newScene);
        while (!load.isDone)
        {
            yield return null;
        }

        AsyncOperation unload = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        while (!unload.isDone)
        {
            yield return null;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    private void SetupPlayer()
    {
        Utility.DestroyWithTag(Utility.TAG_Player);
        if (persistantDataScript.gameMode == Mode.VRMode || persistantDataScript.gameMode == Mode.EditorMode_VR)
        {
            Instantiate(playerVR_Prefab, playerPosition, Quaternion.Euler(new Vector3(0, playerRotationY, 0)));
            player = GameObject.FindGameObjectWithTag(Utility.TAG_Player);
            vrPlayerControllerScript = player.GetComponent<VRPlayerController>();
        }
        else if (persistantDataScript.gameMode == Mode.ComputerMode || persistantDataScript.gameMode == Mode.EditorMode_NonVR)
        {
            Instantiate(playerNon_Prefab, playerPosition, Quaternion.Euler(new Vector3(0, playerRotationY, 0)));
            player = GameObject.FindGameObjectWithTag(Utility.TAG_Player);
            nonVRPlayerControllerScript = player.GetComponent<NonVRPlayerController>();
        }
        gvrReticleScript = player.transform.Find("Main Camera").Find("GvrReticle").GetComponent<GvrReticle>();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    private void SetupGVRViewer()
    {
        Utility.DestroyWithTag(Utility.TAG_GvrViewerMain);
        if (persistantDataScript.gameMode == Mode.VRMode || persistantDataScript.gameMode == Mode.EditorMode_VR)
        {
            Instantiate(gvrViewerMain, new Vector3(0, 0, 0), Quaternion.Euler(new Vector3(0, 0, 0)));
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    private void SetupEventSystem()
    {
        Utility.DestroyWithTag(Utility.TAG_EventSystem);
        if (persistantDataScript.gameMode == Mode.VRMode || persistantDataScript.gameMode == Mode.EditorMode_VR)
        {
            Instantiate(eventSystem, new Vector3(0, 0, 0), Quaternion.Euler(new Vector3(0, 0, 0)));
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    /*  Checks for the PersistantObject and if it doesn't exist it creates one */

    private void SetupPersistantData()
    {
        if (GameObject.FindGameObjectWithTag(Utility.TAG_DoNotDestroy) == null)
        {
            GameObject newObject = Instantiate(persistantObject, new Vector3(0, 0, 0), Quaternion.Euler(new Vector3(0, 0, 0)));
            persistantDataScript = newObject.GetComponent<PersistantData>();
            persistantDataScript.InitializeVariables();
        }
        else
        {
            persistantDataScript = GameObject.FindGameObjectWithTag(Utility.TAG_DoNotDestroy).GetComponent<PersistantData>();
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    private void ParseTextFiles()
    {
        if (persistantDataScript.GetSceneQuestionSetIndex() == -1)
        {
            QuestionParser();
        }
        currentSceneQuestionSet = persistantDataScript.sceneQuestionSetList[persistantDataScript.GetSceneQuestionSetIndex()];

        if (informationTextFile)
        {
            InformationParser();
        }
        else
        {
            Debug.LogWarning(ERROR + "Information text file not set!");
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public void UpdateQuestionsLeft()
    {
        if (persistantDataScript.gameMode == Mode.VRMode || persistantDataScript.gameMode == Mode.EditorMode_VR)
        {
            vrPlayerControllerScript.questionsLeftText.text = currentSceneQuestionSet.GetQuestionCountText();
        }
        else if (persistantDataScript.gameMode == Mode.ComputerMode || persistantDataScript.gameMode == Mode.EditorMode_NonVR)
        {
            nonVRPlayerControllerScript.questionsLeftText.text = currentSceneQuestionSet.GetQuestionCountText();
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    private void InformationParser()
    {
        int lineCount = 1;
        List<string> tempLineParsed = new List<string>();

        tempLineParsed.AddRange(informationTextFile.text.Split(Utility.DEL_NewLine));
        foreach (string currentString in tempLineParsed)
        {
            List<string> tempFullParsed = new List<string>();
            tempFullParsed.AddRange(currentString.Split(Utility.DEL_Dash));
            if (tempFullParsed.Count == 2)
            {
                informationDataList.Add(new Utility.InformationData(tempFullParsed[0], tempFullParsed[1]));
            }
            else
            {
                if (tempFullParsed[0].Trim() != "")
                {
                    Debug.LogError(ERROR + "Information format error on line: " + lineCount);
                }
            }
            lineCount++;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    private void QuestionParser()
    {
        if (questionTextFile)
        {
            int lineCount = 1;
            List<string> tempLineParsed = new List<string>();
            List<Utility.QuestionData> TempQuestionDataList = new List<Utility.QuestionData>();

            tempLineParsed.AddRange(questionTextFile.text.Split(Utility.DEL_NewLine));
            foreach (string currentString in tempLineParsed)
            {
                List<string> tempFullParsed = new List<string>();
                tempFullParsed.AddRange(currentString.Split(Utility.DEL_Dash));
                if (tempFullParsed.Count == 7)
                {
                    TempQuestionDataList.Add(new Utility.QuestionData(tempFullParsed[0], int.Parse(tempFullParsed[1].Trim()), tempFullParsed[2], tempFullParsed[3], tempFullParsed[4], tempFullParsed[5], int.Parse(tempFullParsed[6].Trim())));
                }
                else
                {
                    if (tempFullParsed[0].Trim() != "")
                    {
                        Debug.LogError(ERROR + "Question format error on line: " + lineCount);
                    }
                }
                lineCount++;
            }

            persistantDataScript.sceneQuestionSetList.Add(new Utility.SceneQuestionSet(SceneManager.GetActiveScene().name, TempQuestionDataList));
        }
        else
        {
            Debug.LogWarning(ERROR + "Question text file not set!");
            persistantDataScript.sceneQuestionSetList.Add(new Utility.SceneQuestionSet(SceneManager.GetActiveScene().name, new List<Utility.QuestionData>()));
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    private void InformationObjectConstructor()
    {
        foreach (Utility.InformationData currentData in informationDataList)
        {
            GameObject currentGameObject = GameObject.Find(currentData.name);
            if (currentGameObject)
            {
                GameObject emptyCenter = EmptyCenterConstructor(currentGameObject);
                GameObject informationClone = Instantiate(INFO_PREFAB, currentGameObject.transform.position, Quaternion.identity);
                informationClone.name = currentGameObject.name + Utility.NAME_Information;
                informationClone.tag = Utility.TAG_Information;
                informationClone.transform.SetParent(emptyCenter.transform);
                informationClone.transform.localPosition = new Vector3(0, 0, 0);
                informationClone.GetComponent<InformationController>().InitializeVariables();
                informationClone.GetComponent<InformationController>().SetString(currentData.information);
            }
            else
            {
                Debug.LogError(ERROR + currentData.name + " does not exist in this scene!");
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    private void QuestionObjectConstructor()
    {
        foreach (Utility.QuestionData currentData in currentSceneQuestionSet.questionList)
        {
            GameObject questionClone = GameObject.Find(currentData.name + Utility.NAME_Question);
            if (!questionClone)
            {
                GameObject currentGameObject = GameObject.Find(currentData.name);
                if (currentGameObject)
                {
                    GameObject emptyCenter = EmptyCenterConstructor(currentGameObject);
                    questionClone = Instantiate(QUESTION_PREFAB, currentGameObject.transform.position, Quaternion.identity);
                    questionClone.name = currentGameObject.name + Utility.NAME_Question;
                    questionClone.tag = Utility.TAG_Question;
                    questionClone.transform.SetParent(emptyCenter.transform);
                    questionClone.transform.localPosition = new Vector3(0, 0, 0);
                    questionClone.GetComponent<QuestionController>().InitializeVariables();
                }
                else
                {
                    Debug.LogError(ERROR + currentData.name + " does not exist in this scene!");
                }
            }
            questionClone.GetComponent<QuestionController>().questionDataList.Add(currentData);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    private GameObject EmptyCenterConstructor(GameObject currentObject)
    {
        GameObject emptyCenter = GameObject.Find(currentObject.name + Utility.NAME_Empty);
        if (!emptyCenter)
        {
            emptyCenter = Instantiate(EMPTY_PREFAB, currentObject.transform.position, Quaternion.identity);
            emptyCenter.name = currentObject.name + Utility.NAME_Empty;
            emptyCenter.tag = Utility.TAG_Empty;
            emptyCenter.GetComponent<EmptyController>().InitializeVariables();
            emptyCenter.GetComponent<EmptyController>().SetParent(currentObject);
        }
        return emptyCenter;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    private void InformationAndQuestionCleanup()
    {
        List<GameObject> emptyGameObjectList = new List<GameObject>();
        emptyGameObjectList.AddRange(GameObject.FindGameObjectsWithTag(Utility.TAG_Empty));
        foreach (GameObject currentEmpty in emptyGameObjectList)
        {
            GameObject parent = currentEmpty.GetComponent<EmptyController>().GetParent();
            Transform informationObject = currentEmpty.transform.Find(parent.name + Utility.NAME_Information);
            Transform questionObject = currentEmpty.transform.Find(parent.name + Utility.NAME_Question);
            OffsetValues tempOffsetScript = parent.GetComponent<OffsetValues>();
            if (tempOffsetScript)
            {
                currentEmpty.transform.position = currentEmpty.transform.position + new Vector3(0, tempOffsetScript.verticleOffset, 0);
                if (informationObject)
                {
                    informationObject.localPosition = new Vector3(tempOffsetScript.horizontalOffset, 0, 0);
                }
                if (questionObject)
                {
                    questionObject.localPosition = new Vector3(-tempOffsetScript.horizontalOffset, 0, 0);
                }
            }
            else
            {
                currentEmpty.transform.position = currentEmpty.transform.position + new Vector3(0, Utility.OFFSET_Verticle, 0);
                if (informationObject && questionObject)
                {
                    informationObject.localPosition = new Vector3(Utility.OFFSET_Horizontal, 0, 0);
                    questionObject.localPosition = new Vector3(-Utility.OFFSET_Horizontal, 0, 0);
                }
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
}
