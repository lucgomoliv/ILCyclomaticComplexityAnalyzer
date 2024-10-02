using System.Text;

namespace Complexity
{
	public class Program
	{
		private static readonly Dictionary<ConsoleOption, string> options =
			new()
			{
				{ ConsoleOption.IldasmPath, "\t\t\t\tPath to 'ildasm.exe' file." },
				{ ConsoleOption.Item, "=<class>[::<method>[(<sig>)]\tDisassemble the specified item only." },
				{ ConsoleOption.Output, "\t\t\t\tPath to output folder of the IL and analysis results." }
			};

		private static void Main(string[] args)
		{
			var filePath = args.FirstOrDefault(arg => !arg.StartsWith('/'));
			var arguments = ParseArguments(args);

			if (!ValidateArguments(filePath, arguments!))
				return;

			try
			{
				var outputFile = IldasmRunner.Run(arguments[ConsoleOption.IldasmPath], filePath!, arguments[ConsoleOption.Output], arguments[ConsoleOption.Item]);

				ILInterpreter.AnalyzeFile(outputFile);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
		}

		private static bool ValidateArguments(string? filePath, Dictionary<ConsoleOption, string?> arguments)
		{
			if (arguments.ContainsKey(ConsoleOption.Help))
			{
				ShowHelp();
				return false;
			}

			if (string.IsNullOrEmpty(filePath))
			{
				Console.WriteLine("Provide the file path to analyze!");
				return false;
			}

			if (!arguments.TryGetValue(ConsoleOption.IldasmPath, out string? ildasmPath))
			{
				Console.WriteLine("Provide the ildasm path via '/ildasmPath'!");
				return false;
			}

			arguments.TryAdd(ConsoleOption.Output, null);
			arguments.TryAdd(ConsoleOption.Item, null);

			return true;
		}

		private static Dictionary<ConsoleOption, string> ParseArguments(string[] args)
		{
			var argumentValues = new Dictionary<ConsoleOption, string>();

			foreach (var arg in args)
			{
				if (!arg.StartsWith('/')) continue;

				if (arg.StartsWith("/help"))
					argumentValues[ConsoleOption.Help] = null!;

				foreach (var option in options.Keys)
				{
					var optionDescription = option.GetDescription();

					if (!arg.StartsWith(optionDescription) || arg.Length <= optionDescription.Length + 1)
						continue;

					argumentValues[option] = arg[(optionDescription.Length + 1)..];
				}
			}

			return argumentValues;
		}

		private static void ShowHelp()
		{
			var sb = new StringBuilder();

			sb.AppendLine("Usage: complexity.exe /ildasmPath=\"<ildasmPath>\" <filePath>\n");

			foreach (var option in options)
			{
				sb.Append(option.Key.GetDescription());
				sb.AppendLine(option.Value);
			}

			Console.WriteLine(sb.ToString());
		}
	}
}