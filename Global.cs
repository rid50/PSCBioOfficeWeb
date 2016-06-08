using System;
using Neurotec.Licensing;

public static class Global
{
	//const string Components = "Biometrics.FingerExtraction,Biometrics.FingerMatching,Devices.FingerScanners,Images.WSQ";
	const string Components = "Biometrics.FingerExtractionFast,Biometrics.FingerMatchingFast,Images.WSQ";
	//const string Components = "Images.WSQ";

	static void AppInitialize()
	{

		try
		{
			foreach (string component in Components.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
			{
				NLicense.ObtainComponents("/local", "5000", component);
			}
		}
		catch (Exception ex)
		{
			while (ex.InnerException != null)
				ex = ex.InnerException;

			throw new System.ServiceModel.FaultException("Error FingersExtractor, FingersMatcher: " + ex.Message, System.ServiceModel.FaultCode.CreateSenderFaultCode("a1", "b1"));
		}
		
	}
}