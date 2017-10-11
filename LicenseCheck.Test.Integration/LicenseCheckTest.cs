using System;
using System.IO;
using Xunit;
using LicenseCheck;

namespace LicenseCheck.Test.Integration
{
    public class LicenseCheckTest
    {
        [Fact]
        public void CSharpFileWithValidLicenseHeaderWorks()
        {
            var stdout = new StringWriter();
            var stderr = new StringWriter();

            LicenseCheck.Program.FindSourcesAndCheckLicenseHeaders(new string[] {"../../../FileWithValidLicenseHeader.cs"}, stdout, stderr);

            Assert.Equal("", stdout.ToString());
            Assert.Equal("", stderr.ToString());
        }

        [Fact]
        public void JavascriptFileWithValidLicenseHeaderWorks()
        {
            var stdout = new StringWriter();
            var stderr = new StringWriter();

            LicenseCheck.Program.FindSourcesAndCheckLicenseHeaders(new string[] {"../../../FileWithValidLicenseHeader.js" }, stdout, stderr);

            Assert.Equal("", stdout.ToString());
            Assert.Equal("", stderr.ToString());
        }

        [Fact]
        public void VisualBasicFileWithMissingLicenseHeaderComplains()
        {
            var stdout = new StringWriter();
            var stderr = new StringWriter();

            LicenseCheck.Program.FindSourcesAndCheckLicenseHeaders(new string[] {"../../../FileWithMissingLicenseHeader.vb" }, stdout, stderr);

            Assert.Equal("Missing license header in ../../../FileWithMissingLicenseHeader.vb\n", stdout.ToString());
            Assert.Equal("", stderr.ToString());
        }

        [Fact]
        public void CSharpFileWithBOMAndValidHeaderWorks()
        {
            var stdout = new StringWriter();
            var stderr = new StringWriter();

            LicenseCheck.Program.FindSourcesAndCheckLicenseHeaders(new string[] {"../../../FileWithBOMAndValidHeader.cs" }, stdout, stderr);

            Assert.Equal("", stdout.ToString());
            Assert.Equal("", stderr.ToString());
        }
    }
}
