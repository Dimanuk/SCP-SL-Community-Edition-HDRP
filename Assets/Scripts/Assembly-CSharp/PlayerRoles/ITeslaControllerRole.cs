namespace PlayerRoles
{
    public interface ITeslaControllerRole
    {
        bool CanActivateIdle { get; }

        bool CanActivateShock { get; }
    }
}
