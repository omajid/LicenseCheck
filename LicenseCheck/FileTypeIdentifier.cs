namespace LicenseCheck
{

    public class FileTypeIdentifier
    {

        public FileType Identify(FilePath file)
        {
            string extension = file.GetExtension();
            string fileName = file.GetFileName();

            if (extension.Equals(".asm") ||
                extension.Equals(".inc") ||
                extension.Equals(".S"))
            {
                return FileType.Assembly;
            }
            else if (extension.Equals(".cs") ||
                     extension.Equals(".csx") ||
                     extension.Equals(".cool") ||
                     extension.Equals(".sc"))
            {
                return FileType.CSharp;
            }
            else if (extension.Equals(".cpp") ||
                     extension.Equals(".cxx") ||
                     extension.Equals(".c") ||
                     extension.Equals(".h") ||
                     extension.Equals(".h.in") ||
                     extension.Equals(".hpp") ||
                     extension.Equals(".rc") ||
                     extension.Equals(".idl") ||
                     extension.Equals(".def") ||
                     extension.Equals(".inl"))
            {
                return FileType.C;
            }
            else if (extension.Equals(".css") ||
                     fileName.EndsWith(".css.min"))
            {
                return FileType.Css;
            }
            else if (fileName.Equals("Dockerfile"))
            {
                return FileType.Dockerfile;
            }
            else if (extension.Equals(".src") ||
                     extension.Equals(".ntdef"))
            {
                return FileType.ExportsFile;
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
                return FileType.FSharp;
            }
            else if (extension.Equals(".js"))
            {
                return FileType.JavaScript;
            }
            else if (extension.Equals(".pl"))
            {
                return FileType.Perl;
            }
            else if (extension.Equals(".py"))
            {
                return FileType.Python;
            }
            else if (extension.Equals(".sh") ||
                     extension.Equals(".zsh") ||
                     extension.Equals(".bash") ||
                     extension.Equals(".bats"))
            {
                return FileType.Shell;
            }
            else if (extension.Equals(".ts") ||
                     extension.Equals(".tsx"))
            {
                return FileType.TypeScript;
            }
            else if (extension.Equals(".vb"))
            {
                return FileType.VisualBasic;
            }
            else if (extension.Equals(".xml") ||
                     extension.Equals(".Targets") ||
                     extension.Equals(".targets") ||
                     extension.Equals(".tasks") ||
                     extension.Equals(".vsdconfigxml"))
            {
                return FileType.Xml;
            }
            else if (extension.Equals(".y"))
            {
                return FileType.Yacc;
            }

            return FileType.Unknown;
        }
    }
}
