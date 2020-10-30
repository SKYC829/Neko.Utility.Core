using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Neko.Utility.Core.IO
{
    /// <summary>
    /// 正则帮助类
    /// <para>封装了验证正则字符串的方法和从一个字符串内取出符合正则规则的字符串</para>
    /// </summary>
    public sealed class RegularUtil
    {
        /// <summary>
        /// 匹配邮箱地址的正则表达式
        /// </summary>
        public const string EMAIL = @"\w[-\w.+]*@([A-Za-z0-9][-A-Za-z0-9]+\.)+[A-Za-z]{2,14}";

        /// <summary>
        /// 中国大陆手机号的正则表达式
        /// </summary>
        public const string MAINLAND_CELLPHONE = @"1[3-8][0-9]{9}";

        /// <summary>
        /// 中国台湾手机号的正则表达式
        /// </summary>
        public const string TAIWAN_CELLPHONE = @"09[0-9]{8}";

        /// <summary>
        /// 中国香港手机号的正则表达式
        /// </summary>
        public const string HONGKONG_CELLPHONE = @"(5|6|8|9)[0-9]{7}";

        /// <summary>
        /// 中国澳门手机号的正则表达式
        /// </summary>
        public const string MACAO_CELLPHONE = @"6(6|8)[0-9]{5}";

        /// <summary>
        /// IPV4的正则表达式
        /// </summary>
        public const string IPADDRESS = @"(25[0-5]|2[0-4]\d|[0-1]\d{2}|[1-9]?\d)\.(25[0-5]|2[0-4]\d|[0-1]\d{2}|[1-9]?\d)\.(25[0-5]|2[0-4]\d|[0-1]\d{2}|[1-9]?\d)\.(25[0-5]|2[0-4]\d|[0-1]\d{2}|[1-9]?\d)";

        /// <summary>
        /// 中华人民共和国居民身份证二代身份证(18位)的正则表达式
        /// </summary>
        public const string MAINLAND_IDCARD_18 = @"[1-9]\d{5}(18|19|([23]\d))\d{2}((0[1-9])|(10|11|12))(([0-2][1-9])|10|20|30|31)\d{3}[0-9Xx]";

        /// <summary>
        /// 中华人民共和国居民身份证一代身份证(15位)的正则表达式
        /// </summary>
        public const string MAINLAND_IDCARD_15 = @"[1-9]\d{5}\d{2}((0[1-9])|(10|11|12))(([0-2][1-9])|10|20|30|31)\d{3}";

        /// <summary>
        /// 网址的正则表达式
        /// </summary>
        public const string WEB_URL = @"((https|http|ftp|rtsp|mms)?:\/\/)[^\s]+";

        /// <summary>
        /// 中文字符
        /// </summary>
        public const string CHINESE_CHARACTER = @"[\u4e00-\u9fa5]+";

        /// <summary>
        /// 校验一个字符串是否符合正则规范
        /// </summary>
        /// <param name="value">要校验的字符串</param>
        /// <param name="regex">正则表达式</param>
        /// <returns></returns>
        public static bool VerifyRegex(string value, string regex)
        {
            return Regex.IsMatch(value, regex);
        }

        /// <summary>
        /// 验证邮件地址
        /// </summary>
        /// <param name="emailAddress">邮件地址字符串</param>
        /// <returns></returns>
        public static bool VerifyEmail(string emailAddress)
        {
            return VerifyRegex(emailAddress, EMAIL);
        }

        /// <summary>
        /// 验证手机号
        /// <para>支持中国大陆手机号、中国香港手机号、中国台湾手机号、中国澳门手机号</para>
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public static bool VerifyPhone(string phoneNumber)
        {
            return VerifyRegex(phoneNumber, string.Format(@"^{0}|{1}|{2}|{3}$", MAINLAND_CELLPHONE, TAIWAN_CELLPHONE, HONGKONG_CELLPHONE, MACAO_CELLPHONE));
        }

        /// <summary>
        /// 验证IPv4地址
        /// </summary>
        /// <param name="ipAddress">IPv4地址字符串</param>
        /// <returns></returns>
        public static bool VerifyIPv4(string ipAddress)
        {
            return VerifyRegex(ipAddress, IPADDRESS);
        }

        /// <summary>
        /// 验证身份证号
        /// <para>支持中华人民共和国居民身份证的一代身份证(15位)、
        /// 二代身份证(18位)</para>
        /// </summary>
        /// <param name="idCard">身份证号</param>
        /// <returns></returns>
        public static bool VerifyIDCard(string idCard)
        {
            return VerifyRegex(idCard, string.Format(@"^{0}|{1}$", MAINLAND_IDCARD_15, MAINLAND_IDCARD_18));
        }

        /// <summary>
        /// 验证网址
        /// <para>可以识别的协议:https、http、ftp、rtsp、mms</para>
        /// </summary>
        /// <param name="webUrl">网址</param>
        /// <returns></returns>
        public static bool VerifyWebUrl(string webUrl)
        {
            return VerifyRegex(webUrl, WEB_URL);
        }

        /// <summary>
        /// 获取字符串中符合正则规则的第一个字符串
        /// </summary>
        /// <param name="value">要校验的字符串</param>
        /// <param name="regex">正则表达式</param>
        /// <returns></returns>
        public static string Get(string value, string regex)
        {
            return GetAll(value, regex).FirstOrDefault();
        }

        /// <summary>
        /// 取出字符串中符合正则规则的所有字符串
        /// </summary>
        /// <param name="value"></param>
        /// <param name="regex"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetAll(string value, string regex)
        {
            return Regex.Matches(value, regex).Select(p => p.Value).AsEnumerable();
        }
    }
}
