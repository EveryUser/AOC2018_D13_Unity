using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public Camera CurrentCamera;
    public float Speed = 1;

	// Use this for initialization
	void Start ()
    {
        CurrentCamera = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update ()
    {


		if (Input.GetKey(KeyCode.D))
        {
            transform.position = new Vector3(transform.position.x + Speed, transform.position.y, transform.position.z);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            transform.position = new Vector3(transform.position.x - Speed, transform.position.y, transform.position.z);
        }
        if (Input.GetKey(KeyCode.W))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + Speed, transform.position.z);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - Speed, transform.position.z);
        }

        if (Input.GetKey(KeyCode.Q))
        {
            CurrentCamera.orthographicSize += Speed;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            if (CurrentCamera.orthographicSize > 1)
            {
                CurrentCamera.orthographicSize -= Speed;
                if (CurrentCamera.orthographicSize < 1)
                {
                    CurrentCamera.orthographicSize = 1;
                }
            }
        }
    }
}
