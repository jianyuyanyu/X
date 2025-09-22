# DbTable ʹ��˵��

`NewLife.Data.DbTable` ��һ���������ı������������֧�ִ����ݿ��ȡ��������/Xml/Csv ���л����� `DataTable` ��ת���Լ�ģ�Ͷ�������֮���ӳ�䡣

- �����ռ䣺`NewLife.Data`
- ��Ҫ���ͣ�`DbTable`��`DbRow`
- ���ͳ�����
  - �� `IDataReader`/`DbDataReader` ��ȡ��ѯ���
  - �� `DataTable` ��ת������� ADO.NET �Ļ�����
  - �����������л�Ϊ�����ơ�XML��CSV �����ݰ�
  - ��ģ���б�д��Ϊ���򽫱��ȡΪģ���б�

## ���ĳ�Ա

- �ж���
  - `string[] Columns` ��������
  - `Type[] Types` �����ͼ���
- ����
  - `IList<object?[]> Rows` �м���
  - `int Total` ������
- ��ȡ
  - `Read(IDataReader dr)` / `ReadAsync(DbDataReader dr)`
  - `ReadData(IDataReader dr, int[]? fields = null)`������ȡָ���У�`fields[i]` ��ʾԴ����������ӳ�䵽Ŀ���� `i`
  - `Read(DataTable table)` �� `DataTable` ��ȡ
  - �����ƶ�ȡ��`Read(Stream)`��`ReadHeader(Binary)`��`ReadData(Binary, rows)`��`ReadRows(Binary, rows)`
  - �ļ�/���ݰ���`LoadFile(path)`��`LoadRows(path)`��`Read(IPacket)`��`Read(byte[])`
- д��
  - `Write(Stream)`/`WriteHeader(Binary)`/`WriteData(Binary)`/`WriteData(Binary, int[] fields)`
  - `WriteRows(Binary, IEnumerable<object?[]>, int[]? fields = null)`/`WriteRow(...)`
  - `SaveFile(path)`/`SaveRows(path, rows, fields)`
  - `ToPacket()` ת���ݰ�
- ת��
  - `ToDataTable()`/`Write(DataTable)`
  - `ToJson(...)`/`WriteXml(Stream)`/`GetXml()`/`SaveCsv(path)`/`LoadCsv(path)`
- ģ��ӳ��
  - `WriteModels<T>(IEnumerable<T>)` ��ģ��д��Ϊ���������������ԣ�
  - `Cast<T>(IEnumerable<T>)` ��ģ�Ͱ���˳��תΪ��
  - `ReadModels<T>()`/`ReadModels(Type)` ����תΪģ���б�
- ����
  - `Get<T>(rowIndex, name)`/`TryGet<T>(rowIndex, name, out value)`
  - `GetColumn(name)` ��������������
  - ö�٣�`foreach (var row in table)`��ÿ�� `row` �� `DbRow`

## ��������

### 1) �����ݿ��ȡ

```csharp
using var cmd = connection.CreateCommand();
cmd.CommandText = "select Id, Name, CreateTime from User";
using var reader = cmd.ExecuteReader();

var table = new DbTable();
table.Read(reader);

Console.WriteLine(table.Total);       // ����
Console.WriteLine(table.Columns[0]);  // ����
```

��ѡ�񲿷��У�

```csharp
// fields: ��Ŀ���� i ӳ�䵽Դ reader �������� fields[i]
// ����ֻ���� 0 �͵� 2 ��
int[] fields = [0, 2];
var table = new DbTable();
table.ReadHeader(reader);      // �ȶ�ȡ�ж���
table.ReadData(reader, fields);
```

### 2) �� DataTable ��ת

```csharp
var dataTable = table.ToDataTable();
var table2 = new DbTable();
table2.Read(dataTable);
```

### 3) ���л�

- �����ƣ�

```csharp
using var fs = File.Create("users.db");
table.SaveFile("users.db"); // �� table.Write(fs)

var t2 = new DbTable();
t2.LoadFile("users.db");
```

- XML/Csv��

```csharp
var xml = table.GetXml();
table.SaveCsv("users.csv");
```

- ���ݰ���

```csharp
var pk = table.ToPacket();
```

### 4) ģ��ӳ��

```csharp
public sealed class User
{
    public Int32 Id { get; set; }
    public String Name { get; set; } = "";
    public DateTime CreateTime { get; set; }
}

// ģ�� -> ��
var users = new List<User> { new() { Id = 1, Name = "Stone", CreateTime = DateTime.UtcNow } };
var table = new DbTable();
table.WriteModels(users);

// �� -> ģ��
var list = table.ReadModels<User>().ToList();
```

## ����˵��

- `fields` ӳ�����
  - ��ȡ��`ReadData(dr, fields)` ��Ŀ���� `i` ӳ�䵽Դ������ `fields[i]`����ֵ�ᰴԴ���������Ĭ��ֵ����ֵ 0��`false`��`DateTime.MinValue` �ȣ���
  - д�룺`WriteData(bn, fields)`/`WriteRow(bn, row, fields)` ��Ŀ���� `i` д��Դ�е� `row[fields[i]]`���� `fields[i] == -1` ��Ŀ��������д���ֵ��
- ����Ե�������ʽ���ѣ�`ReadRows(bn, -1)` �ɳ�����ȡ����ĩβ��
- `DbRow` �ṩ��ݷ��ʣ�`row.Get<T>("Name")`��

## ������ע��

- �����Ƹ�ʽͷ��������汾����ǰ�汾 `3`����ǰ���ݾɰ汾ʱ��д���ʽ��
- MySQL �������ڣ��� `0000-00-00`�������ڶ�ȡʱ�쳣���ڲ����� `try/catch` ���Բ����Ĭ��ֵ��
- ������·��������� LINQ/���䣻���͡��������ѻ����� `DbTable` �� `Columns`/`Types`��

## ���ժҪ�������ع���

- �޸� `ReadData/ReadDataAsync` �� `fields` ӳ�䳡���µ�����������λ���⡣
- �޸� `WriteData(Binary, int[])` �� `WriteRow(Binary, object?[], int[]?)` �� `idx < 0` ʱԽ����� `ts[idx]` �����⣬��Ϊ��Ŀ��������д���ֵ��
- `Read(DataTable)` ���� `Total` ��������ȡ��ʽ����һ�¡�
- `GetXml()` ��Ϊͬ���ȴ� `WriteXml` ��ɣ����� `Wait(15000)` ���ܵ������ݲ�������
- �Ľ�ö����ʵ�֣�֧�� `Reset()` ������ö�١�

## �ο�

- �ĵ���https://newlifex.com/core/dbtable
- �����ռ䣺`NewLife.Data`
