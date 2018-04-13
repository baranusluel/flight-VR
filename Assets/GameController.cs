using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wrld;
using Wrld.MapCamera;
using Wrld.Space;
using UnityEngine.UI;


public class GameController : MonoBehaviour {
    CameraApi camapi;
    Camera cam;
    const float distPerFrame = 75;
    float speed = 0;
    bool gameIsOver = false;
    public TextMesh gameOverText;
    public TextMesh respawningText;
    public TextMesh timerText;
    public TextMesh pointsText;
    public TextMesh multiplierText;
    const string gameOverString = "Game Over!";
    const string respawningString = "Respawning in ";
    float startTime = 0;
    Vector3 startPos = new Vector3(100, 500, -1300);
    float points = 0;

    void Start() {
        camapi = Api.Instance.CameraApi;
        cam = transform.GetComponentInChildren<Camera>();
        float latitude = PlayerPrefs.GetFloat("latitude");
        float longitude = PlayerPrefs.GetFloat("longitude");
        Debug.Log(latitude);
        Debug.Log(longitude);
        GameObject.Find("WrldMap").GetComponentInChildren<WrldMap>().m_latitudeDegrees = latitude;
        GameObject.Find("WrldMap").GetComponentInChildren<WrldMap>().m_longitudeDegrees = longitude;
        Api.Instance.SetOriginPoint(LatLongAltitude.FromDegrees(latitude, longitude, 500));
        cam.farClipPlane = cam.farClipPlane * 2/5;
        Input.simulateMouseWithTouches = false;
        spawn();
    }

    void spawn()
    {
        gameOverText.text = "";
        respawningText.text = "";
        startTime = Time.time;
        timerText.text = "Timer: 0.00";
        pointsText.text = "Points: 0";
        multiplierText.text = "1x Multiplier";
        transform.position = startPos;
        gameIsOver = false;
        StartCoroutine("waitLoading");
    }
	
	void Update () {
        if (gameIsOver)
            return;
        if (cam == null || cam.transform == null)
            return;

        timerText.text = "Time: " + (Time.time - startTime).ToString("0.00");

        float multiplier = Mathf.Round(10 * Mathf.Exp(-1f * Mathf.Max(0, transform.position.y - 300) / 75f));
        multiplierText.text = multiplier + "x Multiplier";

        points += Time.deltaTime * multiplier;
        pointsText.text = "Points: " + Mathf.Round(points);

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
        // TODO: Is this necessary? Test performance
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
        yield return null;
    }

    IEnumerator respawnCountdown()
    {
        for (int i = 5; i > 0; i--)
        {
            respawningText.text = respawningString + i + "...";
            yield return new WaitForSeconds(1);
        }
        speed = 0;
        points = 0;
        spawn();
        StartCoroutine("rampSpeed");
    }

    void OnTriggerEnter()
    {
        StartCoroutine("gameOver");
    }
}
