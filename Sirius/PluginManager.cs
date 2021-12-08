using System.Reflection;

namespace Sirius;

internal class PluginManager<T>
{
    private readonly DirectoryInfo _dependenciesDirectory;

    public PluginManager()
    {
        _dependenciesDirectory                  =  Directory.CreateDirectory("Dependencies");
        AppDomain.CurrentDomain.AssemblyResolve += Assembly_Resolve;
    }

    public T? InstallPlugin(string path)
    {
        var pluginAssembly = LoadPlugin(path);
        var plugin         = CreatePlugin(pluginAssembly);

        return plugin;
    }


    private Assembly LoadPlugin(string path)
    {
        var fileData = File.ReadAllBytes(path);
        var assembly = Assembly.Load(fileData);
        CopyDependencies(path, assembly);

        return assembly;
    }

    private static T? CreatePlugin(Assembly assembly)
    {
        foreach (var type in assembly.GetTypes())
            if (typeof(T).IsAssignableFrom(type))
                if (Activator.CreateInstance(type) is T result)
                    return result;

        return default;
    }

    private void CopyDependencies(string path, Assembly assembly)
    {
        var references = assembly.GetReferencedAssemblies();
        var fileReferences = new Dictionary<string, AssemblyName>(references.Length);
        foreach (var reference in references) fileReferences[reference.Name!] = reference;

        var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Select(a => a.GetName().Name)
            .ToArray();

        var sourceDirectory   = new DirectoryInfo(Path.GetDirectoryName(path)!);
        var missingAssemblies = fileReferences.Keys.Except(loadedAssemblies);
        foreach (var missingAssembly in missingAssemblies)
        {
            if (missingAssembly == null) continue;

            var assemblyName   = fileReferences[missingAssembly].FullName;
            var dependencyFile = GetDependencyFile(_dependenciesDirectory, assemblyName);
            if (dependencyFile != null) continue;

            dependencyFile = GetDependencyFile(sourceDirectory, assemblyName);
            if (dependencyFile == null) continue;

            var installDependencyPath = Path.Combine(
                _dependenciesDirectory.FullName, dependencyFile.Name);

            File.Copy(dependencyFile.FullName, installDependencyPath, true);
        }
    }

    private static FileSystemInfo? GetDependencyFile(DirectoryInfo directory, string dependencyName)
    {
        FileSystemInfo?[] libraries = directory.GetFileSystemInfos("*.dll");
        return (from library in libraries
            let libraryName = AssemblyName.GetAssemblyName(library.FullName).FullName
            where libraryName == dependencyName
            select library).FirstOrDefault();
    }

    private Assembly Assembly_Resolve(object? sender, ResolveEventArgs args)
    {
        var dependency = GetDependencyFile(_dependenciesDirectory, args.Name);
        return Assembly.Load(File.ReadAllBytes(dependency?.FullName!));
    }
}