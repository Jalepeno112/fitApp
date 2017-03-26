using System;
namespace fitApp
{
	public class Timer : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		public DateTime date;
		public int startT;
		private HomePage homePage;

		public int starttime
		{
			get
			{
				return startT;
			}
			set
			{
				if (value == 0)
					startT = 1;
				else
					startT = value;
				OnPropertyChanged("starttime");
			}
		}

		bool continueFlag;

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
			BindableProperty.Create("datetime", typeof(DateTime), typeof(Timer), new DateTime(1, 1, 1, 0, 0, 5, 0));

		public static readonly BindableProperty intervalProperty =
			BindableProperty.Create("starttime", typeof(int), typeof(Timer), new int());


		protected void OnPropertyChanged(string name)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null && name != null)
			{
				handler(this,
					new PropertyChangedEventArgs(name));
			}
		}

		public Timer(HomePage home)
		{
			homePage = home;
			datetime = new DateTime(1, 1, 1, 0, 0, 5, 0);
			continueFlag = false;
			startT = 5;
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


		bool HandleFunc()
		{
			if (continueFlag == false)
				return false;
			if (datetime.Equals(new DateTime(1, 1, 1, 0, 0, 0, 0)))
			{
				datetime = datetime.AddSeconds(starttime);
				homePage.nextImage();
				return true;
			}
			datetime = datetime.AddMilliseconds(-2);
			return true;
		}
	}
}
