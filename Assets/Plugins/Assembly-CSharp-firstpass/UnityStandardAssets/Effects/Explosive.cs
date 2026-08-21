namespace UnityStandardAssets.Effects
{
	public class Explosive : global::UnityEngine.MonoBehaviour
	{
		public global::UnityEngine.Transform explosionPrefab;

		public float detonationImpactVelocity = 10f;

		public float sizeMultiplier = 1f;

		public bool reset = true;

		public float resetTimeDelay = 10f;

		private bool m_Exploded;

		private global::UnityStandardAssets.Utility.ObjectResetter m_ObjectResetter;

		private void Start()
		{
			m_ObjectResetter = GetComponent<global::UnityStandardAssets.Utility.ObjectResetter>();
		}

		private global::System.Collections.IEnumerator OnCollisionEnter(global::UnityEngine.Collision col)
		{
			if (base.enabled && col.contacts.Length != 0 && (global::UnityEngine.Vector3.Project(col.relativeVelocity, col.contacts[0].normal).magnitude > detonationImpactVelocity || m_Exploded) && !m_Exploded)
			{
				global::UnityEngine.Object.Instantiate(explosionPrefab, col.contacts[0].point, global::UnityEngine.Quaternion.LookRotation(col.contacts[0].normal));
				m_Exploded = true;
				SendMessage("Immobilize");
				if (reset)
				{
					m_ObjectResetter.DelayedReset(resetTimeDelay);
				}
			}
			yield return null;
		}

		public void Reset()
		{
			m_Exploded = false;
		}
	}
}
