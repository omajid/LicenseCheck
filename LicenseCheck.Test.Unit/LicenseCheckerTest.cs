using LicenseCheck;
using System;
using Xunit;

namespace LicenseCheck.Test.Unit
{
    public class LicenseCheckerTest
    {
        [Fact]
        public void NoCommentIsNotALicense()
        {
            var licenseString = "";
            var result = new LicenseChecker().Check(new FilePath("ignore"), FileType.Unknown, licenseString);
            Assert.Equal(LicenseType.NoLicense, result.License);
        }

        [Fact]
        public void AutoGeneratedHeaderIsNotALicense()
        {
            var licenseString = "<auto-generated />";
            var result = new LicenseChecker().Check(new FilePath("ignore"), FileType.Unknown, licenseString);
            Assert.Equal(LicenseType.NoLicense, result.License);
        }

        [Fact]
        public void SimpleCopyrightIsProbablyAnOpenSourceLicense()
        {
            var licenseString = "Copyright (c) Microsoft. All rights reserved.";
            var result = new LicenseChecker().Check(new FilePath("ignore"), FileType.Unknown, licenseString);
            Assert.Equal(LicenseType.ValidLicense, result.License);
        }

        [Fact]
        public void SimpleCopyrightFollowedByExtraWordsIsAnUnknownLicense()
        {
            var licenseString = "Copyright (c) Microsoft. All rights reserved. You may only use this under the following terms: your firstborn is ours";
            var result = new LicenseChecker().Check(new FilePath("ignore"), FileType.Unknown, licenseString);
            Assert.Equal(LicenseType.UnknownLicense, result.License);
        }

        [Fact]
        public void TodoIsNotALicense()
        {
            var licenseString = "TODO blah for Microsoft";
            var result = new LicenseChecker().Check(new FilePath("ignore"), FileType.Unknown, licenseString);
            Assert.Equal(LicenseType.NoLicense, result.License);
        }
    }

}
