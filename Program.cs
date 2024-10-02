using System.Diagnostics;
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
			var argumentValues = new Dictionary<ConsoleOption, string>();

			if (args.FirstOrDefault() == "/help")
			{
				ShowHelp();
				return;
			}

			foreach (var arg in args)
			{
				if (arg.StartsWith('/'))
				{
					foreach (var option in options.Keys)
					{
						var optionDescription = option.GetDescription();

						if (arg.StartsWith(optionDescription))
						{
							argumentValues[option] = arg[(optionDescription.Length + 1)..];
						}
					}
				}
			}

			if (string.IsNullOrEmpty(filePath))
			{
				Console.WriteLine("Provide the path of the file!");
				return;
			}

			if (!argumentValues.TryGetValue(ConsoleOption.IldasmPath, out string? ildasmPath))
			{
				Console.WriteLine("Provide the ildasm path via '/ildasmPath'!");
				return;
			}

			argumentValues.TryGetValue(ConsoleOption.Output, out string? outputFolder);

			outputFolder = outputFolder != null ? outputFolder + "\\" : string.Empty;

			Directory.CreateDirectory(outputFolder);

			argumentValues.TryGetValue(ConsoleOption.Item, out string? item);

			item = item != null ? "/item=\"" + item + "\"" : string.Empty;

			var ildasmArguments = $"{filePath} /out={outputFolder}code.il {item} /text";

			try
			{
				Process.Start(ildasmPath, ildasmArguments).WaitForExit();

				var interpreter = new ILInterpreter(outputFolder);
				interpreter.AnalyzeFile();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
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