using System;
using System.Collections.Generic;
using AudioPooling;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp939.Mimicry
{
	public class EnvMimicrySequence : ScriptableObject
	{
		[Serializable]
		private class Sound
		{
			public float Range;

			public float Duration;

			public Vector2Int Repeat;

			public AudioClip[] Clips;

			public AudioMixerChannelType Channel;
		}

		[SerializeField]
		private Sound[] _sounds;

		private Sound _currentlyPlayed;

		private readonly Queue<Sound> _queuedSounds = new Queue<Sound>();

		public void EnqueueAll()
		{
			Array.ForEach(_sounds, EnqueueSound);
		}

		public bool UpdateSequence(Transform mimicPoint)
		{
			if (_currentlyPlayed == null)
			{
				if (!_queuedSounds.TryDequeue(out var element))
				{
					return false;
				}
				AudioSourcePoolManager.PlaySound(element.Clips.RandomItem(), mimicPoint, element.Range, 1f, FalloffType.Exponential, element.Channel);
				_currentlyPlayed = element;
			}
			_currentlyPlayed.Duration -= Time.deltaTime;
			if (_currentlyPlayed.Duration <= 0f)
			{
				_currentlyPlayed = null;
			}
			return true;
		}

		private void EnqueueSound(Sound s)
		{
			int num = UnityEngine.Random.Range(s.Repeat.x, s.Repeat.y + 1);
			while (num-- > 0)
			{
				Sound item = new Sound
				{
					Clips = s.Clips,
					Channel = s.Channel,
					Duration = s.Duration,
					Range = s.Range
				};
				_queuedSounds.Enqueue(item);
			}
		}
	}
}
