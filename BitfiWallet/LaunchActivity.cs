using System;
using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using System.ComponentModel;
using System.Threading;
using Android.Graphics;
using Android.Views;
using System.Threading.Tasks;
namespace BitfiWallet
{
    [Activity(Name = "my.LaunchActivity", Label = "", Theme = "@style/FullscreenTheme", ExcludeFromRecents = true, HardwareAccelerated = true)]
    public class LaunchActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            try
            {
                Window.AddFlags(Android.Views.WindowManagerFlags.LayoutNoLimits | WindowManagerFlags.LayoutInOverscan | WindowManagerFlags.HardwareAccelerated);
                base.OnCreate(bundle);
                SetContentView(Resource.Layout.Main);

                LoadViews();

            }
            catch { }
        }
        protected override void OnStart()
        {
            base.OnStart();
            try
            {
                using (BatteryManager bm = (BatteryManager)ApplicationContext.GetSystemService(BatteryService))
                {
                    if (bm != null)
                    {
                        NoxKeys.Sclear.BatLevel = bm.GetIntProperty(4);
                        using (IntentFilter ifilter = new IntentFilter(Intent.ActionBatteryChanged))
                        {
                            using (Intent batteryStatus = RegisterReceiver(null, ifilter))
                            {
                                int status = batteryStatus.GetIntExtra(BatteryManager.ExtraPlugged, -1);
                                if (status \>\ 0 || bm.IsCharging)
                                {
                                    NoxKeys.Sclear.BatCharging = true;
                                }
                                else
                                {
                                    NoxKeys.Sclear.BatCharging = false;
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
            }
            LoadBatStatus();

        }
        protected override void OnResume()
        {
            ResumeModel();
            base.OnResume();
        }
        protected override void OnStop()
        {
            if (HWS != null)
            {
                if (!InPause)
                {
                    ShutDown();
                }
                if (HSesseion != HWS.CurrentHS)
                {
                    ShutDown();
                }
            }
            base.OnStop();
        }
        private void ResumeModel()
        {
            try
            {
                if (HWS != null && HSesseion != HWS.CurrentHS)
                {
                    ShutDown();
                }
                else
                {
                    InPause = false;
                    bwSTime = DateTime.Now.AddMinutes(1);
                    CapCnt = 0;
                    if (!Busy)
                    {
                        StartRequest(true);
                    }
                    else
                    {
                        Recheck = true;
                    }
                }
            }
            catch
            {
                ShutDown();
            }
        }
        private Guid HSesseion = Guid.NewGuid(); private NoxKeys.HWS HWS = null;
        private AlertDialog alert = null; private DateTime bwSTime = DateTime.Now;
        private bool StExp = false; private bool Busy = false;
        private bool Recheck = false; private int CapCnt = 0;
        private bool InPause; private bool GMCSent = false;
        private void StartRequest(bool Check)
        {
            if (HWS != null && HSesseion != HWS.CurrentHS) return;
            if (this.IsFinishing || this.IsDestroyed) return;
            if (InPause) return;
            if (Busy) return;
            Busy = true;
            Task t = Task.Factory.StartNew(() =\>\
            {
                try
                {
                    GC.Collect(0);
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    if (Check)
                    {
                        CapCnt = CapCnt + 1;
                        bool captive = NoxMessage.GetCaptive();
                        this.RunOnUiThread(() =\>\
                        {
                            Busy = false;
                            UIResp(captive, null);

                        });
                    }
                    else
                    {
                        if (HWS == null)
                        {
                            HSesseion = Guid.NewGuid();
                            HWS = new NoxKeys.HWS(HSesseion);
                        }
                        if (GMCSent != true)
                        {
                            HWS.SetGCMToken(HSesseion);
                            GMCSent = true;
                        }
                        Thread.Sleep(500);
                        var resp = HWS.GetCurrentSMSToken(HSesseion);
                        this.RunOnUiThread(() =\>\
                        {
                            Busy = false;
                            UIResp(null, resp);

                        });
                    }
                }
                catch { Busy = false; }
            });
        }
        private void UIResp(bool? captive, NOXWS.FormUserInfo formUserInfo)
        {
            if (this.IsFinishing || this.IsDestroyed) return;
            if (HWS != null && HSesseion != HWS.CurrentHS) return;
            if (captive != null)
            {
                LoadWifiStatus(captive);

            }
            if (captive == false || Recheck)
            {
                if (bwSTime \>\ DateTime.Now)
                {
                    Recheck = false;
                    StartRequest(true);
                }
                else
                {
                    ShutDownAndCloseX();
                }
            }
            else
            {
                if (formUserInfo == null)
                {
                    if (bwSTime \>\ DateTime.Now)
                    {
                        StartRequest(false);
                    }
                    else
                    {
                        ShutDownAndCloseX();
                    }
                }
                else
                {
                    InPause = true;
                    HSesseion = Guid.NewGuid();
                    if (alert.IsShowing) alert.Dismiss();
                    if (formUserInfo.ReqType == "address")
                    {
                        InitAddressSet(formUserInfo);
                    }
                    if (formUserInfo.ReqType == "txn")
                    {
                        InitTxnSet(formUserInfo);
                    }
                    if (formUserInfo.ReqType == "image")
                    {
                        InitImageSet(formUserInfo);
                    }
                    if (formUserInfo.ReqType == "gas")
                    {
                        InitGasSet(formUserInfo);
                    }
                    if (formUserInfo.ReqType == "message")
                    {
                        InitMsgSet(formUserInfo);
                    }
                    if (formUserInfo.ReqType == "signin")
                    {
                        try
                        {
                            AlertDialog.Builder builder = new AlertDialog.Builder(this).SetTitle("REQUEST " + formUserInfo.GMCToken).SetMessage("Sign-in to this request?")
                                .SetCancelable(false)
                                .SetNegativeButton("cancel", (EventHandler\\)null)
                                .SetPositiveButton("continue", (EventHandler\\)null);
                            AlertDialog alert = builder.Create();
                            alert.Show();
                            var okBtn = alert.GetButton((int)DialogButtonType.Positive);
                            okBtn.Click += (asender, args) =\>\
                            {
                                alert.Dismiss();
                                InitSigninModel(formUserInfo.GMCToken);
                            };
                            var cancelBtn = alert.GetButton((int)DialogButtonType.Negative);
                            cancelBtn.Click += (asender, args) =\>\
                            {
                                alert.Dismiss();
                                ShutDown();
                            };
                        }
                        catch { }
                    }
                }
            }
        }
        private void LoadWifiStatus(bool? captive)
        {
            var tbiStatus = FindViewById\\(Resource.Id.txtNoWifi);
            var wifilayout = FindViewById\\(Resource.Id.linearLayoutWifi);
            var w1 = FindViewById\\(Resource.Id.imageViewW1);
            var w2 = FindViewById\\(Resource.Id.imageViewW2);
            try
            {
                if (captive == false)
                {
                    if (CapCnt \>\ 2)
                    {
                        tbiStatus.Text = "There's no WiFi connection.";
                        wifilayout.Visibility = ViewStates.Visible;
                    }
                    w1.Visibility = ViewStates.Gone;
                    w2.Visibility = ViewStates.Visible;
                    FindViewById\\(Resource.Id.btnAddresses).Enabled = false;
                    FindViewById\\(Resource.Id.btnBalances).Enabled = false;
                }
                if (captive == true)
                {
                    tbiStatus.Text = "CONNECTED";
                    wifilayout.Visibility = ViewStates.Gone;
                    w1.Visibility = ViewStates.Visible;
                    w2.Visibility = ViewStates.Gone;
                    FindViewById\\(Resource.Id.btnAddresses).Enabled = true;
                    FindViewById\\(Resource.Id.btnBalances).Enabled = true;
                }
            }
            catch
            {
            }
        }
        private void LoadBatStatus()
        {
            try
            {
                LinearLayout batterylayout = FindViewById\\(Resource.Id.linearLayoutBattery);
                batterylayout.Visibility = ViewStates.Gone;
                TextView tbbStatus = FindViewById\\(Resource.Id.txtBPercent);
                var b1 = FindViewById\\(Resource.Id.imageViewB1);
                var b2 = FindViewById\\(Resource.Id.imageViewB2);
                b1.Visibility = ViewStates.Visible;
                b2.Visibility = ViewStates.Gone;
                tbbStatus.Text = "";
                bool battercharging = NoxKeys.Sclear.BatCharging; int batLevel = NoxKeys.Sclear.BatLevel; bool batterylow = false;
                if (batLevel \                tbbStatus.Text = batLevel + "%";
                if (batterylow)
                {
                    batterylayout.Visibility = ViewStates.Visible;
                }
                else
                {
                    batterylayout.Visibility = ViewStates.Gone;
                }
                if (battercharging)
                {
                    b1.Visibility = ViewStates.Gone;
                    b2.Visibility = ViewStates.Visible;
                }
                else
                {
                    b1.Visibility = ViewStates.Visible;
                    b2.Visibility = ViewStates.Gone;
                }
            }
            catch
            {
            }

        }
        private void LoadViews()
        {
            try
            {
                LinearLayout wifilayout = FindViewById\\(Resource.Id.linearLayoutWifi);
                TextView tbWel = FindViewById\\(Resource.Id.txtAppCode2);
                TextView tbWel22 = FindViewById\\(Resource.Id.txtAppCode22);
                var w1 = FindViewById\\(Resource.Id.imageViewW1);
                var w2 = FindViewById\\(Resource.Id.imageViewW2);
                TextView tbbWifi = FindViewById\\(Resource.Id.txtNoWifi);
                TextView tbbBat = FindViewById\\(Resource.Id.txtBatteryLow);
                Button btnWifi = FindViewById\\(Resource.Id.btnWifi);
                TextView btnWalletID = FindViewById\\(Resource.Id.btnWalletID);
                var btnAddresses = FindViewById\\(Resource.Id.btnAddresses);
                var btnBalances = FindViewById\\(Resource.Id.btnBalances);
                var btnMessage = FindViewById\\(Resource.Id.btnMessage);
                try
                {
                    wifilayout.Visibility = ViewStates.Gone;
                    w1.Visibility = ViewStates.Gone;
                    w2.Visibility = ViewStates.Visible;
                    if (NoxKeys.Sclear.typeface != null)
                    {
                        tbbWifi.Typeface = NoxKeys.Sclear.typeface;
                        tbbBat.Typeface = NoxKeys.Sclear.typeface;
                        btnWifi.Typeface = NoxKeys.Sclear.typeface;
                        btnWalletID.Typeface = NoxKeys.Sclear.typeface;
                        btnAddresses.Typeface = NoxKeys.Sclear.typeface;
                        btnBalances.Typeface = NoxKeys.Sclear.typeface;
                        btnMessage.Typeface = NoxKeys.Sclear.typeface;
                    }
                    if (NoxKeys.Sclear.typefaceI != null)
                    {
                        tbWel.Typeface = NoxKeys.Sclear.typefaceI;
                        tbWel22.Typeface = NoxKeys.Sclear.typefaceI;
                    }
                }
                catch
                {
                }

                btnWifi.Click += delegate
                {
                    var nxact = new Intent(this, typeof(NoxWifi));
                    InPause = true;
                    StartActivity(nxact);
                };
                btnBalances.Click += delegate
                {
                    try
                    {
                        InPause = false;
                        InitViewModel();
                    }
                    catch { }
                };
                btnAddresses.Click += delegate
                {
                    try
                    {
                        InPause = false;
                        InitAccountsModel();
                    }
                    catch { }
                };
                btnMessage.Click += delegate
                {
                    try
                    {
                        var nxact = new Intent(this, typeof(MessageActivity));
                        InPause = true;
                        StartActivity(nxact);
                    }
                    catch { }
                };
                AlertDialog.Builder builder = new AlertDialog.Builder(this)
                .SetTitle("[" + NoxKeys.Sclear.ThisVName + "]")
                .SetMessage("DEVICE ID: " + NoxKeys.Sclear.WALLETID).SetCancelable(true)
                .SetPositiveButton("ok", (EventHandler\\)null);
                alert = builder.Create();
                this.FindViewById(Resource.Id.btnWalletID).Click += delegate
                {
                    try
                    {
                        alert.Show();

                        var okBtn = alert.GetButton((int)DialogButtonType.Positive);
                        okBtn.Click += (asender, args) =\>\
                        {
                            alert.Dismiss();
                            Task t = Task.Factory.StartNew(() =\>\
                            {
                                try
                                {
                                    Thread.Sleep(2000);
                                    HWS.SetGCMToken(HSesseion);
                                    GMCSent = true;
                                    InPause = false;
                                    bwSTime = DateTime.Now.AddSeconds(30);
                                }
                                catch
                                {
                                }
                            });
                        };
                    }
                    catch { }
                };
            }
            catch
            {
                StExp = true;
            }
        }
        public override void OnBackPressed()
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this, Resource.Style.MyAlertDialogTheme).SetTitle("Close wallet?").SetMessage("").SetCancelable(false)
              .SetNegativeButton("NO", (EventHandler\\)null)
           .SetPositiveButton("YES", (EventHandler\\)null);
            AlertDialog talert = builder.Create();
            InPause = true;
            talert.Show();

            var okBtn = talert.GetButton((int)DialogButtonType.Positive);
            okBtn.Click += (asender, args) =\>\
               {
                   talert.Dismiss();
                   Task t = Task.Factory.StartNew(() =\>\
                   {
                       Thread.Sleep(200);
                       Busy = true;
                       InPause = true;
                       HSesseion = Guid.NewGuid();
                       bwSTime = DateTime.Now.AddDays(-2);
                       this.RunOnUiThread(() =\>\
                       {
                           Finish();
                           base.OnBackPressed();
                       });
                   });


               };
            var noBtn = talert.GetButton((int)DialogButtonType.Negative);
            noBtn.Click += (asender, args) =\>\
            {
                talert.Dismiss();
                InPause = false;
                ResumeModel();

            };
        }
        private void InitAccountsModel()
        {
            try
            {
                StartSAct("accounts", "");
            }
            catch
            {
            }
        }
        private void InitSigninModel(string DisplayCode)
        {
            try
            {
                StartSAct("signin", DisplayCode);
            }
            catch
            {
            }
        }
        private void InitViewModel()
        {
            try
            {
                StartSAct("overview", "");
            }
            catch
            {
            }
        }
        private void InitMsgSet(NOXWS.FormUserInfo formUserInfo)
        {
            try
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this)
                .SetTitle("")
                .SetMessage("SIGN MESSAGE?")
                .SetCancelable(true)
                .SetNegativeButton("NO", (EventHandler\\)null)
                .SetPositiveButton("Yes, Continue", (EventHandler\\)null);
                AlertDialog alert = builder.Create();
                alert.Show();
                var okBtn = alert.GetButton((int)DialogButtonType.Positive);
                var nokBtn = alert.GetButton((int)DialogButtonType.Negative);
                nokBtn.Click += (asender, args) =\>\
                {
                    ShutDown();
                    alert.Dismiss();
                };
                okBtn.Click += (asender, args) =\>\
                {
                    StartSAct("message", formUserInfo.SMSToken);
                    alert.Dismiss();
                };
            }
            catch
            {
            }
        }
        private void InitGasSet(NOXWS.FormUserInfo formUserInfo)
        {
            try
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this)
                .SetTitle("")
                .SetMessage("CLAIM GAS?")
                .SetCancelable(true)
                .SetNegativeButton("NO", (EventHandler\\)null)
                .SetPositiveButton("Yes, Continue", (EventHandler\\)null);
                AlertDialog alert = builder.Create();
                alert.Show();
                var okBtn = alert.GetButton((int)DialogButtonType.Positive);
                var nokBtn = alert.GetButton((int)DialogButtonType.Negative);
                nokBtn.Click += (asender, args) =\>\
                {
                    ShutDown();
                    alert.Dismiss();
                };
                okBtn.Click += (asender, args) =\>\
                {
                    StartSAct("gas", formUserInfo.SMSToken);
                    alert.Dismiss();
                };
            }
            catch
            {
            }
        }
        private void InitImageSet(NOXWS.FormUserInfo formUserInfo)
        {
            try
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this)
                .SetTitle("")
                .SetMessage("REBUILD LIST FOR XMR BALANCE?")
                .SetCancelable(true)
                .SetNegativeButton("NO", (EventHandler\\)null)
                .SetPositiveButton("Yes, Continue", (EventHandler\\)null);
                AlertDialog alert = builder.Create();
                alert.Show();
                var okBtn = alert.GetButton((int)DialogButtonType.Positive);
                var nokBtn = alert.GetButton((int)DialogButtonType.Negative);
                nokBtn.Click += (asender, args) =\>\
                {
                    ShutDown();
                    alert.Dismiss();
                };
                okBtn.Click += (asender, args) =\>\
                {
                    StartSAct("image", formUserInfo.SMSToken);
                    alert.Dismiss();
                };
            }
            catch
            {
            }
        }
        private void InitAddressSet(NOXWS.FormUserInfo formUserInfo)
        {
            try
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this)
                .SetTitle("")
                .SetMessage("ADD NEW ADDRESS?")
                .SetCancelable(true)
                .SetNegativeButton("NO", (EventHandler\\)null)
                .SetPositiveButton("Yes, Continue", (EventHandler\\)null);
                AlertDialog alert = builder.Create();
                alert.Show();
                var okBtn = alert.GetButton((int)DialogButtonType.Positive);
                var nokBtn = alert.GetButton((int)DialogButtonType.Negative);
                nokBtn.Click += (asender, args) =\>\
                {
                    ShutDown();
                    alert.Dismiss();
                };
                okBtn.Click += (asender, args) =\>\
                {
                    StartSAct("address", formUserInfo.SMSToken);
                    alert.Dismiss();
                };
            }
            catch
            {
            }
        }
        private void InitTxnSet(NOXWS.FormUserInfo formUserInfo)
        {
            try
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this)
                .SetTitle("SIGN")
                .SetMessage("Sign and pay?")
                .SetCancelable(true)
                .SetNegativeButton("NO", (EventHandler\\)null)
                .SetPositiveButton("Yes, Continue", (EventHandler\\)null);
                AlertDialog alert = builder.Create();
                alert.Show();
                var okBtn = alert.GetButton((int)DialogButtonType.Positive);
                var nokBtn = alert.GetButton((int)DialogButtonType.Negative);
                nokBtn.Click += (asender, args) =\>\
                {
                    ShutDown();
                    alert.Dismiss();
                };
                okBtn.Click += (asender, args) =\>\
                {
                    StartSAct("sign", formUserInfo.SMSToken);
                    alert.Dismiss();
                };
            }
            catch
            {
            }
        }
        private void StartSAct(string action, string task)
        {
            HSesseion = Guid.NewGuid();
            var nxact = new Intent(Application.Context, typeof(SecretActivity));
            nxact.PutExtra("action", action);
            nxact.PutExtra("task", task);
            StartActivity(nxact);
            ShutDown();
        }
        private void ShutDown()
        {
            ShutDownAndCloseX(false);
        }
        private bool disInProc = false;
        private void ShutDownAndCloseX(bool showAlert = true)
        {
            if (disInProc) return;
            disInProc = true;
            Busy = true;
            InPause = true;
            HSesseion = Guid.NewGuid();
            GC.Collect(0);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (showAlert)
            {
                Finish();
            }
            else
            {
                bwSTime = DateTime.Now.AddDays(-2);
                Finish();
            }
        }
    }
}

