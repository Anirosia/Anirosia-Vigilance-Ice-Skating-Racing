using System;
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

	public float speed = 20f;
	public float jumpMultiplier;
	public float fallMultiplier;

	public bool isGrounded;
	public LayerMask layer;

	private Rigidbody2D rb;
	private BoxCollider2D boxCollider2D;

	private void Start() {
		rb = GetComponent<Rigidbody2D>();
		boxCollider2D = GetComponent<BoxCollider2D>();
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


			if (touch.phase == TouchPhase.Began) {
				startTouchPos = touch.position;
				isPressed = true;
			}

			if (touch.phase == TouchPhase.Moved && isPressed) {
				touchDelta = touch.position - startTouchPos;

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
							Jump();
							isPressed = false;
						}
						else {
							Debug.Log("Swipe Down");
							isPressed = false;
						}
					}
				}
				else Debug.Log("tap");
			}

			if (touch.phase == TouchPhase.Stationary) {
				//tuck down method 
			}

			// if (touch.phase == TouchPhase.Ended)
			// 	isPressed = false;
		}
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
		if (isGrounded) {
			Debug.Log("Jumped");
			rb.velocity = Vector2.up * jumpMultiplier;
		}
	}
}