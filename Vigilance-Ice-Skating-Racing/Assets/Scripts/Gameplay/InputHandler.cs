using System;
using System.Collections;
using System.Linq;
using DefaultNamespace;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay
{
	public class InputHandler : MonoBehaviour
	{
		#region Fields

		[Header("Touch Information ")] private Vector2 startTouchPos;
		private Vector2 screenTouchPosition;
		private Vector2 touchDelta;
		private double deadZone = 125;
		[ReadOnlyInspector] [SerializeField] private bool swipeUp, swipeDown, holding, dragging, isPressed;
		private GameObject trailRef;
		private Color debugColor;

		private PlayerActions playerActions;
		private SwipeDirections swipeDirections;

		[SerializeField] private float swipeHeight = 475f;
		[SerializeField] private double holdTime;
		[ReadOnlyInspector] [SerializeField] private double currentHoldTime;
		[SerializeField] private GameObject trailObject;
		[SerializeField] private Camera cam;

		#endregion

		#region Properties

		public bool SwipeUp => swipeUp;
		public bool SwipeDown => swipeDown;
		public bool Holding => holding;
		public bool Dragging => dragging;
		public bool HasPerformed { get; set; }

		public Camera Cam {
			set => cam = value;
		}

		#endregion

		#region Start Up

		private void Awake() => ActionSetUp();
		private void OnEnable() => playerActions.Enable();
		private void OnDisable() => playerActions.Disable();


		private void ActionSetUp() {
			playerActions = new PlayerActions();
			playerActions.Gameplay.PrimaryContact.started += context => OnTouchBegan();
			playerActions.Gameplay.PrimaryContact.canceled += context => OnTouchEnded();
		}

		#endregion

		#region Touch

		private Vector2 TouchPosition() => playerActions.Gameplay.PrimaryPosition.ReadValue<Vector2>();

		//shortcut for point conversion 
		private Vector2 ScreenPointConvert(Vector3 position) => cam.ScreenToWorldPoint(position);

		void OnTouchBegan() {
			startTouchPos = TouchPosition();
			if (trailRef != null)
				trailRef = Instantiate(trailObject, startTouchPos,
					quaternion.identity);
			StartCoroutine(TouchResponseUpdate());
			Debug.Log($"Touch Began {startTouchPos}");
		}

		private void OnTouchEnded() {
			StopAllCoroutines();
			if (trailRef != null) Destroy(trailRef);
			SwipeValue();
			StartCoroutine(Released());
			Debug.Log("Touch Ended");
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

						swipeDirections = SwipeDirections.Up;
						// Debug.Log($"Swiped Up");
					}
					else {
						swipeDirections = SwipeDirections.Down;
						// Debug.Log($"Swiped Down");
					}
				}
			}
		}

		private void SwipeValue() {
			switch (swipeDirections) {
				case SwipeDirections.Up:
					swipeUp = true;
					break;
				case SwipeDirections.Down:
					swipeDown = true;
					break;
			}
		}


		private void TouchReset() {
			HasPerformed = false;
			currentHoldTime = 0;
			dragging = false;
			holding = false;
			swipeUp = false;
			swipeDown = false;
			// Debug.Log("Touch Reset");
		}

		#endregion

		#region Coroutines

		private IEnumerator Released() {
			if (HasPerformed) TouchReset();
			else {
				yield return null; //waits an extra frame 
				TouchReset();
			}
		}

		private IEnumerator TouchResponseUpdate() {
			while (true) {
				screenTouchPosition = ScreenPointConvert(TouchPosition());

				currentHoldTime += Time.deltaTime;

				if (trailRef != null) trailRef.transform.position = screenTouchPosition;

				holding = (currentHoldTime >= holdTime && !dragging);

				touchDelta = TouchPosition() - startTouchPos;
				if (touchDelta.magnitude > deadZone) {
					dragging = true;
					DetermineSwipe(touchDelta);
				}
				else {
					swipeDirections = SwipeDirections.Press;
					debugColor = Color.red;
				}


				Debug.DrawLine(ScreenPointConvert(startTouchPos), screenTouchPosition, debugColor);
				yield return null; //to make it execute once per frame
			}
		}

		#endregion
	}

	enum SwipeDirections
	{
		Up,
		Down,
		Press,
	}
}