
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MyRemoteControl
{
	[Activity (Label = "Select the device", Icon = "@drawable/Remote")]			
	public class DevicesActivity : Activity
	{
		ListView listView;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Devices);

			var devices = new string[] { "Sumar-PC", "Nexus-S", "Sony Ericsson", "iPhone 5", "Win 8 tablet" };
			listView = FindViewById<ListView> (Resource.Id.listViewDevices);
			listView.Adapter = new ArrayAdapter (this, Android.Resource.Layout.SimpleListItemSingleChoice, devices);
			listView.ChoiceMode = ChoiceMode.Single;
		}

	}
}

