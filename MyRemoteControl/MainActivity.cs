using System;
using System.IO;

using System.Linq;
using System.Text;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Util;
using System.Collections.Generic;

namespace MyRemoteControl
{
	[Activity (Label = "My Remote Control", MainLauncher = true, Icon = "@drawable/Remote")]
	public class MainActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Main);

			Button btnLeft = FindViewById<Button> (Resource.Id.btnLeft);
			btnLeft.Click += delegate { Connect ("Left"); };

			Button btnRight = FindViewById<Button> (Resource.Id.btnRight);
			btnRight.Click += delegate { Connect ("Right"); };

			Button btnUp = FindViewById<Button> (Resource.Id.btnUp);
			btnUp.Click += delegate { Connect ("Up"); };

			Button btnDown = FindViewById<Button> (Resource.Id.btnDown);
			btnDown.Click += delegate { Connect ("Down"); };

			Button btnRound = FindViewById<Button> (Resource.Id.btnRound);
			btnRound.Click += delegate { ShowDialog(1); };
		}

		private void Connect (string data)
		{
			BluetoothAdapter bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
			//TextView topStatusTextView = FindViewById<TextView> (Resource.Id.TopStatusTextView);
			if (!bluetoothAdapter.IsEnabled) {
				Intent enableBluetooth = new Intent (BluetoothAdapter.ActionRequestEnable);
				StartActivityForResult (enableBluetooth, 1);
			}                                                   
			Java.Util.UUID serialUUID = Java.Util.UUID.FromString ("00001101-0000-1000-8000-00805F9B34FB");

			BluetoothDevice bluetoothDevice = (from bd in bluetoothAdapter.BondedDevices
			                                   where bd.Name == "81H9YLF12"
			                                   select bd).FirstOrDefault ();

			//BluetoothDevice bluetoothDevice = bluetoothAdapter.GetRemoteDevice ("00:26:5E:DE:7D:FC");
			BluetoothSocket bluetoothSocket = null;
			Stream outStream = null;

			IntPtr createRfcommSocket = JNIEnv.GetMethodID (bluetoothDevice.Class.Handle, "createRfcommSocket", "(I)Landroid/bluetooth/BluetoothSocket;");
			IntPtr socket = JNIEnv.CallObjectMethod (bluetoothDevice.Handle, createRfcommSocket, new Android.Runtime.JValue (1));
			bluetoothSocket = Java.Lang.Object.GetObject<BluetoothSocket> (socket, JniHandleOwnership.TransferLocalRef);

			//bluetoothSocket = bluetoothDevice.CreateRfcommSocketToServiceRecord (serialUUID);   
			bluetoothSocket.Connect ();
			outStream = bluetoothSocket.OutputStream;

			bluetoothSocket.OutputStream.Write (Encoding.ASCII.GetBytes (data), 0, data.Length);
			bluetoothSocket.Close ();
		}

		protected override Dialog OnCreateDialog (int id, Bundle args)
		{
			var builder = new AlertDialog.Builder (this);
			//builder.SetIconAttribute (Android.Resource.Attribute.AlertDialogIcon);
			builder.SetTitle (Resource.String.list_dialog_title);
			var devices = new List<string>();
			BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;
			foreach (var d in adapter.BondedDevices)
				devices.Add (d.Name);

			builder.SetSingleChoiceItems (devices.ToArray(), 0, LinkClicked);
			builder.SetPositiveButton (Resource.String.dialog_ok, OkClicked);
			builder.SetNegativeButton (Resource.String.dialog_cancel, CancelClicked);

			return builder.Create ();
		}

		private void OkClicked(object sender, DialogClickEventArgs args)
		{
			var dialog = (AlertDialog) sender;
			//var devices = new List<string>;
			BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;
			var devices = new List<string>();
			foreach (var d in adapter.BondedDevices)
				devices.Add (d.Name);
			
			var pos = dialog.ListView.CheckedItemPosition;
			Toast.MakeText(this, string.Format("You've selected: {0}, {1}", pos, devices.ToArray()[pos]), ToastLength.Short).Show();

		}

		private void CancelClicked(object sender, DialogClickEventArgs args)
		{
		}

		private void LinkClicked(object sender, DialogClickEventArgs args)
		{
		}
	}
}