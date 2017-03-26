using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace fitApp
{
	public class Stopwatch : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public DateTime date;

		public DateTime datetime
		{
			get
			{
				return date;
			}
			set
			{
				date = value;
				OnPropertyChanged("datetime");
			}
		}
		public static readonly BindableProperty dateProperty =
			BindableProperty.Create("datetime", typeof(DateTime), typeof(Stopwatch), new DateTime(1, 1, 1, 0, 0, 0, 0));


		protected void OnPropertyChanged(string name)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null && name != null)
			{
				handler(this,
					new PropertyChangedEventArgs(name));
			}
		}

		bool continueFlag;


		public Stopwatch()
		{
			datetime = new DateTime(1, 1, 1, 0, 0, 0, 0);
			continueFlag = false;
		}


		public void start()
		{
			continueFlag = true;
			Device.StartTimer(new TimeSpan(0, 0, 0, 0, 2), HandleFunc);
		}

		public void stop()
		{
			continueFlag = false;
		}

		public void reset()
		{
			datetime = new DateTime(1, 1, 1, 0, 0, 0, 0);
		}

		bool HandleFunc()
		{
			if (continueFlag == false)
				return false;
			datetime = datetime.AddMilliseconds(2);
			return true;
		}
	}
}
