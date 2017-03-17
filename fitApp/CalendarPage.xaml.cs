using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Xamarin.Forms;
using System.Windows.Input;


namespace fitApp
{
	public partial class CalendarPage : ContentPage
	{
		CalendarVM vm;
		public CalendarPage()
		{
			vm = new CalendarVM();
			this.BindingContext = vm;

			System.Diagnostics.Debug.WriteLine(vm.Workout);

			// generate fake data
			vm.database.GenerateFakeData();

			InitializeComponent();

			vm.Date = System.DateTime.Now;

			// set the edit button to push to the EditDayPage
			EditBtn.Clicked += async (sender, e) =>
			{
				await Navigation.PushAsync(new EditDayPage(vm));
			};

		}
	}

	/*
	 * Custom view for displaying WorkoutItem objects
	 * 
	 * We couldn't get nested ListViews to work, so we had to create our own
	 * It has 2 bindable propertys:
	 * 	WorkoutList
	 *  Editable
	 * 
	 * WorkoutList is the List of WorkoutItems.  
	 * If editable is set to true, then the view has some delete buttons, 
	 * and has a form to allow a user to add a new workout
	 */
	public class WorkoutItemView : ContentView
	{
		//http://stackoverflow.com/questions/41322399/xamarin-forms-mvvm-stacklayout-content-binding
		// The WorkoutListProperty holds the list of WorkoutItems that we are displaying
		public static readonly BindableProperty WorkoutListProperty = BindableProperty.Create("WorkoutList", // propertyName
			typeof(ObservableCollection<WorkoutItem>),     // returnType
			typeof(WorkoutItemView), // declaringType
            new ObservableCollection<WorkoutItem>(),
			defaultBindingMode:BindingMode.TwoWay,
			propertyChanged: OnWorkoutChange);

		public static readonly BindableProperty WorkoutListEditableProperty = BindableProperty.Create("WorkoutListEditable",
			typeof(bool),     // returnType
			typeof(WorkoutItemView), // declaringType
			false,
			defaultBindingMode: BindingMode.TwoWay,
		    propertyChanged: OnEditableChange);
		                                                                                       
		public WorkoutItemView()
		{
			// DisplayWorkout returns a StackLayout.
			// Since WorkoutItemView is a ScrollView, we can set its Content to the StackLayout
			StackLayout sl = DisplayWorkout();
			this.Content = sl;
		}
		public ObservableCollection<WorkoutItem> WorkoutList
		{
			set { SetValue(WorkoutListProperty, value); }
			get { return (ObservableCollection<WorkoutItem>)GetValue(WorkoutListProperty); }
		}


		// when the WorkoutListProperty changes, we update the display
		static void OnWorkoutChange(BindableObject bindable, object oldValue, object newValue)
		{
			System.Diagnostics.Debug.WriteLine("Workout Changed!");
			WorkoutItemView b = (WorkoutItemView)bindable;
			b.WorkoutChange(
				(ObservableCollection<WorkoutItem>)oldValue, (ObservableCollection<WorkoutItem>)newValue
			);

			b.Content = b.DisplayWorkout();


		}
		void WorkoutChange(ObservableCollection<WorkoutItem> oldValue, ObservableCollection<WorkoutItem> newValue)
		{
			WorkoutList = newValue;
		}

		public bool WorkoutListEditable
		{
			set { SetValue(WorkoutListEditableProperty, value); }
			get { return (bool)GetValue(WorkoutListEditableProperty); }
		}

		static void OnEditableChange(BindableObject bindable, object oldValue, object newValue)
		{
			WorkoutItemView b = (WorkoutItemView)bindable;

			if ((bool)newValue)
			{
				b.Content = b.DisplayWorkout(editable:true);
			}
		}

		// Generate a StackLayout that displays the Workout Information

		public StackLayout DisplayWorkout(bool editable = false)
		{
			ScrollView scroll = new ScrollView();
			StackLayout parent_stack = new StackLayout();
			StackLayout workout_stack = new StackLayout{
				VerticalOptions= LayoutOptions.FillAndExpand
			};


			System.Diagnostics.Debug.WriteLine("Generating view for " + WorkoutList.Count + " items");
			for (int i = 0; i < WorkoutList.Count; i++)
			{
				// add a title
				StackLayout title_container = new StackLayout { Orientation = StackOrientation.Horizontal };
				Label title = new Label { 
					Text = WorkoutList[i].Name, 
					FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
				};

				if (WorkoutListEditable)
				{
					Button b = new Button { 
						Text = "X", 
						StyleId = WorkoutList[i].ID.ToString(), 
						FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
						TextColor = Color.Red
					};
					b.BindingContext = this.BindingContext;
					b.SetBinding(Button.CommandProperty, "RemoveWorkoutItem");
					b.SetBinding(Button.CommandParameterProperty, new Binding { Source = b });
					title_container.Children.Add(b);
				}
				title_container.Children.Add(title);
				workout_stack.Children.Add(title_container);

				// create a grid to hold the underlying workout info
				Grid g = new Grid();

				// there are two columns
				g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
				g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

				//add header row
				g.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

				//add headers
				g.Children.Add(new Label { Text = "Repetition" }, 0, 0);
				g.Children.Add(new Label { Text = WorkoutList[i].Unit }, 1, 0);

				// add one row for the title of each workout
				// add another row for the header for listing each workout
				// then add a row for each repetition
				for (int j = 0; j < WorkoutList[i].Set.Count; j++)
				{
					g.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
					g.Children.Add(new Label { Text = (j + 1).ToString() }, 0, j + 1);
					g.Children.Add(new Label { Text = WorkoutList[i].Set[j].ToString() }, 1, j + 1);
				}
				// add grid to stack
				workout_stack.Children.Add(g);

				// add a stacklayout underneath the grid to create divider
				workout_stack.Children.Add(new StackLayout
				{
					BackgroundColor = Color.Gray,
					HeightRequest = 1,
					HorizontalOptions = LayoutOptions.FillAndExpand
				});
			}
			// add the workout list and it's scroll to the parent stack
			scroll.Content = workout_stack;
			parent_stack.Children.Add(scroll);

			if (WorkoutListEditable)
			{
				StackLayout sl = new StackLayout
				{
					Padding = new Thickness { Top=20 },
					VerticalOptions = LayoutOptions.FillAndExpand
				};
				Label l = new Label
				{
					Text = "Add Another Item",
					FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
					HorizontalOptions = LayoutOptions.CenterAndExpand
				};

				Entry name = new Entry { Placeholder = "Item Name", StyleId="ItemName"};
				Picker unit = new Picker { Title="Unit",  StyleId="ItemUnit"};
				Entry rep1 = new Entry { Placeholder = "Add each rep with a comma in between", StyleId="Reps" };

				// add the unit selection options
				unit.Items.Add("Minutes");
				unit.Items.Add("Repetitions");
				unit.Items.Add("Weight");
				unit.SelectedIndex = 0;

				sl.Children.Add(l);
				sl.Children.Add(name);
				sl.Children.Add(unit);
				sl.Children.Add(rep1);

				Button b = new Button { Text = "Add workout" };
				b.BindingContext = this.BindingContext;

				b.SetBinding(Button.CommandProperty, "AddNewWorkout");
				b.SetBinding(Button.CommandParameterProperty, new Binding { Source = sl });
				sl.Children.Add(b);
				parent_stack.Children.Add(sl);
			}

			return parent_stack;
		}
	}
}
