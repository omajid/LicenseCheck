namespace LicenseCheck
{
    public class LicenseCheckResult
    {
        public LicenseType License { get; set; }
        public FilePath File { get; set; }
        public FileType IdentifiedType { get; set; }
        public string OptionalDetails { get; set; }
    }
}
