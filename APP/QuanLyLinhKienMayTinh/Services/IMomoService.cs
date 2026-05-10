using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using QuanLyLinhKienMayTinh.Models;

namespace QuanLyLinhKienMayTinh.Services
{
    public interface IMomoService
    {
        MomoExecuteResponseModel PaymentExecuteAsync(Dictionary<string, string> collection); // Phương thức thực hiện thanh toán
        Task<MomoResponse> CreatePaymentAsync(HoaDon hd); // Phương thức tạo đơn thanh toán và lấy mã QR

    }
}
