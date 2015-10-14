using System.Reflection;
using System.Configuration;
using System.ComponentModel;
using Neurotec.Biometrics;
using BioProcessor.Properties;

namespace BioProcessor
{
    static class Data
    {
        #region Public static fields

        public static NFExtractor NFExtractor;
        public static NMatcher NMatcher;

        #endregion

        #region Public static methods

        public static void ResetSetting(string name)
        {
            Settings settings = Settings.Default;
            PropertyInfo pi = typeof(Settings).GetProperty(name);
            DefaultSettingValueAttribute dsva = (DefaultSettingValueAttribute)pi.GetCustomAttributes(typeof(DefaultSettingValueAttribute), false)[0];
            pi.SetValue(settings, TypeDescriptor.GetConverter(pi.PropertyType).ConvertFromInvariantString(dsva.Value), null);
        }

        public static void UpdateNfe()
        {
            Settings settings = Settings.Default;
            try { NFExtractor.UseQuality = settings.NfeUseQuality; }
            catch { }
            try { NFExtractor.QualityThreshold = settings.NfeQualityThreshold; }
            catch { }
            try { NFExtractor.Mode = settings.NfeMode; }
            catch { }
            try { NFExtractor.TemplateSize = settings.NfeTemplateSize; }
            catch { }
            try { NFExtractor.ReturnedImage = settings.NfeReturnedImage; }
            catch { }
            try { NFExtractor.GeneralizationThreshold = settings.NfeGeneralizationThreshold; }
            catch { }
            try { NFExtractor.GeneralizationMaximalRotation = settings.NfeGeneralizationMaximalRotation; }
            catch { }
        }

        public static void UpdateNM()
        {
            Settings settings = Settings.Default;
            try { NMatcher.FingersNfmMode = settings.NMMode; }
            catch { }
            try { NMatcher.MatchingThreshold = settings.NMMatchingThreshold; }
            catch { }
            try { NMatcher.FingersNfmMaximalRotation = settings.NMMaximalRotation; }
            catch { }
        }

        #endregion
    }
}
