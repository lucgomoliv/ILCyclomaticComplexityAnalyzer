using System.Diagnostics;
using Complexidade;

internal class Program
{
	private static void Main(string[] args)
	{
		var dllPath = string.Empty;
		var ildasmPath = string.Empty;

		if (args.Length > 0)
		{
			dllPath = args[0];
		}

		if (string.IsNullOrEmpty(dllPath))
		{
			Console.WriteLine("Provide the path of the DLL in arguments!");
			return;
		}

		foreach (var arg in args)
		{
			if (arg.StartsWith("/ildasmPath="))
			{
				ildasmPath = arg[12..];
			}
		}

		var arguments = $"{dllPath} /out=dll.il";

		Process.Start(string.IsNullOrEmpty(ildasmPath) ? @"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools\ildasm.exe" : ildasmPath, arguments).WaitForExit();

		var interpreter = new ILInterpreter("dll.il");
		interpreter.AnalyzeFile();
	}
}