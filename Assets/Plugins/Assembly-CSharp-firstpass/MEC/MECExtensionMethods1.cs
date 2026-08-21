namespace MEC
{
	public static class MECExtensionMethods1
	{
		public static global::MEC.CoroutineHandle RunCoroutine(this global::System.Collections.Generic.IEnumerator<float> coroutine)
		{
			return global::MEC.Timing.RunCoroutine(coroutine);
		}

		public static global::MEC.CoroutineHandle RunCoroutine(this global::System.Collections.Generic.IEnumerator<float> coroutine, global::UnityEngine.GameObject gameObj)
		{
			return global::MEC.Timing.RunCoroutine(coroutine, gameObj);
		}

		public static global::MEC.CoroutineHandle RunCoroutine(this global::System.Collections.Generic.IEnumerator<float> coroutine, int layer)
		{
			return global::MEC.Timing.RunCoroutine(coroutine, layer);
		}

		public static global::MEC.CoroutineHandle RunCoroutine(this global::System.Collections.Generic.IEnumerator<float> coroutine, string tag)
		{
			return global::MEC.Timing.RunCoroutine(coroutine, tag);
		}

		public static global::MEC.CoroutineHandle RunCoroutine(this global::System.Collections.Generic.IEnumerator<float> coroutine, global::UnityEngine.GameObject gameObj, string tag)
		{
			return global::MEC.Timing.RunCoroutine(coroutine, gameObj, tag);
		}

		public static global::MEC.CoroutineHandle RunCoroutine(this global::System.Collections.Generic.IEnumerator<float> coroutine, int layer, string tag)
		{
			return global::MEC.Timing.RunCoroutine(coroutine, layer, tag);
		}

		public static global::MEC.CoroutineHandle RunCoroutine(this global::System.Collections.Generic.IEnumerator<float> coroutine, global::MEC.Segment segment)
		{
			return global::MEC.Timing.RunCoroutine(coroutine, segment);
		}

		public static global::MEC.CoroutineHandle RunCoroutine(this global::System.Collections.Generic.IEnumerator<float> coroutine, global::MEC.Segment segment, global::UnityEngine.GameObject gameObj)
		{
			return global::MEC.Timing.RunCoroutine(coroutine, segment, gameObj);
		}

		public static global::MEC.CoroutineHandle RunCoroutine(this global::System.Collections.Generic.IEnumerator<float> coroutine, global::MEC.Segment segment, int layer)
		{
			return global::MEC.Timing.RunCoroutine(coroutine, segment, layer);
		}

		public static global::MEC.CoroutineHandle RunCoroutine(this global::System.Collections.Generic.IEnumerator<float> coroutine, global::MEC.Segment segment, string tag)
		{
			return global::MEC.Timing.RunCoroutine(coroutine, segment, tag);
		}

		public static global::MEC.CoroutineHandle RunCoroutine(this global::System.Collections.Generic.IEnumerator<float> coroutine, global::MEC.Segment segment, global::UnityEngine.GameObject gameObj, string tag)
		{
			return global::MEC.Timing.RunCoroutine(coroutine, segment, gameObj, tag);
		}

		public static global::MEC.CoroutineHandle RunCoroutine(this global::System.Collections.Generic.IEnumerator<float> coroutine, global::MEC.Segment segment, int layer, string tag)
		{
			return global::MEC.Timing.RunCoroutine(coroutine, segment, layer, tag);
		}

		public static global::MEC.CoroutineHandle RunCoroutineSingleton(this global::System.Collections.Generic.IEnumerator<float> coroutine, global::MEC.CoroutineHandle handle, global::MEC.SingletonBehavior behaviorOnCollision)
		{
			return global::MEC.Timing.RunCoroutineSingleton(coroutine, handle, behaviorOnCollision);
		}

		public static global::MEC.CoroutineHandle RunCoroutineSingleton(this global::System.Collections.Generic.IEnumerator<float> coroutine, global::UnityEngine.GameObject gameObj, global::MEC.SingletonBehavior behaviorOnCollision)
		{
			if (!(gameObj == null))
			{
				return global::MEC.Timing.RunCoroutineSingleton(coroutine, gameObj.GetInstanceID(), behaviorOnCollision);
			}
			return global::MEC.Timing.RunCoroutine(coroutine);
		}

		public static global::MEC.CoroutineHandle RunCoroutineSingleton(this global::System.Collections.Generic.IEnumerator<float> coroutine, int layer, global::MEC.SingletonBehavior behaviorOnCollision)
		{
			return global::MEC.Timing.RunCoroutineSingleton(coroutine, layer, behaviorOnCollision);
		}

		public static global::MEC.CoroutineHandle RunCoroutineSingleton(this global::System.Collections.Generic.IEnumerator<float> coroutine, string tag, global::MEC.SingletonBehavior behaviorOnCollision)
		{
			return global::MEC.Timing.RunCoroutineSingleton(coroutine, tag, behaviorOnCollision);
		}

		public static global::MEC.CoroutineHandle RunCoroutineSingleton(this global::System.Collections.Generic.IEnumerator<float> coroutine, global::UnityEngine.GameObject gameObj, string tag, global::MEC.SingletonBehavior behaviorOnCollision)
		{
			if (!(gameObj == null))
			{
				return global::MEC.Timing.RunCoroutineSingleton(coroutine, gameObj.GetInstanceID(), tag, behaviorOnCollision);
			}
			return global::MEC.Timing.RunCoroutineSingleton(coroutine, tag, behaviorOnCollision);
		}

		public static global::MEC.CoroutineHandle RunCoroutineSingleton(this global::System.Collections.Generic.IEnumerator<float> coroutine, int layer, string tag, global::MEC.SingletonBehavior behaviorOnCollision)
		{
			return global::MEC.Timing.RunCoroutineSingleton(coroutine, layer, tag, behaviorOnCollision);
		}

		public static global::MEC.CoroutineHandle RunCoroutineSingleton(this global::System.Collections.Generic.IEnumerator<float> coroutine, global::MEC.CoroutineHandle handle, global::MEC.Segment segment, global::MEC.SingletonBehavior behaviorOnCollision)
		{
			return global::MEC.Timing.RunCoroutineSingleton(coroutine, handle, segment, behaviorOnCollision);
		}

		public static global::MEC.CoroutineHandle RunCoroutineSingleton(this global::System.Collections.Generic.IEnumerator<float> coroutine, global::MEC.Segment segment, global::UnityEngine.GameObject gameObj, global::MEC.SingletonBehavior behaviorOnCollision)
		{
			if (!(gameObj == null))
			{
				return global::MEC.Timing.RunCoroutineSingleton(coroutine, segment, gameObj.GetInstanceID(), behaviorOnCollision);
			}
			return global::MEC.Timing.RunCoroutine(coroutine, segment);
		}

		public static global::MEC.CoroutineHandle RunCoroutineSingleton(this global::System.Collections.Generic.IEnumerator<float> coroutine, global::MEC.Segment segment, int layer, global::MEC.SingletonBehavior behaviorOnCollision)
		{
			return global::MEC.Timing.RunCoroutineSingleton(coroutine, segment, layer, behaviorOnCollision);
		}

		public static global::MEC.CoroutineHandle RunCoroutineSingleton(this global::System.Collections.Generic.IEnumerator<float> coroutine, global::MEC.Segment segment, string tag, global::MEC.SingletonBehavior behaviorOnCollision)
		{
			return global::MEC.Timing.RunCoroutineSingleton(coroutine, segment, tag, behaviorOnCollision);
		}

		public static global::MEC.CoroutineHandle RunCoroutineSingleton(this global::System.Collections.Generic.IEnumerator<float> coroutine, global::MEC.Segment segment, global::UnityEngine.GameObject gameObj, string tag, global::MEC.SingletonBehavior behaviorOnCollision)
		{
			if (!(gameObj == null))
			{
				return global::MEC.Timing.RunCoroutineSingleton(coroutine, segment, gameObj.GetInstanceID(), tag, behaviorOnCollision);
			}
			return global::MEC.Timing.RunCoroutineSingleton(coroutine, segment, tag, behaviorOnCollision);
		}

		public static global::MEC.CoroutineHandle RunCoroutineSingleton(this global::System.Collections.Generic.IEnumerator<float> coroutine, global::MEC.Segment segment, int layer, string tag, global::MEC.SingletonBehavior behaviorOnCollision)
		{
			return global::MEC.Timing.RunCoroutineSingleton(coroutine, segment, layer, tag, behaviorOnCollision);
		}

		public static float WaitUntilDone(this global::System.Collections.Generic.IEnumerator<float> newCoroutine)
		{
			return global::MEC.Timing.WaitUntilDone(newCoroutine);
		}

		public static float WaitUntilDone(this global::System.Collections.Generic.IEnumerator<float> newCoroutine, string tag)
		{
			return global::MEC.Timing.WaitUntilDone(newCoroutine, tag);
		}

		public static float WaitUntilDone(this global::System.Collections.Generic.IEnumerator<float> newCoroutine, int layer)
		{
			return global::MEC.Timing.WaitUntilDone(newCoroutine, layer);
		}

		public static float WaitUntilDone(this global::System.Collections.Generic.IEnumerator<float> newCoroutine, int layer, string tag)
		{
			return global::MEC.Timing.WaitUntilDone(newCoroutine, layer, tag);
		}

		public static float WaitUntilDone(this global::System.Collections.Generic.IEnumerator<float> newCoroutine, global::MEC.Segment segment)
		{
			return global::MEC.Timing.WaitUntilDone(newCoroutine, segment);
		}

		public static float WaitUntilDone(this global::System.Collections.Generic.IEnumerator<float> newCoroutine, global::MEC.Segment segment, string tag)
		{
			return global::MEC.Timing.WaitUntilDone(newCoroutine, segment, tag);
		}

		public static float WaitUntilDone(this global::System.Collections.Generic.IEnumerator<float> newCoroutine, global::MEC.Segment segment, int layer)
		{
			return global::MEC.Timing.WaitUntilDone(newCoroutine, segment, layer);
		}

		public static float WaitUntilDone(this global::System.Collections.Generic.IEnumerator<float> newCoroutine, global::MEC.Segment segment, int layer, string tag)
		{
			return global::MEC.Timing.WaitUntilDone(newCoroutine, segment, layer, tag);
		}
	}
}
