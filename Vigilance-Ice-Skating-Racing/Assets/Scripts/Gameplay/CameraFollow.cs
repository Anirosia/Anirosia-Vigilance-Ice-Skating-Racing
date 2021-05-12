﻿using System;
using System.Collections;
using System.ComponentModel;
using DefaultNamespace;
using UnityEngine;

namespace Gameplay
{
	public class CameraFollow : MonoBehaviour
	{
		[Header("Camera Settings")] [ReadOnlyInspector] [SerializeReference]
		private float cameraSmoothing = 0.1f;
		public Vector3 offset;
		public Transform target;
		[SerializeField] [Range(0, 1)] private float zoomSmoothSpeed;
		[ReadOnlyInspector] [SerializeField] private int viewDist;
		[ReadOnlyInspector] [SerializeField] private float smooth;

		private int _desiredZoomInDistance = 3;
		private int _desiredZoomOutDistance = 8;
		private bool _isTargetNotNull;
		[HideInInspector]public Camera viewCamera;
		private Vector3 _savedOffset;
		private float _cameraLock = 1;

		[Header("Zoom Settings")]
		[Header("Zoom Values")]
		//setup
		[ReadOnlyInspector]
		[SerializeField]
		private float setOffsetSpeed = 17f;

		[ReadOnlyInspector] [SerializeField] private float setZoomSpeed = 10f;
		[ReadOnlyInspector] [SerializeField] private float setLockSpeed = 4.5f;

		[Header("Zoom Calculations")]
		//execution after calculation
		[ReadOnlyInspector]
		[SerializeField]
		private float offsetSpeed;

		[ReadOnlyInspector] [SerializeField] private float zoomSpeed;
		[ReadOnlyInspector] [SerializeField] private float lockSpeed;
		private bool actionCalled;

		private void Awake() {
			viewCamera = GetComponent<Camera>();
		}


		private void OnEnable() {
			_isTargetNotNull = target != null;
			viewDist = Mathf.RoundToInt(viewCamera.orthographicSize);
			_savedOffset = offset;
			smooth = cameraSmoothing;
			UnitNotionConversion();
		}

		private void OnValidate() {
			UnitNotionConversion();
		}

		private void FixedUpdate() {
			if (_isTargetNotNull) {
				var desiredPos = target.position + offset;
				Vector3 smoothPos = Vector3.Lerp(transform.position, desiredPos, smooth);
				transform.position = smoothPos;
			}
		}

		// public void SelfReference(Transform objectTransform) {
		// 	if (!_isTargetNotNull) {
		// 		target = objectTransform;
		// 		_isTargetNotNull = true;
		// 	}
		// }

		public void CameraWork(bool grounded) {
			//downhill handlers

			var angle = Mathf.Floor(target.localEulerAngles.z);
			if (angle > 180) angle -= 360;
			// Debug.Log($"Player angle {angle}");
			if (angle < -5 || angle > 5) smooth = .5f;
			else smooth = cameraSmoothing;

			//ground handler
			if (!grounded && !actionCalled) {
				StopAllCoroutines();
				StartCoroutine(CameraZoomOut());
				actionCalled = true;
			}
			else if (grounded && actionCalled) {
				StopAllCoroutines();
				StartCoroutine(CameraZoomReset());
				actionCalled = false;
			}
		}

		private void UnitNotionConversion() {
			var power = _desiredZoomInDistance + zoomSmoothSpeed;
			offsetSpeed = (float) Math.Round(setOffsetSpeed * Mathf.Pow(10, -power), 5);
			zoomSpeed = (float) Math.Round(setZoomSpeed     * Mathf.Pow(10, -power), 5);
			lockSpeed = (float) Math.Round(setLockSpeed     * Mathf.Pow(10, -power), 5);
		}

		public IEnumerator CameraZoomIn(float xOffset = default, bool camLock = false,
		                                bool reset = false) {
			// Debug.Log("Camera Zoom In");
			int zoomDistance;
			zoomDistance = reset ? viewDist : _desiredZoomInDistance;

			while (viewCamera.orthographicSize >= zoomDistance) {
				viewCamera.orthographicSize -= zoomSpeed;
				if (offset.x >= xOffset) offset.x -= offsetSpeed;
				yield return new WaitForSeconds(Time.deltaTime);
				if (camLock) {
					if (smooth <= _cameraLock) smooth += lockSpeed;
				}
			}

			if (camLock) smooth = _cameraLock;
			offset.x = xOffset;
			viewCamera.orthographicSize = zoomDistance;
		}

		public IEnumerator CameraZoomOut(float xOffset = default, bool reset = false) {
			// Debug.Log("Camera Zoom Out");
			int zoomDistance;
			zoomDistance = reset ? viewDist : _desiredZoomOutDistance;

			while (viewCamera.orthographicSize <= zoomDistance) {
				viewCamera.orthographicSize += zoomSpeed;
				if (offset.x <= xOffset) offset.x += offsetSpeed;
				if (viewCamera.orthographicSize >= cameraSmoothing) {
					if (smooth <= cameraSmoothing) smooth = cameraSmoothing;
					else smooth -= lockSpeed;
				}

				yield return new WaitForSeconds(Time.deltaTime);
			}

			smooth = cameraSmoothing;
			offset.x = _savedOffset.x;
			viewCamera.orthographicSize = zoomDistance;
		}

		public IEnumerator CameraZoomReset() {
			StopAllCoroutines();
			// Debug.Log("Camera Reset");
			if (viewCamera.orthographicSize < viewDist)
				return CameraZoomOut(_savedOffset.x, true);
			else
				return CameraZoomIn(_savedOffset.x, reset: true);
		}
	}
}