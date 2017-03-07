using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace fitApp
{
	public partial class HomePage : ContentPage
	{
		public HomePage()
		{
			InitializeComponent();

			// enable tap on the Calendar frame
			var sendToCalendar = new TapGestureRecognizer();
			sendToCalendar.Tapped += async (s, e) =>
			{
				await Navigation.PushAsync(new CalendarPage());
			};
			toCalendar.GestureRecognizers.Add(sendToCalendar);

			// enable tap on the analytics frame
			var sendToAnalytics = new TapGestureRecognizer();
			sendToAnalytics.Tapped += async (s, e) =>
			{
				await Navigation.PushAsync(new AnalyticsPage());
			};
			toAnalytics.GestureRecognizers.Add(sendToAnalytics);

			// enable tap on the goals frame
			var sendToGoals = new TapGestureRecognizer();
			sendToGoals.Tapped += async (s, e) =>
			{
				await Navigation.PushAsync(new GoalListPage());
			};
			toGoals.GestureRecognizers.Add(sendToGoals);

			//enable tap on the timer page
			var sendToTimer = new TapGestureRecognizer();
			sendToTimer.Tapped += async (s, e) =>
			{
				await Navigation.PushAsync(new TimerPage());
			};
			toTimer.GestureRecognizers.Add(sendToTimer);

			var sendToStart = new TapGestureRecognizer();
			sendToStart.Tapped += async (s, e) =>
			{
				await Navigation.PushAsync(new StartPage());
			};
			toStart.GestureRecognizers.Add(sendToStart);
		}
	}
}
