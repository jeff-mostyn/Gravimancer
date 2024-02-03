using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaleManager : MonoBehaviour
{

	public static TimeScaleManager Instance;

	[SerializeField] private float slowedTimescale;
	private float defaultTimeScale = 1f;
	private float currentTimeScale = 1f;

	private void Awake() {
		// There can only be one
		if (Instance == null) {
			Instance = this;
		}
		else {
			Instance = this;
			Destroy(this);
		}
	}
	
	public void ChangeTimescale(bool slow) {
		currentTimeScale = slow ? slowedTimescale : defaultTimeScale;
	}

	public float GetDeltaTime(bool scaled) {
		if (scaled) {
			//Debug.Log(currentTimeScale);
			return Time.deltaTime * currentTimeScale;
		}
		else {
			//Debug.Log(defaultTimeScale);
			return Time.deltaTime * defaultTimeScale;
		}
	}
}
