using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using CharacterController = Gameplay.CharacterController;

namespace DefaultNamespace
{
	public class PowerUpManager : MonoBehaviour
	{
		#region Singleton

		private static PowerUpManager instance;

		public static PowerUpManager PowerUp {
			get {
				if (instance == null) {
					var powerUp = new GameObject("PowerUp Manager");
					instance = powerUp.AddComponent<PowerUpManager>();
				}

				return instance;
			}
		}

		private void Awake() {
			if (instance != null) Destroy(this);
			DontDestroyOnLoad(this);
		}

		#endregion

		#region Power Up Calling management

		private CharacterController playersMovementRef;

		public enum Abilities
		{
			Catnip,
			Kibble,
			Cheese,
		}

		public void GetPowerUp(Abilities type, int typeEffect, Collider2D playerCollider) {
			var playerObject = playerCollider.gameObject;
			playersMovementRef = playerObject.GetComponent<CharacterController>();
			switch (type) {
				case Abilities.Catnip:
					Catnip(playerObject, typeEffect);
					break;
				case Abilities.Kibble:
					Kibble(playerObject, typeEffect);
					break;
				case Abilities.Cheese:
					Cheese(playerObject, typeEffect);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(type), type, null);
			}
		}

		#endregion


		#region Power Ups

		public void Catnip(GameObject playerType, int effect) {
			if (playerType.CompareTag("Cat")) playersMovementRef.maxSpeed += effect;
			// else
		}

		public void Kibble(GameObject playerType, int effect) {
			if (playerType.CompareTag("Dog")) {
				//obstacle thing 
			}
		}

		public void Cheese(GameObject playerType, int effect) {
			if (playerType.CompareTag("Mouse")) playersMovementRef.maxJump += effect;
			else {
				var temp = playersMovementRef.maxSpeed;
				playersMovementRef.maxSpeed -= 2f;
				StartCoroutine(EffectTimeLimit(2));
				playersMovementRef.maxSpeed = temp;
			}
		}

		#endregion


		IEnumerator EffectTimeLimit(int limit) {
			yield return new WaitForSeconds(limit);
		}
	}
}