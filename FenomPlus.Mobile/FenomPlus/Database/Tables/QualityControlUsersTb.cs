
namespace FenomPlus.Database.Tables
{
    public class QualityControlUsersTb : BaseTb<QualityControlUsersTb>
    {
        public string DateAdded { get; set; }
        public string User { get; set; }
        public string QCStatus { get; set; }
    }
}