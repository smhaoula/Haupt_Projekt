using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatableData : ScriptableObject
{
    public event System.Action OnValuesUpdated;
    //public bool autoUpdate;

    void OnEnable(){
        UnityEditor.EditorApplication.update -= NotifyOfUpdatedValues;
        UnityEditor.EditorApplication.update += NotifyOfUpdatedValues;
    }

    protected virtual void OnValidate(){
        /*if(autoUpdate){
            UnityEditor.EditorApplication.update += NotifyOfUpdatedValues;
        }*/
    }
    
    public void NotifyOfUpdatedValues(){
        if(OnValuesUpdated!=null){
            OnValuesUpdated();
        }
    }

    
}
