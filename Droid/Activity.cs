using System;
using System.IO;
using Xamarin.Forms;
using fitApp.Droid;

[assembly: Dependency(typeof(FileHelper))]
namespace fitApp.Droid
{
	public class FileHelper : IFileHelper
	{
		public string GetLocalFilePath(string filename)
		{
			string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			string final = Path.Combine(path, filename);
			System.Diagnostics.Debug.WriteLine("Opening database file: ", final);
			return final;
		}
	}
}