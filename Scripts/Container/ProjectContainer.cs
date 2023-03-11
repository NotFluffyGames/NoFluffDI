namespace NotFluffy.NoFluffDI
{
    public static class ProjectContainer
    {
        private static IContainer instance;
        public static IContainer Instance => instance ??= new Container("ProjectContainer");
    }
}