using System;
using System.ComponentModel;
using System.Collections.Generic;

using Xamarin.Forms;

namespace fitApp
{
	public partial class TimerPage : ContentPage
	{
		Timer timer;

		public class Timer : INotifyPropertyChanged
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
				BindableProperty.Create("datetime", typeof(DateTime), typeof(Timer), new DateTime(1, 1, 1, 0, 0, 0, 0));


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

			                                                                               
			public Timer()
			{
				datetime = new DateTime(1,1,1,0,0,0,0);
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

		public TimerPage()
		{
			timer = new Timer();
			InitializeComponent();

			timeLabel.SetBinding(Timer.dateProperty, "Text");

			timeLabel.BindingContext = timer;

			startButton.Clicked += OnButtonClick;
			stopButton.Clicked += OnButtonClick;
			resetButton.Clicked += OnButtonClick;



			stopButton.IsEnabled = false;
			resetButton.IsEnabled = false;
		}

		public void OnButtonClick(object sender, EventArgs e)
		{
			if (sender == startButton)
			{
				timer.start();
				startButton.IsEnabled = false;
				stopButton.IsEnabled = true;
				resetButton.IsEnabled = false;
			}
			else if (sender == stopButton)
			{
				timer.stop();
				startButton.IsEnabled = true;
				stopButton.IsEnabled = false;
				resetButton.IsEnabled = true;
			}
			else if (sender == resetButton)
			{
				timer.reset();
				stopButton.IsEnabled = false;
				resetButton.IsEnabled = false;
				startButton.IsEnabled = true;
			}
		}
			
	}
}
