using System.Diagnostics;

namespace Complexity
{
	public static class IldasmRunner
	{
		private const string IL_FILE_NAME = "code.il";

		public static string Run(string ildasmPath, string filePath, string? outputFolder = null, string? item = null)
		{
			var outArgument = $"/out={(!string.IsNullOrEmpty(outputFolder) ? outputFolder + "\\" : string.Empty)}{IL_FILE_NAME}";
			var itemArgument = !string.IsNullOrEmpty(item) ? $"/item=\"{item}\"" : string.Empty;
			var hasOutputFolder = !string.IsNullOrEmpty(outputFolder);

			if (hasOutputFolder)
				Directory.CreateDirectory(outputFolder!);

			Process.Start(ildasmPath, $"{filePath} /text {outArgument} {itemArgument}").WaitForExit();

			if (hasOutputFolder)
				return Path.Combine(outputFolder!, IL_FILE_NAME);

			return IL_FILE_NAME;
		}
	}
}
