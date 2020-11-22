using Neko.Utility.Core.Data;
using Neko.Utility.Core.IO;
using Neko.Utility.Core.IO.Logging;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Neko.Utility.Core.Net
{
    public sealed partial class NetUtil
    {
        /// <summary>
        /// 创建一个<see cref="MailMessage"/>对象
        /// </summary>
        /// <param name="senderAddress">发信人地址</param>
        /// <param name="title">邮件标题</param>
        /// <returns></returns>
        public static MailMessage CreateMailMessage(string senderAddress, string title = "")
        {
            if (StringUtil.IsNullOrEmpty(senderAddress))
            {
                throw new ArgumentNullException(nameof(senderAddress), "发信人地址不允许为空！");
            }
            if (!RegularUtil.VerifyEmail(senderAddress))
            {
                throw new ArgumentException("发件人地址错误！", nameof(senderAddress));
            }
            MailMessage result = new MailMessage();
            result.From = new MailAddress(senderAddress);
            result.Subject = title;
            result.IsBodyHtml = true;
            result.BodyEncoding = Encoding.Default;
            result.Priority = MailPriority.Normal;
            return result;
        }

        /// <summary>
        /// 添加收信人
        /// </summary>
        /// <param name="mailMessage">要添加收信人的<see cref="MailMessage"/>对象</param>
        /// <param name="receiveAddress">收信人地址</param>
        public static void AddReceiver(MailMessage mailMessage, params string[] receiveAddress)
        {
            if (mailMessage == null)
            {
                throw new NullReferenceException("MailMessage未初始化！");
            }
            foreach (string address in receiveAddress)
            {
                if (string.IsNullOrEmpty(address))
                {
                    continue;
                }
                if (!RegularUtil.VerifyEmail(address))
                {
                    throw new ArgumentException("收信人地址错误！", nameof(receiveAddress));
                }
                mailMessage.To.Add(address);
            }
        }

        /// <summary>
        /// 添加邮件正文内容
        /// </summary>
        /// <param name="mailMessage">要添加正文的<see cref="MailMessage"/>对象</param>
        /// <param name="content">邮件内容</param>
        /// <param name="args">邮件内容参数</param>
        public static void AppendMailBody(MailMessage mailMessage, string content, params object[] args)
        {
            if (mailMessage == null)
            {
                throw new NullReferenceException("MailMessage未初始化！");
            }
            StringBuilder sb = new StringBuilder(mailMessage.Body);
            sb.AppendFormat(content, args);
            mailMessage.Body = sb.ToString();
        }

        /// <summary>
        /// 添加附件
        /// </summary>
        /// <param name="mailMessage">要添加附件的的<see cref="MailMessage"/>对象</param>
        /// <param name="filePath">附件列表</param>
        public static void AddAttachment(MailMessage mailMessage, params string[] filePath)
        {
            foreach (string file in filePath)
            {
                AddAttachment(mailMessage, new FileInfo(file));
            }
        }

        /// <summary>
        /// <inheritdoc cref="AddAttachment(MailMessage, string[])"/>
        /// </summary>
        /// <param name="mailMessage">要添加附件的的<see cref="MailMessage"/>对象</param>
        /// <param name="file">附件文件</param>
        public static void AddAttachment(MailMessage mailMessage, FileInfo file)
        {
            if (mailMessage == null)
            {
                throw new NullReferenceException("MailMessage未初始化！");
            }
            if (!file.Exists)
            {
                throw new FileNotFoundException("文件不存在！无法添加至附件！", file.Name);
            }
            Attachment attachment = new Attachment(file.FullName);
            ContentDisposition contentDisposition = attachment.ContentDisposition;
            contentDisposition.CreationDate = file.CreationTime;
            contentDisposition.ModificationDate = file.LastWriteTime;
            contentDisposition.ReadDate = file.LastAccessTime;
            mailMessage.Attachments.Add(attachment);
        }

        /// <summary>
        /// 异步发送邮件
        /// </summary>
        /// <param name="mailMessage"><see cref="MailMessage"/>对象，包含了邮件信息</param>
        /// <param name="password">发信人密码</param>
        /// <param name="proxy">邮件服务器协议（通常是smtp、pop3）</param>
        /// <param name="useSsl">是否要求使用Ssl</param>
        /// <returns></returns>
        public static async Task SendEmailAsync(MailMessage mailMessage, string password, string proxy = "smtp", bool useSsl = false)
        {
            if (mailMessage == null)
            {
                throw new NullReferenceException("MailMessage未初始化！");
            }
            if (mailMessage.From == null)
            {
                throw new ArgumentException("请先添加发信人！");
            }
            if (mailMessage.To.Count <= 0)
            {
                throw new ArgumentException("请先添加收信人");
            }
            try
            {
                SmtpClient smtpClient = new SmtpClient();
                string senderHost = mailMessage.From.Address;
                int hostStart = senderHost.LastIndexOf('@');
                string host = string.Empty;
                if (proxy.Contains('.'))
                {
                    host = proxy;
                }
                else
                {
                    host = string.Format("{0}{1}", proxy, senderHost.Substring(hostStart, senderHost.Length - hostStart).Replace('@', '.'));
                }
                smtpClient.Host = host;
                smtpClient.EnableSsl = useSsl;
                smtpClient.UseDefaultCredentials = true;
                smtpClient.Credentials = new NetworkCredential(mailMessage.From.Address, password);
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                if (ex.Message == "need EHLO and AUTH first" && !useSsl)
                {
                    LogUtil.WriteWarning(ex, "发送邮件失败！邮件服务器要求启用Ssl，正在启用Ssl并重试！");
                    await SendEmailAsync(mailMessage, password, proxy, true);
                }
                else if (ex.Message.Contains("mail from address must be same as authorization user"))
                {
                    throw new Exception("发送邮件失败!请检查邮箱是否启用了SMTP/POP服务,并且确认密码使用了专用的安全密码！");
                }
                else
                {
                    throw ex;
                }
            }
        }
    }
}
