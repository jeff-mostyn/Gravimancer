using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
	[SerializeField] private float xSensitivity, ySensitivity;
	[SerializeField] private Transform orientation;

	private float xRotation, yRotation;


	// Start is called before the first frame update
	void Start()
    {
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
		// process input
		float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * xSensitivity;
		float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * ySensitivity;

		yRotation += mouseX;

		xRotation -= mouseY;
		xRotation = Mathf.Clamp(xRotation, -90f, 90f);

		// perform rotation on camera in both horizontal and vertical axes
		transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);

		// rotate character in vertical axis
		orientation.rotation = Quaternion.Euler(0, yRotation, 0);
	}
}
