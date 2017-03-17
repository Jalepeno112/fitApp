using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace fitApp
{
	public partial class EditDayPage : ContentPage
	{
		public EditDayPage(CalendarVM vm)
		{
			this.BindingContext = vm;
			InitializeComponent();

		}

		public void onEnter(Object sender, EventArgs e)
		{
			// here is where we will update our SQLite
			// for npw, we do it in memory

		}
	}
}
