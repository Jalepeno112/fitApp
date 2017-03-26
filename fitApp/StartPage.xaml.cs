using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace fitApp
{
	public partial class StartPage : ContentPage
	{
		CalendarVM vm;
		ToolbarItem tbItem;
		Stopwatch stopwatch;
		private FitAppDatabase database = new FitAppDatabase(
			DependencyService.Get<IFileHelper>().GetLocalFilePath("fitAppDatabase.db3")
		);

		public StartPage()
		{
			// setup the page with the data that we need
			vm = new CalendarVM();
			vm.Date = DateTime.Now;
			stopwatch = new Stopwatch();
			tbItem = new ToolbarItem();
			tbItem.Text = "Save";
			tbItem.Clicked += saveButtonOnClick;
			vm.Workout = database.GetWorkouts(vm.Date);

			this.BindingContext = vm;


			InitializeComponent();

			// if there is not workout.  Display a message
			if (vm.Workout.Count == 0)
			{
				System.Diagnostics.Debug.WriteLine("No workout today");
				Label l = new Label
				{
					Text = "No workout scheduled today!",
					VerticalOptions = LayoutOptions.CenterAndExpand,
					HorizontalOptions = LayoutOptions.CenterAndExpand,
					FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label))
				};
				MainContent.Children.Add(l);
			}
			else
			{
				startStopButton.IsVisible = true;
				startStopButton.IsEnabled = true;
				startStopButton.Clicked += startStopButtonOnClick;

			}
		}

		void startStopButtonOnClick(object sender, EventArgs e)
		{
			// If the start button was pressed, start the timer make the start button the stop button
			// Additionally, make the time label visible
			if (startStopButton.BackgroundColor == Color.Lime)
			{
				startStopButton.BackgroundColor = Color.Red;
				startStopButton.Text = "Stop";
				timeLabel.IsVisible = true;
				timeLabel.BindingContext = stopwatch;
				Device.OnPlatform(()=> timeLabel.FontFamily = "Menlo", ()=> timeLabel.FontFamily = "Droid Sans Mono");
				stopwatch.start();

				// If the "Save" button is visible, delete it
				if(ToolbarItems.Contains(tbItem))
					ToolbarItems.Remove(tbItem);
			}

			else if (startStopButton.BackgroundColor == Color.Red)
			{
				startStopButton.BackgroundColor = Color.Lime;
				startStopButton.Text = "Start";
				stopwatch.stop();
				ToolbarItems.Add(tbItem);
			}
		}

		void saveButtonOnClick(object sender, EventArgs e)
		{
			WorkoutTimeDB wtDB = new WorkoutTimeDB()
			{
				Date = DateTime.Now.ToString(),
				Time = stopwatch.date.ToString()
			};
			database.WriteWorkoutTime(wtDB);
			Navigation.PopAsync();
		}
	}
}
