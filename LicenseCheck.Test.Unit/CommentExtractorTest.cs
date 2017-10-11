using LicenseCheck;
using Xunit;
using System.IO;

namespace LicenseCheck.Test.Unit
{

    public class CommentExtractorBlockCommentTest
    {

        [Fact]
        private void SourceContainsBlockComment()
        {
            var comment = @"<!-- DO NOT MODIFY this file -->";
            var commentContent = ExtractFirstBlockComment(comment, "<!--", "-->");
            Assert.Equal("DO NOT MODIFY this file", commentContent);
        }

        [Fact]
        private void SourceContainsBlockCommentMultipleLines()
        {
            var comment = @"<!--
DO NOT MODIFY this file -->";
            var commentContent = ExtractFirstBlockComment(comment, "<!--", "-->");
            Assert.Equal("DO NOT MODIFY this file", commentContent);
        }

        [Fact]
        private void SourceContainsBlockCommentMultipleLinesWithDelimiterOnSeparateLines()
        {
            var comment = @"<!--
DO NOT MODIFY this file
-->";
            var commentContent = ExtractFirstBlockComment(comment, "<!--", "-->");
            Assert.Equal("DO NOT MODIFY this file", commentContent);
        }

        [Fact]
        private void SourceContainsBlockCommentWithLinesOfStars()
        {
            var comment = @"<!--
***********************************************************************************************
Copyright (c) .NET Foundation. All rights reserved.
***********************************************************************************************
-->";
            var commentContent = ExtractFirstBlockComment(comment, "<!--", "-->");
            Assert.DoesNotContain("****", commentContent);
        }

        [Fact]
        private void SourceContainsBlockCommentWithLinesOfDashes()
        {
            var comment = @"/*
---------
FOO
---------
*/";
            var commentContent = ExtractFirstBlockComment(comment, "/*", "*/");
            Assert.Equal("FOO", commentContent);
        }

        [Fact]
        private void SourceContainsBlockCommentWithOnlyLinesOfDashes()
        {
            var comment = @"/*
---------
---------
*/";
            var commentContent = ExtractFirstBlockComment(comment, "/*", "*/");
            Assert.Equal("", commentContent);
        }

        [Fact]
        private void SourceContainsBlockCommentWithOneLineOfDashes()
        {
            var comment = @"(* --------------------------- *)";
            var commentContent = ExtractFirstBlockComment(comment, "(*", "*)");
            Assert.Equal("", commentContent);
        }

        [Fact]
        private void SourceContainsBlockCommentWithOneLineOfEquals()
        {
            var comment = @"(*========================================================================*)";
            var commentContent = ExtractFirstBlockComment(comment, "(*", "*)");
            Assert.Equal("", commentContent);
        }

        [Fact]
        private void SourceContainsSingleLineBlockCommentWithExtraDashesBeforeText()
        {
            var comment = @"(* ----------------------- Foo *)";
            var commentContent = ExtractFirstBlockComment(comment, "(*", "*)");
            Assert.Equal("Foo", commentContent);
        }


        [Fact]
        private void SourceContainsSingleLineBlockCommentWithExtraDashesAfterText()
        {
            var comment = @"(* Foo ----------------------- *)";
            var commentContent = ExtractFirstBlockComment(comment, "(*", "*)");
            Assert.Equal("Foo", commentContent);
        }

        [Fact]
        private void SourceContainsBlockCommentWithLinesPrefixedWithStars()
        {
            var comment =
@"(* --------------------------------------------------------------------
  * Nested letrecs
  * -------------------------------------------------------------------- *)";
            var commentContent = ExtractFirstBlockComment(comment, "(*", "*)");
            Assert.Equal("Nested letrecs", commentContent);
        }

        [Fact]
        private void SourceContainsBlockCommentWhichContainsInlineComment()
        {
            var comment =
@"<!--
// FOO
-->";
            var commentContent = ExtractFirstBlockComment(comment, "<!--", "-->");
            Assert.Equal("FOO", commentContent);
        }

        private string ExtractFirstBlockComment(string sourceText, string commentStart, string commentEnd)
        {
            using (TextReader reader = new StringReader(sourceText))
            {
                return CommentExtractor.ExtractFirstBlockComment(reader, commentStart, commentEnd);
            }
        }

    }

    public class CommentExtractorInlineCommentTest
    {

        [Fact]
        private void EmptySourceContainsNoComment()
        {
            var comment = @"";
            var commentContent = ExtractFirstInlineComment(comment, "//");
            Assert.Equal("", commentContent);
        }

        [Fact]
        private void SourceIsOnlyAComment()
        {
            var comment = @"// Foo";
            var commentContent = ExtractFirstInlineComment(comment, "//");
            Assert.Equal("Foo", commentContent);
        }

        [Fact]
        private void SourceContainsCommentThenCode()
        {
            var comment = @"// Foo
            Bar";
            var commentContent = ExtractFirstInlineComment(comment, "//");
            Assert.Equal("Foo", commentContent);
        }

        [Fact]
        private void SourceContainsMultilineCommentThenCode()
        {
            var comment = @"// Foo
            // Bar
            // Baz";
            var commentContent = ExtractFirstInlineComment(comment, "//");
            Assert.Equal("Foo Bar Baz", commentContent);
        }

        [Fact]
        private void SourceContainsMultilineCommentThenSecondComment()
        {
            var comment = @"// License
            // Info

            // This file is about foobar";
            var commentContent = ExtractFirstInlineComment(comment, "//");
            Assert.Equal("License Info", commentContent);
        }

        [Fact]
        private void SourceContainsMultilineCommentThenSecondCommentWithSeparatorNoSpaces()
        {
            var comment =
@"//License
//Info
//-----
//This file is about foobar";
            var commentContent = ExtractFirstInlineComment(comment, "//");
            Assert.Equal("License Info", commentContent);
        }

        [Fact]
        private void SourceContainsMultilineCommentThenSecondCommentWithSeparator()
        {
            var comment = @"// License
            // Info
            // -----
            // This file is about foobar";
            var commentContent = ExtractFirstInlineComment(comment, "//");
            Assert.Equal("License Info", commentContent);
        }

        [Fact]
        private void SourceContainsCommentThenCodeThenComment()
        {
            var comment = @"// Foo
            Bar
            //Baz";
            var commentContent = ExtractFirstInlineComment(comment, "//");
            Assert.Equal("Foo", commentContent);
        }

        [Fact]
        private void ShebangIsNotPartOfMultilineComment()
        {
            var comment =
@"#!/foo
# Bar
Code";
            var commentContent = ExtractFirstInlineComment(comment, "#");
            Assert.Equal("Bar", commentContent);
        }

        [Fact]
        private void LineOfDashesShouldParseToEmptyComment()
        {
            var comment =
@"//------------------------------------------------------------------------------";
            var commentContent = ExtractFirstInlineComment(comment, "//");
            Assert.Equal("", commentContent);
        }

        [Fact]
        private void LinesOfDashesAreNotPartOfCommentContent()
        {
            var comment =
@"//------------------------------------------------------------------------------
// License
//------------------------------------------------------------------------------
//

namespace System.Xml {";
            var commentContent = ExtractFirstInlineComment(comment, "//");
            Assert.Equal("License", commentContent);
        }

        private string ExtractFirstInlineComment(string sourceText, string commentPrefix)
        {
            using (TextReader reader = new StringReader(sourceText))
            {
                return CommentExtractor.ExtractFirstInlineComment(reader, commentPrefix);
            }
        }




    }
}
