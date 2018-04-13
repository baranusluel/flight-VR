using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour {

    Transform lastHit;
    Transform pointer;
    public TextMesh countdownText;
    IEnumerator countdownCoroutine;
    // New York, San Francisco, Paris, Atlanta
    float[] latitudes = { 40.749642f, 37.808045f, 48.857622f, 33.770855f };
    float[] longitudes = { -73.987144f, -122.475540f, 2.295737f, -84.395587f };
    int city = -1;

    void Start () {
        pointer = GameObject.Find("Cylinder").transform;
        pointer.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        countdownText.text = "";
    }

    void Update() {
        if (Input.GetMouseButton(0))
        {
            float deltaX = Input.GetAxis("Mouse X");
            float deltaY = Input.GetAxis("Mouse Y");
            transform.Rotate(new Vector3(-deltaY, deltaX, 0));
        }
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;    
        if (Physics.Raycast(ray, out hit))
        {
            if (lastHit != null && lastHit != hit.transform)
            {
                if (countdownCoroutine != null)
                {
                    StopCoroutine(countdownCoroutine);
                    countdownCoroutine = null;
                }
                countdownText.text = "";
            }
            lastHit = hit.transform;
            pointer.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            if (countdownCoroutine == null)
            {
                countdownCoroutine = countdown();
                StartCoroutine(countdownCoroutine);
                if (hit.rigidbody == null)
                {
                    Debug.Log("rigid body null");
                }
                switch (hit.rigidbody.gameObject.name)
                {
                    case "New York":
                        city = 0;
                        break;
                    case "San Francisco":
                        city = 1;
                        break;
                    case "Paris":
                        city = 2;
                        break;
                    case "Atlanta":
                        city = 3;
                        break;
                }
            }
        } else if (lastHit != null)
        {
            countdownText.text = "";
            if (countdownCoroutine != null)
            {
                StopCoroutine(countdownCoroutine);
                countdownCoroutine = null;
            }
            pointer.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        }
    }

    IEnumerator countdown()
    {
        yield return new WaitForSeconds(1);
        countdownText.text = "3";
        yield return new WaitForSeconds(1);
        countdownText.text = "2";
        yield return new WaitForSeconds(1);
        countdownText.text = "1";
        yield return new WaitForSeconds(1);
        PlayerPrefs.SetFloat("latitude", latitudes[city]);
        PlayerPrefs.SetFloat("longitude", longitudes[city]);
        Application.LoadLevel("GameScene");
    }
}
