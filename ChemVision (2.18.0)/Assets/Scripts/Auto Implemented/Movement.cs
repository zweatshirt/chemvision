
////////////////////////////////////////////////////////////////////////////////////////////////////
//
//      Movement
//
//      Last Updated:               4/20/2018
//      Oldest Compatible Version:  2.18.0
//      Updated By:                 Kaleb Cole
//      Project:                    Chem Vision
//
////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

////////////////////////////////////////////////////////////////////////////////////////////////////

public class Movement : MonoBehaviour
{

    //  Publics
    public Dictionary<int, List<Utility.AnimationInformation>> animationDictionary;
    public Animate animate;
    public int currentAnimation, currentPhase, currentSpeedMultiplier;
    public bool isPaused, oneRun;

    //  Privates
    private bool isInitialized = false;
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    private void Awake()
    {
        InitializeVariables();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public void InitializeVariables()
    {
        if (!isInitialized)
        {
            isInitialized = true;
            isPaused = false;
            oneRun = false;
            animationDictionary = new Dictionary<int, List<Utility.AnimationInformation>>();
            animate = Animate.None;
            currentPhase = 0;
            currentAnimation = 0;
            currentSpeedMultiplier = 10;
            originalPosition = transform.position;
            originalRotation = transform.rotation;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    private void Update()
    {
        if (!isPaused || oneRun)
        {
            oneRun = false;
            switch (animate)
            {
                case Animate.Forward:
                    if (animationDictionary[currentPhase].Count == 0)
                    {
                        animate = Animate.None;
                    }
                    else if (animationDictionary[currentPhase][currentAnimation].AnimateMe(gameObject, 1, currentSpeedMultiplier))
                    {
                        if (currentAnimation >= animationDictionary[currentPhase].Count - 1)
                        {
                            animate = Animate.None;
                        }
                        else
                        {
                            currentAnimation++;
                        }
                    }
                    break;
                case Animate.Reverse:
                    if (animationDictionary[currentPhase].Count == 0)
                    {
                        animate = Animate.None;
                    }
                    else if (animationDictionary[currentPhase][currentAnimation].AnimateMe(gameObject, -1, currentSpeedMultiplier))
                    {
                        if (currentAnimation <= 0)
                        {
                            animate = Animate.None;
                        }
                        else
                        {
                            currentAnimation--;
                        }
                    }
                    break;
                case Animate.Reset:
                    transform.position = originalPosition;
                    transform.rotation = originalRotation;
                    currentPhase = 0;
                    currentAnimation = 0;
                    animate = Animate.None;
                    break;
                default:
                    break;
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public void SetDefaults(int currentPhase, int currentAnimation, Animate animate)
    {
        for (int i = 0; i < animationDictionary.Count; i++)
        {
            foreach(Utility.AnimationInformation tempAnimation in animationDictionary[i])
            {
                tempAnimation.ResetTime();
            }
        }
        this.currentPhase = currentPhase;
        this.currentAnimation = currentAnimation;
        this.animate = animate;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
}
