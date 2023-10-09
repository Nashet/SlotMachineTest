using UnityEngine;

namespace Nashet.SlotMachine
{
	/// <summary>
	/// The only purpose of this class is to provide non-unity classes with a way to update.
	/// </summary>
	public class SceneRunner : MonoBehaviour
	{
		[SerializeField] private SceneStarter sceneStarter;
		[SerializeField] private float interval = 0.1f; // 0.1 seconds (10 times per second)

		private float timer = 0f;

		private void FixedUpdate()
		{
			timer += Time.fixedDeltaTime;

			if (timer >= interval)
			{
				// Execute the code block here

				timer = 0f; // Reset the timer
			}
		}
	}
}
