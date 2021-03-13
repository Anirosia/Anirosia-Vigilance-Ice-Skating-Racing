using System;
using System.Collections;
using DefaultNamespace;
using TMPro;
using Unity.Collections;
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
		private bool isPressed;
		private double deadZone = 125;
		private bool isDragged;
		public double holdTime;
		[ReadOnlyInspector] public double time;

		[Header("Base Values")] [SerializeField]
		private float baseSpeed;

		[SerializeField] private float baseJumpMultiplier;

		[Header("Physics")] public float fallMultiplier;
		public bool isGrounded;
		public LayerMask layer;
		private Rigidbody2D rb;
		public float timeZeroToMax;
		private float forwardVelocity;
		private float accelerationRatePerSec;

		[Header("Current Values")] [ReadOnlyInspector] public Vector2 currentSpeedVelocity;
		public float maxSpeed;
		public float maxJump;
		

		private CapsuleCollider2D col2D;

		public Camera cam;
		private float cameraOriginalPos;
		public float cameraZoomDist;

		private Vector2 groundNormal;
		private SwipeDirection swipeDirection;

		private void Start() {
			rb = GetComponent<Rigidbody2D>();
			col2D = GetComponent<CapsuleCollider2D>();
			accelerationRatePerSec = baseSpeed / timeZeroToMax;
			// cameraOriginalPos = cam.orthographicSize;
			Reset();
		}


		void Update() {
			ApplyGravity();
			RelativeToGround();
			UserInput();
		}

		private void FixedUpdate() {
			ApplyMovement();
		}

		void UserInput() {
			if (Input.touchCount > 0) {
				touch = Input.GetTouch(0);
				time += Time.deltaTime;

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

						if (Mathf.Abs(x) > Mathf.Abs(y)) {
							if (x > 0) {
								swipeDirection.SwipeRight = true;
								Debug.Log("Swipe Right");
								isPressed = false;
							}
							else {
								swipeDirection.SwipeLeft = true;
								Debug.Log("Swipe Left");
								isPressed = false;
							}
						}
						else {
							if (y > 0) {
								swipeDirection.SwipeUp = true;
								Debug.Log("Swipe Up");
								if (isGrounded) Jump();
								isPressed = false;
							}
							else {
								swipeDirection.SwipeDown = true;
								Debug.Log("Swipe Down");
								StartCoroutine(Slide());
								isPressed = false;
							}
						}
					}
					else Debug.Log("tap");
				}

				if (touch.phase == TouchPhase.Stationary && time > holdTime && !isDragged) {
					Debug.Log("Hold");
					Focus();
					//tuck down method 
				}

				if (touch.phase == TouchPhase.Ended) TouchReset();
			}
		}

		private void TouchReset() {
			time = 0;
			isDragged = false;
			isPressed = false;

			// StartCoroutine(CameraZoomOut());
		}

		private void Reset() {
			maxSpeed = baseSpeed;
			maxJump = baseJumpMultiplier;
			rb.gravityScale = fallMultiplier;
		}

		private void ApplyGravity() {
			// if (rb.velocity.y < 0) {
			// 	rb.velocity += Vector2.up * (Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime);
			// }
			var box = col2D.bounds;
			isGrounded = Physics2D.BoxCast(box.center, box.size, 0f, Vector2.down, 0.01f, layer);
		}

		private void RelativeToGround() {
			var origin = transform.localPosition;
			var dir = -transform.up;
			var dist = 1.5f;
			if (isGrounded) {
				RaycastHit2D hit = new RaycastHit2D();
				hit = (Physics2D.Raycast(origin, dir, dist, layer));
				groundNormal = hit.normal;
				Debug.DrawRay(origin, dir * dist);

				Quaternion toRotation = Quaternion.FromToRotation(transform.up, groundNormal) * transform.rotation;
				transform.rotation = toRotation;
			}
			else {
				transform.rotation = Quaternion.Euler(Vector3.zero);
			}
		}

		private void ApplyMovement() {
			forwardVelocity += accelerationRatePerSec * Time.deltaTime;
			forwardVelocity = Mathf.Min(forwardVelocity, maxSpeed);

			rb.velocity = new Vector2(forwardVelocity, rb.velocity.y);
			//other value management
			currentSpeedVelocity = rb.velocity;
		}

		private void Jump() {
			Debug.Log("Jumped");
			rb.velocity = new Vector2(rb.velocity.x, maxJump);
			// rb.velocity = Vector2.up * jumpMultiplier;
		}

		private void Focus() {
			// StartCoroutine(CameraZoomIn());
		}

		#region Coroutine Methods

		// private IEnumerator CameraZoomIn() {
		// 	while (cam.orthographicSize <= cameraZoomDist)
		// 		cam.orthographicSize -= Time.deltaTime;
		// 	yield return null;
		// }
		//
		// private IEnumerator CameraZoomOut() {
		// 	while (cam.orthographicSize >= cameraZoomDist)
		// 		cam.orthographicSize += Time.deltaTime;
		// 	cam.orthographicSize = cameraOriginalPos;
		// 	yield return null;
		// }

		private IEnumerator Slide() {
			var size = col2D.size;
			var originalSize = size;
			col2D.size = new Vector2(size.x, size.y / 2);
			if (!isGrounded) rb.gravityScale *= 2;
			yield return new WaitForSeconds(1.5f);
			size = originalSize;
			col2D.size = size;
			rb.gravityScale = fallMultiplier;
		}

		#endregion
	}

	struct SwipeDirection
	{
		public bool SwipeUp, SwipeDown, SwipeLeft, SwipeRight;
	}
}