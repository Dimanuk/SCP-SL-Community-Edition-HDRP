using PlayerStatsSystem;
using UnityEngine;

public interface IDestructible
{
	uint NetworkId { get; }

	Vector3 CenterOfMass { get; }

	bool Damage(float damage, DamageHandlerBase handler, Vector3 exactHitPos);
}
