using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct vec3 {
	public float x, y, z;

	public vec3(float _x, float _y, float _z) {
		x = _x;
		y = _y;
		z = _z;
	}

	public static vec3 operator +(vec3 _vec1, vec3 _vec2) {
		return new vec3(
			_vec1.x + _vec2.x,
			_vec1.y + _vec2.y,
			_vec1.z + _vec2.z
		);
	}

	public static vec3 operator +(vec3 _vec1, Vector3 _vec2) {
		return new vec3(_vec1.x + _vec2.x, _vec1.y + _vec2.y, _vec1.z + _vec2.z);
	}

	public static Vector3 operator +(Vector3 _vec1, vec3 _vec2) {
		return new Vector3(_vec1.x + _vec2.x, _vec1.y + _vec2.y, _vec1.z + _vec2.z);
	}

	public static vec3 operator *(vec3 _vec, float _scalar) {
		return new vec3(_vec.x * _scalar, _vec.y * _scalar, _vec.z * _scalar);
	}

	public static float dot(vec3 _vec1, vec3 _vec2) {
		return (
			(_vec1.x * _vec2.x) +
			(_vec1.y * _vec2.y) +
			(_vec1.z * _vec2.z)
		);
	}

	public static implicit operator Vector3(vec3 _vec) {
		return new Vector3(_vec.x, _vec.y, _vec.z);
	}
}

public class Mat3 {
	public vec3[] mat3;

	/// <summary>
	/// Create column-indexed 3x3 matrix of floats
	/// </summary>
	public Mat3() {
		mat3 = new vec3[3];

		for (int i = 0; i < 3; i++) {
			mat3[i].x = 0f;
			mat3[i].y = 0f;
			mat3[i].z = 0f;
		}
	}

	/// <summary>
	/// Create column-indexed 3x3 matrix of floats
	/// </summary>
	public Mat3(vec3 _vec0, vec3 _vec1, vec3 _vec2) {
		mat3 = new vec3[3];

		mat3[0] = _vec0;
		mat3[1] = _vec1;
		mat3[2] = _vec2;
	}

	public static Mat3 Rotate(vec3 euler) {
		return RotateX(euler.x) * RotateY(euler.y) * RotateZ(euler.z);
	}

	public static Mat3 RotateX(float angle) {
		return new Mat3(
			new vec3(1f, 0f, 0f),
			new vec3(0f, Mathf.Cos(angle), Mathf.Sin(angle)),
			new vec3(0f, -Mathf.Sin(angle), Mathf.Cos(angle))
		);
	}

	public static Mat3 RotateY(float angle) {
		return new Mat3(
			new vec3(Mathf.Cos(angle), 0f, -Mathf.Sin(angle)),
			new vec3(0f, 1f, 0f),
			new vec3(Mathf.Sin(angle), 0f, Mathf.Cos(angle))
		);
	}

	public static Mat3 RotateZ(float angle) {
		return new Mat3(
			new vec3(Mathf.Cos(angle), Mathf.Sin(angle), 0f),
			new vec3(-Mathf.Sin(angle), Mathf.Cos(angle), 0f),
			new vec3(0f, 0f, 1f)
		);
	}

	public vec3 this[int i] {
		get { return mat3[i]; }
		set { mat3[i] = value; }
	}

	public static vec3 operator *(Mat3 _mat, vec3 _vec) {
		vec3 prod = new vec3(0f, 0f, 0f);

		prod.x = vec3.dot(new vec3(_mat[0].x, _mat[1].x, _mat[2].x), _vec);
		prod.y = vec3.dot(new vec3(_mat[0].y, _mat[1].y, _mat[2].y), _vec);
		prod.z = vec3.dot(new vec3(_mat[0].z, _mat[1].z, _mat[2].z), _vec);

		return prod;
	}

	public static Mat3 operator *(Mat3 _mat1, Mat3 _mat2) {
		vec3 col0 = _mat1 * _mat2[0];
		vec3 col1 = _mat1 * _mat2[1];
		vec3 col2 = _mat1 * _mat2[2];

		Mat3 prod = new Mat3(col0, col1, col2);

		return prod;
	}
}
