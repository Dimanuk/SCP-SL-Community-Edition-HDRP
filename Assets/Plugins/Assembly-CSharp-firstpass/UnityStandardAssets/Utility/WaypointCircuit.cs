namespace UnityStandardAssets.Utility
{
	public class WaypointCircuit : global::UnityEngine.MonoBehaviour
	{
		[global::System.Serializable]
		public class WaypointList
		{
			public global::UnityStandardAssets.Utility.WaypointCircuit circuit;

			public global::UnityEngine.Transform[] items = new global::UnityEngine.Transform[0];
		}

		public struct RoutePoint
		{
			public global::UnityEngine.Vector3 position;

			public global::UnityEngine.Vector3 direction;

			public RoutePoint(global::UnityEngine.Vector3 position, global::UnityEngine.Vector3 direction)
			{
				this.position = position;
				this.direction = direction;
			}
		}

		public global::UnityStandardAssets.Utility.WaypointCircuit.WaypointList waypointList = new global::UnityStandardAssets.Utility.WaypointCircuit.WaypointList();

		[global::UnityEngine.SerializeField]
		private bool smoothRoute = true;

		private int numPoints;

		private global::UnityEngine.Vector3[] points;

		private float[] distances;

		public float editorVisualisationSubsteps = 100f;

		private int p0n;

		private int p1n;

		private int p2n;

		private int p3n;

		private float i;

		private global::UnityEngine.Vector3 P0;

		private global::UnityEngine.Vector3 P1;

		private global::UnityEngine.Vector3 P2;

		private global::UnityEngine.Vector3 P3;

		public float Length { get; private set; }

		public global::UnityEngine.Transform[] Waypoints => waypointList.items;

		private void Awake()
		{
			if (Waypoints.Length > 1)
			{
				CachePositionsAndDistances();
			}
			numPoints = Waypoints.Length;
		}

		public global::UnityStandardAssets.Utility.WaypointCircuit.RoutePoint GetRoutePoint(float dist)
		{
			global::UnityEngine.Vector3 routePosition = GetRoutePosition(dist);
			return new global::UnityStandardAssets.Utility.WaypointCircuit.RoutePoint(routePosition, (GetRoutePosition(dist + 0.1f) - routePosition).normalized);
		}

		public global::UnityEngine.Vector3 GetRoutePosition(float dist)
		{
			int i = 0;
			if (Length == 0f)
			{
				Length = distances[distances.Length - 1];
			}
			for (dist = global::UnityEngine.Mathf.Repeat(dist, Length); distances[i] < dist; i++)
			{
			}
			p1n = (i - 1 + numPoints) % numPoints;
			p2n = i;
			this.i = global::UnityEngine.Mathf.InverseLerp(distances[p1n], distances[p2n], dist);
			if (smoothRoute)
			{
				p0n = (i - 2 + numPoints) % numPoints;
				p3n = (i + 1) % numPoints;
				p2n %= numPoints;
				P0 = points[p0n];
				P1 = points[p1n];
				P2 = points[p2n];
				P3 = points[p3n];
				return CatmullRom(P0, P1, P2, P3, this.i);
			}
			p1n = (i - 1 + numPoints) % numPoints;
			p2n = i;
			return global::UnityEngine.Vector3.Lerp(points[p1n], points[p2n], this.i);
		}

		private global::UnityEngine.Vector3 CatmullRom(global::UnityEngine.Vector3 p0, global::UnityEngine.Vector3 p1, global::UnityEngine.Vector3 p2, global::UnityEngine.Vector3 p3, float i)
		{
			return 0.5f * (2f * p1 + (-p0 + p2) * i + (2f * p0 - 5f * p1 + 4f * p2 - p3) * i * i + (-p0 + 3f * p1 - 3f * p2 + p3) * i * i * i);
		}

		private void CachePositionsAndDistances()
		{
			points = new global::UnityEngine.Vector3[Waypoints.Length + 1];
			distances = new float[Waypoints.Length + 1];
			float num = 0f;
			for (int i = 0; i < points.Length; i++)
			{
				global::UnityEngine.Transform transform = Waypoints[i % Waypoints.Length];
				global::UnityEngine.Transform transform2 = Waypoints[(i + 1) % Waypoints.Length];
				if (transform != null && transform2 != null)
				{
					global::UnityEngine.Vector3 position = transform.position;
					global::UnityEngine.Vector3 position2 = transform2.position;
					points[i] = Waypoints[i % Waypoints.Length].position;
					distances[i] = num;
					num += (position - position2).magnitude;
				}
			}
		}

		private void OnDrawGizmos()
		{
			DrawGizmos(selected: false);
		}

		private void OnDrawGizmosSelected()
		{
			DrawGizmos(selected: true);
		}

		private void DrawGizmos(bool selected)
		{
			waypointList.circuit = this;
			if (Waypoints.Length <= 1)
			{
				return;
			}
			numPoints = Waypoints.Length;
			CachePositionsAndDistances();
			Length = distances[distances.Length - 1];
			global::UnityEngine.Gizmos.color = (selected ? global::UnityEngine.Color.yellow : new global::UnityEngine.Color(1f, 1f, 0f, 0.5f));
			global::UnityEngine.Vector3 vector = Waypoints[0].position;
			if (smoothRoute)
			{
				for (float num = 0f; num < Length; num += Length / editorVisualisationSubsteps)
				{
					global::UnityEngine.Vector3 routePosition = GetRoutePosition(num + 1f);
					global::UnityEngine.Gizmos.DrawLine(vector, routePosition);
					vector = routePosition;
				}
				global::UnityEngine.Gizmos.DrawLine(vector, Waypoints[0].position);
			}
			else
			{
				for (int i = 0; i < Waypoints.Length; i++)
				{
					global::UnityEngine.Vector3 position = Waypoints[(i + 1) % Waypoints.Length].position;
					global::UnityEngine.Gizmos.DrawLine(vector, position);
					vector = position;
				}
			}
		}
	}
}
