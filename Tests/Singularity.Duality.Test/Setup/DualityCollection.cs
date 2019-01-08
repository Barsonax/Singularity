using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Duality;
using Duality.Backend;
using Duality.Serialization;
using Xunit;

namespace Singularity.Duality.Test.Setup
{
	[CollectionDefinition(nameof(DualityCollection))]
	public class DualityCollection : ICollectionFixture<DualityTestFixture> { }

	public class DualityTestFixture : IDisposable
	{
		private string _oldEnvDir;
		private INativeWindow _dummyWindow;
		private TextWriterLogOutput _consoleLogOutput;

		public DualityTestFixture()
		{
			Console.WriteLine("----- Beginning Duality environment setup -----");

			// Set environment directory to Duality binary directory
			_oldEnvDir = Environment.CurrentDirectory;
			var codeBaseUri = typeof(DualityApp).Assembly.CodeBase;
			var codeBasePath = codeBaseUri.StartsWith("file:") ? codeBaseUri.Remove(0, "file:".Length) : codeBaseUri;
			codeBasePath = codeBasePath.TrimStart('/');
			Console.WriteLine("Testing Core Assembly: {0}", codeBasePath);
			Environment.CurrentDirectory = Path.GetDirectoryName(codeBasePath);

			// Add some Console logs manually for NUnit
			if (_consoleLogOutput == null)
				_consoleLogOutput = new TextWriterLogOutput(Console.Out);
			Logs.AddGlobalOutput(_consoleLogOutput);

			// Initialize Duality
			DualityApp.Init(
				DualityApp.ExecutionEnvironment.Launcher,
				DualityApp.ExecutionContext.Game,
				new DefaultAssemblyLoader(),
				null);

			// Create a dummy window, to get access to all the device contexts
			if (_dummyWindow == null)
			{
				var options = new WindowOptions
				{
					Size = new Point2(800, 600),
				};
				_dummyWindow = DualityApp.OpenWindow(options);
			}

			// Load local testing memory
			TestHelper.LocalTestMemory = Serializer.TryReadObject<TestMemory>(TestHelper.LocalTestMemoryFilePath, typeof(XmlSerializer));

			Console.WriteLine("----- Duality environment setup complete -----");
		}

		public void Dispose()
		{
			Console.WriteLine("----- Beginning Duality environment teardown -----");

			// Remove NUnit Console logs
			Logs.RemoveGlobalOutput(_consoleLogOutput);
			_consoleLogOutput = null;

			if (_dummyWindow != null)
			{
				ContentProvider.ClearContent();
				_dummyWindow.Dispose();
				_dummyWindow = null;
			}

			/*// Save local testing memory. As this uses Duality serializers, 
			// it needs to be done before terminating Duality.
			if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Passed && !System.Diagnostics.Debugger.IsAttached)
			{
				Serializer.WriteObject(TestHelper.LocalTestMemory, TestHelper.LocalTestMemoryFilePath, typeof(XmlSerializer));
			}*/

			try
			{
				DualityApp.Terminate();
			}
			catch (BackendException)
			{

			}

			Environment.CurrentDirectory = _oldEnvDir;

			Console.WriteLine("----- Duality environment teardown complete -----");
		}
	}


	public static class TestHelper
	{
		private static TestMemory _localTestMemory = null;

		private static string EmbeddedResourcesDir => Path.Combine("..", "EmbeddedResources");

		public static string LocalTestMemoryFilePath
		{
			get
			{
				string appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
				string testingDir = Path.Combine(appDataDir, "Duality", "UnitTesting");
				var testingFile = "DualityTests";
#if DEBUG
				testingFile += "Debug";
#else
				testingFile += "Release";
#endif
				testingFile += ".dat";
				return Path.Combine(testingDir, testingFile);
			}
		}
		public static TestMemory LocalTestMemory
		{
			get => _localTestMemory;
			internal set => _localTestMemory = value ?? new TestMemory();
		}

		public static string GetEmbeddedResourcePath(string resName, string resEnding)
		{
			return Path.Combine(EmbeddedResourcesDir, resName + resEnding);
		}
		public static void LogNumericTestResult(object testFixture, string testName, long resultValue, string unit)
		{
			if (!string.IsNullOrEmpty(unit)) unit = " " + unit;

		    if (!LocalTestMemory.GetValue(testFixture, testName, out List<long> lastValueList))
			{
				lastValueList = new List<long>();
			}
			lastValueList.Add(resultValue);
			if (lastValueList.Count > 10) lastValueList.RemoveAt(0);
			LocalTestMemory.SetValue(testFixture, testName, lastValueList);

			var localAverage = (long)lastValueList.Average();

			string nameStr = testFixture.GetType().Name + "." + testName;
			string newValueStr = $"{resultValue}{unit}";
			string lastValueStr = $"{localAverage}{unit}";

			double relativeChange = (resultValue - (double)localAverage) / localAverage;
			LogNumericTestResult(nameStr, newValueStr, lastValueStr, relativeChange);
		}
		public static void LogNumericTestResult(object testFixture, string testName, double resultValue, string unit)
		{
			if (!string.IsNullOrEmpty(unit)) unit = " " + unit;

		    if (!LocalTestMemory.GetValue(testFixture, testName, out List<double> lastValueList))
			{
				lastValueList = new List<double>();
			}
			lastValueList.Add(resultValue);
			if (lastValueList.Count > 10) lastValueList.RemoveAt(0);
			LocalTestMemory.SetValue(testFixture, testName, lastValueList);

			double localAverage = lastValueList.Average();

			string nameStr = testFixture.GetType().Name + "." + testName;
			string newValueStr = $"{resultValue:F}{unit}";
			string lastValueStr = $"{localAverage:F}{unit}";

			double relativeChange = (resultValue - localAverage) / localAverage;
			LogNumericTestResult(nameStr, newValueStr, lastValueStr, relativeChange);
		}
		private static void LogNumericTestResult(string nameStr, string newValueStr, string lastValueStr, double relativeChange)
		{
			if (Math.Abs(relativeChange) > 0.03)
			{
				Console.WriteLine("{0}: {2} --> {1} Changed by {3}%", nameStr.PadRight(50), newValueStr.PadRight(12), lastValueStr.PadRight(12), (int)Math.Round(100.0d * relativeChange));
			}
			else
			{
				Console.WriteLine($"{nameStr.PadRight(50)}: {newValueStr}");
			}
		}
	}

	public class TestMemory
	{
		private readonly Dictionary<string, object> _data = new Dictionary<string, object>();

		public bool SwitchValue<T>(object testFixture, string key, out T oldValue, T newValue)
		{
			bool result = GetValue(testFixture, key, out oldValue);
			SetValue(testFixture, key, newValue);
			return result;
		}
		public void SetValue<T>(object testFixture, string key, T value)
		{
			if (testFixture != null)
			{
				key = testFixture.GetType().Name + "_" + key;
			}
			_data[key] = value;
		}
		public bool GetValue<T>(object testFixture, string key, out T value)
		{
			if (testFixture != null)
			{
				key = testFixture.GetType().Name + "_" + key;
			}

		    if (_data.TryGetValue(key, out object valueObj) && valueObj is T variable)
			{
				value = variable;
				return true;
			}
			else
			{
				value = default(T);
				return false;
			}
		}
	}
}
