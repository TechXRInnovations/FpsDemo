namespace TechXR.Core.Sense
{
    internal interface ILicense
    {
        /// <summary>
        /// To check if the given DevKey is valid key or not
        /// </summary>
        /// <param name="licenseKey"></param>
        /// <returns></returns>
        bool VerifyLicenseKey(string licenseKey);
    }
}
