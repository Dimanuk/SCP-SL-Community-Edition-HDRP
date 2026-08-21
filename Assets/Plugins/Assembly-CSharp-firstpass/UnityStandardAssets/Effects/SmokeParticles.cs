namespace UnityStandardAssets.Effects
{
	public class SmokeParticles : global::UnityEngine.MonoBehaviour
	{
		public global::UnityEngine.AudioClip[] extinguishSounds;

		private void Start()
		{
			GetComponent<global::UnityEngine.AudioSource>().clip = extinguishSounds[global::UnityEngine.Random.Range(0, extinguishSounds.Length)];
			GetComponent<global::UnityEngine.AudioSource>().Play();
		}
	}
}
