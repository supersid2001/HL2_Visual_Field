using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.MixedReality.OpenXR;
using Microsoft.MixedReality.EyeTracking;
using Microsoft.MixedReality.Toolkit.Input;


public class LogEyeData : BaseInputHandler, IMixedRealityPointerHandler
{
  public MoveCanvas canvasObj;
  public Text debugText;
  public Text fixationScript;
  public ExtendedEyeGazeDataProvider extendedEyeGazeDataProvider;
  public GameObject camera;

  public MixedRealityInputAction ClickerAction;

  public AudioSource source;

  public AudioClip fixationLossClip;

  public AudioClip eyeGazeMissingClip;

  public AudioClip clickClip;

  GameObject currObject;

  [SerializeField]
  Material highlightMaterial;

  Material previousMaterial;

  public GetTimeValue script;

  float timer_fixation = 0.0f;
  float timer_gaze = 0.0f;
  void Update()
  {
    var timestamp = System.DateTime.Now;

    var leftGazeReadingInWorldSpace = extendedEyeGazeDataProvider.GetWorldSpaceGazeReading(ExtendedEyeGazeDataProvider.GazeType.Left, timestamp);
    var rightGazeReadingInWorldSpace = extendedEyeGazeDataProvider.GetWorldSpaceGazeReading(ExtendedEyeGazeDataProvider.GazeType.Right, timestamp);
    var combinedGazeReadingInWorldSpace = extendedEyeGazeDataProvider.GetWorldSpaceGazeReading(ExtendedEyeGazeDataProvider.GazeType.Combined, timestamp);
    if (leftGazeReadingInWorldSpace != null)
    {
      timer_gaze = 0.0f;
      //Version 1: Raycast vector and see if it hits object 
      //Version 2: Get eye position and vector direction to make canvas follow
      //Currently using version 2 
      debugText.text = leftGazeReadingInWorldSpace.GazeDirection.x + "," + leftGazeReadingInWorldSpace.GazeDirection.y + "," + leftGazeReadingInWorldSpace.GazeDirection.z;
      canvasObj.MoveCanvasToGazeDistance(leftGazeReadingInWorldSpace.EyePosition, leftGazeReadingInWorldSpace.GazeDirection, 5.0f);
      var ray = new Ray(leftGazeReadingInWorldSpace.EyePosition, leftGazeReadingInWorldSpace.GazeDirection * 10);
      RaycastHit hit;
      if (Physics.Raycast(ray, out hit, 100000))
      {
        QuadScript val = hit.transform.gameObject.GetComponent<QuadScript>();
        if (val != null)
        {
          val.addHitPoint(hit.textureCoord.x * 4 - 2, hit.textureCoord.y * 4 - 2);
          Debug.Log("Hit object at: " + hit.transform.InverseTransformPoint(hit.point));
        }
        var lasthit = hit.transform.gameObject;
        if (lasthit.tag == "FixationPoint")
        {
          timer_fixation = 0.0f;
          source.Pause();
          fixationScript.text = "No fixation loss";
        }
        // else if (lasthit.GetComponent<Object_Tag>())
        // {
        //   script.UpdateObj(lasthit.GetComponent<Object_Tag>().name);
        // }
        else
        {
          fixationScript.text = "Fixation loss";
          timer_fixation += Time.deltaTime;
          if (timer_fixation >= 2.0f)
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
        timer_fixation += Time.deltaTime;
        if (timer_fixation >= 2.0f)
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
      timer_gaze += Time.deltaTime;
      if (timer_gaze >= 2.0f)
      {
        source.Pause();
        source.clip = eyeGazeMissingClip;
        source.Play();
      }
      debugText.text = "Error! Gaze data could not be found!";
    }
    //Raycast left/rightGazeReadingInWorld space vector and see if there are fixation errors 
    var combinedGazeReadingInCameraSpace = extendedEyeGazeDataProvider.GetCameraSpaceGazeReading(ExtendedEyeGazeDataProvider.GazeType.Combined, timestamp);
  }

  public void OnPointerClicked(MixedRealityPointerEventData eventData)
  {
    if (eventData.MixedRealityInputAction == ClickerAction)
    {
      source.PlayOneShot(clickClip);
    }
  }

  public void OnPointerUp(MixedRealityPointerEventData eventData) { }
  public void OnPointerDown(MixedRealityPointerEventData eventData)
  {
    if (eventData.MixedRealityInputAction == ClickerAction)
    {
      source.PlayOneShot(clickClip);
    }
  }

  public void OnPointerDragged(MixedRealityPointerEventData eventData) { }

  protected override void UnregisterHandlers() { }
  protected override void RegisterHandlers() { }
}
