using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

namespace LicenseCheck
{
    class SourceFileFinder
    {

        private static string[] DIR_NAMES_TO_IGNORE = { ".git" };

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
            "copyright", // debian packaging
            "control", // debian packaging
            "compat", // debian packaging
            "changelog", // debian packaging
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
            "postinstall", // debian packaging
            "PULL_REQUEST_TEMPLATE",
            "rules", // debian packaging
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

        public void Find(string path, Action<string> action, TextWriter stdout, TextWriter stderr)
        {
            if (Directory.Exists(path))
            {
                ScanDirectory(path, action, stdout, stderr);
            }
            else if (File.Exists(path))
            {
                ScanFile(path, action, stdout, stderr);
            }
            else
            {
                // throw exception
                stderr.WriteLine("Invalid path: {0}", path);
            }
        }

        private void ScanDirectory(string dir, Action<string> action, TextWriter stdout, TextWriter stderr)
        {
            if (DIR_NAMES_TO_IGNORE.Any(toIgnore => new DirectoryInfo(dir).Name.Equals(toIgnore)))
            {
                return;
            }

            foreach (var subdir in Directory.GetDirectories(dir))
            {
                ScanDirectory(subdir, action, stdout, stderr);
            }
            Parallel.ForEach<string>(Directory.GetFiles(dir), file => {
                ScanFile(file, action, stdout, stderr);
            });
        }

        private void ScanFile(string file, Action<string> action, TextWriter stdout, TextWriter stderr)
        {
            if ( PATH_COMPONENTS_TO_IGNORE.Any(path => file.Contains(path)) ) {
                return;
            }

            if ( FILE_SUFFIXES_TO_IGNORE.Any(suffix => file.EndsWith(suffix)) ) {
                return;
            }

            if ( FILE_NAMES_TO_IGNORE.Any(name => file.EndsWith("/" + name)) ) {
                return;
            }

            action(file);
        }

    }
}
