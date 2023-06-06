namespace NotFluffy.NoFluffDI
{
    public delegate void Installable(IContainerBuilder builder);

    public interface IInstallable
    {
        void InstallBindings(IContainerBuilder builder);
    }
}