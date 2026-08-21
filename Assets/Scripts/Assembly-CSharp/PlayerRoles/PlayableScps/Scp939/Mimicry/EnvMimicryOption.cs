using System;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp939.Mimicry
{
	[Serializable]
	public class EnvMimicryOption
	{
		private EnvMimicrySequence _currentlyPlayed;

		private bool _hasSound;

		[SerializeField]
		private EnvMimicrySequence[] _randomSequences;

		[field: SerializeField]
		public RoleTypeId RoleCondition { get; private set; } = RoleTypeId.None;

		[field: SerializeField]
		public Texture MainIcon { get; private set; }

		[field: SerializeField]
		public Texture CategoryIcon { get; private set; }

		public void Play(byte id)
		{
			_currentlyPlayed = UnityEngine.Object.Instantiate(_randomSequences[id % _randomSequences.Length]);
			if (_currentlyPlayed == null)
			{
				_hasSound = false;
				return;
			}
			_hasSound = true;
			_currentlyPlayed.EnqueueAll();
		}

		public void UpdateSequence(MimicPointController mimicPoint)
		{
			if (_hasSound && !(_currentlyPlayed == null) && !_currentlyPlayed.UpdateSequence(mimicPoint.MimicPointTransform))
			{
				_hasSound = false;
				_currentlyPlayed = null;
				UnityEngine.Object.Destroy(_currentlyPlayed);
			}
		}
	}
}
