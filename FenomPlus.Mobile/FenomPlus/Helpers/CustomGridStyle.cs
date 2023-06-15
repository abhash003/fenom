using System;
using Syncfusion.SfDataGrid.XForms;
using Xamarin.Forms;

namespace FenomPlus.Helpers
{
    public class CustomGridStyle : DataGridStyle
    {
        public CustomGridStyle()
        {
        }
        public override Color GetStackedHeaderBackgroundColor(int rowIndex)
        {
            return Color.Black;
        }

        public override Color GetStackedHeaderForegroundColor(int rowIndex)
        {
            return Color.FromRgb(255, 255, 255);
        }

        public override Color GetGroupSummaryRowBackgroundColor()
        {
            return Color.FromRgb(0xDD, 0xDD, 0xDD);
        }

        public override Color GetGroupSummaryRowForegroundColor()
        {
            return Color.FromRgb(0x00, 0x00, 0x00);
        }

        public override Color GetHeaderBackgroundColor()
        {
            return Color.FromRgb(15, 15, 15);
        }

        public override Color GetHeaderForegroundColor()
        {
            return Color.FromRgb(255, 255, 255);
        }

        public override Color GetRecordBackgroundColor()
        {
            return Color.FromRgb(255, 255, 255);
        }

        public override Color GetRecordForegroundColor()
        {
            return Color.FromRgb(0, 0, 0);
        }

        public override Color GetSelectionBackgroundColor()
        {
            return Color.FromRgb(42, 159, 214);
        }

        public override Color GetSelectionForegroundColor()
        {
            return Color.FromRgb(255, 255, 255);
        }

        public override Color GetCaptionSummaryRowBackgroundColor()
        {
            return Color.FromRgb(0xbb, 0xbb, 0xbb);
        }

        [Obsolete]
        public override Color GetCaptionSummaryRowForeGroundColor()
        {
            return Color.FromRgb(0x00, 0x00, 0x00);
        }

        public override Color GetBorderColor()
        {
            return Color.FromRgb(81, 83, 82);
        }

        public override Color GetLoadMoreViewBackgroundColor()
        {
            return Color.FromRgb(242, 242, 242);
        }

        public override Color GetLoadMoreViewForegroundColor()
        {
            return Color.FromRgb(34, 31, 31);
        }

        public override Color GetAlternatingRowBackgroundColor()
        {
            return Color.FromRgb(0xEE, 0xEE, 0xEE);
        }
    }
}

