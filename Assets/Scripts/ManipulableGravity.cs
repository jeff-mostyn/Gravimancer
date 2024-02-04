using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManipulableGravity : MonoBehaviour
{
	private BoxCollider col;
	private Transform transform;
	[SerializeField] private float bodyHeight;
	[SerializeField] private float raycastLeeway;
	[SerializeField] private LayerMask stoppingSurface;

	[SerializeField] private float acclerationDueToGravity;
	[SerializeField] private float terminalVelocity;
	private Vector3 accumulatedVelocity;

	[SerializeField] private float rotationCooldown;
	[SerializeField] private float gravityChangeDelay;
	private bool gravityChangePending;
	private Mat3 pendingRotation;
	private Mat3 gravityRotation;


	vec3 defaultGrav;
	private bool canChangeDirection;

	[Header("Materials")]
	[SerializeField] private Material defaultMat;
	[SerializeField] private Material focusedMat;
	private MeshRenderer myMeshRenderer;

	private void Awake() {
		defaultGrav.x = 0f;
		defaultGrav.y = -1f;
		defaultGrav.z = 0f;
	}

	// Start is called before the first frame update
	void Start()
    {
		transform = GetComponent<Transform>();
		col = GetComponent<BoxCollider>();

		myMeshRenderer = GetComponent<MeshRenderer>();
		myMeshRenderer.material = defaultMat;

		canChangeDirection = true;

		gravityRotation = new Mat3();
		gravityRotation = Mat3.Rotate(new vec3(0f, 0f, 0f));

		pendingRotation = gravityRotation;
    }

    // Update is called once per frame
    void Update()
    {
		Debug.DrawRay(transform.position, pendingRotation * defaultGrav, Color.red);
    }

	private void FixedUpdate() {
		PhysicsSolver();
	}

	private void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.layer == stoppingSurface) {

		}
	}

	private void PhysicsSolver() {
		// Raycast in direction of gravity. Translate at full calculated velocity if raycast does not hit surface.
		// If raycast hits surface, transform so that the object rests on top of the surface, and set velocity to 0
		RaycastHit hit;
		Ray gravDirectionRay = new Ray(transform.position, GetGravity());
		if (Physics.Raycast(gravDirectionRay, out hit, (bodyHeight * .5f) + raycastLeeway, stoppingSurface)) {

			float hitDistFromObject = hit.distance - ((bodyHeight * 0.5f) + raycastLeeway);

			transform.Translate(GetGravity() * Mathf.Min((accumulatedVelocity * TimeScaleManager.Instance.GetDeltaTime(true)).magnitude, hitDistFromObject));

			accumulatedVelocity = Vector3.zero;
		}
		else {
			transform.Translate(accumulatedVelocity * TimeScaleManager.Instance.GetDeltaTime(true));

			Vector3 vel0 = accumulatedVelocity;
			Vector3 vel1 = vel0 + (GetGravity() * acclerationDueToGravity * TimeScaleManager.Instance.GetDeltaTime(true));
			accumulatedVelocity = vel1;
		}
	}

	public void SetFocus(bool focused) {
		myMeshRenderer.material = focused ? focusedMat : defaultMat;
	}

	public void ChangeGravityDirection(float x, float y, PlayerMovement player) {
		Vector3 currentGravity = GetGravity();
		Vector3 positionOffset = transform.position - player.transform.position;
		if (canChangeDirection) {
			if (Mathf.Abs(y) > Mathf.Abs(x)) {
				if (positionOffset.z > positionOffset.x) {
					pendingRotation = Mat3.RotateX(y > 0 ? 90f * Mathf.Deg2Rad : -90f * Mathf.Deg2Rad) * pendingRotation;
				} 
				else {
					pendingRotation = Mat3.RotateZ(y > 0 ? 90f * Mathf.Deg2Rad : -90f * Mathf.Deg2Rad) * pendingRotation;
				}
			}
			else {
				pendingRotation = Mat3.RotateY(x > 0 ? 90f * Mathf.Deg2Rad : -90f * Mathf.Deg2Rad) * pendingRotation;
			}

			canChangeDirection = false;
			gravityChangePending = true;
			CancelInvoke(nameof(ContinueGravity));

			// delay execution of function
			Invoke(nameof(ResetGravityDirectionChange), rotationCooldown);
			Invoke(nameof(ContinueGravity), gravityChangeDelay);
		}

	}

	public void Unfocus() {
		CancelInvoke(nameof(ContinueGravity));
		ContinueGravity();
	}

	private void ResetGravityDirectionChange() {
		canChangeDirection = true;
	}

	private void ContinueGravity() {
		gravityChangePending = false;
		gravityRotation = pendingRotation;
	}

	private vec3 GetGravity() {
		vec3 grav = gravityRotation * defaultGrav;

		return grav;
	}
}
