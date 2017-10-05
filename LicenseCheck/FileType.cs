using System;
using System.Linq;

namespace LicenseCheck
{
    public enum FileType
    {
        Assembly,
        C,
        CSharp,
        Css,
        Dockerfile,
        ExportsFile,
        FSharp,
        JavaScript,
        Perl,
        Python,
        Shell,
        TypeScript,
        Unknown,
        VisualBasic,
        Xml,
        Yacc,
    }

    public class FileTypeDef {
        public readonly FileType type;
        public readonly string[] fileEndsWith;
        public readonly string[] fileContains;
        public readonly Func<FilePath, string> getLicenseHeader;

        public FileTypeDef(
            FileType type,
            Func<FilePath, string> getLicenseHeader,
            string[] fileEndsWith = null,
            string[] fileContains = null
        ) {
            this.type             = type;
            this.fileEndsWith     = fileEndsWith;
            this.fileContains     = fileContains;
            this.getLicenseHeader = getLicenseHeader;
        }

        public bool givenFileApplies(FilePath file) {
            return this.givenFileApplies(file.ToString());
        }

        public bool givenFileApplies(string file) {
            if (   this.fileEndsWith != null
                && this.fileEndsWith.Any(s => file.EndsWith(s))
            ) {
                return true;
            }

            if (   this.fileContains != null
                && this.fileContains.Any(s => file.Contains(s))
            ) {
                return true;
            }

            return false;
        }
    }
}
