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

	private Vector3 currentGravityDirection;
	public Vector3 CurrentGravityDirection {
		get {
			return currentGravityDirection;
		}
		set {
			currentGravityDirection = value / value.magnitude;
		}
	}

	[Header("Materials")]
	[SerializeField] private Material defaultMat;
	[SerializeField] private Material focusedMat;
	private MeshRenderer myMeshRenderer;

    // Start is called before the first frame update
    void Start()
    {
		transform = GetComponent<Transform>();
		col = GetComponent<BoxCollider>();
		CurrentGravityDirection = Vector3.down;

		myMeshRenderer = GetComponent<MeshRenderer>();
		myMeshRenderer.material = defaultMat;
    }

    // Update is called once per frame
    void Update()
    {
        
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
		Ray gravDirectionRay = new Ray(transform.position, CurrentGravityDirection);
		if (Physics.Raycast(gravDirectionRay, out hit, (bodyHeight * .5f) + raycastLeeway, stoppingSurface)) {

			float hitDistFromObject = hit.distance - ((bodyHeight * 0.5f) + raycastLeeway);

			transform.Translate(CurrentGravityDirection * Mathf.Min((accumulatedVelocity * Time.deltaTime).magnitude, hitDistFromObject));

			accumulatedVelocity = Vector3.zero;
		}
		else {
			transform.Translate(accumulatedVelocity * Time.deltaTime);

			Vector3 vel0 = accumulatedVelocity;
			Vector3 vel1 = vel0 + (CurrentGravityDirection * acclerationDueToGravity * Time.deltaTime);
			accumulatedVelocity = vel1;
		}
	}

	public void SetFocus(bool focused) {
		myMeshRenderer.material = focused ? focusedMat : defaultMat;
	}

	public void FlipGravity() {
		currentGravityDirection *= -1;
	}
}
