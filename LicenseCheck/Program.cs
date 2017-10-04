using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace LicenseCheck
{
    public class Program
    {
        public static String Usage => "usage: dotnet run /path/to/dir/to/scan [/path/to/more/dirs]";

        public static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.Error.WriteLine(Usage);
                Environment.Exit(1);
            }

            IList<string> fileNames = args;
            FindSourcesAndCheckLicenseHeaders(fileNames, Console.Out, Console.Error);
        }

        public static void FindSourcesAndCheckLicenseHeaders(IList<string> searchRoots, TextWriter stdout, TextWriter stderr)
        {
            SourceFileFinder finder = new SourceFileFinder();
            foreach (var root in searchRoots)
            {
                finder.Find(root, ((file) => CheckLicenseHeader(file, stdout, stderr)), stdout, stderr);
            }
        }

        private static void CheckLicenseHeader(string file, TextWriter stdout, TextWriter stderr)
        {
            // Console.WriteLine($"{file}");
            LicenseCheckResult result = new LicenseChecker().Check(new FilePath(file));

            switch (result.License)
            {
            case LicenseType.NoLicense:
                stdout.WriteLine("Missing license header in {0}", file);
                break;
            case LicenseType.UnknownLicense:
                stdout.WriteLine("Unknown license header: {0}", file);
                stdout.WriteLine(result.OptionalDetails);
                break;
            case LicenseType.DontKnowHowToParseThisFile:
                stdout.WriteLine("Dont know how to parse {0}", file);
                break;
            case LicenseType.ValidLicense:
                // okay, nothing to do
                break;
            }
        }

    }
}
