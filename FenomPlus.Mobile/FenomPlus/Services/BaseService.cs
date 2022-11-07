using FenomPlus.Interfaces;
using TinyIoC;

namespace FenomPlus.Services
{
	public class BaseService
	{
		protected IAppServices Services;

        public TinyIoCContainer Container => TinyIoCContainer.Current;

        public BaseService(IAppServices services)
		{
			this.Services = services;
		}

		public BaseService()
		{
			this.Services = IOC.Services;
		}
    }
}
