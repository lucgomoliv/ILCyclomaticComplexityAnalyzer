using System.Text;

namespace Complexity
{
	public static class ILInterpreter
	{
		private const string RESULTS_FILE_NAME = "results.txt";

		private static readonly List<Method> methods = [];
		private static readonly IList<string> complexityOperations = [
			"beq",
			"bge",
			"bgt",
			"ble",
			"blt",
			"bne",
			"brtrue",
			"brfalse"
		];

		public static void AnalyzeFile(string filePath)
		{
			using (var stream = OpenFile(filePath))
			{
				SplitMethods(stream);
			}

			GenerateResultsFile(Path.GetDirectoryName(filePath)!);
		}

		private static void GenerateResultsFile(string folderPath)
		{
			var sb = new StringBuilder();

			foreach (var classes in methods.GroupBy(x => x.Classe))
			{
				sb.AppendLine(classes.Key);

				foreach (var method in classes)
				{
					sb.Append('\t');
					sb.Append(method.Name);
					sb.Append('\t');
					sb.AppendLine(method.CyclomaticComplexity.ToString());
				}
			}

			File.WriteAllText(Path.Combine(folderPath, RESULTS_FILE_NAME), sb.ToString());
		}

		private static void SplitMethods(StreamReader stream)
		{
			string line;
			var node = 0;
			const string instance = "instance";
			const string beforefieldinit = "beforefieldinit";
			var @class = string.Empty;
			var filter = false;

			while ((line = stream.ReadLine()?.Trim()!) != null)
			{
				if (line.StartsWith(".class") && !line.Contains("nested"))
				{
					var signature = string.Empty;

					while (!line.StartsWith('{'))
					{
						signature += line + " ";
						line = stream.ReadLine()!.Trim();
					}

					@class = signature[(signature.IndexOf(beforefieldinit) + beforefieldinit.Length + 1)..];
					@class = @class[..@class.IndexOf(' ')];
				}

				if (line.StartsWith(".method"))
				{
					var signature = string.Empty;

					while (!line.StartsWith('{'))
					{
						signature += line + " ";
						line = stream.ReadLine()!.Trim();
					}

					var start = signature[(signature.IndexOf(instance) + instance.Length)..];

					methods.Add(new Method()
					{
						Classe = @class,
						Name = start[..start.IndexOf('(')],
					});

					while (true)
					{
						line = stream.ReadLine()!.Trim();

						if (line.StartsWith('}'))
						{
							if (node == 0)
							{
								if (filter && methods.Last().CyclomaticComplexity <= 10)
								{
									methods.Remove(methods.Last());
								}

								break;
							}

							node -= 1;
						}

						if (line.StartsWith('{'))
							node += 1;

						if (VerifyIfNeedsToIncreaseComplexity(line))
							methods.Last().CyclomaticComplexity += 1;
					}
				}
			}
		}

		private static bool VerifyIfNeedsToIncreaseComplexity(string line)
		{
			var operation = GetOperation(line);

			return complexityOperations.Any(operation.Contains);
		}

		private static string GetOperation(string line)
		{
			if (line.StartsWith("IL_") && line.Contains(':'))
				return line.Split(' ')[2];

			return string.Empty;
		}

		private static StreamReader OpenFile(string filePath) => File.OpenText(filePath);
	}
}