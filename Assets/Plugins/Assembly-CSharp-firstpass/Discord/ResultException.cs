namespace Discord
{
	public class ResultException : global::System.Exception
	{
		public readonly global::Discord.Result Result;

		public ResultException(global::Discord.Result result)
			: base(result.ToString())
		{
		}
	}
}
