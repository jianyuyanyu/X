﻿using NewLife;
using NewLife.Data;
using NewLife.Log;
using NewLife.Serialization;
using Xunit;

namespace XUnitTest.Serialization;

public class JsonWriterTests
{
    [Fact]
    public void Utc_Time()
    {
        var writer = new JsonWriter();

        var dt = DateTime.UtcNow;
        writer.Write(new { time = dt });

        var str = writer.GetString();
        Assert.NotEmpty(str);

        var js = new JsonParser(str);
        var dic = js.Decode() as IDictionary<String, Object>;
        Assert.NotNull(dic);

        var str2 = dic["time"];
        Assert.EndsWith(" UTC", str2 + "");
        Assert.Equal(dt.ToFullString() + " UTC", str2);

        var dt2 = dic["time"].ToDateTime();
        Assert.Equal(DateTimeKind.Utc, dt2.Kind);
        Assert.Equal(dt.Trim(), dt2.Trim());
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void UseUtc_Setting(Boolean useUTCDateTime)
    {
        var writer = new JsonWriter { UseUTCDateTime = useUTCDateTime };

        var dt = DateTime.Now;
        writer.Write(new { time = dt });

        var str = writer.GetString();
        Assert.NotEmpty(str);

        var js = new JsonParser(str);
        var dic = js.Decode() as IDictionary<String, Object>;
        Assert.NotNull(dic);

        if (useUTCDateTime)
        {
            var str2 = dic["time"];
            Assert.EndsWith(" UTC", str2 + "");
            Assert.Equal(dt.ToUniversalTime().ToFullString() + " UTC", str2);

            var dt2 = dic["time"].ToDateTime();
            Assert.Equal(DateTimeKind.Utc, dt2.Kind);
            Assert.Equal(dt.ToUniversalTime().Trim(), dt2.Trim());
        }
        else
        {
            var str2 = dic["time"];
            Assert.False((str2 + "").EndsWith(" UTC"));
            Assert.Equal(dt.ToFullString(), str2);

            var dt2 = dic["time"].ToDateTime();
            Assert.NotEqual(DateTimeKind.Utc, dt2.Kind);
            Assert.Equal(dt.Trim(), dt2.Trim());
        }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void FullTime_Setting(Boolean fullTime)
    {
        var writer = new JsonWriter();
        writer.Options.FullTime = fullTime;

        var dt = DateTime.Now;
        writer.Write(new { time = dt });

        var str = writer.GetString();
        Assert.NotEmpty(str);

        var js = new JsonParser(str);
        var dic = js.Decode() as IDictionary<String, Object>;
        Assert.NotNull(dic);

        if (fullTime)
        {
            Assert.Contains("T", str);
            Assert.Contains("+", str);

            var dto = DateTimeOffset.Now;

            var str2 = dic["time"];
            // +08:00
            Assert.EndsWith(dto.Offset.ToString(), str2 + ":00");
            Assert.Equal($"{dt:yyyy-MM-ddTHH:mm:ss.fffffff}+{dto.Offset.Hours:00}:00", str2);

            var dt2 = dic["time"].ToDateTime();
            Assert.Equal(DateTimeKind.Local, dt2.Kind);
            Assert.Equal(dt.Trim(), dt2.Trim());
        }
        else
        {
            var str2 = dic["time"];
            Assert.False((str2 + "").EndsWith(" UTC"));
            Assert.Equal(dt.ToFullString(), str2);

            var dt2 = dic["time"].ToDateTime();
            Assert.Equal(DateTimeKind.Unspecified, dt2.Kind);
            Assert.Equal(dt.Trim(), dt2.Trim());
        }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void LowerCase_Setting(Boolean lowerCase)
    {
        var writer = new JsonWriter { LowerCase = lowerCase };

        writer.Write(new { UserName = "Stone" });

        var str = writer.GetString();
        var js = new JsonParser(str);
        var dic = js.Decode() as IDictionary<String, Object>;

        var key = dic.Keys.First();
        if (lowerCase)
            Assert.Equal("username", key);
        else
            Assert.Equal("UserName", key);
        Assert.Equal("Stone", dic["UserName"]);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void CamelCase_Setting(Boolean camelCase)
    {
        var writer = new JsonWriter();
        writer.Options.CamelCase = camelCase;

        writer.Write(new { UserName = "Stone" });

        var str = writer.GetString();
        var js = new JsonParser(str);
        var dic = js.Decode() as IDictionary<String, Object>;

        var key = dic.Keys.First();
        if (camelCase)
            Assert.Equal("userName", key);
        else
            Assert.Equal("UserName", key);
        Assert.Equal("Stone", dic["UserName"]);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void IgnoreNullValues_Setting(Boolean ignoreNullValues)
    {
        var writer = new JsonWriter { IgnoreNullValues = ignoreNullValues };

        writer.Write(new { Name = "", UserName = "Stone" });

        var str = writer.GetString();
        var js = new JsonParser(str);
        var dic = js.Decode() as IDictionary<String, Object>;

        var key = dic.Keys.First();
        if (ignoreNullValues)
            Assert.Equal("UserName", key);
        else
            Assert.Equal("Name", key);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void IgnoreReadOnlyProperties_Setting(Boolean ignoreReadOnlyProperties)
    {
        var writer = new JsonWriter { IgnoreReadOnlyProperties = ignoreReadOnlyProperties };

        writer.Write(new Model("Stone", "PPP"));

        var str = writer.GetString();
        var js = new JsonParser(str);
        var dic = js.Decode() as IDictionary<String, Object>;

        var key = dic.Keys.Last();
        if (ignoreReadOnlyProperties)
            Assert.Equal("Name", key);
        else
            Assert.Equal("Password", key);
    }

    class Model(String name, String pass)
    {
        public String Name { get; set; } = name;
        public String Password { get; } = pass;
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void EnumTest(Boolean enumString)
    {
        // 字符串
        var writer = new JsonWriter();
        writer.Options.EnumString = enumString;

        var data = new { Level = LogLevel.Fatal };
        writer.Write(data);

        var js = new JsonParser(writer.GetString());
        var dic = js.Decode() as IDictionary<String, Object>;
        Assert.NotNull(dic);

        if (enumString)
            Assert.Equal("Fatal", dic["Level"]);
        else
        {
            Assert.Equal(5, dic["Level"]);
            Assert.Equal((Int32)LogLevel.Fatal, dic["Level"].ToInt());
        }
    }

    [Fact]
    public void ArrayTest()
    {
        var arr = new[] { 12, 34, 56, 78 };
        var str = JsonWriter.ToJson(arr);
        Assert.Equal("[12,34,56,78]", str);
    }

    [Fact]
    public void Array_匿名()
    {
        var arr = new[] { 12, 34, 56, 78 };
        var str = JsonWriter.ToJson(arr.Select(e => e + 100));
        Assert.Equal("[112,134,156,178]", str);
    }

    [Fact]
    public void Array_DbTable()
    {
        var dt = new DbTable
        {
            Columns = new[] { "id1", "id1", "id1", "id1" },
            Rows = new List<Object[]>(),
            Total = 1234,
        };
        dt.Rows.Add(new Object[] { 12, 34, 56, 78 });
        dt.Rows.Add(new Object[] { 87, 65, 43, 32 });

        var str = JsonWriter.ToJson(dt);
        Assert.Equal("{\"Columns\":[\"id1\",\"id1\",\"id1\",\"id1\"],\"Rows\":[[12,34,56,78],[87,65,43,32]],\"Total\":1234}", str);
    }

    [Fact]
    public void UnicodeEncode()
    {
        var writer = new JsonWriter();

        writer.Write(new Model("Hello\u0001World", "智能Stone"));

        var str = writer.GetString();
        Assert.Equal("""{"Name":"Hello\u0001World","Password":"智能Stone"}""", str);

        var js = new JsonParser(str);
        var dic = js.Decode() as IDictionary<String, Object>;
        Assert.NotNull(dic);
        Assert.Equal("Hello\u0001World", dic["Name"]);
        Assert.Equal("智能Stone", dic["Password"]);
    }
}