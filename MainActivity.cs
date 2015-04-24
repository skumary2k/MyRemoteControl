using System;
using System.Linq;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Bluetooth;
using Java.Util;
using System.Text;
using System.IO;

namespace MyRemoteControl
{
	[Activity (Label = "My Remote Control", MainLauncher = true, Icon = "@drawable/Remote")]
	public class MainActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			Button btnLeft = FindViewById<Button> (Resource.Id.btnLeft);
			btnLeft.Click += delegate {
				Connect("Left");
			};

			Button btnRight = FindViewById<Button> (Resource.Id.btnRight);
			btnRight.Click += delegate {
				Connect("Right");
			};

			Button btnUp = FindViewById<Button> (Resource.Id.btnUp);
			btnUp.Click += delegate {
				Connect("Up");
			};

			Button btnDown = FindViewById<Button> (Resource.Id.btnDown);
			btnDown.Click += delegate {
				Connect("Down");
			};
		}

		private void CreateBluetoothConnection (Button button)
		{
			button.Text = "Started...";

			BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;
			if (adapter == null)
				throw new Exception ("No Bluetooth adapter found.");

			if (!adapter.IsEnabled)
				throw new Exception ("Bluetooth adapter is not enabled.");

			BluetoothDevice device = (from bd in adapter.BondedDevices
			                          where bd.Name == "81H9YLF12"
			                          select bd).FirstOrDefault ();
			
			if (device == null) {
				button.Text = "Device is Null";
				//throw new Exception ("Named device not found.");
			}

			BluetoothSocket _socket = null;
			// Get a BluetoothSocket for a connection with the
			// given BluetoothDevice
			//00001101-0000-1000-8000-00805f9b34fb
			try {
				//_socket = device.CreateRfcommSocketToServiceRecord (UUID.FromString ("fa87c0d0-afac-11de-8a39-0800200c9a66"));


				IntPtr createRfcommSocket = JNIEnv.GetMethodID(device.Class.Handle, "createRfcommSocket", "(I)Landroid/bluetooth/BluetoothSocket;");
				IntPtr socket = JNIEnv.CallObjectMethod(device.Handle, createRfcommSocket, new Android.Runtime.JValue(1));
				_socket = Java.Lang.Object.GetObject<BluetoothSocket>(socket, JniHandleOwnership.TransferLocalRef);



			} catch (Java.IO.IOException ex) {
				Console.WriteLine ("create() failed: ", ex.Message);
			}

			// Make a connection to the BluetoothSocket
			try {
				// This is a blocking call and will only return on a
				// successful connection or an exception
				_socket.Connect ();
				string data = "Next";
				// Write data to the device
				_socket.OutputStream.Write (Encoding.ASCII.GetBytes (data), 0, data.Length);
			} catch (Java.IO.IOException ex) {
				// Close the socket
				try {
					_socket.Close ();
				} catch (Java.IO.IOException ex1) {
					Console.WriteLine ("Unable to close() socket during connection failure: ", ex1.Message);
				}
			}
		}

		private void Connect(string data)
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

			IntPtr createRfcommSocket = JNIEnv.GetMethodID(bluetoothDevice.Class.Handle, "createRfcommSocket", "(I)Landroid/bluetooth/BluetoothSocket;");
			IntPtr socket = JNIEnv.CallObjectMethod(bluetoothDevice.Handle, createRfcommSocket, new Android.Runtime.JValue(1));
			bluetoothSocket = Java.Lang.Object.GetObject<BluetoothSocket>(socket, JniHandleOwnership.TransferLocalRef);

			//bluetoothSocket = bluetoothDevice.CreateRfcommSocketToServiceRecord (serialUUID);   
			bluetoothSocket.Connect();
			outStream = bluetoothSocket.OutputStream;

			bluetoothSocket.OutputStream.Write (Encoding.ASCII.GetBytes (data), 0, data.Length);
			bluetoothSocket.Close ();
		}
	}
}