using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.MixedReality.OpenXR;
using Microsoft.MixedReality.EyeTracking;


public class LogEyeData : MonoBehaviour
{
    public MoveCanvas canvasObj;
    public Text debugText;
    public Text fixationScript;
    public ExtendedEyeGazeDataProvider extendedEyeGazeDataProvider;
    public GameObject camera;
    void Update() {
        var timestamp = System.DateTime.Now;

        var leftGazeReadingInWorldSpace = extendedEyeGazeDataProvider.GetWorldSpaceGazeReading(ExtendedEyeGazeDataProvider.GazeType.Left, timestamp);
        var rightGazeReadingInWorldSpace = extendedEyeGazeDataProvider.GetWorldSpaceGazeReading(ExtendedEyeGazeDataProvider.GazeType.Right, timestamp);
        var combinedGazeReadingInWorldSpace = extendedEyeGazeDataProvider.GetWorldSpaceGazeReading(ExtendedEyeGazeDataProvider.GazeType.Combined, timestamp);
        Debug.Log(leftGazeReadingInWorldSpace);
        if(leftGazeReadingInWorldSpace != null){
            //Version 1: Raycast vector and see if it hits object 
            //Version 2: Get eye position and vector direction to make canvas follow
            //Currently using version 2 
            debugText.text = leftGazeReadingInWorldSpace.GazeDirection.x + "," + leftGazeReadingInWorldSpace.GazeDirection.y + "," + leftGazeReadingInWorldSpace.GazeDirection.z;
            canvasObj.MoveCanvasToGazeDistance(leftGazeReadingInWorldSpace.EyePosition, leftGazeReadingInWorldSpace.GazeDirection, 5.0f);
            var ray = new Ray(leftGazeReadingInWorldSpace.EyePosition, leftGazeReadingInWorldSpace.GazeDirection * 10);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, 10000)){
                var lasthit = hit.transform.gameObject;
                if(lasthit.tag == "FixationPoint"){
                    fixationScript.text = "No fixation loss";
                } else {
                    fixationScript.text = "Fixation loss";
                }
            } else{
                 fixationScript.text = "Fixation loss";
            }
        } else {
            debugText.text = "Error! Gaze data could not be found!";
        }
        //Raycast left/rightGazeReadingInWorld space vector and see if there are fixation errors 
        var combinedGazeReadingInCameraSpace = extendedEyeGazeDataProvider.GetCameraSpaceGazeReading(ExtendedEyeGazeDataProvider.GazeType.Combined, timestamp);
    }
}
