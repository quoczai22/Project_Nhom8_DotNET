using Newtonsoft.Json;
using QuanLyLinhKienMayTinh.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyLinhKienMayTinh.Services
{
    public class MomoService : IMomoService
    {
        
        static readonly HttpClient _client = new HttpClient(); // Tạo 1 hằng số HttpClient để tái sử dụng trong suốt vòng đời của ứng dụng, tránh việc tạo nhiều instance HttpClient

        string MomoApiUrl = "https://test-payment.momo.vn/v2/gateway/api/create"; // điểm đến API của MoMo để tạo đơn thanh toán
        string MomoQueryApiUrl = "https://test-payment.momo.vn/v2/gateway/api/query"; // điểm đến API của MoMo để kiểm tra trạng thái giao dịch
        string PartnerCode = "MOMO";
        string AccessKey = "F8BBA842ECF85";
        string SecretKey = "K951B6PE1waDMi640xX08PD3vg6EkVlz";
        string ReturnUrl = "https://momo.vn"; // trả về trang chủ Mo Mo sau khi đã thanh toán 
        string NotifyUrl = "https://ignition-good-urethane.ngrok-free.dev/api/payment/momo-ipn"; // đường dẫn để MoMo thông báo về trạng thái thanh toán 
        string RequestType = "captureWallet"; // xác thực thông tin thanh toán bằng ví MoMo

        public MomoService() { }

        string CreateSignature(string message, string secretKey) // Tạo chữ ký
        {
            byte[] keyByte = Encoding.UTF8.GetBytes(secretKey);  // Chuyển secretKey thành mảng byte để dùng trong HMAC
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);    // Chuyển message thành mảng byte để tính hash

            using (var hmac = new HMACSHA256(keyByte)) // Tạo đối tượng HMACSHA256 với secretKey đã được chuyển thành mảng byte
            {
                byte[] hash = hmac.ComputeHash(messageBytes); // Tính hash của message bằng HMACSHA256
                return BitConverter.ToString(hash).Replace("-", "").ToLower(); // Chuyển hash thành chuỗi hex và trả về
            }
        }

        public async Task<MomoResponse> CreatePaymentAsync(HoaDon hd)
        {
            string orderId = hd.MaHd + "_" + DateTimeOffset.UtcNow.ToUnixTimeSeconds(); // Tạo orderId duy nhất trong mỗi lần tạo đơn
            string orderInfo = "Thanh toan hoa don " + hd.MaHd;
            long amount = hd.TongTien ?? 0;         // Số tiền phải thanh toán, lấy từ hóa đơn
            string requestId = Guid.NewGuid().ToString(); // Tạo requestId duy nhất cho mỗi request

            var rawData =
                $"accessKey={AccessKey}" +
                $"&amount={amount}" +
                $"&extraData=" +
                $"&ipnUrl={NotifyUrl}" +
                $"&orderId={orderId}" +
                $"&orderInfo={orderInfo}" +
                $"&partnerCode={PartnerCode}" +
                $"&redirectUrl={ReturnUrl}" +
                $"&requestId={requestId}" +
                $"&requestType={RequestType}";

            string signature = CreateSignature(rawData, SecretKey);

            var requestData = new
            {
                partnerCode = PartnerCode,
                accessKey = AccessKey,
                requestId,
                amount,
                orderId,
                orderInfo,
                redirectUrl = ReturnUrl,  
                ipnUrl = NotifyUrl,  
                requestType = RequestType,
                extraData = "",
                lang = "vi",
                signature
            };

            {
                var json = JsonConvert.SerializeObject(requestData); // Chuyển requestData thành chuỗi JSON để gửi lên MoMo
                var content = new StringContent(json, Encoding.UTF8, "application/json"); // Tạo HttpContent từ chuỗi JSON để gửi lên MoMo

                var response = await _client.PostAsync(MomoApiUrl, content);          // Gửi request lên MoMo và nhận về response
                var responseContent = await response.Content.ReadAsStringAsync();           // Đọc nội dung response từ MoMo dưới dạng chuỗi

                return JsonConvert.DeserializeObject<MomoResponse>(responseContent); // Trả về đối tượng MomoResponse được tạo từ chuỗi JSON response của MoM 
            }
        }

        public async Task<MomoResponse> QueryPaymentStatusAsync(string orderId)
        {
            string requestId = Guid.NewGuid().ToString();

            var rawData =
                $"accessKey={AccessKey}" +
                $"&orderId={orderId}" +
                $"&partnerCode={PartnerCode}" +
                $"&requestId={requestId}";

            string signature = CreateSignature(rawData, SecretKey);

            var requestData = new
            {
                partnerCode = PartnerCode,
                requestId,
                orderId,
                lang = "vi",
                signature
            };

            var json = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(MomoQueryApiUrl, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<MomoResponse>(responseContent);
        }

        public MomoExecuteResponseModel PaymentExecuteAsync(Dictionary<string, string> collection)
        {
            collection.TryGetValue("amount", out var amount); // Lấy giá trị amount từ collection, nếu không tồn tại sẽ trả về null
            collection.TryGetValue("orderId", out var orderId); // Lấy giá trị orderId từ collection, nếu không tồn tại sẽ trả về null
            return new MomoExecuteResponseModel() { Amount = amount, OrderId = orderId }; 
        }
    }
}
