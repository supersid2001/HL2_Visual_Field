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

  public AudioSource source;

  public AudioClip fixationLossClip;

  public AudioClip eyeGazeMissingClip;

  GameObject currObject;

  [SerializeField]
  Material highlightMaterial;

  Material previousMaterial;

  public GetTimeValue script;

  float timer = 0.0f;
  void Update()
  {
    var timestamp = System.DateTime.Now;

    var leftGazeReadingInWorldSpace = extendedEyeGazeDataProvider.GetWorldSpaceGazeReading(ExtendedEyeGazeDataProvider.GazeType.Left, timestamp);
    var rightGazeReadingInWorldSpace = extendedEyeGazeDataProvider.GetWorldSpaceGazeReading(ExtendedEyeGazeDataProvider.GazeType.Right, timestamp);
    var combinedGazeReadingInWorldSpace = extendedEyeGazeDataProvider.GetWorldSpaceGazeReading(ExtendedEyeGazeDataProvider.GazeType.Combined, timestamp);
    Debug.Log(leftGazeReadingInWorldSpace);
    if (leftGazeReadingInWorldSpace != null)
    {
      //Version 1: Raycast vector and see if it hits object 
      //Version 2: Get eye position and vector direction to make canvas follow
      //Currently using version 2 
      debugText.text = leftGazeReadingInWorldSpace.GazeDirection.x + "," + leftGazeReadingInWorldSpace.GazeDirection.y + "," + leftGazeReadingInWorldSpace.GazeDirection.z;
      canvasObj.MoveCanvasToGazeDistance(leftGazeReadingInWorldSpace.EyePosition, leftGazeReadingInWorldSpace.GazeDirection, 5.0f);
      var ray = new Ray(leftGazeReadingInWorldSpace.EyePosition, leftGazeReadingInWorldSpace.GazeDirection * 10);
      RaycastHit hit;
      if (Physics.Raycast(ray, out hit, 100000))
      {
        var lasthit = hit.transform.gameObject;
        if (lasthit.tag == "FixationPoint")
        {
          source.Pause();
          fixationScript.text = "No fixation loss";
          timer = 0.0f;
        }
        // else if (lasthit.GetComponent<Object_Tag>())
        // {
        //   script.UpdateObj(lasthit.GetComponent<Object_Tag>().name);
        // }
        else
        {
          fixationScript.text = "Fixation loss";
          timer += Time.deltaTime;
          if (timer >= 2.0f)
          {
            source.Pause();
            source.clip = fixationLossClip;
            source.Play();
          }
          //script.UpdateObj("Default");
        }
      }
      else
      {
        timer += Time.deltaTime;
        if (timer >= 2.0f)
        {
          source.Pause();
          source.clip = fixationLossClip;
          source.Play();
        }
        fixationScript.text = "Fixation loss";
        //script.UpdateObj("Default");
      }
    }
    else
    {
      source.Pause();
      source.clip = eyeGazeMissingClip;
      source.Play();
      debugText.text = "Error! Gaze data could not be found!";
    }
    //Raycast left/rightGazeReadingInWorld space vector and see if there are fixation errors 
    var combinedGazeReadingInCameraSpace = extendedEyeGazeDataProvider.GetCameraSpaceGazeReading(ExtendedEyeGazeDataProvider.GazeType.Combined, timestamp);
  }
}
