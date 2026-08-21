namespace UnityStandardAssets.Effects
{
	public class ExplosionFireAndDebris : global::UnityEngine.MonoBehaviour
	{
		public global::UnityEngine.Transform[] debrisPrefabs;

		public global::UnityEngine.Transform firePrefab;

		public int numDebrisPieces;

		public int numFires;

		private global::System.Collections.IEnumerator Start()
		{
			float multiplier = GetComponent<global::UnityStandardAssets.Effects.ParticleSystemMultiplier>().multiplier;
			for (int i = 0; (float)i < (float)numDebrisPieces * multiplier; i++)
			{
				global::UnityEngine.Transform original = debrisPrefabs[global::UnityEngine.Random.Range(0, debrisPrefabs.Length)];
				global::UnityEngine.Vector3 position = base.transform.position + global::UnityEngine.Random.insideUnitSphere * 3f * multiplier;
				global::UnityEngine.Quaternion rotation = global::UnityEngine.Random.rotation;
				global::UnityEngine.Object.Instantiate(original, position, rotation);
			}
			yield return null;
			float num = 10f * multiplier;
			global::UnityEngine.Collider[] array = global::UnityEngine.Physics.OverlapSphere(base.transform.position, num);
			foreach (global::UnityEngine.Collider collider in array)
			{
				if (numFires > 0)
				{
					global::UnityEngine.Ray ray = new global::UnityEngine.Ray(base.transform.position, collider.transform.position - base.transform.position);
					if (collider.Raycast(ray, out var hitInfo, num))
					{
						AddFire(collider.transform, hitInfo.point, hitInfo.normal);
						numFires--;
					}
				}
			}
			float num2 = 0f;
			while (numFires > 0 && num2 < num)
			{
				if (global::UnityEngine.Physics.Raycast(new global::UnityEngine.Ray(base.transform.position + global::UnityEngine.Vector3.up, global::UnityEngine.Random.onUnitSphere), out var hitInfo2, num2))
				{
					AddFire(null, hitInfo2.point, hitInfo2.normal);
					numFires--;
				}
				num2 += num * 0.1f;
			}
		}

		private void AddFire(global::UnityEngine.Transform t, global::UnityEngine.Vector3 pos, global::UnityEngine.Vector3 normal)
		{
			pos += normal * 0.5f;
			global::UnityEngine.Object.Instantiate(firePrefab, pos, global::UnityEngine.Quaternion.identity).parent = t;
		}
	}
}
