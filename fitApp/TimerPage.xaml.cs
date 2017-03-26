using System;
using System.ComponentModel;
using System.Collections.Generic;

using Xamarin.Forms;

namespace fitApp
{
	public partial class TimerPage : ContentPage
	{
		Stopwatch stopwatch;
		public TimerPage()
		{
			stopwatch = new Stopwatch();
			InitializeComponent();

			timeLabel.SetBinding(Stopwatch.dateProperty, "Text");

			timeLabel.BindingContext = stopwatch;

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
				stopwatch.start();
				startButton.IsEnabled = false;
				stopButton.IsEnabled = true;
				resetButton.IsEnabled = false;
			}
			else if (sender == stopButton)
			{
				stopwatch.stop();
				startButton.IsEnabled = true;
				stopButton.IsEnabled = false;
				resetButton.IsEnabled = true;
			}
			else if (sender == resetButton)
			{
				stopwatch.reset();
				stopButton.IsEnabled = false;
				resetButton.IsEnabled = false;
				startButton.IsEnabled = true;
			}
		}
			
	}
}
