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
#### 命名空间: Neko.Utility.Core.Common
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
#### 命名空间: Neko.Utility.Core.Common
该类封装了一些对于二维码、条形码的生成，读取方法

------
#### <a id="GenerateCodeConfiguration">生成二维码配置信息</a>
#### 命名空间: Neko.Utility.Core.Common
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
#### 输出结果
<small>抱歉我懒得上传图片</small>