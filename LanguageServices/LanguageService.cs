using Microsoft.Extensions.Localization;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Localization;

namespace computerwala.LanguageServices
{
	public class SharedResource
    {
	}
    public class Resource
    {
    }

    public class LanguageService
    {

		private readonly IStringLocalizer _localizer;


		public LanguageService(IStringLocalizerFactory localizer)
		{

			var type = typeof(SharedResource);
			var assembly = new AssemblyName(type.GetTypeInfo().Assembly.FullName);
			_localizer = localizer.Create("Resource", assembly.Name);
		}


		public string GetValue(string key)
		{
			var value = _localizer[key];
			return value;
		}


	}
}
