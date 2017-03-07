using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace fitApp
{
	public partial class GoalListPage : ContentPage
	{
		FitAppDatabase database = new FitAppDatabase(DependencyService.Get<IFileHelper>().GetLocalFilePath("fitAppDatabase.db3"));

		public GoalListPage()
		{
			InitializeComponent();

			var workoutitems = database.GetExercises();
			goalListView.ItemsSource = workoutitems;
			goalListView.ItemTemplate = new DataTemplate(typeof(TextCell));
			goalListView.ItemTemplate.SetBinding(TextCell.TextProperty, "Name");

			// Add an on tap handler and send the name of the exercise to the graph page
			goalListView.ItemTapped += async (s, e) =>
			{
				await Navigation.PushAsync(new GoalsPage(((WorkoutItemDB)e.Item).Name));
				((ListView)s).SelectedItem = null;
			};
		}
	}
}
