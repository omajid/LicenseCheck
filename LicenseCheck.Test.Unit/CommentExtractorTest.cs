using LicenseCheck;
using Xunit;
using System.IO;

namespace LicenseCheck.Test.Unit
{

    public class CommentExtractorBlockCommentTest
    {


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

        private string ExtractFirstInlineComment(string sourceText, string commentPrefix)
        {
            using (TextReader reader = new StringReader(sourceText))
            {
                return CommentExtractor.ExtractFirstInlineComment(reader, commentPrefix);
            }
        }




    }
}
