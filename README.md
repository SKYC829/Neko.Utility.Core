## <a id="Neko.Utility">Neko.Utility.Core</a>

 A Utility project for netcore

-----

[目录]

- <a id="Common">Common</a>
    1. [调用代码帮助类](#InvokeCode)
    2. [二维码/条形码帮助类](#QrCodeUtil)
       - [生成二维码配置类](#GenerateCodeConfiguration)
    3. [随机数帮助类](#RandomUtil)
    4. [反射帮助类](#ReferenceUtil)
    5. [序列化帮助类](#SerializeUtil)
- <a id="Data">Data</a>
    1. [键值对帮助类](#DictionaryUtil)
    2. [枚举帮助类](#EnumUtil)
    3. [Object对象帮助类](#ObjectUtil)
    4. [DataRow/DataTable帮助类](#RowUtil)
    5. [字符串帮助类](#StringUtil)
- <a id="IO">IO</a>
    1. [压缩帮助类](#CompressUtil)
    2. [加密帮助类](#EncryptionUtil)
    3. [正则帮助类](#RegularUtil)
    4. [XML帮助类](#XmlUtil)
    5. [日志](#Logging)<br/>
    	5.1 [日志类](#Logger)<br/>
    		5.1.1 [日志信息](#LogInfo)<br/>
    	5.2 [日志帮助类](#LogUtil) 
- <a id="Net">Net</a>
	1. [网络相关的帮助类](#NetUtil)
- <a id="Threading">Threading</a>
	1. [线程帮助类](#ThreadUtil)
		1.1 [线程信息的实体类对象](#IntervalInfo)
- <a id="DelegateCodes">DelegateCodes</a>
	1. [EmptyDelegateCode](#EmptyDelegateCode)<br/>
		1.1 [EmptyDelegateCode的重载](#EmptyDelegateCode_1)
	2. [ParameterDelegateCode](#ParameterDelegateCode)
	3. [CompressDelegateCode](#CompressDelegateCode)
- <a id="Extension">扩展方法</a>
	1. [扩展方法](#ExtensionCodes)
------
### <a id="InvokeCode">调用代码帮助类</a>
#### 命名空间: Neko.Utility.Core.Common.InvokeCode
该类封装了一些对于委托方法流水线式执行的方法。<strong>(该类需要先初始化(new)才能使用)</strong>
以下三个方法可以将一个无参的委托方法添加到执行流水线堆栈(CodeStacks)中。

```C#
public void Add(Neko.Utility.Core.EmptyDelegateCode executeCode)
public void Insert(int index, Neko.Utility.Core.EmptyDelegateCode executeCode)
public void Shift(Neko.Utility.Core.EmptyDelegateCode executeCode)
```
若要将一个方法移出执行流水线堆栈,可以使用下面的方法。
```C#
public void RemoveAt(int index)
public void Remove(Neko.Utility.Core.EmptyDelegateCode executeCode)
```
使用以下方法将会开始从执行流水线堆栈(CodeStacks)顶部向下顺序执行委托方法直到标记中断或堆栈中所有委托方法已执行完成
```C#
public void Execute() //同步执行
public Task ExecuteAsync([System.Threading.CancellationToken cancelToken = null]) //异步执行
```
使用以下方法可以仅执行堆栈中的第一个委托方法
```C#
public void ExecuteNext() //同步执行
public Task ExecuteNextAsync([System.Threading.CancellationToken cancelToken = null]) //异步执行
```

### 使用示例
```C#
static Neko.Utility.Core.Common.InvokeCode ic = new Neko.Utility.Core.Common.InvokeCode();

static void AddCode()
{
    for (int i = 0; i < 5; i++)
    {
        ic.Add(delegate ()
        {
            Console.WriteLine("我是第{0}个方法哦", i);
            Thread.Sleep(1000);
        });
    }
}
        
static async void RunCode()
{
    Task cancelTask = Task.Run(() =>{
        while(!Console.ReadKey().Key.Equals(ConsoleKey.Enter))
        {
            Console.WriteLine("Error Key");
        }
        Console.WriteLine("Cancel");
        _cancelToken.Cancel();
    });
    await ic.ExecuteAsync(_cancelToken.Token);
}
```
#### 输出结果
``` Shell
> 我是第0个方法哦
> 我是第1个方法哦
> 我是第2个方法哦
> 我是第3个方法哦
> 我是第4个方法哦
```
------
### <a id="QrCodeUtil">二维码帮助类</a>
#### 命名空间: Neko.Utility.Core.Common.QrCodeUtil
该类封装了一些对于二维码、条形码的生成，读取方法

------
#### <a id="GenerateCodeConfiguration">生成二维码配置信息</a>
#### 命名空间: Neko.Utility.Core.Configurations.GenerateCodeConfiguration
该配置文件设置了一些默认的二维码/条形码生成参数，可以用以下方法属性获取默认参数
```C#
public static Neko.Utility.Core.Configurations.GenerateCodeConfiguration BarCodeDefault { get; } //条形码的默认参数
public static Neko.Utility.Core.Configurations.GenerateCodeConfiguration QrCodeDefault { get; } //二维码的默认参数
```
------
以下方法可以生成一个条形码（一维码）
<small>当参数configuration为空时，将会使用[默认的条形码生成参数](#GenerateCodeConfiguration)</small>
```C#
public static Bitmap GenerateBarCode(string content, [Neko.Utility.Core.Configurations.GenerateCodeConfiguration configuration = null])
```
以下方法可以生成一个二维码
- <small>当参数configuration为空时，将会使用[默认的二维码生成参数](#GenerateCodeConfiguration)</small>
- <small>当参数logoPath或参数logo为空时，将不会为二维码绘制logo</small>
```C#
public static Bitmap GenerateQrCode(string content, [Neko.Utility.Core.Configurations.GenerateCodeConfiguration configuration = null])
public static Bitmap GenerateQrCode(string content, string logoPath, [Neko.Utility.Core.Configurations.GenerateCodeConfiguration configuration = null])
public static Bitmap GenerateQrCode(string content, System.Drawing.Bitmap logo, [Neko.Utility.Core.Configurations.GenerateCodeConfiguration configuration = null])
```
如果生成二维码后又想要在二维码上绘制logo，可以使用以下方法
<strong>参数matrixRectangle为一个矩阵，数据含义如下</strong>
- matrixRectangle[0] : logo距二维码左边距
- matrixRectangle[1] : logo距二维码上边距
- matrixRectangle[2] : 二维码的宽 (这里用来限制logo的最小宽度，默认值为三分之一)
- matrixRectangle[3] : 二维码的高 (这里用来限制logo的最小高度，默认值为三分之一)
<strong>如果参数configuration为空将不会绘制logo，直接返回原二维码</strong>
```C#
public static System.Drawing.Bitmap DrawCodeLogo(System.Drawing.Bitmap codeBitmap, int[] matrixRectangle, System.Drawing.Bitmap codeLogo, Neko.Utility.Core.Configurations.GenerateCodeConfiguration configuration)
```
### 使用示例
#### 输出结果
<small>抱歉我懒得上传图片</small>

------
### <a id="RandomUtil">随机数帮助类</a>
#### 命名空间:Neko.Utility.Core.Common.RandomUtil
该类封装了一些获取随机数的快速操作
你可以用以下方法快速的生成一组随机的Int数组
<small>参数count可以控制生成的数组的数量</small>
<small>参数canRepeat可以控制是否允许生成重复的数字</small>
```C#
public static int[] Next(int count, [bool canRepeat = False])
public static int[] Next(System.Random random, int count, [bool canRepeat = False])
```
### 使用示例
```C#
int[] results = Neko.Utility.Core.Common.RandomUtil.Next(5);
foreach (int result in results)
{
    Console.WriteLine(result);
}
```
#### 输出结果
```Shell
> 1
> 5
> 5
> 6
> 2
```
你可以使用以下方法实现一些抽奖场景的随机数生成操作
- <small>参数 items为随机抽取对象仓储(可以理解为奖池)</small>
- <small>参数 count为随机抽取的次数(比如幸运十连抽)</small>
- <small>参数 odds为抽取对象仓储(奖池)中每个对象的权重(从0-1，0永远不会被抽中,如果权重都是0的话除外)</small>
- <small>参数 oddsMap为绑定好权重的抽取对象仓储(奖池)</small>
```C#
public static Titem[] Draw<Titem>(System.Collections.Generic.IList<Titem> items, int count)
public static Titem[] Draw<Titem>(System.Collections.Generic.IList<Titem> items, System.Collections.Generic.IList<double> odds, int count)
public static Titem[] Draw<Titem>(System.Collections.Generic.IDictionary<Titem, double> oddsMap, int count)
```
### 使用示例
```C#
 List<string> items = new List<string>()
 {
     "圣剑",
     "高斯光剑",
     "沼跃鱼",
     "青钢剑",
     "黄金胖次",
     "以太斗篷"
 };
 List<double> itemsOdds = new List<double>()
 {
     0.3d,
     0.2d,
     0.6d,
     0.9d,
     0.1d,
     0.4d
 };
 var results = Neko.Utility.Core.Common.RandomUtil.Draw<string>(items, itemsOdds, 2);
foreach (var result in results)
{
    Console.WriteLine(result);
}
```
#### 输出结果
```Shell
> 开箱结果1:
> 以太斗篷
> 青钢剑
```
```Shell
> 开箱结果2:
> 以太斗篷
> 沼跃鱼
```
------
### <a id="ReferenceUtil">反射帮助类</a>
#### 命名空间:Neko.Utility.Core.Common.ReferenceUtil
你可以通过以下方法快速的加载程序集
```C#
public static System.Reflection.Assembly GetDefaultAssembly() //获取当前程序集
public static System.Reflection.Assembly GetAssembly(string assemblyName) //通过assemblyName自动判断是从文件加载程序集还是直接获取程序集
public static System.Reflection.Assembly GetAssemblyByName(string assemblyName) //通过名称获取程序集
public static System.Reflection.Assembly GetAssemblyByDll(string dllName) //从Dll文件加载程序集
```
你也可以用以下方法快速的从程序集中获取类型
```C#
public static System.Type GetType(string typeName)
public static System.Type GetType(System.Reflection.Assembly assembly, string typeName)
```
你还可以通过以下方法实例化类型对象
- <small>这两个方法都是通过构造函数来实例化对象，从某种意义上来说类似于可依赖注入的实例化</small>
```C#
public static object Instance(string typeName, params object[] constructParams)
public static Ttype Instance<Ttype>(string typeName, params object[] constructParams)
```
### 使用示例
#### 输出结果
------
### <a id="SerializeUtil">序列化帮助类</a>
#### 命名空间:Neko.Utility.Core.Common.SerializeUtil
你可以使用以下方法快速的将对象在二进制数组和对象之间转换
```C#
public static object FromBinary(byte[] binaryBytes)
public static object FromBinary(System.IO.Stream binaryStream)
public static Tobject FromBinary<Tobject>(byte[] binaryBytes)
public static Tobject FromBinary<Tobject>(System.IO.Stream binaryStream)
public static byte[] ToBinary(object fromObject)
```
### 使用示例
```C#
var binaryBytes = Neko.Utility.Core.Common.SerializeUtil.ToBinary("Hello World");
var str = Neko.Utility.Core.Common.SerializeUtil<string>(binaryBytes);
```
#### 输出结果
str = "Hello World";
你还可以用以下方法将对象在Xml、Json之间相互转换
```C#
#region xml相关
public static object FromXml(byte[] xmlBytes)
public static object FromXml(string xmlString)
public static object FromXml(System.IO.Stream xmlStream)
public static Tvalue FromXml<Tvalue>(byte[] xmlBytes)
public static Tvalue FromXml<Tvalue>(string xmlString)
public static Tvalue FromXml<Tvalue>(System.IO.Stream xmlStream)
public static byte[] ToXml(object fromObject)
#endregion
#region json相关
public static object FromJson(string jsonString)
public static Tobject FromJson<Tobject>(string jsonString)
public static string ToJson(object fromObject)
public static string ToJson(object fromObject, bool formatJson)
#endregion
```
### 使用示例
```C#
string json = Neko.Utility.Core.Common.SerializeUtil.ToJson("Hello World");
Console.WriteLine(json);
string str = Neko.Utility.Core.Common.SerializeUtil.FromJson(json);
Console.WriteLine(str);
```
#### 输出结果
```Shell
> [\"Hello World\"]
> Hello World
```
你还可以使用以下方法从Json字符串中快速获取一个节点的值
```C#
public static object GetJson(string jsonString, string key)
public static Tvalue GetJson<Tvalue>(string jsonString, string key)
```
### 使用示例
```C#
string json = "{\"field\":\"Hello World\"}";
string result = Neko.Utility.Core.Common.SerializeUtil.GetJson<string>(json,"field");
Console.WriteLine(result);
```
#### 输出结果
Hello World

------

### <a id="DictionaryUtil">键值对帮助类</a>
#### 命名空间:Neko.Utility.Core.Data.DictionaryUtil
你可以用以下方法将一个对象转换为键值对
```C#
public static System.Collections.IDictionary Convert(object target)
```
也可以用以下方法从键值对中获取某个key的值
```C#
public static object Get(System.Collections.IDictionary dictionary, string key)
public static Tvalue Get<Tkey, Tvalue>(System.Collections.Generic.IDictionary<Tkey, Tvalue> dictionary, Tkey key)
public static Tvalue Get<Tvalue>(System.Collections.Generic.IDictionary<string, Tvalue> dictionary, string key)
```
或者用以下方法将一个键值对按照键或者值进行排序
- <small>参数sortByValue表示是否使用键值对的值进行排序,默认为true</small>
- <small>方法限制了要求键值对的值必须是值类型,但是为了避免不必要的异常,建议用于排序的参数的类型为可以相减的类型(即值类型)</small>
```C#
public static System.Collections.Generic.IList<System.Collections.Generic.KeyValuePair<Tkey, Tvalue>> SortDictionary<Tkey, Tvalue>(System.Collections.Generic.IDictionary<Tkey, Tvalue> dictionary, [bool sortByValue = True])
```
------
### <a id="EnumUtil">枚举帮助类</a>
#### 命名空间:Neko.Utility.Core.Data.EnumUtil
这个帮助类可以让你快速的把一个对象转换为枚举类型,当然,前提是它可以是一个枚举类型
```C#
public static object Convert(System.Type enumType, string value)
public static object Convert(System.Type enumType, string value, object defaultValue)
public static TEnum Convert<TEnum>(string value)
public static TEnum Convert<TEnum>(string value, TEnum defaultValue)
```
------
### <a id="ObjectUtil">对象帮助类</a><small>(这个帮助类并不能帮你找到对象 :/ )</small>
#### 命名空间:Neko.Utility.Core.Data.ObjectUtil
这个帮助类包含了对于对象的一些快速操作,你可以用以下方法将一个对象转换为System.Data.DataRow添加到一个System.Data.DataTable中
```C#
public static System.Data.DataTable ToTable(object fromObject)
public static System.Data.DataTable AddTable(System.Data.DataTable dataTable, object fromObject)
public static System.Data.DataTable AddTable(System.Data.DataTable dataTable, object fromObject, int offset)
public static System.Data.DataTable AddTable(System.Data.DataTable dataTable, System.Collections.IList fromObjects)
public static System.Data.DataTable AddTable(System.Data.DataTable dataTable, System.Collections.IList fromObjects, int offset)
```
你也可以用以下方法将一个对象转换为另一个类型的对象
```C#
public static TObject Convert<TObject>(object fromObject)
public static TObject Convert<TObject>(object fromObject, TObject targetObject)
public static object WriteTo(object fromObject, object targetObject)
```
或者用以下方法从一个对象中获取值或设置值
```C#
public static object Get(object target)
public static void Set(object target, string fieldName, object fieldValue)
public static void Set(System.Type targetType, object target, string fieldName, object fieldValue)
```
------
### <a id="RowUtil">数据行帮助类</a>
#### 命名空间:Neko.Utility.Core.Data.RowUtil
这个帮助类可以帮你对System.Data.DataRow进行一些快速操作,比如你可以用以下方法向一个DataTable添加列
```C#
public static void AddColumn(System.Data.DataTable dataTable, params string[] columnNames)
public static void AddColumn(System.Data.DataTable dataTable, params System.Data.DataColumn[] dataColumns)
```
或者用以下方法从DataRow获取数据或给DataRow设置数据
- <small>给DataRow设置数据时如果数据所在的列不存在会自动添加列</small>
```C#
public static object Get(System.Data.DataRow dataRow, string columnName)
public static object Get(System.Data.DataRow dataRow, string columnName, System.Type valueType)
public static Tvalue Get<Tvalue>(System.Data.DataRow dataRow, string columnName)
public static void Set(System.Data.DataRow dataRow, string columnName, object fieldValue)
```
或者用以下方法将一个DataRow从DataTable中取出来
```C#
public static System.Data.DataRow GetFirstRow(System.Data.DataTable dataTable)
public static System.Data.DataRow GetRow(int index, System.Data.DataTable dataTable)
```
------
### <a id="StringUtil">字符串帮助类</a>
#### 命名空间:Neko.Utility.Core.Data.StringUtil
这个帮助类可以对字符串进行一些快速操作,相当于一个弱一些的[对象帮助类](#ObjectUtil)
例如你可以用以下方法对比两个类型是否一致
```C#
public static bool CompareType(System.Type fromType, string typeName)
public static bool CompareType(System.Type fromType, System.Type toType)
```
你也可以用以下方法将对象从一个类型转换为另一个类型(这里大多是值类型的操作)
```C#
public static object Get(System.Type targetType, object value)
public static TObject Get<TObject>(object value)
```
用这个方法可以判断一个对象是不是空的
```C#
public static bool IsNullOrEmpty(object value)
```
用以下方法可以对比两个对象是否一致(类似于JavaScript的'===')
```C#
public static bool SafeCompare(object fromValue, object toValue)
public static bool SafeCompare(object fromValue, object toValue, bool ignoreCase)
public static bool SafeCompare(string fromValue, string toValue)
public static bool SafeCompare(string fromValue, string toValue, bool ignoreCase)
```
用以下方法可以获取一个时间的十三位Unix时间戳或将十三位的Unix时间戳转换为一个时间
```C#
//DataTime -> TimeStamp
public static string GetTimeStamp()
public static System.DateTime GetTimeStamp(string timeStamp)
//TimeStamp -> DataTime
public static string GetTimeStamp(System.DateTime time)
```
其他文档不定期更新中。。。

