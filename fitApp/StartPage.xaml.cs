using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace fitApp
{
	public partial class StartPage : ContentPage
	{
		CalendarVM vm;
		private FitAppDatabase database = new FitAppDatabase(
			DependencyService.Get<IFileHelper>().GetLocalFilePath("fitAppDatabase.db3")
		);

		public StartPage()
		{
			// setup the page with the data that we need
			vm = new CalendarVM();
			vm.Date = DateTime.Now;

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
		}
	}
}
