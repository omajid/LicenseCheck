using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace LicenseCheck
{
    class Program
    {
        public static String Usage => "usage: dotnet run /path/to/dir/to/scan [/path/to/more/dirs]";

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


        // FIXME later
        private static string[] PATH_COMPONENTS_TO_IGNORE = {
            "/palsuite/",
            "/CoreMangLib/",
            "/ToolBox/SOS/tests/",
            "/clrcompression/zlib/",
            
        };

        private static string[] FILE_SUFFIXES_TO_IGNORE = {
            ".1",
            ".aml",
            ".analyzerdata",
            ".appxmanifest",
            ".aspx",
            ".ashx",
            ".awk",
            ".bat",
            ".bin",
            ".bmp",
            ".bowerrc",
            ".bsl",
            ".builds",
            ".ccproj",
            ".cd",
            ".cfg",
            ".cmake",
            ".cmd",
            ".conf",
            ".config",
            ".css.map",
            ".cscfg", // xml
            ".cshtml", // html
            ".csproj",
            ".csv",
            ".dat",
            ".DAT",
            ".db",
            ".depproj",
            ".dgml", // xml, image
            ".dll",
            ".dll.lcl", // xml
            ".Dll",
            ".dllx",
            ".doc",
            ".docx",
            ".dtd", // xml
            ".eot", // font
            ".ent", // xml entity
            ".exe",
            ".exports",
            ".fsproj",
            ".gif",
            ".git",
            ".gitkeep",
            ".gitmodules",
            ".groovy",
            ".htm",
            ".html",
            ".ico",
            ".il",
            ".ilproj",
            ".jpg",
            ".jpeg",
            ".json",
            ".jsproj",
            ".locproj",
            ".lsml",
            ".lst",
            ".manifest", // xml
            ".md",
            ".metadata",
            ".metadatax",
            ".min.css.map", // minified js?
            ".min.map", // minified js?
            ".mod", // dll
            ".msbuild",
            ".nativeproj",
            ".netmodule",
            ".nuspec",
            ".nupkg",
            ".nuproj",
            ".ruleset",
            ".obj",
            ".patch",
            ".pdb",
            ".pdbx",
            ".pdf",
            ".pfx",
            ".pkgdef",
            ".pkgprops",
            ".pkgproj",
            ".ppt",
            ".pptx",
            ".proj",
            ".projitems",
            ".props",
            ".png",
            ".ps1",
            ".ps1xml", // xml
            ".pubxml",
            ".res",
            ".resources", // binary
            ".resx",
            ".rsp",
            ".rtf",
            ".settings", // xml
            ".shfbproj",
            ".shproj",
            ".sln",
            ".snk",
            ".StyleCop",
            ".svg",
            ".swixproj",
            ".swr",
            ".testlist",
            ".tt", // text, template
            ".ttf",
            ".txt",
            ".TXT",
            ".unknownproj",
            ".vbproj",
            ".vcxproj",
            ".vcxproj.filters",
            ".vsct",
            ".vsbsl", // F# compiler output
            ".vsd",
            ".vsmanproj",
            ".vsixmanifest",
            ".vstemplate",
            ".vssettings", // xml
            ".wav",
            ".webproj",
            ".winmd",
            ".wixproj",
            ".woff", // font
            ".woff2", // font
            ".wxl",
            ".wxs",
            ".wxi",
            ".xaml",
            ".xdt",
            ".xlf",
            ".xlsx", // microsoft excel
            ".xslt", // xml
            ".xproj",
            ".xunit",
            ".xsd",
            ".xsl",
            ".yml",
            ".zip",
        };

        private static string[] FILE_NAMES_TO_IGNORE = {
            "-.-",
            "_._",
            ".clang-format",
            "CMakeLists.txt",
            "cmake.definitions",
            ".editorconfig",
            ".gitattributes",
            ".gitignore",
            ".gitmirror",
            ".gitmirrorall",
            ".gitmirrorselective",
            "ISSUE_TEMPLATE",
            "LICENSE",
            "Makefile",
            "makefile",
            ".noautobuild",
            "nuget.config",
            "Nuget.Config",
            "NuGet.Config",
            "PULL_REQUEST_TEMPLATE",
            "SOURCES",
            "sources.list.jessie",
            "sources.list.trusty",
            "sources.list.vivid",
            "sources.list.wily",
            "sources.list.xenial",
            "sources.list.zesty",
            "THIRD-PARTY-NOTICE",
            "THIRD-PARTY-NOTICES",
            ".vsixignore.vs14",
            ".vsixignore.vs15",
        };

        private static string[] DIR_NAMES_TO_IGNORE = { ".git" };

        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.Error.WriteLine(Usage);
                Environment.Exit(1);
            }

            IList<string> fileNames = args;
            foreach (var fileName in fileNames)
            {
                ScanPath(fileName);
            }
        }


        private static void ScanPath(string path)
        {
            if (Directory.Exists(path))
            {
                ScanDirectory(path);    
            }
            else if (File.Exists(path))
            {
                ScanFile(path);
            }
            else
            {
                Console.Error.WriteLine("Invalid path: {0}", path);
            }
        }

        private static void ScanDirectory(string dir)
        {
            foreach (var subdir in Directory.GetDirectories(dir))
            {
                ScanDirectory(subdir);
            }
            foreach (var file in Directory.GetFiles(dir))
            {
                ScanFile(file);
            }
        }

        private static void ScanFile(string file)
        {
            foreach (var path in PATH_COMPONENTS_TO_IGNORE)
            {
                if (file.Contains(path))
                {
                    return;
                }
            }

            foreach (var suffix in FILE_SUFFIXES_TO_IGNORE)
            {
                if (file.EndsWith(suffix))
                {
                    return;
                }
            }

            foreach (var fileName in FILE_NAMES_TO_IGNORE)
            {
                if (file.EndsWith("/" + fileName))
                {
                    return;
                }
            }

            // Console.WriteLine($"{file}");

            string licenseHeader = null;
            if (file.EndsWith(".asm") ||
                file.EndsWith(".inc") ||
                file.EndsWith(".S"))
            {
                licenseHeader = GetLicenseHeaderFromAssemblyFile(file);
            }
            else if (file.EndsWith(".cs") ||
                     file.EndsWith(".csx") ||
                     file.EndsWith(".cool") ||
                     file.EndsWith(".sc"))
            {
                licenseHeader = GetLicenseHeaderFromCSharpFile(file);
            }
            else if (file.EndsWith(".cpp") ||
                     file.EndsWith(".cxx") ||
                     file.EndsWith(".c") ||
                     file.EndsWith(".h") ||
                     file.EndsWith(".h.in") ||
                     file.EndsWith(".hpp") ||
                     file.EndsWith(".S") ||
                     file.EndsWith(".rc") ||
                     file.EndsWith(".idl") ||
                     file.EndsWith(".def") ||
                     file.EndsWith(".inl"))
            {
                licenseHeader = GetLicenseHeaderFromCppFile(file);
            }
            else if (file.EndsWith(".css") ||
                     file.EndsWith(".css.min"))
            {
                licenseHeader = GetLicenseHeaderFromCssFile(file);
            }
            else if (file.EndsWith("/Dockerfile"))
            {
                licenseHeader = GetLicenseHeaderFromDockerFile(file);
            }
            else if (file.EndsWith(".src") ||
                     file.EndsWith(".ntdef"))
            {
                licenseHeader = GetLicenseHeaderFromExportsFile(file);
            }
            else if (file.EndsWith(".fs") ||
                     file.EndsWith(".fsi") ||
                     file.EndsWith(".fsl") ||
                     file.EndsWith(".fsscript") ||
                     file.EndsWith(".fsx") ||
                     file.EndsWith(".fsy") ||
                     file.EndsWith(".mli") ||
                     file.EndsWith(".ml"))
            {
                licenseHeader = GetLicenseHeaderFromFSharpFile(file);
                licenseHeader = RemoveBogusFSharpLicenses(licenseHeader);
            }
            else if (file.EndsWith(".js"))
            {
                licenseHeader = GetLicenseHeaderFromJavascriptFile(file);
            }
            else if (file.EndsWith(".pl"))
            {
                licenseHeader = GetLicenseHeaderFromPerlFile(file);
            }
            else if (file.EndsWith(".py"))
            {
                licenseHeader = GetLicenseHeaderFromPythonFile(file);
            }
            else if (file.EndsWith(".sh") ||
                     file.EndsWith(".zsh") ||
                     file.EndsWith(".bash") ||
                     file.EndsWith(".bats"))
            {
                licenseHeader = GetLicenseHeaderFromShellFile(file);
            }
            else if (file.EndsWith(".ts") ||
                     file.EndsWith(".tsx"))
            {
                licenseHeader = GetLicenseHeaderFromTypescriptFile(file);
            }
            else if (file.EndsWith(".vb"))
            {
                licenseHeader = GetLicenseHeaderFromVBFile(file);
            }
            else if (file.EndsWith(".xml") ||
                     file.EndsWith(".Targets") ||
                     file.EndsWith(".targets") ||
                     file.EndsWith(".tasks") ||
                     file.EndsWith(".vsdconfigxml"))
            {
                // licenseHeader = GetLicenseHeaderFromXmlFile(file);
            }
            else if (file.EndsWith(".y"))
            {
                licenseHeader = GetLicenseHeaderFromYaccFile(file);
            }
            else if (file.Contains("/cpp/") ||
                     file.Contains("/inc/clr_std/"))
            {
                licenseHeader = GetLicenseHeaderFromCppFile(file);
            }
            else
            {
                Console.WriteLine("Unknown file type {0}", file);
            }

            licenseHeader = RemoveBogusLicenseHeader(licenseHeader, file);

            if (string.IsNullOrEmpty(licenseHeader))
            {
                Console.Error.WriteLine("Missing license header in {0}", file);
            }
            else
            { 
                bool found = false;
                foreach (var license in LICENSES)
                {
                    var licenseText = String.Format(license, Path.GetFileName(file));
                    //Console.WriteLine(licenseText);
                    if (licenseHeader.StartsWith(licenseText))
                    {
                        found = true;
                    }
                }

                if (!found)
                {
                    Console.Error.WriteLine("Unknown license header: {0}", file);
                    Console.Error.WriteLine(licenseHeader);
                }
            }
        }

        private static string RemoveBogusLicenseHeader(string licenseHeader, string filePath)
        {
            if (String.IsNullOrEmpty(licenseHeader))
            {
                return null;
            }

            foreach (var license in NOT_LICENSES)
            {
                string text = String.Format(license, Path.GetFileName(filePath));
                if (licenseHeader.StartsWith(text))
                {
                    return null;
                }
            }
            return licenseHeader;
        }

        private static String RemoveBogusFSharpLicenses(string licenseHeader)
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

        private static string GetLicenseHeaderFromAssemblyFile(string file)
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

        private static string GetLicenseHeaderFromFSharpFile(string file)
        {
            string header = GetLicenseHeaderFromFirstBlockCommentHeader(file, "(*", "*)", "*");
            if (!String.IsNullOrEmpty(header))
            {
                return header;
            }
            header = GetLicenseHeaderFromFirstInlineCommentHeaderWithoutShebang(file, "//");
            return header;
        }

        private static string GetLicenseHeaderFromCSharpFile(string file)
        {
            string commentPrefix = "//";
            return GetLicenseHeaderFromFirstInlineCommentHeaderWithoutShebang(file, commentPrefix);
        }

        private static string GetLicenseHeaderFromCppFile(string file)
        {
            string commentPrefix = "//";
            return GetLicenseHeaderFromFirstInlineCommentHeaderWithoutShebang(file, commentPrefix);
        }

        private static string GetLicenseHeaderFromCssFile(string file)
        {
            return GetLicenseHeaderFromFirstBlockCommentHeader(file, "/*", "*/", "*");
        }

        private static string GetLicenseHeaderFromDockerFile(string file)
        {
            string commentPrefix = "#";
            return GetLicenseHeaderFromFirstInlineCommentHeaderWithoutShebang(file, commentPrefix);
        }

        private static string GetLicenseHeaderFromExportsFile(string file)
        {
            string commentPrefix = ";";
            return GetLicenseHeaderFromFirstInlineCommentHeaderWithoutShebang(file, commentPrefix);
        }

        private static string GetLicenseHeaderFromJavascriptFile(string file)
        {
            return GetLicenseHeaderFromFirstBlockCommentHeader(file, "/*", "*/", "*");
        }

        private static string GetLicenseHeaderFromPerlFile(string file)
        {
            string commentPrefix = "#";
            return GetLicenseHeaderFromFirstInlineCommentHeaderWithoutShebang(file, commentPrefix);
        }

        private static string GetLicenseHeaderFromPythonFile(string file)
        {
            string commentPrefix = "#";
            return GetLicenseHeaderFromFirstInlineCommentHeaderWithoutShebang(file, commentPrefix);
        }

        private static string GetLicenseHeaderFromShellFile(string file)
        {
            string commentPrefix = "#";
            return GetLicenseHeaderFromFirstInlineCommentHeaderWithoutShebang(file, commentPrefix);
        }

        private static string GetLicenseHeaderFromTypescriptFile(string file)
        {
            string commentPrefix = "//";
            return GetLicenseHeaderFromFirstInlineCommentHeaderWithoutShebang(file, commentPrefix);
        }

        private static string GetLicenseHeaderFromVBFile(string file)
        {
            string commentPrefix = "'";
            return GetLicenseHeaderFromFirstInlineCommentHeaderWithoutShebang(file, commentPrefix);
        }

        private static string GetLicenseHeaderFromXmlFile(string file)
        {
            return GetLicenseHeaderFromFirstBlockCommentHeader(file, "<!--", "-->", null);
        }

        private static string GetLicenseHeaderFromYaccFile(string file)
        {
            string commentPrefix = "//";
            return GetLicenseHeaderFromFirstInlineCommentHeaderWithoutShebang(file, commentPrefix);
        }

        private static string GetLicenseHeaderFromFirstInlineCommentHeaderWithoutShebang(string file, string commentPrefix)
        {
            string[] firstCommentLines = ExtractFirstInlineComment(file, commentPrefix);
            string[] firstRealCommentLines = StripShebangLine(firstCommentLines);
            string[] firstCommentContents = StripCommentCharacters(firstRealCommentLines, commentPrefix);
            string cleanedUpHeader = String.Join(" ", firstCommentContents).Trim();
            cleanedUpHeader = cleanedUpHeader.Replace("  ", " ");
            return cleanedUpHeader;
        }

        private static string GetLicenseHeaderFromFirstBlockCommentHeader(string file, string blockStart, string blockEnd, string optionalPrefix)
        {
            string[] firstCommentLines = ExtractFirstBlockComment(file, blockStart, blockEnd, optionalPrefix);
            string cleanedUpHeader = String.Join(" ", firstCommentLines).Trim();
            cleanedUpHeader = cleanedUpHeader.Replace("  ", " ");
            return cleanedUpHeader;
        }

        private static string[] ExtractFirstInlineComment(string file, string commentPrefix)
        {
            List<string> lines = new List<string>();
            using (StreamReader sr = new StreamReader(file))
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

        private static string[] ExtractFirstBlockComment(string file, string start, string end, string optionalPrefix)
        {
            // Console.WriteLine("");
            List<string> lines = new List<string>();
            using (StreamReader sr = new StreamReader(file))
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
                    // Console.WriteLine(line);
                    // Console.WriteLine(line.Length);
                    // Console.WriteLine(line.IndexOf(start));
                    // Console.WriteLine(start.Length);
                    // Console.WriteLine(line.IndexOf(end));
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

        private static string[] StripShebangLine(string[] lines)
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

        private static string[] StripCommentCharacters(string[] lines, string commentPrefix)
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
