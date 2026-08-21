namespace RemoteAdmin.Interfaces
{
	public interface ICustomRADisplay
	{
		string DisplayName { get; }

		bool CanBeDisplayed { get; }
	}
}
