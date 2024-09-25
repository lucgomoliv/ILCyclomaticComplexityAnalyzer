using System.Text;

namespace Complexidade
{
	public class ILInterpreter(string pathIl)
	{
		private string pathIl = pathIl;
		private List<Method> methods = [];
		private readonly IList<string> complexityOperations = [
			"beq",
			"bge",
			"bgt",
			"ble",
			"blt",
			"bne",
			"brtrue",
			"brfalse"
		];

		public void AnalisarArquivo()
		{
			SepararMetodos();

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

			File.WriteAllText("teste.txt", sb.ToString());
		}

		private void SepararMetodos()
		{
			using (var stream = AbrirArquivo())
			{
				string line;
				var node = 0;
				const string instance = "instance";
				const string beforefieldinit = "beforefieldinit";
				var classe = string.Empty;
				var filtrarMaiorQue10 = true;

				while ((line = stream.ReadLine()?.Trim()!) != null)
				{
					if (line.StartsWith(".class") && !line.Contains("nested"))
					{
						var assinatura = string.Empty;

						while (!line.StartsWith('{'))
						{
							assinatura += line + " ";
							line = stream.ReadLine()!.Trim();
						}

						classe = assinatura[(assinatura.IndexOf(beforefieldinit) + beforefieldinit.Length + 1)..];
						classe = classe[..classe.IndexOf(' ')];
					}

					if (line.StartsWith(".method"))
					{
						var assinatura = string.Empty;

						while (!line.StartsWith('{'))
						{
							assinatura += line + " ";
							line = stream.ReadLine()!.Trim();
						}

						var comeco = assinatura.Substring(assinatura.IndexOf(instance) + instance.Length);

						methods.Add(new Method()
						{
							Classe = classe,
							Name = comeco[..comeco.IndexOf('(')],
						});

						while (true)
						{
							line = stream.ReadLine()!.Trim();

							if (line.StartsWith('}'))
							{
								if (node == 0)
								{
									if (filtrarMaiorQue10 && methods.Last().CyclomaticComplexity <= 10)
									{
										methods.Remove(methods.Last());
									}

									break;
								}

								node -= 1;
							}

							if (line.StartsWith('{'))
							{
								node += 1;
							}

							if (VerificarSeDeveIncremenetarComplexidade(line))
							{
								if (methods.Last().Name.Contains("CriteriosInformadosEstaoDeAcordoComConfiguracao"))
								{
									Console.WriteLine(line);
								}

								methods.Last().CyclomaticComplexity += 1;
							}
						}
					}
				}
			}
		}

		private bool VerificarSeDeveIncremenetarComplexidade(string line)
		{
			var operacao = PegarOperacao(line);

			return complexityOperations.Any(operacao.Contains);
		}

		private string PegarOperacao(string line)
		{
			if (line.StartsWith("IL_") && line.Contains(':'))
			{
				return line.Split(' ')[2];
			}

			return string.Empty;
		}

		private StreamReader AbrirArquivo()
		{
			return File.OpenText(pathIl);
		}
	}
}