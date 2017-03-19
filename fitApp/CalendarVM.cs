using System;
using Xamarin.Forms;

using System.ComponentModel;
using System.Windows.Input;

using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Collections.Specialized;

using SQLite;



namespace fitApp
{

	public class CalendarVM : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		private DateTime _date;
		private bool _editMode;
		private ObservableCollection<WorkoutItem> _workoutList = new ObservableCollection<WorkoutItem>();
		private StackLayout _workoutView;

		public FitAppDatabase database = new FitAppDatabase(DependencyService.Get<IFileHelper>().GetLocalFilePath("fitAppDatabase.db3"));

		public CalendarVM()
		{
		}

		// the selected date
		public DateTime Date
		{
			get
			{
				return _date;
			}
			set
			{
				_date = value;
				OnPropertyChanged("Date");
			}
		}

		// whether or not the workout is in editable mode
		// this changes the way we display the data
		public bool Editable
		{
			get
			{
				return (bool)_editMode;
			}
			set
			{
				_editMode = value;
				OnPropertyChanged("Editable");
			}
		}

		// the list of workout items for the given day
		public ObservableCollection<WorkoutItem> Workout
		{
			get
			{
				return _workoutList;
			}
			set
			{
				System.Diagnostics.Debug.WriteLine("Workout changed in VM");
				_workoutList = value;
				OnPropertyChanged("Workout");
			}
		}

		// Remove a given workout item from the database
		// this will also remove it from our object in memory in order to update the view
		public ICommand RemoveWorkoutItem
		{
			get
			{
				return new Command<Button>((obj) =>
				{
					System.Diagnostics.Debug.WriteLine("Removing ID: " + obj.StyleId);

					// the styleId is the ID of the object
					int id = Convert.ToInt32(obj.StyleId);

					// find the item
					for (int i = 0; i < _workoutList.Count; i++)
					{
						if (_workoutList[i].ID == id)
						{
							// remove from database, remove from list
							database.RemoveWorkout(_workoutList[i]);
							_workoutList.RemoveAt(i);
							break;
						}
					}

					// In order for the property changed to fire, we have to recreate the list
					// not ideal at all, but it works for now
					Workout = new ObservableCollection<WorkoutItem>(_workoutList);
					OnPropertyChanged("Workout");
				});
			}
		}

		// parse the add new workout form and add the information into our database
		public ICommand AddNewWorkout
		{
			get
			{
				return new Command<StackLayout>((sl) =>
				{
					WorkoutItem wi = new WorkoutItem();

					Entry item_name = new Entry();
					Picker item_unit;
					Entry reps_field = new Entry();

					// for each child of the stack layout, try and convert it to an Entry
					// if it can't be converted, we aren't interested in it.
					// Depending on what the name of the field is, 
					// it represents a different property in the workout item we are creating
					foreach (View child in sl.Children)
					{
						View e;
						try
						{
							e = (View)child;
						}
						catch (System.InvalidCastException)
						{
							continue;
						}

						if (e == null)
						{
							continue;
						}

						// check the views style_id to get the corresponding values
						if (String.Compare(e.StyleId, "ItemName") == 0)
						{
							item_name = (Entry)child;
							if (String.Compare(item_name.Text, "") == 0 || item_name.Text == null)
							{
								return;
							}
							wi.Name = item_name.Text;
						}
						else if (String.Compare(e.StyleId, "ItemUnit") == 0)
						{
							item_unit = (Picker)child;
							string s = item_unit.Items[item_unit.SelectedIndex];
							if (String.Compare(s, "") == 0 || s == null)
							{
								return;
							}
							wi.Unit = s;
						}
						else if (String.Compare(e.StyleId, "Reps") == 0)
						{
							reps_field = (Entry)child;
							if (String.Compare(reps_field.Text, "") == 0 || reps_field.Text == null)
							{
								return;
							}
							ObservableCollection<double> reps = new ObservableCollection<double>();
							List<string> split = new List<string>(reps_field.Text.Split(','));
							foreach (string s in split)
							{
								// try and convert each value into a double
								// if it can't be converted, raise an error
								try
								{
									reps.Add(Convert.ToDouble(s));
								}catch(System.FormatException) {
									String err_msg = String.Format("Value '{0}' is not a number. Please try again.", s);
									App.Current.MainPage.DisplayAlert(
										"Error", 
										err_msg,
										"OK"
									);
									return;
								}
							}
							wi.Set = reps;
						}
					}
					wi.Date = Date;

					// clear the existing fields
					item_name.Text = "";
					reps_field.Text = "";

					// add to the list and commit to database
					int index = database.WriteWorkout(wi);
					wi.ID = index;
					_workoutList.Add(wi);

					// in order for the ObservableCollection to register the change, we have to assign it to a new object
					// not pretty, but it gets the job done
					Workout = new ObservableCollection<WorkoutItem>(_workoutList);
				});
			}
		}

		// this fires whenever the date changes
		public ICommand DateChosen
		{
			get
			{
				return new Command((obj) =>
				{
					// get the new datetime and find the workouts associated with it
					DateTime dt = (DateTime)obj;
					System.Diagnostics.Debug.WriteLine(obj as DateTime?);
					// run SQL command and populate bottom of view with Workout details
					Workout = database.GetWorkouts(dt);

					System.Diagnostics.Debug.WriteLine("Received: " + Workout.Count);
				});
			}

		}
		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this,
					new PropertyChangedEventArgs(propertyName));
			}
		}
	}

	// main class that our view uses for presenting data
	public class WorkoutItem
	{

		public string Name { get; set; }
		public ObservableCollection<double> Set { get; set; }
		public string Unit { get; set; }
		public DateTime Date { get; set; }
		public int ID { get; set; }
	}

}

