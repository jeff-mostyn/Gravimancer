using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	[Header("Movement")]
	[SerializeField] private float lateralMoveSpeed;
	[SerializeField] private float speedMultiplier;
	[SerializeField] private float groundDrag;
	[SerializeField] private float inputDeadzone;

	[Header("Ground Check")]
	[SerializeField] private float playerHeight;
	[SerializeField] private LayerMask groundLayer;
	private bool grounded;

	[Header("Midair Movement")]
	[SerializeField] private float jumpForce;
	[SerializeField] private float jumpCooldown;
	[SerializeField] private float airMultiplier;
	[SerializeField] private bool canJump;

	[Header("Midair Movement")]
	[SerializeField] private KeyCode jumpKey = KeyCode.Space;

	[Header("Camera")]
	[SerializeField] private Camera cam;

	[SerializeField] private Transform orientation;

	[Header("Interactables")]
	[SerializeField] private LayerMask interactableLayer;
	[SerializeField] private MouseButton focusKey = MouseButton.Left;
	[SerializeField] private KeyCode gravityFlipKey = KeyCode.LeftShift;
	private bool focusMode;
	private GameObject focusedObject;
	private ManipulableGravity focusedGravityController;

	private float xInput, yInput;

	private Vector3 moveDirection;

	private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
		rb = GetComponent<Rigidbody>();
		rb.freezeRotation = true;

		focusMode = false;

		ResetJump();
    }

    // Update is called once per frame
    void Update()
    {
		// ground check
		grounded = Physics.Raycast(transform.position, Vector3.down, (playerHeight * 0.5f) + 0.1f);

		MyInput();
		SpeedControl();

		if (grounded) {
			rb.drag = groundDrag;
		}
		else {
			rb.drag = 0f;
		}

		// Check for interactable objects
		Ray reticleRay = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
		RaycastHit reticleHit;
		Physics.Raycast(reticleRay, out reticleHit, 50f, interactableLayer);

		Collider hitObject = reticleHit.collider;

		if (hitObject) {
			if (!focusedObject) {
				focusedObject = hitObject.gameObject;
				focusedGravityController = focusedObject.GetComponent<ManipulableGravity>();
				focusedGravityController.SetFocus(true);
			}
		}
		else {
			if (focusedObject) {
				focusedGravityController.SetFocus(false);
				focusedObject = null;
				focusedGravityController = null;
			}
		}
	}

	private void FixedUpdate() {
		if (!focusMode) {
			MovePlayer();
		}
	}

	private void MyInput() {
		xInput = Input.GetAxisRaw("Horizontal");
		yInput = Input.GetAxisRaw("Vertical");

		if ((Mathf.Abs(xInput) > inputDeadzone || Mathf.Abs(yInput) > inputDeadzone) && focusedGravityController && focusMode) {
			focusedGravityController.ChangeGravityDirection(xInput, yInput, this);
		}

		// jumping
		if (Input.GetKey(jumpKey) && canJump && grounded) {
			canJump = false;

			Jump();

			// delay execution of function
			Invoke(nameof(ResetJump), jumpCooldown);
		}

		if (Input.GetMouseButtonDown(0)) {
			Debug.Log("slowing timescale");
			TimeScaleManager.Instance.ChangeTimescale(true);
			focusMode = true;
		}
		else if (Input.GetMouseButtonUp(0)) {
			TimeScaleManager.Instance.ChangeTimescale(false);
			focusMode = false;
			if (focusedGravityController) focusedGravityController.Unfocus();
		}
	}

	private void MovePlayer() {
		// calculate Movement
		moveDirection = (orientation.forward * yInput) + (orientation.right * xInput);

		if (grounded) {
			rb.AddForce(moveDirection * lateralMoveSpeed * speedMultiplier, ForceMode.Force);
		}
		else  {
			rb.AddForce(moveDirection * lateralMoveSpeed * speedMultiplier * airMultiplier, ForceMode.Force);
		}
	}

	private void SpeedControl() {
		Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

		if (flatVel.magnitude > lateralMoveSpeed) {
			Vector3 clampedVelocity = flatVel.normalized * lateralMoveSpeed;
			rb.velocity = new Vector3(clampedVelocity.x, rb.velocity.y, clampedVelocity.z);
		}
	}

	private void Jump() {
		// reset y velocity
		rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // zero y velocity so height of jump is always the same

		rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
	}

	private void ResetJump() {
		canJump = true;
	}
}
