using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Point_Spawner : MonoBehaviour
{
  public GameObject imageObject;
  bool canSpawn = true;
  public BrightnessOverrideComponent bOverride;

  void Start()
  {
    bOverride.Brightness = 1.0f;
  }

  public void SpawnObject(float x_pos, float y_pos, float brightness)
  {
    if (canSpawn)
    {
      imageObject.transform.localPosition = new Vector3(x_pos, y_pos, 0.0f);
      Image image = imageObject.GetComponent<Image>();
      var tempColor = image.color;
      tempColor.a = brightness;
      image.color = tempColor;
      StartCoroutine("activateImage");
    }
  }

  IEnumerator activateImage()
  {
    imageObject.SetActive(true);
    canSpawn = false;
    yield return new WaitForSeconds(0.2f);
    imageObject.SetActive(false);
    yield return new WaitForSeconds(1.0f);
    canSpawn = true;
  }

  void Update()
  {
    float brightness = Random.Range(0.0f, 1.0f);
    int x_cord = Random.Range(-200, 200);
    int y_cord = Random.Range(-200, 200);
    SpawnObject(x_cord, y_cord, brightness);
  }

  public void spawnLeft()
  {
    SpawnObject(-88.0f, 0.0f, 1.0f);
  }

  public void spawnLeftDim()
  {
    SpawnObject(-88.0f, 0.0f, 0.5f);
  }

  public void spawnRight()
  {
    SpawnObject(88.0f, 0.0f, 1.0f);
  }

  public void spawnRightDim()
  {
    SpawnObject(88.0f, 0.0f, 0.5f);
  }

  public void spawnUp()
  {
    SpawnObject(0.0f, 88.0f, 1.0f);
  }

  public void spawnUpDim()
  {
    SpawnObject(0.0f, 88.0f, 0.5f);
  }
}
