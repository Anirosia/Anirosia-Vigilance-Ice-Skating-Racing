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

		[Header("Touch Information ")] private Vector2 _startTouchPos;
		private Vector2 _screenTouchPosition;
		private Vector2 _touchDelta;
		private double _deadZone = 125;
		[ReadOnlyInspector] [SerializeField] private bool swipeUp, swipeDown, holding, dragging, isPressed;
		private GameObject _trailRef;
		private Color _debugColor;

		private PlayerActions _playerActions;
		private SwipeDirections _swipeDirections;

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
		private void OnEnable() => _playerActions.Enable();
		private void OnDisable() => _playerActions.Disable();


		private void ActionSetUp() {
			_playerActions = new PlayerActions();
			_playerActions.Gameplay.PrimaryContact.started += context => OnTouchBegan();
			_playerActions.Gameplay.PrimaryContact.canceled += context => OnTouchEnded();
		}

		#endregion

		#region Touch

		private Vector2 TouchPosition() => _playerActions.Gameplay.PrimaryPosition.ReadValue<Vector2>();

		//shortcut for point conversion 
		private Vector2 ScreenPointConvert(Vector3 position) => cam.ScreenToWorldPoint(position);

		void OnTouchBegan() {
			_startTouchPos = TouchPosition();
			if (_trailRef != null)
				_trailRef = Instantiate(trailObject, _startTouchPos,
					quaternion.identity);
			StartCoroutine(TouchResponseUpdate());
			Debug.Log($"Touch Began {_startTouchPos}");
		}

		private void OnTouchEnded() {
			StopAllCoroutines();
			if (_trailRef != null) Destroy(_trailRef);
			SwipeValue();
			StartCoroutine(Released());
			Debug.Log("Touch Ended");
		}

		private void DetermineSwipe(Vector2 swipeDelta) {
			if (swipeDelta.magnitude > _deadZone) {
				if (Mathf.Abs(swipeDelta.x) < Mathf.Abs(swipeDelta.y)) {
					if (swipeDelta.y > _deadZone) {
						// if (swipeDelta.y > swipeHeight) {
						// 	//high jump
						// 	debugColor = Color.green;
						// }
						// else {
						// 	//low jump
						//
						// 	debugColor = Color.white;
						// }
						_swipeDirections = SwipeDirections.Up;
						// Debug.Log($"Swiped Up");
					}
					else {
						_swipeDirections = SwipeDirections.Down;
						// Debug.Log($"Swiped Down");
					}
				}
			}
		}

		private void SwipeValue() {
			switch (_swipeDirections) {
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
				_screenTouchPosition = ScreenPointConvert(TouchPosition());

				currentHoldTime += Time.deltaTime;

				if (_trailRef != null) _trailRef.transform.position = _screenTouchPosition;

				holding = (currentHoldTime >= holdTime && !dragging);

				_touchDelta = TouchPosition() - _startTouchPos;
				if (_touchDelta.magnitude > _deadZone) {
					dragging = true;
					DetermineSwipe(_touchDelta);
				}
				else {
					_swipeDirections = SwipeDirections.Press;
					_debugColor = Color.red;
				}


				Debug.DrawLine(ScreenPointConvert(_startTouchPos), _screenTouchPosition, _debugColor);
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