namespace Stephs_Shop.Models.Options
{
	public class MicroServiceOption
	{
		public string FlutterWaveUrl { get; set; }
		public string ExpressPayUrl { get; set; }
		public string FlutterWaveApiKey { get; set; }
		public FileStack FileStack { get; set; }
	}


	public class FileStack
	{
		public string Url { get; set; }
		public string ApiKey { get; set; }
	}
}
