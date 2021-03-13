using System;
using UnityEngine;

namespace Gameplay
{
	public class CameraFollow : MonoBehaviour
	{
		public float smooth;
		public Vector3 offset;
		public Transform target;

		private void FixedUpdate() {
			var desiredPos = target.position + offset;
			Vector3 smoothPos = Vector3.Lerp(transform.position, desiredPos, smooth);
			transform.position = smoothPos;
		}
	}
}