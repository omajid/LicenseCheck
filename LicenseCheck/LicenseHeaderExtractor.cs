using System;

namespace LicenseCheck
{
    public class LicenseHeaderExtractor
    {

        private CommentExtractor Extractor;

        public LicenseHeaderExtractor()
        {
            Extractor = new CommentExtractor();
        }

        public LicenseHeaderExtractor(CommentExtractor extractor)
        {
            Extractor = extractor;
        }

        public string Extract(FilePath file, FileType fileType)
        {
            string licenseHeader = null;
            switch (fileType)
            {
                case FileType.Assembly:
                    licenseHeader = GetLicenseHeaderFromAssemblyFile(file);
                    break;
                case FileType.CSharp:
                    licenseHeader = GetLicenseHeaderFromCSharpFile(file);
                    break;
                case FileType.C:
                    licenseHeader = GetLicenseHeaderFromCppFile(file);
                    break;
                case FileType.Css:
                    licenseHeader = GetLicenseHeaderFromCssFile(file);
                    break;
                case FileType.Dockerfile:
                    licenseHeader = GetLicenseHeaderFromDockerFile(file);
                    break;
                case FileType.ExportsFile:
                    licenseHeader = GetLicenseHeaderFromExportsFile(file);
                    break;
                case FileType.FSharp:
                    licenseHeader = GetLicenseHeaderFromFSharpFile(file);
                    licenseHeader = RemoveBogusFSharpLicenses(licenseHeader);
                    break;
                case FileType.JavaScript:
                    licenseHeader = GetLicenseHeaderFromJavascriptFile(file);
                    break;
                case FileType.Perl:
                    licenseHeader = GetLicenseHeaderFromPerlFile(file);
                    break;
                case FileType.Python:
                    licenseHeader = GetLicenseHeaderFromPythonFile(file);
                    break;
                case FileType.Shell:
                    licenseHeader = GetLicenseHeaderFromShellFile(file);
                    break;
                case FileType.TypeScript:
                    licenseHeader = GetLicenseHeaderFromTypescriptFile(file);
                    break;
                case FileType.VisualBasic:
                    licenseHeader = GetLicenseHeaderFromVBFile(file);
                    break;
                case FileType.Xml:
                    licenseHeader = GetLicenseHeaderFromXmlFile(file);
                    break;
                case FileType.Yacc:
                    licenseHeader = GetLicenseHeaderFromYaccFile(file);
                    break;
            }
            return licenseHeader;
        }

        private string GetLicenseHeaderFromAssemblyFile(FilePath file)
        {
            string commentPrefix = ";";
            string header = GetLicenseHeaderFromFirstInlineCommentHeaderWithoutShebang(file, commentPrefix);
            if (!String.IsNullOrEmpty(header))
            {
                return header;
            }
            commentPrefix = "//";
            header = GetLicenseHeaderFromFirstInlineCommentHeaderWithoutShebang(file, commentPrefix);
            return header;
        }

        private string GetLicenseHeaderFromCSharpFile(FilePath file)
        {
            string commentPrefix = "//";
            return GetLicenseHeaderFromFirstInlineCommentHeaderWithoutShebang(file, commentPrefix);
        }

        private string GetLicenseHeaderFromCppFile(FilePath file)
        {
            string commentPrefix = "//";
            return GetLicenseHeaderFromFirstInlineCommentHeaderWithoutShebang(file, commentPrefix);
        }

        private string GetLicenseHeaderFromCssFile(FilePath file)
        {
            return GetLicenseHeaderFromFirstBlockCommentHeader(file, "/*", "*/", "*");
        }

        private string GetLicenseHeaderFromDockerFile(FilePath file)
        {
            string commentPrefix = "#";
            return GetLicenseHeaderFromFirstInlineCommentHeaderWithoutShebang(file, commentPrefix);
        }

        private string GetLicenseHeaderFromExportsFile(FilePath file)
        {
            string commentPrefix = ";";
            return GetLicenseHeaderFromFirstInlineCommentHeaderWithoutShebang(file, commentPrefix);
        }

        private string GetLicenseHeaderFromFSharpFile(FilePath file)
        {
            string header = GetLicenseHeaderFromFirstBlockCommentHeader(file, "(*", "*)", "*");
            if (!String.IsNullOrEmpty(header))
            {
                return header;
            }
            header = GetLicenseHeaderFromFirstInlineCommentHeaderWithoutShebang(file, "//");
            return header;
        }

        private string RemoveBogusFSharpLicenses(string licenseHeader)
        {
            if (String.IsNullOrEmpty(licenseHeader))
            {
                return null;
            }

            if (licenseHeader.StartsWith("#Regression") ||
                    licenseHeader.StartsWith("#Conformance") ||
                    licenseHeader.StartsWith("#Libraries") ||
                    licenseHeader.StartsWith("#NoMT") ||
                    licenseHeader.StartsWith("#NoMono") ||
                    licenseHeader.StartsWith("#Warnings") ||
                    licenseHeader.StartsWith("[Test Strategy]") ||
                    licenseHeader.StartsWith("[<StructuralComparison(true)>]") ||
                    licenseHeader.StartsWith("[<ReferenceEquality(true)>]") ||
                    licenseHeader.StartsWith("Learn more about F#"))
            {
                return null;
            }
            return licenseHeader;
        }

        private string GetLicenseHeaderFromJavascriptFile(FilePath file)
        {
            return GetLicenseHeaderFromFirstBlockCommentHeader(file, "/*", "*/", "*");
        }

        private string GetLicenseHeaderFromPerlFile(FilePath file)
        {
            string commentPrefix = "#";
            return GetLicenseHeaderFromFirstInlineCommentHeaderWithoutShebang(file, commentPrefix);
        }

        private string GetLicenseHeaderFromPythonFile(FilePath file)
        {
            string commentPrefix = "#";
            return GetLicenseHeaderFromFirstInlineCommentHeaderWithoutShebang(file, commentPrefix);
        }

        private string GetLicenseHeaderFromShellFile(FilePath file)
        {
            string commentPrefix = "#";
            return GetLicenseHeaderFromFirstInlineCommentHeaderWithoutShebang(file, commentPrefix);
        }

        private string GetLicenseHeaderFromTypescriptFile(FilePath file)
        {
            string commentPrefix = "//";
            return GetLicenseHeaderFromFirstInlineCommentHeaderWithoutShebang(file, commentPrefix);
        }

        private string GetLicenseHeaderFromVBFile(FilePath file)
        {
            string commentPrefix = "'";
            return GetLicenseHeaderFromFirstInlineCommentHeaderWithoutShebang(file, commentPrefix);
        }

        private string GetLicenseHeaderFromXmlFile(FilePath file)
        {
            return GetLicenseHeaderFromFirstBlockCommentHeader(file, "<!--", "-->", null);
        }

        private string GetLicenseHeaderFromYaccFile(FilePath file)
        {
            string commentPrefix = "//";
            return GetLicenseHeaderFromFirstInlineCommentHeaderWithoutShebang(file, commentPrefix);
        }

        private string GetLicenseHeaderFromFirstInlineCommentHeaderWithoutShebang(FilePath file, string commentPrefix)
        {
            return Extractor.ExtractFirstInlineComment(file, commentPrefix);
        }

        private string GetLicenseHeaderFromFirstBlockCommentHeader(FilePath file, string blockStart, string blockEnd, string optionalPrefix)
        {
            return Extractor.ExtractFirstBlockComment(file, blockStart, blockEnd, optionalPrefix);
        }

    }
}
