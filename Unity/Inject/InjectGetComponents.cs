namespace NotFluffy.NoFluffDI.Unity
{
    public sealed class InjectGetComponents : MonoInstaller
    {
        public override void InstallBindings(IContainerBuilder builder)
        {
            foreach (var injectable in GetComponents<IInjectable>()) 
                builder.AddInjectable(injectable);
        }
    }
}
