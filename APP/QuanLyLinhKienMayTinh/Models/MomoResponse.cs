namespace QuanLyLinhKienMayTinh.Models
{
    public class MomoResponse
    {
        public string partnerCode { get; set; }
        public string orderId { get; set; }
        public string requestId { get; set; }
        public long amount { get; set; }
        public string orderInfo { get; set; }
        public int resultCode { get; set; }
        public string message { get; set; }
        public string payUrl { get; set; } // trả về link chứa mã QR, cần xử lý để lấy mã QR
        public string qrCodeUrl { get; set; } // trả về mã QR
        public string signature { get; set; }

    }
}