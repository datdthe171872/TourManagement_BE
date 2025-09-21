using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit.Security;
using MimeKit;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Text.RegularExpressions;
class PaymentIMAP
{
    private static readonly HttpClient _httpClient = new HttpClient();
    private const string ImapHost = "imap.gmail.com";
    private const int ImapPort = 993;
    private const string EmailAddress = "HieuNTHE171606@fpt.edu.vn";
    private const string AppPassword = "oynh flrj ktme zbvz";
    private const string WebhookUrl = "http://localhost:5298/api/PurchasedServicePackages/payment-webhook";

    public static async Task Main(string[] args)
    {
        await CheckEmailAndProcessPayment();
    }

    public static async Task CheckEmailAndProcessPayment()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("🔄 Bắt đầu kiểm tra email...");
        Console.WriteLine($"⏱️ Thời gian bắt đầu: {DateTime.Now}");

        ImapClient client = null;
        try
        {
            client = new ImapClient();
            client.Timeout = 60000;

            Console.WriteLine("🔗 Kết nối IMAP...");
            await client.ConnectAsync(ImapHost, ImapPort, SecureSocketOptions.SslOnConnect);
            Console.WriteLine("✅ Kết nối thành công");

            Console.WriteLine("🔐 Xác thực...");
            await client.AuthenticateAsync(EmailAddress, AppPassword);
            Console.WriteLine("✅ Xác thực thành công");

            var inbox = client.GetFolder("INBOX");

            if (inbox == null)
            {
                Console.WriteLine("🚨 Không thể truy cập thư mục INBOX. Kiểm tra xem email có hỗ trợ IMAP không.");
                return;
            }

            await inbox.OpenAsync(FolderAccess.ReadWrite);

            Console.WriteLine("📂 Đang mở hộp thư đến...");
            await inbox.OpenAsync(FolderAccess.ReadWrite);
            Console.WriteLine($"📊 Tổng số email: {inbox.Count}, Chưa đọc: {inbox.Unread}");

            var searchQuery = SearchQuery.NotSeen.And(SearchQuery.DeliveredAfter(DateTime.Now.AddDays(-7)));
            var uids = await inbox.SearchAsync(searchQuery);
            Console.WriteLine($"📨 Tìm thấy {uids.Count} email chưa đọc");

            foreach (var uid in uids)
            {
                try
                {
                    await ProcessEmailAsync(inbox, uid);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Lỗi khi xử lý email UID {uid}: {ex.Message}");
                }
            }

            Console.WriteLine("✅ Đã hoàn tất xử lý email");
        }
        catch (AuthenticationException ex)
        {
            Console.WriteLine($"🔒 Lỗi xác thực: {ex.Message}");
        }
        catch (ImapProtocolException ex)
        {
            Console.WriteLine($"📨 Lỗi giao thức IMAP: {ex.Message}");
        }
        catch (SocketException ex)
        {
            Console.WriteLine($"🌐 Lỗi kết nối mạng: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"🚨 Lỗi tổng quát: {ex.GetType().Name}: {ex.Message}");
        }
        finally
        {
            if (client != null)
            {
                if (client.IsConnected)
                {
                    Console.WriteLine("♻️ Ngắt kết nối...");
                    await client.DisconnectAsync(true);
                }
                client.Dispose();
            }

            Console.WriteLine($"⏱️ Thời gian kết thúc: {DateTime.Now}");
        }
    }

    private static async Task ProcessEmailAsync(IMailFolder inbox, UniqueId uid)
    {
        Console.WriteLine($"\n--- 📬 Đang xử lý email UID {uid} ---");

        var message = await inbox.GetMessageAsync(uid);
        if (message == null)
        {
            Console.WriteLine("⚠️ Không thể đọc nội dung email");
            return;
        }

        Console.WriteLine($"📩 From: {message.From.FirstOrDefault()}");
        Console.WriteLine($"📌 Subject: {message.Subject}");
        Console.WriteLine($"📅 Date: {message.Date.LocalDateTime}");

        string body = message.TextBody;
        if (string.IsNullOrWhiteSpace(body))
        {
            Console.WriteLine("⚠️ TextBody trống, thử dùng HtmlBody...");
            body = message.HtmlBody;

            if (!string.IsNullOrWhiteSpace(body))
            {
                body = StripHtmlTags(body);
            }
        }

        if (string.IsNullOrWhiteSpace(body))
        {
            Console.WriteLine("📭 Không có nội dung hợp lệ");
            await MarkAsReadAsync(inbox, uid);
            return;
        }

        if (body.Contains("PKG"))
        {
            var contentCode = ExtractContentCode(body);
            var amount = ExtractAmount(body);

            if (!string.IsNullOrEmpty(contentCode) && amount > 0)
            {
                Console.WriteLine($"💰 Phát hiện mã: {contentCode}, Số tiền: {amount:N0} VND");
                await ProcessPaymentAsync(contentCode, amount);
            }
            else
            {
                Console.WriteLine("⚠️ Không trích xuất được thông tin hợp lệ");
            }
        }
        else
        {
            Console.WriteLine("📭 Không chứa mã PKG");
        }

        await MarkAsReadAsync(inbox, uid);
    }

    private static async Task ProcessPaymentAsync(string contentCode, int amount)
    {
        try
        {
            var payload = new { ContentCode = contentCode, Amount = amount };
            Console.WriteLine($"🌍 Gửi webhook đến {WebhookUrl}...");

            var response = await _httpClient.PostAsJsonAsync(WebhookUrl, payload);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"✅ Webhook thành công: {responseContent}");
            }
            else
            {
                Console.WriteLine($"❌ Webhook lỗi [{response.StatusCode}]: {responseContent}");
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"🌐 Lỗi webhook: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Lỗi khác: {ex.Message}");
        }
    }

    private static async Task MarkAsReadAsync(IMailFolder inbox, UniqueId uid)
    {
        try
        {
            await inbox.AddFlagsAsync(uid, MessageFlags.Seen, true);
            Console.WriteLine("✔️ Đã đánh dấu là đã đọc");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Lỗi khi đánh dấu đã đọc: {ex.Message}");
        }
    }

    private static string StripHtmlTags(string html)
    {
        return Regex.Replace(html, "<.*?>", string.Empty);
    }

    private static string ExtractContentCode(string body)
    {
        var match = Regex.Match(body, @"(PKG[\w\d]+)");
        return match.Success ? match.Value.Trim() : null;
    }

    private static int ExtractAmount(string body)
    {
        var match = Regex.Match(body, @"(\d{1,3}(?:,\d{3})*)\s*VND");
        if (match.Success)
        {
            var value = match.Groups[1].Value.Replace(",", "");
            return int.TryParse(value, out int result) ? result : 0;
        }
        return 0;
    }
}
