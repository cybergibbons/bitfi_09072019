using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using System.Collections.Generic;
using Newtonsoft.Json;
using ZXing;
using ZXing.Common;
namespace BitfiWallet
{
    [Activity(Label = "", Name = "my.NoxViewModel", Theme = "@style/FullscreenTheme", ExcludeFromRecents = true)]
    public class NoxViewModel : ListActivity
    { 
        public static string action;
        public static string token;
        public static string addresses;
        BackgroundWorker xbw = new BackgroundWorker();
        public static SGADWS.WalletList[] filtered = new SGADWS.WalletList[0];
        public static BitfiWallet.NOXWS.NoxAddresses[] NWSfiltered = new BitfiWallet.NOXWS.NoxAddresses[0];
        static Activity activity;
        public static Typeface typeface;
        public ProgressBar pb;
        NoxKeys.NWS WS = new NoxKeys.NWS();
        protected override void OnCreate(Bundle bundle)
        {
            try
            {


            xbw.DoWork += new DoWorkEventHandler(xbw_DoWork);
            xbw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(xbw_RunWorkerCompleted);
            if (string.IsNullOrEmpty(action)) action = Intent.GetStringExtra("action");
            token = Intent.GetStringExtra("token");
            if (action == "accounts")
            {
                addresses = Intent.GetStringExtra("adrlist");
            }
                RequestWindowFeature(WindowFeatures.NoTitle);
                SetContentView(Resource.Layout.viewmodel_maina);
                activity = this;
                typeface = Typeface.CreateFromAsset(activity.Assets, "Rubik-Medium.ttf");
                xbw.RunWorkerAsync();
                base.OnCreate(bundle);
                RequestWindowFeature(WindowFeatures.NoTitle);
            }
            catch
            { }
        }
        private void xbw_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                e.Result = 0;
                filtered = new SGADWS.WalletList[0];
                NWSfiltered = new BitfiWallet.NOXWS.NoxAddresses[0];
                FindViewById\\(Resource.Id.linlaHeaderProgress).Visibility = ViewStates.Visible;
                pb = (ProgressBar)FindViewById(Resource.Id.progressBar);
                pb.Indeterminate = true;
                pb.Max = 100;
                pb.Min = 0;
                pb.SetProgress(0, true);
                if (action == "accounts")
                {
                    try
                    {
                        NWSfiltered = JsonConvert.DeserializeObject\\(addresses);
                        if (NWSfiltered != null && NWSfiltered.Length \>\ 1)
                        {
                            List\\ lnoxAddresses = new List\\();
                            BitfiWallet.NOXWS.NoxAddresses noxAddr = new NOXWS.NoxAddresses();
                            noxAddr.ViewKey = "Click on address to display QR code.";
                            noxAddr.BlkNet = "";
                            lnoxAddresses.Add(noxAddr);
                            lnoxAddresses.AddRange(NWSfiltered);
                            NWSfiltered = lnoxAddresses.ToArray();
                            Thread.Sleep(1000);
                        }
                        else
                        {
                            e.Result = 2;
                        }
                    }
                    catch
                    {
                        e.Result = 3;
                    }
                }
                else
                {
                    try
                    {

                        var VModel = WS.GetOverviewModel(token);
                        if (VModel != null && VModel.OverviewTableViewModel != null && VModel.OverviewTableViewModel.Wallets != null)
                        {
                            filtered = VModel.OverviewTableViewModel.Wallets;
                        }
                        else
                        {
                            e.Result = 1;
                        }
                    }
                    catch
                    {
                        e.Result = 3;
                    }
                }
            }
            catch
            {
                e.Result = 3;
            }
        }
        private void xbw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            pb.Indeterminate = false;
            pb.Max= 100;
            pb.SetProgress(100, true);
            FindViewById\\(Resource.Id.linlaHeaderProgress).Visibility = ViewStates.Gone;
            ListAdapter = new ViewListAdapter(this);
            if ((int) e.Result == 1)
            {
                DisplayMSG("Network error, please check connection.");
                return;
            }
            if ((int)e.Result == 2)
            {
                DisplayMSG("Processing error, please try again");
                return;
            }
            if ((int)e.Result == 3)
            {
                DisplayMSG("Error, please try again");
                return;
            }


        }
        public override void OnBackPressed()
        {
            AlertDialog.Builder tbuilder = new AlertDialog.Builder(this).SetTitle("").SetMessage("Exit now?").SetCancelable(true).SetNegativeButton("NO", (EventHandler\\)null).SetPositiveButton("YES", (EventHandler\\)null);
            AlertDialog talert = tbuilder.Create();
            talert.Show();
            var okBtn = talert.GetButton((int)DialogButtonType.Positive);
            okBtn.Click += (asender, args) =\>\
            {
                talert.Dismiss();
                NoxDispose();
                base.OnBackPressed();

            };
        }
        public void DisplayMSG(string msg)
        {

            AlertDialog.Builder builder = new AlertDialog.Builder(this).SetTitle("")
                .SetMessage(msg).SetCancelable(false)
                .SetPositiveButton("Ok", (EventHandler\\)null);
            AlertDialog alert = builder.Create();
            alert.Show();
            var okBtn = alert.GetButton((int)DialogButtonType.Positive);
            okBtn.Click += (asender, args) =\>\
            {

                alert.Dismiss();
                NoxDispose();

            };
        }
        private void NoxDispose()
        {
            try
            {

                addresses = null;
                token = null;
                action = null;
               xbw.DoWork -= new DoWorkEventHandler(xbw_DoWork);
               xbw.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(xbw_RunWorkerCompleted);


                NWSfiltered = null;
                Finish();
            }
            catch
            {
                Finish();
            }
        }
        class ViewListAdapter : BaseAdapter
        {
            Context context;     
            public ViewListAdapter(Context c)
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
            public override int Count
            {
                get
                {
                    if (action == "overview")
                    {
                        if (filtered == null)
                        {
                            return 0;
                        }
                        else
                        {
                            int count = filtered.Length;
                            return count;
                        }
                    }
                    else
                    {
                        if (NWSfiltered == null)
                        {
                            return 0;
                        }
                        else
                        {
                            int count = NWSfiltered.Length;
                            return count;
                        }
                    }
                }
            }
            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                try
                {

                    convertView = LayoutInflater.From(context).Inflate(Resource.Layout.overview, parent, false);
                  //  convertView.FindViewById\\(Resource.Id.ovt1).Typeface = typeface;
                  //  convertView.FindViewById\\(Resource.Id.ovt1b).Typeface = typeface;
                    convertView.FindViewById\\(Resource.Id.ovt2).Typeface = typeface;
                    convertView.FindViewById\\(Resource.Id.ovt3).Typeface = typeface;
                    if (action == "overview" && filtered != null)
                    {

                        TextView tv1 = convertView.FindViewById\\(Resource.Id.ovt1);
                        tv1.Text = filtered[position].Currency;
                        convertView.FindViewById\\(Resource.Id.ovt2).Text = filtered[position].Balance;
                        convertView.FindViewById\\(Resource.Id.ovt3).Text = filtered[position].USD;
                        convertView.FindViewById\\(Resource.Id.ovt1b).Visibility = ViewStates.Gone;
                        convertView.FindViewById\\(Resource.Id.ovt2).Visibility = ViewStates.Visible;
                        convertView.FindViewById\\(Resource.Id.ovt3).Visibility = ViewStates.Visible;
                        if (position == 0)
                        {
              //              convertView.FindViewById\\(Resource.Id.ovt1b).SetTextColor(new Color(Resource.Color.black_overlay));
                            convertView.FindViewById\\(Resource.Id.ovt1b).Text = "[refresh]";
                            convertView.FindViewById\\(Resource.Id.ovt1b).SetTypeface(Typeface.Monospace, TypefaceStyle.Italic);
                            convertView.FindViewById\\(Resource.Id.ovt1b).SetTextSize(Android.Util.ComplexUnitType.Sp, 13);
                            convertView.FindViewById\\(Resource.Id.ovt1b).Visibility = ViewStates.Visible;
                            convertView.FindViewById\\(Resource.Id.ovt3).Visibility = ViewStates.Gone;
                            convertView.FindViewById\\(Resource.Id.ovt2).Visibility = ViewStates.Gone;
                        }

                        convertView.FindViewById\\(Resource.Id.ovt1b).Click += delegate{
                            if (position == 0)
                            {
                                convertView.FindViewById\\(Resource.Id.ovt1b).Enabled = false;
                                convertView.FindViewById\\(Resource.Id.ovt1).Enabled = false;
                                activity.Recreate();
                            }
                        };
                        convertView.FindViewById\\(Resource.Id.ovt1).Click += delegate {
                            if (position == 0)
                            {
                                convertView.FindViewById\\(Resource.Id.ovt1b).Enabled = false;
                                convertView.FindViewById\\(Resource.Id.ovt1).Enabled = false;
                                activity.Recreate();
                            }
                        };
                    }
                    if (action == "accounts" && NWSfiltered != null)
                    {

                        TextView tv1 = convertView.FindViewById\\(Resource.Id.ovt1);
                        tv1.Visibility = ViewStates.Gone;
                        convertView.FindViewById\\(Resource.Id.ovt2).Text = NWSfiltered[position].BlkNet.ToUpper();
                        convertView.FindViewById\\(Resource.Id.ovt3).Text = NWSfiltered[position].BTCAddress;
                        convertView.FindViewById\\(Resource.Id.ovt2).SetTextSize(Android.Util.ComplexUnitType.Sp, 16);
                        convertView.FindViewById\\(Resource.Id.ovt3).SetTextSize(Android.Util.ComplexUnitType.Sp, 13); 

                        if (position == 0)
                        {
                          //  convertView.FindViewById\\(Resource.Id.ovt1b).SetTextColor(new Color(Resource.Color.black_overlay));
                            convertView.FindViewById\\(Resource.Id.ovt1b).Text = "[balances]";
                            convertView.FindViewById\\(Resource.Id.ovt1b).SetTypeface(Typeface.Monospace, TypefaceStyle.Italic);
                            convertView.FindViewById\\(Resource.Id.ovt1b).SetTextSize(Android.Util.ComplexUnitType.Sp, 12);
                            convertView.FindViewById\\(Resource.Id.ovt1b).Visibility = ViewStates.Visible;
                            convertView.FindViewById\\(Resource.Id.ovt3).Visibility = ViewStates.Gone;
                            convertView.FindViewById\\(Resource.Id.ovt2).Visibility = ViewStates.Gone;
                            tv1.Visibility = ViewStates.Visible;
                            tv1.Text = NWSfiltered[position].ViewKey;
                        }
                        convertView.FindViewById\\(Resource.Id.ovt1b).Click += delegate {
                            if (position == 0)
                            {
                                action = "overview";
                                activity.Recreate();
                            }
                        };
                        convertView.FindViewById(Resource.Id.ovinf).Click += delegate {
                            if (position \>\ 0)
                            {
                                ImageView image = new ImageView(context);
                                image.SetImageBitmap(ConvertQr(NWSfiltered[position].BTCAddress, NWSfiltered[position].BlkNet));
                                AlertDialog.Builder builder = new AlertDialog.Builder(context).SetTitle(NWSfiltered[position].BlkNet.ToUpper()).SetMessage(NWSfiltered[position].BTCAddress).SetCancelable(true).SetPositiveButton("CLOSE", (EventHandler\\)null);
                                AlertDialog alert = builder.Create();
                                alert.SetView(image);
                                alert.Show();
                                var okBtn = alert.GetButton((int)DialogButtonType.Positive);
                                okBtn.Click += (asender, args) =\>\
                                {
                                    alert.Dismiss();
                                };
                            }
                        };
                    }
                }
                catch (Exception)
                {
                }
                return convertView;
            }
            public void DisplayMSG(string msg)
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(context).SetTitle("").SetMessage(msg).SetCancelable(false).SetPositiveButton("Ok", (EventHandler\\)null);
                AlertDialog alert = builder.Create();
                alert.Show();
                var okBtn = alert.GetButton((int)DialogButtonType.Positive);
                okBtn.Click += (asender, args) =\>\
                {
                    alert.Dismiss();

                };
            }
            public Bitmap GenerateQrCodeRaw(string url, int height = 300, int width = 300, int margin = 10)
            {
                BarcodeWriter qrWriter = new BarcodeWriter();
                 qrWriter.Format = BarcodeFormat.QR_CODE;
                 qrWriter.Options = new EncodingOptions()
                 {
                     Height = height,
                     Width = width,
                     Margin = margin
                  };

                var barcode = qrWriter.Write(url);
                 return barcode;

            }
            public Bitmap ConvertQr(string address, string blk)
            {
                return GenerateQrCodeRaw(address);
            }
        }
    }
}