using System;
using System.Collections;
using System.Net.Http.Headers;
using DefaultNamespace;
using TMPro;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

namespace Gameplay
{
	[RequireComponent(typeof(Rigidbody2D))]
	public class CharacterController : MonoBehaviour
	{
		[Header("Touch Information")] private Touch touch;
		private float timeTouchEnded;
		private float displayTime = .5f;
		private Vector2 startTouchPos;
		private Vector2 touchDelta;
		[ReadOnlyInspector] [SerializeField] private bool isPressed;
		[ReadOnlyInspector] [SerializeField] private bool isHeld;
		[ReadOnlyInspector] [SerializeField] private bool isDragged;
		private double deadZone = 125;

		public double holdTime;
		[ReadOnlyInspector] public double currentHoldTime;

		[Header("Base Values")] [SerializeField]
		private float baseSpeed;

		[SerializeField] private float baseJumpMultiplier;
		[Range(-45, 0)] [SerializeField] private double slopeAngle;
		[SerializeField] private float slopeSpeedIncrease;

		[Header("Physics")] public float fallMultiplier;
		public bool isGrounded;
		public LayerMask layer;
		private Rigidbody2D rb;
		[SerializeField] private float timeZeroToMax;
		[SerializeField] private float timeMaxToZero;
		private float forwardVelocity;
		private float accelerationRatePerSec;

		[Header("Current Values")] [ReadOnlyInspector]
		public Vector2 currentSpeedVelocity;

		public float maxSpeed;

		public float maxJump;

		//for value reset
		private float speedHolder;

		// private float jumpHolder;
		private bool focusActionReset;
		private bool isSliding;
		private Vector2 colliderOriginalHeight;


		public CameraFollow cameraFollow;
		private CapsuleCollider2D col2D;
		private Vector2 groundNormal;
		private SwipeDirection swipeDirection;


		private void Awake() {
			rb = GetComponent<Rigidbody2D>();
			col2D = GetComponent<CapsuleCollider2D>();
		}

		private void Start() {
			focusActionReset = false;
			accelerationRatePerSec = baseSpeed / timeZeroToMax;
			colliderOriginalHeight = col2D.size;
			BaseReset();
		}

		void Update() {
			ApplyGravity();
			RelativeToGround();
			UserInput();
			cameraFollow.CameraWork(isGrounded);
		}

		private void FixedUpdate() {
			ApplyMovement();
		}

		void UserInput() {
			if (Input.touchCount > 0) {
				touch = Input.GetTouch(0);

				if (!isHeld) currentHoldTime += Time.deltaTime;

				if (touch.phase == TouchPhase.Began) {
					startTouchPos = touch.position;
					isPressed = true;
				}

				if (touch.phase == TouchPhase.Moved && isPressed) {
					touchDelta = touch.position - startTouchPos;
					isDragged = true;
					if (touchDelta.magnitude > deadZone) {
						var x = touchDelta.x;
						var y = touchDelta.y;

						if (Mathf.Abs(x) < Mathf.Abs(y)) {
							if (y > 0) {
								swipeDirection.SwipeUp = true;
								Debug.Log("Swipe Up");
								if (isGrounded) Jump();
								isPressed = false;
							}
							else {
								swipeDirection.SwipeDown = true;
								Debug.Log("Swipe Down");
								if (!isSliding) StartCoroutine(Slide());
								isPressed = false;
							}
						}
					}
					else Debug.Log("tap");
				}

				if (touch.phase == TouchPhase.Stationary && currentHoldTime > holdTime && !isDragged) {
					Debug.Log("Hold");
					Focus();
					//tuck down method 
				}

				if (touch.phase == TouchPhase.Ended) {
					if (isHeld) cameraFollow.StartCoroutine(cameraFollow.CameraZoomReset());
					TouchReset();
					ValueReset();
				}
			}
		}

		private void TouchReset() {
			currentHoldTime = 0;
			isDragged = false;
			isPressed = false;
			isHeld = false;
			swipeDirection.SwipeUp = false;
			swipeDirection.SwipeDown = false;
			// StartCoroutine(CameraZoomOut());
		}

		private void BaseReset() {
			maxSpeed = baseSpeed;
			maxJump = baseJumpMultiplier;
			rb.gravityScale = fallMultiplier;
		}

		private void ValueReset() {
			maxSpeed = speedHolder;
			// maxJump = jumpHolder;
		}

		private void ApplyGravity() {
			// if (rb.velocity.y < 0) {
			// 	rb.velocity += Vector2.up * (Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime);
			// }
			var box = col2D.bounds;
			isGrounded = Physics2D.BoxCast(box.center, box.size, 0f, Vector2.down, 0.01f, layer);
		}

		private void RelativeToGround() {
			Vector3 origin = transform.localPosition;
			Vector2 dir = Vector2.down;
			float dist = 10f;
			Quaternion targetRotation;
			RaycastHit2D hit = new RaycastHit2D();
			hit = (Physics2D.Raycast(origin, dir, dist, layer));
			groundNormal = hit.normal;
			Quaternion groundRotation = Quaternion.FromToRotation(transform.up, groundNormal) * transform.rotation;
			if (isGrounded) transform.rotation = groundRotation;
			else if (!isGrounded && hit) {
				targetRotation = Quaternion.Slerp(transform.rotation, groundRotation, Time.deltaTime * 2f);
				transform.rotation = targetRotation;
			}
			else {
				targetRotation = Quaternion.Slerp(transform.rotation, quaternion.identity, Time.deltaTime * 2f);
				transform.rotation = targetRotation;
			}

			Debug.DrawRay(origin, dir * dist);
		}

		private void ApplyMovement() {
			var velocity = rb.velocity;
			var decelerationRate = -maxSpeed / timeMaxToZero;

			if (forwardVelocity > maxSpeed) {
				forwardVelocity += decelerationRate * Time.deltaTime;
				forwardVelocity = Mathf.Max(forwardVelocity, maxSpeed);
			}
			else {
				forwardVelocity += accelerationRatePerSec * Time.deltaTime;
				forwardVelocity = Mathf.Min(forwardVelocity, maxSpeed);
			}

			velocity = new Vector2(forwardVelocity, velocity.y);
			rb.velocity = velocity;
			//other value management
			currentSpeedVelocity = velocity;
		}

		private void Jump() {
			StopCoroutine(Slide());
			isSliding = false;
			col2D.size = colliderOriginalHeight;
			Debug.Log("Jumped");
			rb.velocity = new Vector2(rb.velocity.x, maxJump);
			// rb.velocity = Vector2.up * jumpMultiplier;
		}

		private void Focus() {
			float speedIncrease = maxSpeed;
			var angle = transform.eulerAngles.z;
			if (angle > 180) angle -= 360;
			Debug.Log($"Player angle {angle}");
			if (angle < slopeAngle) {
				if (!isHeld) {
					speedHolder = maxSpeed;
					speedIncrease += slopeSpeedIncrease;
					cameraFollow.StopAllCoroutines();
					cameraFollow.StartCoroutine(cameraFollow.CameraZoomIn(xOffset: 2, camLock: true));
					maxSpeed = speedIncrease;
					isHeld = true;
					focusActionReset = false;
				}
			}
			else {
				if (isHeld && !focusActionReset) {
					if (maxSpeed <= speedHolder || maxSpeed >= speedHolder) {
						Debug.Log($"Focus Reset");
						cameraFollow.StartCoroutine(cameraFollow.CameraZoomReset());
						maxSpeed = speedHolder;
					}

					focusActionReset = true;
				}
			}
		}

		void SlideCancel() {
		}

		#region Coroutine Methods

		private IEnumerator Slide() {
			var size = col2D.size;
			col2D.size = new Vector2(size.x, size.y / 2);
			if (!isGrounded) rb.gravityScale *= 2;
			isSliding = true;
			yield return new WaitForSeconds(1.5f);
			isSliding = false;
			col2D.size = colliderOriginalHeight;
			rb.gravityScale = fallMultiplier;
		}

		#endregion
	}

	struct SwipeDirection
	{
		public bool SwipeUp, SwipeDown, SwipeLeft, SwipeRight;
	}
}