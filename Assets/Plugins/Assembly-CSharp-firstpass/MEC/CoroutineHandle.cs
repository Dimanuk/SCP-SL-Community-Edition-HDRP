namespace MEC
{
	public struct CoroutineHandle : global::System.IEquatable<global::MEC.CoroutineHandle>
	{
		private const byte ReservedSpace = 15;

		private static readonly int[] NextIndex = new int[16]
		{
			16, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0
		};

		private readonly int _id;

		public byte Key => (byte)(_id & 0xF);

		public string Tag
		{
			get
			{
				return global::MEC.Timing.GetTag(this);
			}
			set
			{
				global::MEC.Timing.SetTag(this, value);
			}
		}

		public int? Layer
		{
			get
			{
				return global::MEC.Timing.GetLayer(this);
			}
			set
			{
				if (!value.HasValue)
				{
					global::MEC.Timing.RemoveLayer(this);
				}
				else
				{
					global::MEC.Timing.SetLayer(this, value.Value);
				}
			}
		}

		public global::MEC.Segment Segment
		{
			get
			{
				return global::MEC.Timing.GetSegment(this);
			}
			set
			{
				global::MEC.Timing.SetSegment(this, value);
			}
		}

		public bool IsRunning
		{
			get
			{
				return global::MEC.Timing.IsRunning(this);
			}
			set
			{
				if (!value)
				{
					global::MEC.Timing.KillCoroutines(this);
				}
			}
		}

		public bool IsAliveAndPaused
		{
			get
			{
				return global::MEC.Timing.IsAliveAndPaused(this);
			}
			set
			{
				if (value)
				{
					global::MEC.Timing.PauseCoroutines(this);
				}
				else
				{
					global::MEC.Timing.ResumeCoroutines(this);
				}
			}
		}

		public bool IsValid => Key != 0;

		public CoroutineHandle(byte ind)
		{
			if (ind > 15)
			{
				ind -= 15;
			}
			_id = NextIndex[ind] + ind;
			NextIndex[ind] += 16;
		}

		public CoroutineHandle(global::MEC.CoroutineHandle other)
		{
			_id = other._id;
		}

		public bool Equals(global::MEC.CoroutineHandle other)
		{
			return _id == other._id;
		}

		public override bool Equals(object other)
		{
			if (other is global::MEC.CoroutineHandle)
			{
				return Equals((global::MEC.CoroutineHandle)other);
			}
			return false;
		}

		public static bool operator ==(global::MEC.CoroutineHandle a, global::MEC.CoroutineHandle b)
		{
			return a._id == b._id;
		}

		public static bool operator !=(global::MEC.CoroutineHandle a, global::MEC.CoroutineHandle b)
		{
			return a._id != b._id;
		}

		public override int GetHashCode()
		{
			return _id;
		}

		public override string ToString()
		{
			if (global::MEC.Timing.GetTag(this) == null)
			{
				if (!global::MEC.Timing.GetLayer(this).HasValue)
				{
					return global::MEC.Timing.GetDebugName(this);
				}
				return global::MEC.Timing.GetDebugName(this) + " Layer: " + global::MEC.Timing.GetLayer(this);
			}
			if (!global::MEC.Timing.GetLayer(this).HasValue)
			{
				return global::MEC.Timing.GetDebugName(this) + " Tag: " + global::MEC.Timing.GetTag(this);
			}
			return global::MEC.Timing.GetDebugName(this) + " Tag: " + global::MEC.Timing.GetTag(this) + " Layer: " + global::MEC.Timing.GetLayer(this);
		}
	}
}
