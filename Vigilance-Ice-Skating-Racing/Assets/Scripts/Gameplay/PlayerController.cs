using System;
using System.Collections;
using DefaultNamespace;
using Unity.Mathematics;
using UnityEngine;

namespace Gameplay
{
	[RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(InputHandler))]
	public class PlayerController : MonoBehaviour
	{
		#region Fields

		[Header("Base Values")] [SerializeField]
		private float baseSpeed;

		[SerializeField] private float baseJumpMultiplier;
		[Range(-45, 0)] [SerializeField] private double slopeAngle;
		[SerializeField] private float slopeSpeedIncrease;

		[Header("Physics")] public float fallMultiplier;
		[SerializeField] private LayerMask layer;
		[SerializeField] private float timeZeroToMax;
		[SerializeField] private float timeMaxToZero;

		private float _forwardVelocity;
		private float _accelerationRatePerSec;
		private bool _isSliding;
		private Rigidbody2D _rigidbody;

		[Header("Current Values")] [ReadOnlyInspector] [SerializeField]
		private Vector2 currentSpeedVelocity;

		[ReadOnlyInspector] public float maxSpeed;
		[ReadOnlyInspector] public float maxJump;
		[ReadOnlyInspector] [SerializeField] private bool canFocus;

		[Header("Script References")] [ReadOnlyInspector] [SerializeField]
		private InputHandler playerInput;

		[ReadOnlyInspector] [SerializeField] private CameraFollow cameraFollow;

		// ======================================================================== for value reset
		private float _speedHolder;
		private bool _focusActionReset;
		private Vector2 _colliderOriginalHeight;

		private CapsuleCollider2D _collider;
		private Vector2 _groundNormal;

		#endregion

		private void Awake() {
			playerInput = GetComponent<InputHandler>();
			
			_rigidbody = GetComponent<Rigidbody2D>();
			_collider = GetComponent<CapsuleCollider2D>();
			if (cameraFollow == null) cameraFollow = FindObjectOfType<CameraFollow>();
		}

		private void Start() {
			playerInput.Cam = cameraFollow.viewCamera;
			_focusActionReset = false;
			_accelerationRatePerSec = baseSpeed / timeZeroToMax;
			_colliderOriginalHeight = _collider.size;
			BaseReset();
		}

		void Update() {
			RelativeToGround();
			UserInput();
		}

		private void FixedUpdate() {
			ApplyMovement();
		}

		private void LateUpdate() {
			cameraFollow.CameraWork(IsGrounded());
		}

		void UserInput() {
			if (playerInput.SwipeUp) {
				if (IsGrounded()) Jump();
				playerInput.HasPerformed = true;
			}
				Focus(playerInput.Holding);
			if (playerInput.SwipeDown) {
				StartCoroutine(Slide());
				playerInput.HasPerformed = true;
			}
		}

		private void BaseReset() {
			maxSpeed = baseSpeed;
			_speedHolder = baseSpeed;
			maxJump = baseJumpMultiplier;
			_rigidbody.gravityScale = fallMultiplier;
		}

		private void ValueReset() {
			maxSpeed = _speedHolder;
			// maxJump = jumpHolder;
		}

		private bool IsGrounded() =>
			Physics2D.BoxCast(_collider.bounds.center, _collider.bounds.size, 0f, Vector2.down, 0.01f, layer);

		private void RelativeToGround() {
			Vector3 origin = transform.localPosition;
			Vector2 dir = Vector2.down;
			float dist = 10f;
			Quaternion targetRotation;
			RaycastHit2D hit = new RaycastHit2D();
			hit = (Physics2D.Raycast(origin, dir, dist, layer));
			_groundNormal = hit.normal;
			Quaternion groundRotation = Quaternion.FromToRotation(transform.up, _groundNormal) * transform.rotation;
			if (IsGrounded()) transform.rotation = groundRotation;
			else if (!IsGrounded() && hit) {
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
			var velocity = _rigidbody.velocity;
			var decelerationRate = -maxSpeed / timeMaxToZero;

			if (_forwardVelocity > maxSpeed) {
				_forwardVelocity += decelerationRate * Time.deltaTime;
				_forwardVelocity = Mathf.Max(_forwardVelocity, maxSpeed);
			}
			else {
				_forwardVelocity += _accelerationRatePerSec * Time.deltaTime;
				_forwardVelocity = Mathf.Min(_forwardVelocity, maxSpeed);
			}

			velocity = new Vector2(_forwardVelocity, velocity.y);
			_rigidbody.velocity = velocity;
			//other value management
			currentSpeedVelocity = velocity;
		}

		private void Jump() {
			StopCoroutine(Slide());
			_isSliding = false;
			_collider.size = _colliderOriginalHeight;
			_rigidbody.gravityScale = fallMultiplier;
			Debug.Log("Jumped");
			_rigidbody.velocity = new Vector2(_rigidbody.velocity.x, maxJump);
		}

		private void Focus(bool inputValue) {
			float speedIncrease = maxSpeed;
			var angle = transform.eulerAngles.z;
			if (angle > 180) angle -= 360;
			Debug.Log($"Player angle {angle}");
			if (angle < slopeAngle && inputValue) {
				if (!canFocus) {
					_speedHolder = maxSpeed;
					speedIncrease += slopeSpeedIncrease;
					cameraFollow.StopAllCoroutines();
					cameraFollow.StartCoroutine(cameraFollow.CameraZoomIn(xOffset: 2, camLock: true));
					maxSpeed = speedIncrease;
					canFocus = true;
					_focusActionReset = false;
				}
			}
			else {
				if (canFocus && !_focusActionReset) {
					if (maxSpeed <= _speedHolder || maxSpeed >= _speedHolder) {
						Debug.Log($"Focus Reset");
						cameraFollow.StartCoroutine(cameraFollow.CameraZoomReset());
						maxSpeed = _speedHolder;
					}
					_focusActionReset = true;
				}
			}
		}

		#region Coroutine Methods

		private IEnumerator Slide() {
			var size = _collider.size;
			_collider.size = new Vector2(size.x, size.y / 2);
			if (!IsGrounded()) _rigidbody.gravityScale *= 2;
			_isSliding = true;
			yield return new WaitForSeconds(1.5f);
			_isSliding = false;
			_collider.size = _colliderOriginalHeight;
			_rigidbody.gravityScale = fallMultiplier;
		}

		#endregion
	}
}