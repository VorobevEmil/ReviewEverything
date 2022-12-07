namespace ReviewEverything.Server.Installers
{
    public interface IInstaller
    {
        void InstallerServices(WebApplicationBuilder builder);
    }
}