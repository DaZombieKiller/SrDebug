using System.IO;
using System.Linq;
using System.Collections.Generic;

using Mono.Cecil;
using Mono.Cecil.Cil;

internal class Program
{
    private const string ContentPath = "SrDebug-Content";
    private static string _dataPath;
    private static Dictionary<string, ModuleDefinition> _modules;
    private static Dictionary<string, string> _modulePaths;
    private static DefaultAssemblyResolver _resolver;

    private static readonly string[] PotentialDataPaths =
    {
        "Content/Resources/Data",
        "Resources/Data",
        "Data",
        "SlimeRancher_Data",
    };
    
    /// <summary>The program entry point.</summary>
    private static void Main()
    {
        // search for the data path
        foreach (var path in PotentialDataPaths)
        {
            if (!Directory.Exists(path)) continue;
            _dataPath = path;
            break;
        }
        
        // did we find it?
        if (!Directory.Exists(_dataPath))
            throw new DirectoryNotFoundException("Could not locate the Data folder.");

        _resolver = new DefaultAssemblyResolver();
        _resolver.AddSearchDirectory($"{_dataPath}/Managed");
        _modulePaths = new Dictionary<string, string>();
        _modules = new Dictionary<string, ModuleDefinition>();
        
        // copy SrDebug assembly to managed folder
        File.Copy($"{ContentPath}/Assembly-SrDebug.dll", $"{_dataPath}/Managed/Assembly-SrDebug.dll", true);

        // load game assembly
        var targetModule = LoadModule("Assembly-CSharp");

        // load debug assembly
        var debugModule = LoadModule("Assembly-SrDebug");

        // retrieve types from the assemblies
        var targetType = targetModule.GetType("DebugDirector");
        var debugType = debugModule.GetType("SrDebugDirector");

        // clear the DebugDirector::Awake() method if it exists
        if (targetType.Methods.Any(m => m.Name == "Awake"))
            targetType.Methods.Remove(targetType.Methods.Single(m => m.Name == "Awake"));

        // inject our own Awake() method
        var method = new MethodDefinition("Awake", MethodAttributes.Public, targetModule.TypeSystem.Void);
        targetType.Methods.Add(method);
        
        // construct the IL for our method
        var il = method.Body.GetILProcessor();
        method.Body.Instructions.Add(il.Create(OpCodes.Call,
            targetModule.Import(debugType.Methods.Single(m => m.Name == "Init"))));
        method.Body.Instructions.Add(il.Create(OpCodes.Ret));
        
        // write changes
        targetModule.Write(_modulePaths["Assembly-CSharp"]);

        System.Console.WriteLine("Debug mode should now be accessible.");
        System.Console.WriteLine("Press any key to exit . . . ");
        System.Console.ReadKey();
    }

    /// <summary>Returns a module, loading it if necessary.</summary>
    /// <param name="name">The name of the module</param>
    private static ModuleDefinition LoadModule(string name)
    {
        if (_modules.ContainsKey(name)) return _modules[name];
        _modulePaths[name] = $"{_dataPath}/Managed/{name}.dll";
        _modules[name] = ModuleDefinition.ReadModule(
            new MemoryStream(File.ReadAllBytes(_modulePaths[name])),
            new ReaderParameters { AssemblyResolver = _resolver });
        return _modules[name];
    }
}
