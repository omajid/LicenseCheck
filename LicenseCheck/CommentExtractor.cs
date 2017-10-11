using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace LicenseCheck
{

    public static class CommentExtractor
    {
        public static string ExtractFirstInlineComment(FilePath sourceCodeFile, string commentPrefix)
        {
            using (StreamReader sourceCodeStream = sourceCodeFile.Read())
            {
                return ExtractFirstInlineComment(sourceCodeStream, commentPrefix);
            }
        }

        public static string ExtractFirstInlineComment(TextReader sourceCodeStream, string commentPrefix)
        {
            string[] firstCommentLines = ExtractFirstInlineCommentLines(sourceCodeStream, commentPrefix);
            string[] firstRealCommentLines = StripShebangLine(firstCommentLines);
            string[] firstCommentContents = StripCommentCharacters(firstRealCommentLines, commentPrefix);
            string cleanedUpHeader = string.Join(" ", firstCommentContents).Trim();
            cleanedUpHeader = cleanedUpHeader.Replace("  ", " ");
            return cleanedUpHeader;
        }

        private static string[] ExtractFirstInlineCommentLines(TextReader sourceCodeStream, string commentPrefix)
        {
            List<string> lines = new List<string>();
            string line = sourceCodeStream.ReadLine();
            bool readFirstLine = false;
            while((line != null) && (line.Length == 0))
            {
                line = sourceCodeStream.ReadLine();
            }

            while ((line != null) && (line.StartsWith(commentPrefix)))
            {
                if (readFirstLine && !LineHasContent(line))
                {
                    break;
                }

                if (LineHasContent(line))
                {
                    lines.Add(CleanLine(line));
                }
                line = sourceCodeStream.ReadLine()?.Trim();
                readFirstLine = true;
            }

            return lines.ToArray();
        }

        public static string ExtractFirstBlockComment(FilePath file, string blockStart, string blockEnd)
        {
            using (StreamReader sourceCodeStream = file.Read())
            {
                return ExtractFirstBlockComment(sourceCodeStream, blockStart, blockEnd);
            }
        }

        public static string ExtractFirstBlockComment(TextReader sourceCodeStream, string blockStart, string blockEnd)
        {
            string[] firstCommentLines = ExtractFirstBlockCommentLines(sourceCodeStream, blockStart, blockEnd);
            string cleanedUpHeader = string.Join(" ", firstCommentLines).Trim();
            cleanedUpHeader = cleanedUpHeader.Replace("  ", " ");
            return cleanedUpHeader;
        }

        private static string[] ExtractFirstBlockCommentLines(TextReader sourceCodeStream, string start, string end)
        {
            List<string> lines = new List<string>();
            string line = sourceCodeStream.ReadLine()?.Trim();
            // look for block comment in first column (sans spaces) only
            while((line != null) && !(line.StartsWith(start)))
            {
                line = sourceCodeStream.ReadLine()?.Trim();
            }

            if (line == null)
            {
                return lines.ToArray();
            }

            if (line.Contains(end))
            {
                int startPosition = line.IndexOf(start) + start.Length;
                int endPosition = line.IndexOf(end);
                int length = endPosition - startPosition;
                line = line.Substring(startPosition, length).Trim();
                if (LineHasContent(line))
                {
                    line = CleanLine(line);
                    lines.Add(line);
                }
                return lines.ToArray();
            }

            line = line.Substring(line.IndexOf(start)+start.Length).Trim();
            if (LineHasContent(line))
            {
                line = CleanLine(line);
                lines.Add(line);
            }

            line = sourceCodeStream.ReadLine()?.Trim();
            while ((line != null))
            {
                bool foundEnd = false;
                if (line.Contains(end))
                {
                    foundEnd = true;
                    line = line.Substring(0, line.IndexOf(end)).Trim();
                }

                if (LineHasContent(line))
                {
                    line = CleanLine(line);
                    lines.Add(line);
                }
                if (foundEnd) break;
                line = sourceCodeStream.ReadLine()?.Trim();
            }

            return lines.ToArray();
        }

        private static bool LineHasContent(string line)
        {
            line = line.Trim();
            if (string.IsNullOrEmpty(line))
            {
                return false;
            }

            string separatorLines = @"^(?://|\*|!)? ?(?:=|-|\+|\*|/)+$";
            return !Regex.IsMatch(line, separatorLines);
        }

        private static string CleanLine(string line)
        {
            Regex separators = new Regex(@"(?:=|-|\+|\*|/){2,}");
            line = separators.Replace(line, "").Trim();
            if (line.StartsWith("*"))
            {
                line = line.Substring("*".Length).Trim();
            }
            return line;
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
