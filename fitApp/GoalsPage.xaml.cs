using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace fitApp
{
	public partial class GoalsPage : ContentPage
	{
		FitAppDatabase database = new FitAppDatabase(DependencyService.Get<IFileHelper>().GetLocalFilePath("fitAppDatabase.db3"));
		string _name;
		public GoalsPage(string name)
		{
			InitializeComponent();
			Refresh();
			exerciseName.Text = name;
			_name = name;
			editGoalButton.Clicked += OnButtonPress;
			goToAnalyticsButton.Clicked += OnButtonPress;
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			Refresh();
		}

		private async void OnButtonPress(object sender, EventArgs e)
		{
			if (sender == editGoalButton)
				await Navigation.PushAsync(new EditGoalPage(_name));
			else if (sender == goToAnalyticsButton)
				await Navigation.PushAsync(new GraphPage(_name));
		}

		private void Refresh()
		{
			database = new FitAppDatabase(DependencyService.Get<IFileHelper>().GetLocalFilePath("fitAppDatabase.db3"));
			GoalDB gdb = database.GetGoal(_name);
			if (gdb == null)
				goalValue.Text = "  None";
			else
			{
				goalValue.Text = gdb.goal.ToString() + " " + gdb.unit.ToString();
			}
		}
	}
}
