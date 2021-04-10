using System;
using DefaultNamespace;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace Gameplay
{
	public class InputHandler : MonoBehaviour
	{
		#region Fields

		[Header("Touch Information ")] private Vector2 startTouchPos;
		private Vector2 touchPosition;
		private Vector2 touchDelta;
		private double deadZone = 125;
		private bool swipeUp, swipeDown, holding, dragging;
		private GameObject trailRef;
		private Color debugColor;

		[SerializeField] private float swipeHeight = 475f;
		[SerializeField] private double holdTime;
		[ReadOnlyInspector] [SerializeField] private double currentHoldTime;
		[SerializeField] private GameObject trailObject;
		[SerializeField] private Camera cam;

		#endregion

		#region Propertys

		public bool SwipeUp => swipeUp;
		public bool SwipeDown => swipeDown;
		public bool Holding => holding;

		public bool Dragging => dragging;

		#endregion

		private void Update() {
			if (Input.touchCount > 0)
				TouchResponse(Input.GetTouch(0));
		}

		private void TouchResponse(Touch currentTouch) {
			touchPosition = cam.ScreenToWorldPoint(currentTouch.position);
			if (!dragging) currentHoldTime += Time.deltaTime;
			if (currentTouch.phase == TouchPhase.Began) {
				startTouchPos = currentTouch.position;
				trailRef = Instantiate(trailObject, touchPosition,
					quaternion.identity);
				debugColor = Color.white;
				// Debug.Log(touchPosition);
				// Debug.Log("Touch Began");
			}

			if (currentTouch.phase == TouchPhase.Moved) {
				trailRef.transform.position = touchPosition;

				touchDelta = currentTouch.position - startTouchPos;

				if (touchDelta.magnitude > deadZone) {
					dragging = true;
					DetermineSwipe(touchDelta);
				}
				else debugColor = Color.red;
			}

			holding = (currentTouch.phase == TouchPhase.Stationary && currentHoldTime >= holdTime && !dragging);

			if (currentTouch.phase == TouchPhase.Ended) {
				Destroy(trailRef);
				TouchReset();
			}

			Debug.DrawLine(cam.ScreenToWorldPoint(startTouchPos), touchPosition, debugColor);
		}

		private void DetermineSwipe(Vector2 swipeDelta) {
			if (swipeDelta.magnitude > deadZone) {
				if (Mathf.Abs(swipeDelta.x) < Mathf.Abs(swipeDelta.y)) {
					if (swipeDelta.y > deadZone) {
						if (swipeDelta.y > swipeHeight) {
							//high jump
							debugColor = Color.green;
						}
						else {
							//low jump
							debugColor = Color.white;
						}

						swipeUp = true;
						// Debug.Log($"Swiped Up");
					}
					else {
						swipeDown = true;
						// Debug.Log($"Swiped Down");
					}
				}
			}
		}


		private void TouchReset() {
			currentHoldTime = 0;
			dragging = false;
			holding = false;
			swipeUp = false;
			swipeDown = false;
			// Debug.Log("Touch Reset");
		}
	}
}