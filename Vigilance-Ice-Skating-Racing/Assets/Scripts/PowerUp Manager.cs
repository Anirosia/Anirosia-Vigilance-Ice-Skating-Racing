using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace DefaultNamespace
{
	public class PowerUp_Manager : MonoBehaviour
	{
		#region Singleton

		private static PowerUp_Manager instance;

		public static PowerUp_Manager PowerUpManager {
			get {
				if (instance == null) {
					var powerUp = new GameObject("PowerUp Manager");
					instance = powerUp.AddComponent<PowerUp_Manager>();
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

		public void GetPowerUp(Abilities type, int typeEffect, Collider playerCollider) {
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
			if (playerType.CompareTag("Cat")) playersMovementRef.speed += effect;
			// else
		}

		public void Kibble(GameObject playerType, int effect) {
			if (playerType.CompareTag("Dog")) {
				//obstacle thing 
			}
		}

		public void Cheese(GameObject playerType, int effect) {
			if (playerType.CompareTag("Mouse")) playersMovementRef.jumpMultiplier += effect;
			else {
				var temp = playersMovementRef.speed;
				playersMovementRef.speed -= 2f;
				StartCoroutine(EffectTimeLimit(2));
				playersMovementRef.speed = temp;
			}
		}

		#endregion


		IEnumerator EffectTimeLimit(int limit) {
			yield return new WaitForSeconds(limit);
		}
	}
}