namespace Serenity.Data;

public class ReservedKeywordTests
{
    [Fact]
    public void SqlSyntax_Recognizes_Reserved_Keyword()
    {
        Assert.True(SqlSyntax.IsReservedKeywordForAny("SELECT"));
        Assert.False(SqlSyntax.IsReservedKeywordForAny("NOTAKEYWORD"));
    }

    [Fact]
    public void Dialects_Recognize_Reserved_Words()
    {
        var dialects = new ISqlDialect[]
        {
            FirebirdDialect.Instance,
            MySqlDialect.Instance,
            OracleDialect.Instance,
            PostgresDialect.Instance,
            SqliteDialect.Instance,
            SqlServer2000Dialect.Instance
        };

        foreach (var d in dialects)
        {
            Assert.True(d.IsReservedKeyword("SELECT"));
            Assert.False(d.IsReservedKeyword("NOTAKEYWORD"));
        }
    }
}
