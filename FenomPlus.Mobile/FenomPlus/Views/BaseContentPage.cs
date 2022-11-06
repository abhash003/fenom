using System;
using FenomPlus.Database.Repository.Interfaces;
using FenomPlus.Interfaces;
using FenomPlus.Services;
using Xamarin.Forms;

namespace FenomPlus.Views
{
    public class BaseContentPage : ContentPage //IBaseServices
    {
        //public IAppServices Services => IOC.Services;
        //public ICacheService Cache => Services.Cache;
        //public IBleHubService BleHub => Services.BleHub;

        //// repos here
        //public IBreathManeuverErrorRepository ErrorsRepo => Services.Database.BreathManeuverErrorRepo;
        //public IBreathManeuverResultRepository ResultsRepo => Services.Database.BreathManeuverResultRepo;
        //public IQualityControlRepository QCRepo => Services.Database.QualityControlRepo;
        //public IQualityControlDevicesRepository QCDevicesRepo => Services.Database.QualityControlDevicesRepo;
        //public IQualityControlUsersRepository QCUsersRepo => Services.Database.QualityControlUsersRepo;


        public BaseContentPage()
        {
            // Set Background
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        public virtual void NewGlobalData()
        {
        }
    }
}
