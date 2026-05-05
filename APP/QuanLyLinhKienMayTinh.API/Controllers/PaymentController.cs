using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuanLyLinhKienMayTinh.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;

namespace QuanLyLinhKienMayTinh.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly QL_LinhKien_PC_NETContext _context;
        private readonly IConfiguration _config;
        private readonly ILogger<PaymentController> _logger;
        private readonly string _logsFolder;

        public PaymentController(
            QL_LinhKien_PC_NETContext context,
            IConfiguration config,
            ILogger<PaymentController> logger)
        {
            _context = context;
            _config = config;
            _logger = logger;
            _logsFolder = Path.Combine(
                Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!,
                "..", "..", "..", "..", "MomoLogs");
            Directory.CreateDirectory(_logsFolder);
        }

        [AllowAnonymous]
        [HttpPost("momo-ipn")]
        public async Task<IActionResult> ReceiveMomoIPN([FromBody] MomoResponse momoResponse)
        {
            if (momoResponse == null) return BadRequest();

            // 0. Ghi raw debug
            try
            {
                var debugPath = Path.Combine(_logsFolder, "raw_debug.json");
                var debugEntry = new
                {
                    ThoiGian = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Data = momoResponse
                };
                var debugJson = System.Text.Json.JsonSerializer.Serialize(debugEntry, new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });
                await System.IO.File.AppendAllTextAsync(debugPath, debugJson + ",\n");
            }
            catch { }

            // 1. Xác thực chữ ký
            if (!VerifyMomoSignature(momoResponse))
            {
                _logger.LogWarning("MoMo IPN: Chữ ký không hợp lệ. orderId={OrderId}", momoResponse.orderId);
                return Unauthorized("Chữ ký không hợp lệ.");
            }

            // 2. Thanh toán thành công
            if (momoResponse.resultCode == 0)
            {
                string maHdThat = momoResponse.orderId.Split('_')[0];
                var hoaDon = await _context.HoaDons.FirstOrDefaultAsync(h => h.MaHd == maHdThat);

                if (hoaDon == null)
                {
                    _logger.LogWarning("MoMo IPN: Không tìm thấy hóa đơn. MaHd={MaHd}", maHdThat);
                    return NotFound($"Không tìm thấy hóa đơn: {maHdThat}");
                }

                if (hoaDon.TrangThai == "Đã thanh toán")
                {
                    _logger.LogInformation("MoMo IPN: Hóa đơn đã xử lý trước đó. MaHd={MaHd}", maHdThat);
                    return Ok();
                }

                hoaDon.TrangThai = "Đã thanh toán";
                await _context.SaveChangesAsync();
                await SaveTransactionToJsonAsync(momoResponse, maHdThat);

                _logger.LogInformation(
                    "MoMo IPN: Thanh toán thành công. MaHd={MaHd}, TransId={TransId}, Amount={Amount}",
                    maHdThat, momoResponse.transId, momoResponse.amount);

                return Ok();
            }

            // 3. Thanh toán thất bại
            if (momoResponse.resultCode != 0)
            {
                string maHdThat = momoResponse.orderId.Split('_')[0];
                var hoaDon = await _context.HoaDons.FirstOrDefaultAsync(h => h.MaHd == maHdThat);
                if (hoaDon != null && hoaDon.TrangThai != "Đã thanh toán")
                {
                    hoaDon.TrangThai = "Thanh toán thất bại";
                    await _context.SaveChangesAsync();
                }

                _logger.LogWarning(
                    "MoMo IPN thất bại: orderId={OrderId}, resultCode={ResultCode}, message={Message}",
                    momoResponse.orderId, momoResponse.resultCode, momoResponse.message);
            }

            return NoContent();
        }

        private async Task SaveTransactionToJsonAsync(MomoResponse response, string maHd)
        {
            try
            {
                var fileName = $"momo_transactions_{DateTime.Now:yyyy-MM-dd}.json";
                var filePath = Path.Combine(_logsFolder, fileName);

                var record = new
                {
                    ThoiGian = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    MaHoaDon = maHd,
                    MaGiaoDich = response.transId,
                    SoTien = response.amount,
                    OrderId = response.orderId,
                    RequestId = response.requestId,
                    PartnerCode = response.partnerCode,
                    PayType = response.payType,
                    ResultCode = response.resultCode,
                    Message = response.message,
                    ResponseTime = response.responseTime
                };

                List<object> records = new();
                if (System.IO.File.Exists(filePath))
                {
                    var existing = await System.IO.File.ReadAllTextAsync(filePath);
                    records = JsonSerializer.Deserialize<List<object>>(existing) ?? new();
                }

                records.Add(record);

                var json = JsonSerializer.Serialize(records, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });

                await System.IO.File.WriteAllTextAsync(filePath, json);
                _logger.LogInformation("Đã ghi log giao dịch: {FilePath}", filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi ghi file JSON giao dịch MoMo.");
            }
        }

        private bool VerifyMomoSignature(MomoResponse response)
        {
            try
            {
                var secretKey = _config["MomoConfig:SecretKey"];
                var accessKey = _config["MomoConfig:AccessKey"];

                var rawHash = $"accessKey={accessKey}" +
                              $"&amount={response.amount}" +
                              $"&extraData={response.extraData ?? ""}" +
                              $"&message={response.message ?? ""}" +
                              $"&orderId={response.orderId}" +
                              $"&orderInfo={response.orderInfo ?? ""}" +
                              $"&orderType={response.orderType ?? ""}" +
                              $"&partnerCode={response.partnerCode}" +
                              $"&payType={response.payType ?? ""}" +
                              $"&requestId={response.requestId}" +
                              $"&responseTime={response.responseTime}" +
                              $"&resultCode={response.resultCode}" +
                              $"&transId={response.transId}";

                System.IO.File.WriteAllText(
                    Path.Combine(_logsFolder, "signature_debug.txt"),
                    $"RawHash:\n{rawHash}\n\nSignatureMoMo: {response.signature}\n\nSecretKey: {secretKey}\n\nAccessKey: {accessKey}");

                using var hmac = new System.Security.Cryptography.HMACSHA256(
                    System.Text.Encoding.UTF8.GetBytes(secretKey));
                var hashBytes = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(rawHash));
                var hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

                System.IO.File.AppendAllText(
                    Path.Combine(_logsFolder, "signature_debug.txt"),
                    $"\n\nHashTính được: {hash}\n\nKhớp: {hash == response.signature}");

                return hash == response.signature;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xác thực chữ ký MoMo.");
                return false;
            }
        }

        [AllowAnonymous]
        [HttpGet("check-status/{maHd}")]
        public async Task<IActionResult> CheckStatus(string maHd)
        {
            var hoaDon = await _context.HoaDons.FirstOrDefaultAsync(h => h.MaHd == maHd);
            if (hoaDon == null) return NotFound();
            return Ok(new { trangThai = hoaDon.TrangThai });
        }
    }
}