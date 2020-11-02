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
    5. [日志](#Logger)<br/>
    	5.1 [日志类](#Logger)
    	5.2 [日志帮助类](#LogUtil) 
- <a id="Net">Net</a>
	1. [网络相关的帮助类](#NetUtil)
- <a id="Threading">Threading</a>
	1. [线程帮助类](#ThreadUtil)<br/>
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
str = "Hello World";<br/>
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
------

### <a id="CompressUtil">压缩帮助类</a>
#### 命名空间:Neko.Utility.Core.IO.CompressUtil
这个帮助类可以将文件压缩到zip文件里或解压zip文件,也可以用zip的形式压缩解压byte[]数组

------
### 压缩/解压文件
使用以下方法将文件/文件夹添加到待压缩文件列表内
- 如果是文件夹将会自动添加文件夹下的子文件夹和文件
```C#
public static void Add(string fileName)
public static void Add(System.IO.FileSystemInfo fileSystem)
public static void AddEntry(System.IO.FileSystemInfo fileSystem, ICSharpCode.SharpZipLib.Zip.ZipOutputStream zipStream, [string root = ])
```
使用以下方法将文件/文件夹从待压缩文件列表中移除
```C#
public static void Remove(string fileName)
public static void Remove(System.IO.FileSystemInfo fileSystem)
```
如果是操作已有的压缩文件,则需要使用以下方法先打开压缩文件,将压缩文件内的文件列表加载出来
```C#
public static ICSharpCode.SharpZipLib.Zip.ZipInputStream OpenZip(string fileName)
```
然后再使用添加文件的方法将文件加入压缩文件列表或使用移除文件的方法将文件移除。
使用以下方法可以开始压缩/解压缩zip文件
```C#
//压缩文件
public static void Compress(string zipFile)
public static void Compress(string zipFile, string passCode)
//解压文件
public static void Decompress(string zipFile, string unzipPath)
public static void Decompress(string zipFile, string unzipPath, string passCode)
```
以下方法可以校验zip文件是否有错误
```C#
public static bool VerifyArchive(string fileName)
```
在压缩/解压文件时，将会通过以下事件通知外部正在压缩/解压的进度
- public static event Neko.Utility.Core.CompressDelegateCode OnCompress
- public static event Neko.Utility.Core.CompressDelegateCode OnDecompress

------
### 压缩/解压byte[]数组
使用以下方法可以压缩/解压byte[]数组，在压缩/解压byte[]数组时，不会触发OnCompress/OnDecompress事件
```C#
public static byte[] CompressBytes(byte[] bytes)
public static byte[] DeCompressBytes(byte[] bytes)
```
### <a id="EncryptionUtil">加密帮助类</a>
#### 命名空间:Neko.Utility.Core.IO.EncryptionUtil
这里封装了一些常见的对称加密/非对称加密的方法<strong>AES/DES/RSA加密后的字符串均为Base64字符串</strong>
所有加密对象均可以是实体或数据
- AES加密/解密
```C#
//加密
public static byte[] EncryptAES(object content, string privateKey)
public static string EncryptAESToString(object content, string privateKey)
//解密
public static object DeEncryptAES(byte[] encryptBytes, string privateKey)
public static Tobject DeEncryptAES<Tobject>(string encryptContent, string privateKey)
```
- DES加密/解密
```C#
//加密
public static byte[] EncryptDES(object content, string privateKey)
public static string EncryptDESToString(object content, string privateKey)
//解密
public static object DeEncryptDES(byte[] encryptBytes, string privateKey)
public static Tobject DeEncryptDES<Tobject>(string encryptContent, string privateKey)
```
- RSA加密/解密
```C#
//加密
public static byte[] EncryptRSA(object content, string publicKey)
public static string EncryptRSAToString(object content, string publicKey)
//解密
public static object DeEncryptRSA(byte[] encryptBytes, string privateKey)
public static Tobject DeEncryptRSA<Tobject>(string encryptContent, string privateKey)
```
- MD5加密
```C#
public static string EncryptMD5(object content)
```
- SHA1加密
	- 生成一对随机的RSA公钥和私钥(在不想自己做公钥和私钥的时候可以使用，每次都是随机生成) public static (string, string) GeneralRSAKey()
```C#
public static string EncryptSHA1(object content)
```
- SHA256加密
```C#
public static string EncryptSHA256(object content)
```
- 特殊的加密/解密
	- 你可以使用自己的加密器来进行加密/解密，前提是你的加密/解密器实现了ICryptoTransform方法 
```C#
public static byte[] Encrypt(System.Security.Cryptography.ICryptoTransform cryptoTransform, byte[] value)
```
### 使用示例
```C#
TestClass test = new TestClass();
test.Name = "Hello MD5";
string md5Content = Neko.Utility.Core.IO.EncryptionUtil.EncryptMD5(test);
Console.WriteLine("md5Content:{0}",md5Content);
```
#### 输出结果
```Shell
-> #MD5String#
```
------
### <a id="RegularUtil">正则帮助类</a>
#### 命名空间:Neko.Utility.Core.IO.RegularUtil
# <strong><font color="red">特此声明:</font><strong>
# <strong><font color="red">因中国大陆手机号规则和中国台湾、中国香港、中国澳门手机号规则不同，所以分别以不同的变量存储了对应的正则表达式，中国大陆身份证(15位、18位)同理。</font><strong>
# <strong><font color="red">本人在此特别声明坚决维护中国统一，反对中国分裂，以防某些别有用心的人用这些内容来做文章。</font><strong>
这个帮助类封装了验证正则字符串的方法和从一个字符串内取出符合正则规则的字符串
- 属性
	-  CHINESE_CHARACTER 所有的中文字符的正则表达式
	-  EMAIL 邮箱地址的正则表达式
	-  HONGKONG_CELLPHONE 中国香港手机号的正则表达式
	-  IPADDRESS IPV4的正则表达式（为啥只有IPv4:因为IPv6太难太长了，我不会）
	-  MACAO_CELLPHONE 中国澳门手机号的正则表达式
	-  MAINLAND_CELLPHONE 中国大陆手机号的正则表达式
	-  MAINLAND_IDCARD_15 中华人民共和国居民身份证一代身份证(15位)的正则表达式
	-  MAINLAND_IDCARD_18 中华人民共和国居民身份证二代身份证(18位)的正则表达式
	-  TAIWAN_CELLPHONE 中国台湾手机号的正则表达式
	-  WEB_URL 网址的正则表达式
- 方法
用以下方法可以从一串字符串中取出符合正则表达式的内容
```C#
public static string Get(string value, string regex)
public static System.Collections.Generic.IEnumerable<string> GetAll(string value, string regex)
```
用以下方法则可以验证一个字符串是否符合正则表达式
```C#
public static bool VerifyRegex(string value, string regex)
```
### 使用示例
```C#
string str1 = "阿巴阿巴阿巴https://github.com/";
string str1_result = Neko.Utility.Core.IO.RegularUtil.Get(str1,Neko.Utility.Core.IO.RegularUtil.WEB_URL);
Console.WriteLine("URL:{0}",str1_result);
string str2 = "HelloWorld@Gmail.com";
bool str2_result = Neko.Utility.Core.IO.RegularUtil.VerifyEmail(str2);
Console.WriteLine("Is Email? {0}",str2_result);
```
#### 输出结果
```Shell
-> URL:https://github.com/
-> Is Email? True
```
------
### <a id="XmlUtil">Xml文档帮助类</a>(<small>注:上文中曾经出现的[Xml操作](#SerializeUtil)为操作Xml文件，而这里的Xml操作为操作Xml文档(即XmlDocument)</small>)
#### 命名空间:Neko.Utility.Core.IO.XmlUtil
这个帮助类可以快速获取一个System.Xml.XmlElement或者快速从System.Xml.XmlElement中获取System.Xml.XmlAttribute的值
你可以使用以下方法判断一个XmlNodeList是否为空
```C#
public static bool IsNullOrEmpty(System.Xml.XmlNodeList nodeList)
```
你也可以通过以下方法设置XmlElement元素上一个特性的值
```C#
public static void Set(System.Xml.XmlElement xmlElement, string attributeName, object attributeValue)
```
或者使用以下方法获取XmlElement元素上特性的值
```C#
public static string Get(System.Xml.XmlElement xmlElement, string attributeName)
public static Tvalue Get<Tvalue>(System.Xml.XmlElement xmlElement, string attributeName)
```
------
### <a id="Logger">日志类</a>
#### 命名空间:Neko.Utility.Core.IO.Logging.Logger
这个类是一个可实例化的帮助类，它可以帮助记录日志和当前日志开始记录时到下一次提交日志时的耗时。耗时记录将在日志类被实例化的时候开始计算。
你可以使用以下方法来进行单次计时(只会重置从上一次提交日志到此次提交日志的耗时间隔)
- 参数 [日志配置信息](#LogConfiguration)
- 参数 [日志等级](#LogLevel)
```C#
public void Commit()
public void Commit(Neko.Utility.Core.Configurations.LogLevel logLevel, string logMessage, params object[] messageParameters)
public void CommitException(string logMessage, params object[] messageParameters)
public void CommitException(System.Exception innerException)
public void CommitInformation(string logMessage, params object[] messageParameters)
public void CommitTrack(string logMessage, params object[] messageParameters)
public void CommitWarning(string logMessage, params object[] messageParameters)
```
你可以使用以下方法调用[日志帮助类](#LogUtil)将已记录的日志输出到日志文件并重置所有计时
```C#
public void WriteLog()
```
------
### <a id="LogUtil">日志帮助类</a>
#### 命名空间:Neko.Utility.Core.IO.Logging.LogUtil
这个帮助类是[日志类](#Logger)的补充，可以将日志信息输出到文件中
你可以使用以下方法将日志添加到日志队列，将会自动启用一个线程将队列中的日志信息输出到文件
- 参数 [日志配置信息](#LogConfiguration)
- 参数 [日志等级](#LogLevel)
```C#
public static void WriteException(System.Exception innerException, [string caption = null])
public static void WriteInformation(string logMessage, params object[] messageParameters)
public static void WriteLog(Neko.Utility.Core.Configurations.LogLevel logLevel, [string logMessage = null], [System.Exception innerException = null])
public static void WriteWarning(System.Exception warningException, string logMessage, params object[] messageParameters)
```
------
### <a id="LogConfiguration">日志配置信息</a>
#### 命名空间:Neko.Utility.Core.Configurations.LogConfiguration
- 属性:
	- AddConsole 是否输出到控制台
	- AddDebug 是否输出到编译器的Debug窗口
	- WriteToEventLog 是否输出到Windows的系统日志(仅在Windows系统下有效)
	- NoLocalFile 是否不输出本地日志文件
	- LogPath 记录日志的路径
	- LogFileName 本地日志文件的文件名
	- LogLevel [日志等级](#LogLevel)，用于控制输出哪些日志
	- RecordMinimumInterval 记录日志最小间隔(单位毫秒)当两次提交的日志耗时小于此间隔时，将不会记录此日志
	- Instance 配置信息的单例
- 方法
	- GetConfiguration() 获取日志配置信息的单例

------
### <a id="LogLevel">日志等级</a>
#### 命名空间:Neko.Utility.Core.Configurations.LogLevel
- 成员
	- Track 步骤日志，一般用于开发时记录执行步骤信息
	- Information 普通信息日志
	- Warning 警告信息日志
	- Exception 异常信息日志

------
### <a id="NetUtil">网络相关的帮助类</a>
#### 命名空间:Neko.Utility.Core.Net.NetUtil
这个帮助类可以获取本机的IP、查询一个站点的延迟、发送基于smtp/POP协议的邮件
你可以使用以下方法获取本机的IP地址(注:获取局域网IP时，如果包含有多个网卡，默认获取第一个符合IPv4地址的网卡的地址)
- 参数 internetIp 表示是否获取公网IP地址 false为获取局域网地址，默认为false
```C#
public static string GetIP([bool internetIp = False])
```
你可以使用以下方法获取本机与公网的延迟
```C#
public static int Ping()
public static int Ping(string host)
public static System.Net.NetworkInformation.PingReply Ping(string host, int timeout)
```
当你想要使用此帮助类发送邮件时，你可以使用以下方法创建一个邮件对象
```C#
public static System.Net.Mail.MailMessage CreateMailMessage(string senderAddress, [string title = ])
```
然后使用以下方法给邮件对象添加收信人
```C#
public static void AddReceiver(System.Net.Mail.MailMessage mailMessage, params string[] receiveAddress)
```
使用以下方法可以给邮件添加附件
```C#
public static void AddAttachment(System.Net.Mail.MailMessage mailMessage, params string[] filePath)
public static void AddAttachment(System.Net.Mail.MailMessage mailMessage, System.IO.FileInfo file)
```
使用以下方法输入邮件内容
```C#
public static void AppendMailBody(System.Net.Mail.MailMessage mailMessage, string content, params object[] args)
```
最后使用以下方法发送邮件
```C#
public static System.Threading.Tasks.Task SendEmailAsync(System.Net.Mail.MailMessage mailMessage, string password, [string proxy = smtp], [bool useSsl = False])
```
### 获取IP使用示例
```C#
string localIP = Neko.Utility.Core.Net.NetUtil.GetIP();
Console.WriteLine(localIP);
string netIP = Neko.Utility.Core.Net.NetUtil.GetIP(true);
Console.WriteLine(netIP);
```
#### 获取IP输出结果
```Shell
-> 192.168.*.*
-> 255.255.*.*
```
### 发送邮件使用示例
```C#
var mailMessage = Neko.Utility.Core.Net.NetUtil.CreateMailMessage("HelloWorld@Gmail.com");
Neko.Utility.Core.Net.NetUtil.AddReceiver(mailMessage,new string[]{"helloCSharp@gmail.com"});
Neko.Utility.Core.Net.NetUtil.AddAttachment(mailMessage,new FileInfo("test.txt"));
Neko.Utility.Core.Net.NetUtil.AppendMailBody(mailMessage,"yo CSharp :P ");
Neko.Utility.Core.Net.NetUtil.SendEmailAsync(mailMessage,"#Password#");
```
其他文档不定期更新中。。。