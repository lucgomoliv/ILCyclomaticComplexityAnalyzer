using System.ComponentModel;

namespace Complexity
{
	enum ConsoleOption
	{
		[Description("/help")]
		Help,

		[Description("/ildasmPath")]
		IldasmPath,

		[Description("/item")]
		Item,

		[Description("/outputPath")]
		Output
	}

	public static class EnumUtils
	{
		public static string GetDescription(this Enum value)
		{
			return (value.GetType().GetField(value.ToString())?.GetCustomAttributes(typeof(DescriptionAttribute), false)?.FirstOrDefault() as DescriptionAttribute)?.Description!;
		}
	}
}
