using System.Globalization;
using System.Text;

namespace Naninovel.Csv.Test
{
    public class ReaderTest
    {
        [Fact]
        public void CanParseCsv ()
        {
            var reader = new Reader(new StreamReader(new MemoryStream(
                """
                    A,B,C
                    1,5.5,5 Jun 2014
                    2,0,6/6/2014
                    3,4.6,6-7-2014
                    4,2,6-5-2014
                    """u8.ToArray())));
            var sumA = 0;
            decimal sumB = 0;
            var line = 0;
            while (reader.ReadRow())
            {
                line++;
                if (line == 1) continue; // skip header row
                sumA += Convert.ToInt32(reader[0], CultureInfo.InvariantCulture);
                sumB += Convert.ToDecimal(reader[1], CultureInfo.InvariantCulture);
            }
            Assert.Equal(5, line);
            Assert.Equal(10, sumA);
            Assert.Equal(12.1M, sumB);
        }

        [Fact]
        public void CanParseMalformedCsv ()
        {
            var reader = new Reader(new StreamReader(new MemoryStream("\"A\nA\",B  B,\"C\r\n\tC\"\n1,5.5,5 Jun 2014"u8.ToArray())));
            Assert.True(reader.ReadRow());
            Assert.Equal("A\nA", reader[0]);
            Assert.Equal("B  B", reader[1]);
            Assert.Equal("C\r\n\tC", reader[2]);
            Assert.True(reader.ReadRow());
            Assert.Equal("1", reader[0]);
            Assert.Equal("5.5", reader[1]);
            Assert.Equal("5 Jun 2014", reader[2]);
        }

        [Fact]
        public void RespectsOptions ()
        {
            var input = new[] {
                "A,B,C \r\n1,2, 3\r\n\r\n5,,\n\"6\", \"7\"\"\" ,\"\"\"\"",
                "A,B,C\n1 , 2  ,3 \n  4,5, 6\n \"7\",\"\"8 ,\"9\"",
                "A,B,C",
                "A,B,C\n1,2,3"
            };
            var trim = new[] { true, false, true, true };
            var size = new[] { 1024, 1024, 5, 5 };
            var expected = new[] {
                "1|2|3|#5|||#6|7\"|\"|#",
                "1 | 2  |3 |#  4|5| 6|# \"7\"|\"\"8 |9|#",
                "",
                "1|2|3|#"
            };
            for (var i = 0; i < input.Length; i++)
            {
                var reader = new Reader(new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(input[i]))), new() {
                    TrimFields = trim[i],
                    BufferSize = size[i]
                });
                var builder = new StringBuilder();
                reader.ReadRow(); // skip header row
                while (reader.ReadRow())
                {
                    builder.Append(reader[0] + "|");
                    builder.Append(reader[1] + "|");
                    builder.Append(reader[2] + "|");
                    builder.Append('#');
                }
                Assert.Equal(expected[i], builder.ToString());
            }
        }

        [Fact]
        public void CanParseLinesThatExceedBufferSize ()
        {
            var reader = new Reader(
                new StreamReader(new MemoryStream("ABCDEF,123456"u8.ToArray())), new() {
                    BufferSize = 5
                });
            Assert.Throws<InvalidDataException>(() => reader.ReadRow());
        }

        [Fact]
        public void ThrowsWhenCantReadQuotedFieldDueToInsufficientBufferLength ()
        {
            var test = "\"xxx\"";
            var reader = new Reader(
                new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(test))), new() {
                    BufferSize = 2
                });
            Assert.Throws<InvalidDataException>(() => reader.ReadRow());
        }

        [Fact]
        public void DoesntThrowsWhenCantReadQuotedFieldDueToInsufficientBufferLengthWhileEof ()
        {
            var stream = new MemoryStream("\"x\n"u8.ToArray());
            var reader = new Reader(new StreamReader(stream), new() { BufferSize = 2 });
            reader.ReadRow();
            Assert.Equal("x", reader[0]);
        }

        [Fact]
        public void CanReadQuotedWithMinBuffer ()
        {
            var stream = new MemoryStream("\"x\",y"u8.ToArray());
            var reader = new Reader(new StreamReader(stream), new() { BufferSize = 3 });
            reader.ReadRow();
            reader.ReadRow();
            Assert.Equal("y", reader[1]);
        }
    }
}
