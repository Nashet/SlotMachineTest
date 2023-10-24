using UnityEngine;

namespace Assets.SlotMachineNetTest.Scripts.Universal.Patterns
{
	//Singletons are know as a bad pattern.
	public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
	{
		private static T instance;

		public static T Instance
		{
			get
			{
				// Check if an instance already exists
				if (instance == null)
				{
					// If not, find or create the instance
					instance = FindObjectOfType<T>();

					// If no instance exists in the scene, create a new GameObject with the SingletonExample attached
					if (instance == null)
					{
						GameObject singletonObject = new GameObject(typeof(T).Name);
						instance = singletonObject.AddComponent<T>();
					}
				}

				return instance;
			}
		}

		private void Awake()
		{
			// Ensure that only one instance of the Singleton exists
			if (instance != null && instance != this)
			{
				Destroy(gameObject);
				throw new System.Exception("An instance of this singleton already exists.");
			}
			else
			{
				// Set the instance if it's null
				instance = GetComponent<T>();
			}

			// Keep the SingletonExample persistent between scenes
			DontDestroyOnLoad(gameObject);
		}
	}
}