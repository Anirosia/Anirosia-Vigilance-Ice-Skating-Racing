using DefaultNamespace;
using UnityEngine;

namespace Gameplay
{
	public class PowerUpItem : MonoBehaviour
	{
		public PowerUpManager.Abilities ability;
		public int abilityEffect;

		private void OnTriggerEnter2D(Collider2D other) {
			PowerUpManager.PowerUp.GetPowerUp(ability, abilityEffect, other);
			Debug.Log("Triggered");
			Destroy(this);
		}
			
	}
}