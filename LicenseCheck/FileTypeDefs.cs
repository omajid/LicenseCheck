using System;
using System.Linq;

namespace LicenseCheck {

    public static class FileTypeDefs {
        public static FileTypeDef[] FILETYPEDEFS = new FileTypeDef[] {
            new FileTypeDef(
                type: FileType.Assembly,
                fileEndsWith: new string[] {
                    ".asm",
                    ".inc",
                    ".S"
                },
                getLicenseHeader: (file) => {
                    string header = CommentExtractor.ExtractFirstInlineComment(file, ";");
                    if ( string.IsNullOrEmpty(header) ) {
                        header = CommentExtractor.ExtractFirstInlineComment(file, "//");
                    }
                    return header;
                }
            ),
            new FileTypeDef(
                type: FileType.CSharp,
                fileEndsWith: new string[] {
                    ".cs",
                    ".csx",
                    ".cool",
                    ".sc"
                },
                getLicenseHeader: (file) => {
                    return CommentExtractor.ExtractFirstInlineComment(file, "//");
                }
            ),
            new FileTypeDef(
                type: FileType.C,
                fileEndsWith: new string[] {
                    ".cpp",
                    ".cxx",
                    ".c",
                    ".h",
                    ".h.in",
                    ".hpp",
                    ".S",
                    ".rc",
                    ".idl",
                    ".def",
                    ".inl"
                },
                fileContains: new string[] {
                    "/cpp/",
                    "/inc/clr_std/"
                },
                getLicenseHeader: (file) => {
                    return CommentExtractor.ExtractFirstInlineComment(file, "//");
                }
            ),
            new FileTypeDef(
                type: FileType.Css,
                fileEndsWith: new string[] {
                    ".css",
                    ".css.min"
                },
                getLicenseHeader: (file) => {
                    return CommentExtractor.ExtractFirstBlockComment(file, "/*", "*/");
                }
            ),
            new FileTypeDef(
                type: FileType.Dockerfile,
                fileEndsWith: new string[] {
                    "/Dockerfile"
                },
                getLicenseHeader: (file) => {
                    return CommentExtractor.ExtractFirstInlineComment(file, "#");
                }
            ),
            new FileTypeDef(
                type: FileType.ExportsFile,
                fileEndsWith: new string[] {
                    ".src",
                    ".ntdef"
                },
                getLicenseHeader: (file) => {
                    return CommentExtractor.ExtractFirstInlineComment(file, ";");
                }
            ),
            new FileTypeDef(
                type: FileType.FSharp,
                fileEndsWith: new string[] {
                    ".fs",
                    ".fsi",
                    ".fsl",
                    ".fsscript",
                    ".fsx",
                    ".fsy",
                    ".mli",
                    ".ml"
                },
                getLicenseHeader: (file) => {
                    string header = CommentExtractor.ExtractFirstBlockComment(file, "(*", "*)");
                    if ( string.IsNullOrEmpty(header) ) {
                        header = CommentExtractor.ExtractFirstInlineComment(file, "//");
                    }

                    string[] ignore = {
                        "#ByRef",
                        "#CodeGen",
                        "#Conformance",
                        "#Diagnostics",
                        "#ErrorMessages",
                        "<Expects>",
                        "#FSharpQA",
                        "#FSI",
                        "#Globalization",
                        "#Import",
                        "#in #BindingExpressions",
                        "#inline #FSharpQA",
                        "Learn more about F#",
                        "#Libraries",
                        "#Misc",
                        "#NativePtr",
                        "#NoMono",
                        "#NoMT",
                        "#OCaml",
                        "#Query",
                        "[<ReferenceEquality(true)>]",
                        "#Regression",
                        "[<StructuralComparison(true)>]",
                        "[Test Strategy]",
                        "#Warnings",
                        "#XMLDoc",
                    };
                    if (ignore.Any( i => header.StartsWith(i) )) {
                        header = null;
                    }

                    return header;
                }
            ),
            new FileTypeDef(
                type: FileType.JavaScript,
                fileEndsWith: new string[] {
                    ".js"
                },
                getLicenseHeader: (file) => {
                    return CommentExtractor.ExtractFirstBlockComment(file, "/*", "*/");
                }
            ),
            new FileTypeDef(
                type: FileType.Perl,
                fileEndsWith: new string[] {
                    ".pl"
                },
                getLicenseHeader: (file) => {
                    return CommentExtractor.ExtractFirstInlineComment(file, "#");
                }
            ),
            new FileTypeDef(
                type: FileType.Python,
                fileEndsWith: new string[] {
                    ".py"
                },
                getLicenseHeader: (file) => {
                    return CommentExtractor.ExtractFirstInlineComment(file, "#");
                }
            ),
            new FileTypeDef(
                type: FileType.Shell,
                fileEndsWith: new string[] {
                    ".sh",
                    ".zsh",
                    ".bash",
                    ".bats"
                },
                getLicenseHeader: (file) => {
                    return CommentExtractor.ExtractFirstInlineComment(file, "#");
                }
            ),
            new FileTypeDef(
                type: FileType.TypeScript,
                fileEndsWith: new string[] {
                    ".ts",
                    ".tsx"
                },
                getLicenseHeader: (file) => {
                    return CommentExtractor.ExtractFirstInlineComment(file, "//");
                }
            ),
            new FileTypeDef(
                type: FileType.VisualBasic,
                fileEndsWith: new string[] {
                    ".vb"
                },
                getLicenseHeader: (file) => {
                    return CommentExtractor.ExtractFirstInlineComment(file, "'");
                }
            ),
            new FileTypeDef(
                type: FileType.Xml,
                fileEndsWith: new string[] {
                    ".xml",
                    ".Targets",
                    ".targets",
                    ".tasks",
                    ".vsdconfigxml"
                },
                getLicenseHeader: (file) => {
                    return CommentExtractor.ExtractFirstBlockComment(file, "<!--", "-->");
                }
            ),
            new FileTypeDef(
                type: FileType.Yacc,
                fileEndsWith: new string[] {
                    ".y"
                },
                getLicenseHeader: (file) => {
                    return CommentExtractor.ExtractFirstInlineComment(file, "//");
                }
            ),
        }; // public static FileTypeDef[] FILETYPES = new FileTypeDef[] {

        public static FileTypeDef getFileTypeDef(FileType type) {
            return FILETYPEDEFS.FirstOrDefault(def => def.type == type);
        }

        public static FileTypeDef getFileTypeDef(FilePath file) {
            return FILETYPEDEFS.FirstOrDefault(def => def.givenFileApplies(file));
        }
    }
}
