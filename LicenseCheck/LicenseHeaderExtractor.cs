using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace LicenseCheck
{
    public class LicenseHeaderExtractor
    {

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
            string[] firstCommentLines = ExtractFirstInlineComment(file, commentPrefix);
            string[] firstRealCommentLines = StripShebangLine(firstCommentLines);
            string[] firstCommentContents = StripCommentCharacters(firstRealCommentLines, commentPrefix);
            string cleanedUpHeader = String.Join(" ", firstCommentContents).Trim();
            cleanedUpHeader = cleanedUpHeader.Replace("  ", " ");
            return cleanedUpHeader;
        }

        private string GetLicenseHeaderFromFirstBlockCommentHeader(FilePath file, string blockStart, string blockEnd, string optionalPrefix)
        {
            string[] firstCommentLines = ExtractFirstBlockComment(file, blockStart, blockEnd, optionalPrefix);
            string cleanedUpHeader = String.Join(" ", firstCommentLines).Trim();
            cleanedUpHeader = cleanedUpHeader.Replace("  ", " ");
            return cleanedUpHeader;
        }

        private string[] ExtractFirstInlineComment(FilePath file, string commentPrefix)
        {
            List<string> lines = new List<string>();
            using (StreamReader sr = file.Read())
            {
                string line = sr.ReadLine();
                bool readFirstLine = false;
                while((line != null) && (line.Length == 0))
                {
                    line = sr.ReadLine();
                }

                while ((line != null) && (line.StartsWith(commentPrefix)))
                {
                    if (readFirstLine &&
                        (line.Equals(commentPrefix) ||
                         line.StartsWith(commentPrefix + "=====") ||
                         line.StartsWith(commentPrefix + " ====") ||
                         line.StartsWith(commentPrefix + "*****") ||
                         line.StartsWith(commentPrefix + "-----") ||
                         line.StartsWith(commentPrefix + " ----") ||
                         line.StartsWith(commentPrefix + "+++++") ||
                         line.StartsWith(commentPrefix + "+----") ||
                         line.StartsWith(commentPrefix + "/////")))
                    {
                        break;
                    }
                    lines.Add(line);
                    line = sr.ReadLine()?.Trim();
                    readFirstLine = true;
                }
            }

            return lines.ToArray();
        }

        private string[] ExtractFirstBlockComment(FilePath file, string start, string end, string optionalPrefix)
        {
            // stdout.WriteLine("");
            List<string> lines = new List<string>();
            using (StreamReader sr = file.Read())
            {
                string line = sr.ReadLine()?.Trim();
                // look for block comment in first column (sans spaces) only
                while((line != null) && !(line.StartsWith(start)))
                {
                    line = sr.ReadLine()?.Trim();
                }

                if (line == null)
                {
                    return lines.ToArray();
                }

                if (line.Contains(end))
                {
                    // stdout.WriteLine(line);
                    // stdout.WriteLine(line.Length);
                    // stdout.WriteLine(line.IndexOf(start));
                    // stdout.WriteLine(start.Length);
                    // stdout.WriteLine(line.IndexOf(end));
                    int startPosition = line.IndexOf(start) + start.Length;
                    int endPosition = line.IndexOf(end);
                    int length = endPosition - startPosition;
                    line = line.Substring(startPosition, length);
                    lines.Add(line);
                    return lines.ToArray();
                }
                else
                {
                    lines.Add(line.Substring(line.IndexOf(start)+start.Length));
                }

                line = sr.ReadLine()?.Trim();
                while ((line != null))
                {
                    bool foundEnd = false;
                    if (line.Contains(end))
                    {
                        foundEnd = true;
                        line = line.Substring(0, line.IndexOf(end));
                    }

                    if (optionalPrefix != null && line.StartsWith(optionalPrefix))
                    {
                        line = line.Substring(optionalPrefix.Length);
                    }
                    lines.Add(line);
                    if (foundEnd) break;
                    line = sr.ReadLine()?.Trim();
                }
            }

            return lines.ToArray();
        }

        private string[] StripShebangLine(string[] lines)
        {
            List<string> result = new List<string>();
            foreach (var line in lines)
            {
                if (line.StartsWith("#!"))
                {
                    continue;
                }
                result.Add(line);
            }
            return result.ToArray();
        }

        private string[] StripCommentCharacters(string[] lines, string commentPrefix)
        {
            string[] result = new string[lines.Length];
            for (int i = 0; i < lines.Length; i++)
            {
                string temp = lines[i];
                if (!temp.StartsWith(commentPrefix))
                {
                    Debug.Assert(false, "Comment doesnt start with comment char!");
                }
                temp = temp.Remove(0, commentPrefix.Length).Trim();
                while (temp.StartsWith(commentPrefix))
                {
                    temp = temp.Remove(0, commentPrefix.Length).Trim();
                }
                result[i] = temp;
            }
            return result;
        }

    }
}
