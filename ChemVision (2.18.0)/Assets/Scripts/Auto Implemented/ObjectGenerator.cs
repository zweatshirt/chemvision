
////////////////////////////////////////////////////////////////////////////////////////////////////
//
//      Object Generator
//
//      Last Updated:               3/29/2018
//      Oldest Compatible Version:  2.15.3
//      Updated By:                 Kaleb Cole
//      Project:                    Chem Vision
//
////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

////////////////////////////////////////////////////////////////////////////////////////////////////

public class ObjectGenerator : MonoBehaviour
{

    //  Publics
    public GameObject objectToGenerate;
    public Vector3 generationPosition;
    public Vector3 generationRotaion;
    public int maximumGenerations;

    //  Privates
    private Queue<GameObject> gameObjectQueue;
    private int totalGenerations = 0;
    private GameObject generationContainer;

    //  Constants
    public const float GROW_SHRINK = 10f;
    private static readonly string ERROR = "CVE [Object Generator] | ";

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    void Start()
    {
        maximumGenerations = Mathf.Max(1, maximumGenerations);
        gameObjectQueue = new Queue<GameObject>();
        generationContainer = new GameObject();
        generationContainer.name = transform.name + Utility.NAME_Generation;
        generationContainer.tag = Utility.TAG_Generation;
        generationContainer.transform.position = generationPosition;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public void GenerateNew()
    {
        totalGenerations++;

        if(gameObjectQueue.Count == maximumGenerations)
        {
            Destroy(gameObjectQueue.Dequeue());
        }

        GameObject tempGameObject = Instantiate(objectToGenerate, generationPosition, Quaternion.Euler(generationRotaion));
        tempGameObject.transform.SetParent(generationContainer.transform);
        tempGameObject.name = objectToGenerate.name + " [" + totalGenerations + "]";
        gameObjectQueue.Enqueue(tempGameObject);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public void DeleteNext()
    {
        if (gameObjectQueue.Count != 0)
        {
            Destroy(gameObjectQueue.Dequeue());
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public void ClearAll()
    {
        while(gameObjectQueue.Count != 0)
        {
            Destroy(gameObjectQueue.Dequeue());
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    /*  Enlarges the highlighted object */

    public void OnRaycastEnter(GameObject currentObject)
    {
        if (currentObject.GetComponent<Image>())
        {
            currentObject.GetComponent<Image>().rectTransform.sizeDelta += new Vector2(GROW_SHRINK, GROW_SHRINK);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    /*  Shrinks the object that was highlighted but is no longer being looked at */

    public void OnRaycastExit(GameObject currentObject)
    {
        if (currentObject.GetComponent<Image>())
        {
            currentObject.GetComponent<Image>().rectTransform.sizeDelta -= new Vector2(GROW_SHRINK, GROW_SHRINK);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public void OnRaycastClick(GameObject current)
    {
        switch (current.transform.name)
        {
            case "Generate New":
                GenerateNew();
                break;
            case "Delete Next":
                DeleteNext();
                break;
            case "Clear All":
                ClearAll();
                break;
            default:
                Debug.LogError(ERROR + "Error with Generation!");
                break;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
}
