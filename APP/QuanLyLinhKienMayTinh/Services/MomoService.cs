using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyLinhKienMayTinh.Services
{
    public class MomoService
    {
        // ==========================================
        // CẤU HÌNH MOMO - Lấy từ MoMo Developer
        // https://developers.momo.vn
        // ==========================================
        private const string Endpoint = "https://test-payment.momo.vn/v2/gateway/api/create";
        private const string PartnerCode = "MOMO";
        private const string AccessKey = "F8BBA842ECF85";
        private const string SecretKey = "K951B6PE1waDMi640xX08PD3vg6EkVlz";

        // URL MoMo sẽ gọi vào sau khi thanh toán xong (IPN)
        // Đây là Static Domain ngrok — cố định, không đổi mỗi lần mở máy
        private const string IpnUrl = "https://ignition-good-urethane.ngrok-free.dev/api/payment/momo-ipn";

        // URL trang web mở ra trên điện thoại sau khi thanh toán xong
        // Đang để Google, có thể đổi thành trang cảm ơn của bạn
        private const string RedirectUrl = "https://google.com";

        // ==========================================
        // TẠO CHỮ KÝ HMAC-SHA256
        // MoMo dùng chữ ký này để xác minh request có hợp lệ không
        // ==========================================
        public string CreateSignature(string rawHash, string secretKey)
        {
            byte[] keyByte = Encoding.UTF8.GetBytes(secretKey);
            byte[] messageBytes = Encoding.UTF8.GetBytes(rawHash);

            using (var hmac = new System.Security.Cryptography.HMACSHA256(keyByte))
            {
                byte[] hash = hmac.ComputeHash(messageBytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

        // ==========================================
        // TẠO ĐƠN HÀNG MOMO VÀ LẤY QR CODE URL
        // Trả về chuỗi qrCodeUrl dùng để vẽ QR trong WPF
        // ==========================================
        public async Task<string> GetMomoPaymentUrl(string maHd, long soTien)
        {
            // orderId phải là duy nhất mỗi lần tạo đơn
            // Dùng định dạng "MAHD_TIMESTAMP" để sau này tách lại được mã hóa đơn
            // Ví dụ: "HD01_1714925432"
            string orderId = maHd + "_" + DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            string requestId = Guid.NewGuid().ToString(); // ID request, phải unique
            string orderInfo = "Thanh toán hóa đơn " + maHd;
            string extraData = ""; // Dữ liệu thêm, để trống nếu không dùng

            // ==========================================
            // CHUỖI RAW ĐỂ TẠO CHỮ KÝ
            // QUAN TRỌNG: Phải đúng thứ tự A-Z theo tài liệu MoMo v2
            // Sai thứ tự → chữ ký sai → MoMo từ chối request
            // ==========================================
            string rawHash = "accessKey=" + AccessKey +
                             "&amount=" + soTien +
                             "&extraData=" + extraData +
                             "&ipnUrl=" + IpnUrl +
                             "&orderId=" + orderId +
                             "&orderInfo=" + orderInfo +
                             "&partnerCode=" + PartnerCode +
                             "&redirectUrl=" + RedirectUrl +
                             "&requestId=" + requestId +
                             "&requestType=captureWallet";

            string signature = CreateSignature(rawHash, SecretKey);

            // Đóng gói body JSON gửi lên MoMo
            var requestBody = new
            {
                partnerCode = PartnerCode,
                accessKey = AccessKey,
                requestId,
                amount = soTien,
                orderId,
                orderInfo,
                redirectUrl = RedirectUrl,
                ipnUrl = IpnUrl,
                extraData,
                requestType = "captureWallet",
                signature,
                lang = "vi"
            };

            // Gửi request lên MoMo và nhận về qrCodeUrl
            using (var client = new HttpClient())
            {
                var content = new StringContent(
                    JsonConvert.SerializeObject(requestBody),
                    Encoding.UTF8,
                    "application/json");

                var response = await client.PostAsync(Endpoint, content);
                var result = await response.Content.ReadAsStringAsync();

                dynamic data = JsonConvert.DeserializeObject(result);

                // resultCode = 0 là thành công, khác 0 là lỗi
                if (data.resultCode != 0)
                    throw new Exception("Lỗi tạo đơn hàng MoMo: " + data.message);

                // Trả về qrCodeUrl để WPF dùng vẽ QR
                return data.qrCodeUrl;
            }
        }
    }
}