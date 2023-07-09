using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    public bool isComplete {get;set;}
    public bool UpdateProgress(IObjective[] objectives)
    {
        for (int i = 0; i < objectives.Length; i++)
        {
            if (objectives[i] != null && !objectives[i].isComplete) return false;
        }
        return true;
    }
}
