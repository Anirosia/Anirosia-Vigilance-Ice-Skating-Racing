using System;
using System.Collections;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterController : MonoBehaviour
{
	private Touch touch;
	private float timeTouchEnded;
	private float displayTime = .5f;

	private Vector2 startTouchPos;
	private Vector2 touchDelta;
	[SerializeField] private bool isPressed;
	[SerializeField] private double deadZone = 125;
	private bool isDragged;

	[Header("Base Values")] [SerializeField]
	private float baseSpeed = 20f;

	[SerializeField] private float baseJumpMultiplier;

	[Header("Current Values")] public float speed;
	public float jumpMultiplier;

	public float fallMultiplier;

	public bool isGrounded;
	public LayerMask layer;


	public double holdTime;
	public double time;

	private Rigidbody2D rb;
	private BoxCollider2D boxCollider2D;

	public Camera cam;
	private float cameraOriginalPos;
	public float cameraZoomDist;

	private void Start() {
		rb = GetComponent<Rigidbody2D>();
		boxCollider2D = GetComponent<BoxCollider2D>();
		cameraOriginalPos = cam.orthographicSize;
		Reset();
	}

	void Update() {
		ApplyGravity();
		UserInput();
	}

	private void FixedUpdate() {
		// ApplyMovement();
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
							Debug.Log("Swipe Right");
							isPressed = false;
						}
						else {
							Debug.Log("Swipe Left");
							isPressed = false;
						}
					}
					else {
						if (y > 0) {
							Debug.Log("Swipe Up");
							if (isGrounded) Jump();
							isPressed = false;
						}
						else {
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
		StartCoroutine(CameraZoomOut());
	}

	private void Reset() {
		speed = baseSpeed;
		jumpMultiplier = baseJumpMultiplier;
	}

	private void ApplyGravity() {
		if (rb.velocity.y < 0) {
			rb.velocity += Vector2.up * (Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime);
		}

		var box = boxCollider2D.bounds;
		isGrounded = Physics2D.BoxCast(box.center, box.size, 0f, Vector2.down, 0.01f, layer);
	}

	private void ApplyMovement() {
		rb.AddForce(transform.right * speed);
	}

	private void Jump() {
		Debug.Log("Jumped");
		rb.velocity = Vector2.up * jumpMultiplier;
	}

	private void Focus() {
		StartCoroutine(CameraZoomIn());
	}


	#region Coroutine Methods

	private IEnumerator CameraZoomIn() {
		while (cam.orthographicSize <= cameraZoomDist)
			cam.orthographicSize -= Time.deltaTime;
		yield return null;
	}

	private IEnumerator CameraZoomOut() {
		while (cam.orthographicSize >= cameraZoomDist)
			cam.orthographicSize += Time.deltaTime;
		cam.orthographicSize = cameraOriginalPos;
		yield return null;
	}

	private IEnumerator Slide() {
		var size = boxCollider2D.size;
		var originalSize = size;
		boxCollider2D.size = new Vector2(size.x, size.y / 2);
		yield return new WaitForSeconds(1f);
		size = originalSize;
		boxCollider2D.size = size;
	}

	#endregion
}