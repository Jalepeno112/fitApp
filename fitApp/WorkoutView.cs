using System;

using Xamarin.Forms;

namespace fitApp
{
	public partial class WorkoutViews : ContentView
	{
		public WorkoutViews()
		{

			StackLayout sl = DisplayWorkout();
			Content = sl;
		}

		public static readonly BindableProperty WorkoutProperty = BindableProperty.Create(
			propertyName: "Workout",
			returnType: typeof(CurrentWorkout),
			declaringType: typeof(CurrentWorkout),
			defaultValue: null
		);

		public CurrentWorkout Workout
		{
			get { return (CurrentWorkout)GetValue(WorkoutProperty); }
			set { SetValue(WorkoutProperty, value); }
		}


		public StackLayout DisplayWorkout()
		{
			StackLayout stack = new StackLayout();

			for (int i = 0; i < Workout.Items.Count; i++)
			{
				// add a title
				Label title = new Label { Text = Workout.Items[i].Name };
				stack.Children.Add(title);

				// create a grid to hold the underlying workout info
				Grid g = new Grid();

				// there are two columns
				g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
				g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

				//add header row
				g.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

				//add headers
				g.Children.Add(new Label { Text = "Repetition" }, 0, 0);
				g.Children.Add(new Label { Text = Workout.Items[i].Unit }, 1, 0);

				// add one row for the title of each workout
				// add another row for the header for listing each workout
				// then add a row for each repetition
				for (int j = 0; j < Workout.Items[i].Set.Count; j++)
				{
					g.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
					g.Children.Add(new Label { Text = (j + 1).ToString() }, 0, j + 1);
					g.Children.Add(new Label { Text = Workout.Items[i].Set[j].ToString() }, 1, j + 1);
				}

				stack.Children.Add(g);

			}

			return stack;
		}
	}
}

