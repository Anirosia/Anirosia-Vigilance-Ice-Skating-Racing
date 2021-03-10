using System;
using UnityEngine;

namespace DefaultNamespace
{
	public class PowerUpItem : MonoBehaviour
	{
		public PowerUp_Manager.Abilities ability;
		public int abilityEffect;

		private void OnTriggerEnter(Collider other) =>
			PowerUp_Manager.PowerUpManager.GetPowerUp(ability, abilityEffect, other);
	}
}