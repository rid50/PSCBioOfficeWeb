using System;
using System.Globalization;
using System.Reflection;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.IO;

#if !N_PRODUCT_HAS_NO_LICENSES
using Neurotec.Licensing;
#endif

namespace SplitFingerTemplates
{
	public static class Helpers
	{
		private const string LicensesConfiguration = "NLicenses.cfg";

		public static void PrintTutorialHeader()
		{
			string description = ((AssemblyDescriptionAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false)[0]).Description;
			string version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
			string copyright = ((AssemblyCopyrightAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false)[0]).Copyright;
			Console.WriteLine(GetAssemblyName());
			Console.WriteLine("");
			Console.WriteLine("{0} (Version: {1})", description, version);
			Console.WriteLine(copyright.Replace("?", "(C)"));
			Console.WriteLine();
		}

		private static Dictionary<string, string> _licenseCfg;

		// Load configuration file 
		public static Dictionary<string, string> LoadConfigurations(string filename)
		{
			Dictionary<string, string> config = new Dictionary<string, string>();

			string[] lines = File.ReadAllLines(filename);

			foreach (string line in lines)
			{
				if (line.TrimStart().StartsWith("#") || line.Trim() == string.Empty)
				{
					//ignore comment or empty line
					continue;
				}

				string[] values = line.Split('=');
				string value = (values.Length > 1) ? values[1] : "";
				if (values.Length > 0)
				{
					// values[0] is the key
					config.Add(values[0].Trim(), value.Trim());
				}
			}

			return config;
		}

		// Load licenses configuration file with names of licenses to obtain
		private static void LoadLicenseConfiguration()
		{
			string path = Path.Combine(GetAssemblyPath(), LicensesConfiguration);

			_licenseCfg = LoadConfigurations(path);
		}

	#if !N_PRODUCT_HAS_NO_LICENSES

		public static void ObtainLicenses(string license)
		{
			ObtainLicenses(new string[] { license });
		}

		public static void ObtainLicenses(IList<string> licenses)
		{
			int i;

			if (_licenseCfg == null)
			{
				LoadLicenseConfiguration();
			}

			for (i = 0; i < licenses.Count; i++)
			{
				if (_licenseCfg.ContainsKey(licenses[i]))
					licenses[i] = _licenseCfg[licenses[i]];
				else
					licenses[i] = string.Empty;
			}

			// Remove duplicates
			for (i = 0; i < licenses.Count - 1; i++)
			{
				if (licenses[i] == string.Empty)
				{
					continue;
				}

				int j;
				for (j = i + 1; j < licenses.Count; j++)
				{
					if (licenses[i] == licenses[j])
					{
						licenses[j] = string.Empty;
					}
				}
			}

			string licenseServer = _licenseCfg["Address"];
			string licensePort = _licenseCfg["Port"];

			for (i = 0; i < licenses.Count; i++)
			{
				if (licenses[i] == string.Empty)
					continue;

				try
				{
					bool available = NLicense.Obtain(licenseServer, licensePort, licenses[i]);
					
					if (!available)
					{
						throw new Exception(string.Format("license for {0} was not obtained", licenses[i]));
					}
				}
				catch (Exception ex)
				{
					throw new Exception(string.Format("Error while obtaining license. Please check if Neurotec Activation Service is running. Details: {0}", ex));
				}
			}
		}

		public static void ReleaseLicenses(IList<string> licenses)
		{
			for (int i = 0; i < licenses.Count; i++)
			{
				if (licenses[i] == string.Empty)
					continue;

				try
				{
					NLicense.Release(licenses[i]);
				}
				catch (Exception)
				{
				}
			}
		}

	#endif // !N_PRODUCT_HAS_NO_LICENSES

		public static string GetAssemblyName()
		{
			return Assembly.GetExecutingAssembly().GetName().Name;
		}

		public static string GetAssemblyPath()
		{
			return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
		}

		public static int QualityToPercent(byte value)
		{
			return (2 * value * 100 + 255) / (2 * 255);
		}

		public static byte QualityFromPercent(int value)
		{
			return (byte)((2 * value * 255 + 100) / (2 * 100));
		}

		public static string MatchingThresholdToString(int value)
		{
			double p = -value / 12.0;
			return string.Format(string.Format("{{0:P{0}}}", Math.Max(0, (int)Math.Ceiling(-p) - 2)), Math.Pow(10, p));
		}

		public static int MatchingThresholdFromString(string value)
		{
			double p = Math.Log10(Math.Max(double.Epsilon, Math.Min(1,
				double.Parse(value.Replace(CultureInfo.CurrentCulture.NumberFormat.PercentSymbol, "")) / 100)));
			return Math.Max(0, (int)Math.Round(-12 * p));
		}

		public static int MaximalRotationToDegrees(byte value)
		{
			return (2 * value * 360 + 256) / (2 * 256);
		}

		public static byte MaximalRotationFromDegrees(int value)
		{
			return (byte)((2 * value * 256 + 360) / (2 * 360));
		}

		public static string GetUserLocalDataDir(string productName)
		{
			string localDataDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
			localDataDir = Path.Combine(localDataDir, "Neurotechnology");
			if (!Directory.Exists(localDataDir))
			{
				Directory.CreateDirectory(localDataDir);
			}
			localDataDir = Path.Combine(localDataDir, productName);
			if (!Directory.Exists(localDataDir))
			{
				Directory.CreateDirectory(localDataDir);
			}

			return localDataDir;
		}
	}
}
