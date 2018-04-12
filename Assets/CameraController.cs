using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wrld;
using Wrld.MapCamera;
using Wrld.Space;
using UnityEngine.UI;


public class CameraController : MonoBehaviour {
    CameraApi camapi;
    Camera cam;
    const float distPerFrame = 75;
    float speed = 0;
    bool gameIsOver = false;
    public TextMesh gameOverText;
    public TextMesh respawningText;
    public TextMesh timerText;
    const string gameOverString = "Game Over!";
    const string respawningString = "Respawning in ";
    float startTime = 0;
    Vector3 startPos = new Vector3(100, 500, -1300);

    void Start() {
        camapi = Api.Instance.CameraApi;
        cam = transform.GetComponentInChildren<Camera>();
        //cam.farClipPlane = cam.farClipPlane * 4;
        Input.simulateMouseWithTouches = false;
        spawn();
    }

    void spawn()
    {
        gameOverText.text = "";
        respawningText.text = "";
        startTime = Time.time;
        timerText.text = "Timer: 0.00";
        transform.position = startPos;
        gameIsOver = false;
        StartCoroutine("waitLoading");
    }
	
	void Update () {
        if (gameIsOver)
            return;
        timerText.text = "Timer: " + (Time.time - startTime).ToString("0.00");
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
            transform.Rotate(new Vector3(-touchDeltaPosition.y, touchDeltaPosition.x, 0) / 2);
        }
        else if (Input.GetMouseButton(0))
        {
            float deltaX = Input.GetAxis("Mouse X");
            float deltaY = Input.GetAxis("Mouse Y");
            transform.Rotate(new Vector3(-deltaY, deltaX, 0));
        }
        transform.position += cam.transform.forward * speed * Time.deltaTime;
        float angle = cam.transform.eulerAngles.z;
        if (angle > 180)
            angle -= 360;
        transform.Rotate(new Vector3(0, -angle / 20, 0));
        Api.Instance.StreamResourcesForCamera(cam);
    }

    IEnumerator waitLoading()
    {
        yield return new WaitForSeconds(2);
        StartCoroutine("rampSpeed");
    }

    IEnumerator rampSpeed()
    {
        while (speed < distPerFrame)
        {
            speed += 5;
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator gameOver()
    {
        gameIsOver = true;
        gameOverText.text = gameOverString;
        respawningText.text = respawningString;
        StartCoroutine("respawnCountdown");
        yield return new WaitForSeconds(3);
        speed = 0;
        spawn();
        StartCoroutine("rampSpeed");
    }

    IEnumerator respawnCountdown()
    {
        respawningText.text += "3...";
        yield return new WaitForSeconds(1);
        respawningText.text = respawningString + "2...";
        yield return new WaitForSeconds(1);
        respawningText.text = respawningString + "1...";
    }

    void OnTriggerEnter()
    {
        StartCoroutine("gameOver");
    }
}
