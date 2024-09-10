namespace Naninovel.Csv.Test
{
    public class WriterTests
    {
        [Fact]
        public void CanWriteCsv ()
        {
            var buffer = new StringWriter();
            var writer = new Writer(buffer);
            writer.WriteField("xxx");
            writer.WriteField("x\"xx");
            writer.WriteField(" xxx ");
            writer.WriteField("x, x");
            writer.WriteField("x\nx x");
            writer.NextRecord();
            writer.WriteField("x");
            writer.NextRecord();
            Assert.Equal("xxx,\"x\"\"xx\",\" xxx \",\"x, x\",\"x\nx x\"\nx\n", buffer.ToString());
        }

        [Fact]
        public void RespectsOptions ()
        {
            var buffer = new StringWriter();
            var writer = new Writer(buffer, new() { Trim = true, QuoteAll = true });
            writer.WriteField(" x ");
            writer.WriteField("x");
            Assert.Equal("\"x\",\"x\"", buffer.ToString());
        }
    }
}
