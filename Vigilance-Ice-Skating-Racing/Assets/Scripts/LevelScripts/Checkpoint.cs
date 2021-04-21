using System;
using UnityEngine;

namespace LevelScripts
{
	public class Checkpoint : MonoBehaviour
	{
		private EndlessLevel endlessLevelData;
		private void OnTriggerEnter2D(Collider2D other) {
			endlessLevelData = FindObjectOfType<EndlessLevel>();
			endlessLevelData.RespawnLocation = (Vector2) gameObject.transform.position;
		}
	}
}