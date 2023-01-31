using FenomPlus.ViewModels;
using FenomPlus.ViewModels.QualityControl.Models;
using System;
using System.Globalization;

namespace FenomPlus.Controls
{
    public partial class QcButtonViewModel : BaseViewModel
    {
        private QCUser _qcUserModel;
        public QCUser QCUserModel
        {
            get => _qcUserModel;
            set
            {
                _qcUserModel = value;

                Assigned = _qcUserModel != null;
            }
        }

        public bool Assigned { get; set; } = false;

        public string UserName
        {
            get => QCUserModel.UserName;
            set => QCUserModel.UserName = value;
        }

        public string CurrentStatus
        {
            get => QCUserModel.CurrentStatus;
            set => QCUserModel.CurrentStatus = value;
        }

        public DateTime ExpiresDate
        {
            get => QCUserModel.ExpiresDate;
            set
            {
                QCUserModel.ExpiresDate = value;
                ExpiresDateString = QCUserModel.ExpiresDate != DateTime.MinValue ? QCUserModel.ExpiresDate.ToString(Constants.PrettyDateFormatString, CultureInfo.CurrentCulture) : string.Empty;
            }
        }

        public string ExpiresDateString { get; set; } = string.Empty;

        public DateTime NextTestDate
        {
            get => QCUserModel.NextTestDate;
            set
            {
                QCUserModel.NextTestDate = value;
                NextTestDateString = QCUserModel.NextTestDate != DateTime.MinValue ? QCUserModel.NextTestDate.ToString(Constants.PrettyHoursFormatString, CultureInfo.CurrentCulture) : string.Empty;
            }
        }

        public string NextTestDateString { get; set; } = string.Empty;

        public QcButtonViewModel()
        {
                
        }

        //public QcButtonViewModel(QCUser userModel)
        //{
        //    QCUserModel = userModel;

        //    if (QCUserModel != null)
        //    {
        //        Assigned = true;
        //        UserName = QCUserModel.UserName;
        //        CurrentStatus = QCUserModel.CurrentStatus;
        //        ExpiresDate = QCUserModel.ExpiresDate;
        //        NextTestDate = QCUserModel.NextTestDate;
        //    }
        //    else
        //    {
        //        Assigned = false;
        //        UserName = string.Empty;
        //        CurrentStatus = string.Empty;
        //        ExpiresDateString = string.Empty;
        //        NextTestDateString = string.Empty;
        //    }
        //}
    }
}
