using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace LicenseCheck
{

    public class LicenseChecker
    {

        private static string[] LICENSES = {
            // Apache
            "Copyright (c) Microsoft. All Rights Reserved. Licensedf under the Apache License, Version 2.0. See License.txt in the project root for license information.",
            "Copyright (c) Microsoft. All Rights Reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.",
            "Copyright(c) Microsoft.All Rights Reserved.Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.",
            "Copyright (c) Microsoft Corporation. All Rights Reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.",
            "Copyright (c) .NET Foundation. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.",
            "Copyright (c) Microsoft. All Rights Reserved. Licensed under the Apache License, Version 9.0. See License.txt in the project root for license information.",

            // BSD
            "! Modernizr v2.8.3 www.modernizr.com  Copyright (c) Faruk Ates, Paul Irish, Alex Sexton Available under the BSD and MIT licenses: www.modernizr.com/license/",
            "* A JavaScript implementation of the JSON-LD API.  @author Dave Longley  BSD 3-Clause License Copyright (c) 2011-2013 Digital Bazaar, Inc. All rights reserved.",

            // MIT
            "! Bootstrap v3.3.4 (http://getbootstrap.com) Copyright 2011-2015 Twitter, Inc. Licensed under MIT (https://github.com/twbs/bootstrap/blob/master/LICENSE)",
            "! Bootstrap v3.3.7 (http://getbootstrap.com) Copyright 2011-2016 Twitter, Inc. Licensed under MIT (https://github.com/twbs/bootstrap/blob/master/LICENSE)",
            "! Bootstrap v3.3.7 (http://getbootstrap.com) Copyright 2011-2016 Twitter, Inc. Licensed under the MIT license",
            "Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. See LICENSE file in the project root for full license information",
            "Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license. See LICENSE file in the project root for full license information.",
            "Copyright(c) Microsoft Corporation.All rights reserved. Licensed under the MIT license. See LICENSE file in the project root for full license information.",
            "Copyright (c) .NET Foundation and contributors. All rights reserved. Licensed under the MIT license. See LICENSE file in the project root for full license info",
            "Copyright(c) .NET Foundation and contributors.All rights reserved. Licensed under the MIT license. See LICENSE file in the project root for full license information.",
            "Copyright (c) .NET Foundation and contributors. All rights reserved. Licensed under the MIT license. See LICENSE file in the project root for full license information",
            "! jQuery JavaScript Library v1.4.1 http://jquery.com/  Copyright 2010, John Resig  Includes Sizzle.js http://sizzlejs.com/ Copyright 2010, The Dojo Foundation",
            "! jQuery JavaScript Library v2.2.0 http://jquery.com/  Includes Sizzle.js http://sizzlejs.com/  Copyright jQuery Foundation and other contributors Released under the MIT license",
            "! jQuery JavaScript Library v2.1.3 http://jquery.com/  Includes Sizzle.js http://sizzlejs.com/  Copyright 2005, 2014 jQuery Foundation, Inc. and other contributors Released under the MIT license http://jquery.org/license",
            "! jQuery v2.1.3 | (c) 2005, 2014 jQuery Foundation, Inc. | jquery.org/license",
            "! jQuery v2.2.0 | (c) jQuery Foundation | jquery.org/license",
            "! jQuery Validation Plugin v1.13.1  http://jqueryvalidation.org/  Copyright (c) 2014 Jörn Zaefferer Released under the MIT license",
            "! jQuery Validation Plugin - v1.13.1 - 10/14/2014 http://jqueryvalidation.org/ Copyright (c) 2014 Jörn Zaefferer; Licensed MIT",
            "! jQuery Validation Plugin - v1.14.0 - 6/30/2015 http://jqueryvalidation.org/ Copyright (c) 2015 Jörn Zaefferer; Licensed MIT",
            "! matchMedia() polyfill - Test a CSS media type/query in JS. Authors & copyright (c) 2012: Scott Jehl, Paul Irish, Nicholas Zakas. Dual MIT/BSD license",
            "! jQuery Validation Plugin v1.14.0  http://jqueryvalidation.org/  Copyright (c) 2015 Jörn Zaefferer Released under the MIT license",
            "Licensed to the .NET Foundation under one or more agreements. The .NET Foundation licenses this file to you under the MIT license. See the LICENSE file in the project root for more information",
            "Licensed to the .NET Foundation under one or more agreements.--- The .NET Foundation licenses this file to you under the MIT license. See the LICENSE file in the project root for more information.",
            "Licensed to the.NET Foundation under one or more agreements. The .NET Foundation licenses this file to you under the MIT license. See the LICENSE file in the project root for more information.",
            "Licensed under the MIT license. See LICENSE file in the project root for full license information.",
            "! Respond.js v1.4.2: min/max-width media query polyfill * Copyright 2013 Scott Jehl Licensed under https://github.com/scottjehl/Respond/blob/master/LICENSE-MIT",
            "The .NET Foundation licenses this file to you under the MIT license. See the LICENSE file in the project root for more information.",

            // Unknown
            "Copyright (c) Microsoft. All rights reserved.", // TODO too generic with a StartsWith clause
            "Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.",
            "----------------------------------------------------------------------- <copyright file=\"{0}\" company=\"Microsoft\"> Copyright (c) Microsoft Corporation. All rights reserved. </copyright>",
            "------------------------------------------------------------------------------ <copyright file=\"{0}\" company=\"Microsoft\"> Copyright (c) Microsoft Corporation. All rights reserved. </copyright>",
            "------------------------------------------------------------------------------ <copyright file=\"{0}\" company=\"Microsoft\"> Copyright (c) Microsoft Corporation. All rights reserved. </copyright>",
            "<copyright file=\"{0}\" company=\"Microsoft\"> Copyright © Microsoft. All Rights Reserved. </copyright>",
            "----------------------------------------------------------------------- <copyright file=\"{0}\" company=\"Microsoft\"> Copyright © Microsoft. All Rights Reserved. </copyright>",
            "------------------------------------------------------------------------------ <copyright file=\"{0}\" company=\"Microsoft\"> Copyright © Microsoft. All Rights Reserved. </copyright>",
            "jQuery validation plug-in 1.6  http://bassistance.de/jquery-plugins/jquery-plugin-validation/ http://docs.jquery.com/Plugins/Validation  Copyright (c) 2006 - 2008 Jörn Zaefferer",
            "Licensed to the .NET Foundation under one or more agreements. See the LICENSE file in the project root for more information.",
        };

        private static string[] NOT_LICENSES =
        {
            "------------------------------------------------------------------------------ <auto-generated> This code was generated by a tool.",
            "------------------------------------------------------------------------------ This code was generated by a tool",
            "Definition of syntax model. Generated by a tool from",
            "<auto-generated />",
            "---------------------------------------------------------------------------- <autogenerated> This code was generated by a tool.",
            "This file was autogenerated by running the script in this directory",
            "------------------------------------------------------------------------------ <autogenerated> This code was generated by a tool."
        };

        public LicenseCheckResult Check(FilePath file)
        {
            string licenseHeader = null;

            string extension = file.GetExtension();
            string fileName = file.GetFileName();
            FileType fileType = FileType.Unknown;
            
            if (extension.Equals(".asm") ||
                extension.Equals(".inc") ||
                extension.Equals(".S"))
            {
                licenseHeader = GetLicenseHeaderFromAssemblyFile(file);
                fileType = FileType.Assembly;
            }
            else if (extension.Equals(".cs") ||
                     extension.Equals(".csx") ||
                     extension.Equals(".cool") ||
                     extension.Equals(".sc"))
            {
                licenseHeader = GetLicenseHeaderFromCSharpFile(file);
                fileType = FileType.CSharp;
            }
            else if (extension.Equals(".cpp") ||
                     extension.Equals(".cxx") ||
                     extension.Equals(".c") ||
                     extension.Equals(".h") ||
                     extension.Equals(".h.in") ||
                     extension.Equals(".hpp") ||
                     extension.Equals(".S") ||
                     extension.Equals(".rc") ||
                     extension.Equals(".idl") ||
                     extension.Equals(".def") ||
                     extension.Equals(".inl"))
            {
                licenseHeader = GetLicenseHeaderFromCppFile(file);
                fileType = FileType.C;
            }
            else if (extension.Equals(".css") ||
                     fileName.EndsWith(".css.min"))
            {
                licenseHeader = GetLicenseHeaderFromCssFile(file);
                fileType = FileType.Css;
            }
            else if (fileName.Equals("Dockerfile"))
            {
                licenseHeader = GetLicenseHeaderFromDockerFile(file);
                fileType = FileType.Dockerfile;
            }
            else if (extension.Equals(".src") ||
                     extension.Equals(".ntdef"))
            {
                licenseHeader = GetLicenseHeaderFromExportsFile(file);
                fileType = FileType.ExportsFile;
            }
            else if (extension.Equals(".fs") ||
                     extension.Equals(".fsi") ||
                     extension.Equals(".fsl") ||
                     extension.Equals(".fsscript") ||
                     extension.Equals(".fsx") ||
                     extension.Equals(".fsy") ||
                     extension.Equals(".mli") ||
                     extension.Equals(".ml"))
            {
                licenseHeader = GetLicenseHeaderFromFSharpFile(file);
                licenseHeader = RemoveBogusFSharpLicenses(licenseHeader);
                fileType = FileType.FSharp;
            }
            else if (extension.Equals(".js"))
            {
                licenseHeader = GetLicenseHeaderFromJavascriptFile(file);
                fileType = FileType.JavaScript;
            }
            else if (extension.Equals(".pl"))
            {
                licenseHeader = GetLicenseHeaderFromPerlFile(file);
                fileType = FileType.Perl;
            }
            else if (extension.Equals(".py"))
            {
                licenseHeader = GetLicenseHeaderFromPythonFile(file);
                fileType = FileType.Python;
            }
            else if (extension.Equals(".sh") ||
                     extension.Equals(".zsh") ||
                     extension.Equals(".bash") ||
                     extension.Equals(".bats"))
            {
                licenseHeader = GetLicenseHeaderFromShellFile(file);
                fileType = FileType.Shell;
            }
            else if (extension.Equals(".ts") ||
                     extension.Equals(".tsx"))
            {
                licenseHeader = GetLicenseHeaderFromTypescriptFile(file);
                fileType = FileType.TypeScript;
            }
            else if (extension.Equals(".vb"))
            {
                licenseHeader = GetLicenseHeaderFromVBFile(file);
                fileType = FileType.VisualBasic;
            }
            else if (extension.Equals(".xml") ||
                     extension.Equals(".Targets") ||
                     extension.Equals(".targets") ||
                     extension.Equals(".tasks") ||
                     extension.Equals(".vsdconfigxml"))
            {
                licenseHeader = GetLicenseHeaderFromXmlFile(file);
                fileType = FileType.Xml;
            }
            else if (extension.Equals(".y"))
            {
                licenseHeader = GetLicenseHeaderFromYaccFile(file);
                fileType = FileType.Yacc;
            }
            else if (file.ContainsPath("/cpp/") ||
                     file.ContainsPath("/inc/clr_std/"))
            {
                licenseHeader = GetLicenseHeaderFromCppFile(file);
            }
            else
            {
                return new LicenseCheckResult() { License = LicenseType.DontKnowHowToParseThisFile, File = file, IdentifiedType = FileType.Unknown , OptionalDetails = null};
            }

            licenseHeader = RemoveBogusLicenseHeader(licenseHeader, file);

            if (string.IsNullOrEmpty(licenseHeader))
            {
                // stdout.WriteLine("Missing license header in {0}", file);
                return new LicenseCheckResult() { License = LicenseType.NoLicense, File = file, IdentifiedType = fileType, OptionalDetails = null};
            }
            else
            { 
                bool found = false;
                foreach (var license in LICENSES)
                {
                    var licenseText = String.Format(license, file.GetFileName());
                    //stdout.WriteLine(licenseText);
                    if (licenseHeader.StartsWith(licenseText))
                    {
                        found = true;
                    }
                }

                if (found)
                {
                    // TODO fix type
                    return new LicenseCheckResult() { License = LicenseType.ValidLicense, File = file, IdentifiedType = fileType, OptionalDetails = null};
                }
                else
                {
                    return new LicenseCheckResult() { License = LicenseType.UnknownLicense, File = file, IdentifiedType = fileType, OptionalDetails = licenseHeader};
                }
            }
            
        }

        private string RemoveBogusLicenseHeader(string licenseHeader, FilePath filePath)
        {
            if (String.IsNullOrEmpty(licenseHeader))
            {
                return null;
            }

            foreach (var license in NOT_LICENSES)
            {
                string text = String.Format(license, filePath.GetFileName());
                if (licenseHeader.StartsWith(text))
                {
                    return null;
                }
            }
            return licenseHeader;
        }

        private String RemoveBogusFSharpLicenses(string licenseHeader)
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
