using Autofac;

namespace Mecanica.Hermes.Infrastructure.Autofac;

internal class AutofacConfiguration : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(c => c.FullName!.Contains("Mecanica.Hermes"))
            .ToArray();

        builder.RegisterAssemblyTypes(assemblies)
            .Where(t => t.Name.EndsWith("Repository"))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();
    }
}