using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Net;
using Android.Net.Wifi;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
namespace BitfiWallet
{
    [Activity(Label = "", Name = "my.NoxWifi", Theme = "@style/FullscreenTheme", ExcludeFromRecents = true)]
    public class NoxWifi : ListActivity
    {

        string[] wifis;
        public static List<string> filtered = null;
        static Activity activity;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            RequestWindowFeature(WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.wifi_maina);
            activity = this;
        }
        protected override void OnStop()
        {
            base.OnStop();
        }
        protected override void OnPause()
        {
            base.OnPause();
        }
        public override void OnBackPressed()
        {
            Finish();
            base.OnBackPressed();
        }
        protected override void OnResume()
        {
            try
            {
                string status = "WIFI NETWORKS: ";
                string SSID = "";
                using (Android.Net.Wifi.WifiManager wifi = (Android.Net.Wifi.WifiManager)ApplicationContext.GetSystemService(Context.WifiService))
                {
                    if (wifi != null)
                    {
                        if (wifi.IsWifiEnabled)
                        {
                            using (WifiInfo wifiInfo = wifi.ConnectionInfo)
                            {
                                if (wifiInfo != null)
                                {
                                    using (NetworkInfo.DetailedState state = WifiInfo.GetDetailedStateOf(wifiInfo.SupplicantState))
                                    {
                                        if (state == NetworkInfo.DetailedState.Connected || state == NetworkInfo.DetailedState.ObtainingIpaddr)
                                        {
                                            SSID = wifiInfo.SSID;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            wifi.SetWifiEnabled(true);
                        }

                        using (ConnectivityManager cm = (ConnectivityManager)GetSystemService(Context.ConnectivityService))
                        {
                            if (cm != null)
                            {
                                using (NetworkInfo activeNetwork = cm.ActiveNetworkInfo)
                                {
                                    if (activeNetwork != null && activeNetwork.IsConnected)
                                    {
                                        SSID = SSID.Replace("\"", "");
                                        status = ": CONNECTED TO INTERNET. Select a network from the list below to modify or establish a connection.";
                                        status = SSID + status;
                                    }
                                    else
                                    {
                                        if (activeNetwork == null)
                                        {
                                            status = "WIFI NETWORKS: ";
                                        }
                                        else
                                        {
                                            if (!string.IsNullOrEmpty(SSID))
                                            {
                                                status = " OBTAINING IP ADDRESS...";
                                                status = SSID + status;
                                            }
                                            else
                                            {
                                                status = "WIFI NETWORKS: ATTEMPTING CONNECTION...";
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        IList<ScanResult> wifiScanList = wifi.ScanResults;
                        if (wifiScanList != null)
                        {
                            wifis = new string[wifiScanList.Count];
                            for (int i = 0; i < wifiScanList.Count; i++)
                        {
                                wifis[i] = ((wifiScanList[i]).ToString());
                            }
                            filtered = new List<string>();
                            filtered.Add(status);
                            int counter = 0;
                            foreach (string eachWifi in wifis)
                            {
                                var rx = new string[] { "," };
                                string[] temp = eachWifi.Split(rx, StringSplitOptions.None);
                                string lval = temp[0].Substring(5).Trim();
                                if (!string.IsNullOrEmpty(lval) && lval.Length > 3)
                            {
                                    filtered.Add(lval);
                                    counter++;
                                }
                            }
                        }
                    }
                }
                ListAdapter = new MyListAdapter(this);
            }
            catch (Exception)
            {
            }

            base.OnResume();
        }
        class MyListAdapter : BaseAdapter
        {
            Context context;
            public MyListAdapter(Context c)
            {
                context = c;
            }
            public override Java.Lang.Object GetItem(int position)
            {
                return null;
            }
            public override long GetItemId(int position)
            {
                return position + 1;
            }
            int check = 0;
            public override int Count
            {
                get
                {
                    if (filtered == null)
                    {
                        return 0;
                    }
                    else
                    {
                        int count = filtered.Count;
                        return count;
                    }
                }
            }
            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                try
                {
                    convertView = LayoutInflater.From(context).Inflate(Resource.Layout.nlist, parent, false);
                    TextView tv1 = convertView.FindViewById<TextView>(Resource.Id.label);
                    tv1.Text = filtered[position];
                    check = check + 1;
                    convertView.FindViewById(Resource.Id.label).Click += delegate {
                        string ssid = filtered[position];
                        if (!ssid.Contains("WIFI NETWORKS:"))
                        {
                            connectToWifi(ssid);
                        }
                    };
                }
                catch (Exception)
                {
                }
                return convertView;
            }
            private void connectToWifi(string wifiSSID)
            {
                String SSID = "";
                bool connected = false;
                bool hasEverConnected = false;
                int NetworkId = 0;
                try
                {
                    using (Android.Net.Wifi.WifiManager wifi = (Android.Net.Wifi.WifiManager)context.ApplicationContext.GetSystemService(Context.WifiService))
                    {
                        if (wifi != null && wifi.IsWifiEnabled)
                        {
                            using (WifiInfo wifiInfo = wifi.ConnectionInfo)
                            {
                                if (wifiInfo != null)
                                {
                                    using (NetworkInfo.DetailedState state = WifiInfo.GetDetailedStateOf(wifiInfo.SupplicantState))
                                    {
                                        if (state == NetworkInfo.DetailedState.Connected || state == NetworkInfo.DetailedState.ObtainingIpaddr)
                                        {
                                            SSID = wifiInfo.SSID.Replace("\"", "").Trim();
                                            SSID = SSID.Replace(" ", "");
                                            if (SSID == wifiSSID.Trim().Replace(" ", ""))
                                            {
                                                connected = true;
                                            }
                                        }
                                    }
                                }
                            }

                            IList<WifiConfiguration> wifiScanList = wifi.ConfiguredNetworks;
                            for (int i = 0; i < wifiScanList.Count; i++)
                            {
                                if (((wifiScanList[i]).ToString()).Contains("hasEverConnected: true"))
                                {
                                    String cw = wifiScanList[i].Ssid.Replace("\"", "").Trim();
                                    cw = cw.Replace(" ", "");
                                    if (cw == wifiSSID.Trim().Replace(" ", ""))
                                    {
                                        NetworkId = wifiScanList[i].NetworkId;
                                        hasEverConnected = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                }



                var nxact = new Intent(Application.Context, typeof(ConnectActivity));
                nxact.PutExtra("SSID", wifiSSID);
                nxact.PutExtra("connected", connected);
                nxact.PutExtra("hasEverConnected", hasEverConnected);
                nxact.PutExtra("NetworkId", NetworkId);
                context.StartActivity(nxact);
                activity.Finish();
            }

        }
    }
}