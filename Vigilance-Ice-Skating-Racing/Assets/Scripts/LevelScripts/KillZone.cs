using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LevelScripts
{
	[RequireComponent(typeof(BoxCollider2D))]
	public class KillZone : MonoBehaviour
	{
		private void OnTriggerEnter2D(Collider2D other) {
			var scene = SceneManager.GetActiveScene();
			SceneManager.LoadScene(scene.buildIndex);
		}
	}
}