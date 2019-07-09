using System;
using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using System.ComponentModel;
using NoxKeys;
using System.Threading.Tasks;
using System.Threading;
using Android.Graphics;
using Android.Views;
namespace BitfiWallet
{
   [Activity(Name = "my.SecretActivity", Label = "", Theme = "@style/FullscreenTheme", HardwareAccelerated = true, ExcludeFromRecents = true)]

    public class SecretActivity : Activity
    {
        private List\\ buttonLIst;
        private bool capson;
        private bool SaltSelected;
        private bool SecrSelected;
        private int[] knoxsecr;
        private int[] knoxsalt;
        private LinearLayout tbsalt;
        private LinearLayout tbsecr;
        private BackgroundWorker bw = new BackgroundWorker();
        private BackgroundWorker tbw = new BackgroundWorker();
        private BackgroundWorker abw = new BackgroundWorker();
        private BackgroundWorker gbw = new BackgroundWorker();
        private BackgroundWorker ibw = new BackgroundWorker();
        private BackgroundWorker vbw = new BackgroundWorker();
        private string action;
        private string actiontask;
        private NWS WS = new NWS();
        private NOXWS.NoxTxnProcess tproc;
        private SignTransferResponse SendResponse;
        private AlertDialog bwalert;
        private string TryAddressHash;
        protected override void OnCreate(Bundle bundle)
        {
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
            tbw.DoWork += new DoWorkEventHandler(tbw_DoWork);
            tbw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(tbw_RunWorkerCompleted);
            abw.DoWork += new DoWorkEventHandler(abw_DoWork);
            abw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(abw_RunWorkerCompleted);
            ibw.DoWork += new DoWorkEventHandler(ibw_DoWork);
            ibw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ibw_RunWorkerCompleted);
            gbw.DoWork += new DoWorkEventHandler(gbw_DoWork);
            gbw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(gbw_RunWorkerCompleted);
            vbw.DoWork += new DoWorkEventHandler(vbw_DoWork);
            vbw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(vbw_RunWorkerCompleted);

            buttonLIst = null;
            capson = false;
            SaltSelected = false;
            SecrSelected = false;
            knoxsecr = null;
            knoxsalt = null;
            tbsalt = null;
            tbsecr = null;
            action = null;
            actiontask = null;
            tproc = null;
            SendResponse = null;
            bwalert = null;
            TryAddressHash = null;
            Window.AddFlags(Android.Views.WindowManagerFlags.LayoutNoLimits | WindowManagerFlags.LayoutInOverscan | WindowManagerFlags.HardwareAccelerated | WindowManagerFlags.Secure);
            SetContentView(Resource.Layout.Secret);
            base.OnCreate(bundle);
            action = Intent.GetStringExtra("action");
            actiontask = Intent.GetStringExtra("task");
            AlertDialog.Builder builder = new AlertDialog.Builder(this, Resource.Style.MyAlertDialogTheme).SetTitle("Processing request, please wait...").SetMessage("").SetCancelable(false);
            bwalert = builder.Create();
            tbsalt = FindViewById\\(Resource.Id.pos1);
            tbsecr = FindViewById\\(Resource.Id.pos2);
            knoxsalt = new int[0];
            knoxsecr = new int[0];
            LoadBEvents();
            LoadEEvent();
            if (action == "sign")
            {
                Task t = Task.Factory.StartNew(() =\>\
                {
                    try
                    {
                        tproc = null;
                        NOXWS.NoxTxnProcess req = WS.GetTxnRequest(actiontask);
                        if (req != null)
                        {
                            tproc = req;
                        }
                    }
                    catch
                    {
                        tproc = null;
                    }
                });
            }
        }
        private void KnoxAdd(int cval)
        {
            Vibe();
            if (SaltSelected)
            {
                NXArray_Add(cval, knoxsalt, out knoxsalt);
                KnoxUpdateView(knoxsalt, tbsalt);
            }
            else
            {
                NXArray_Add(cval, knoxsecr, out knoxsecr);
                KnoxUpdateView(knoxsecr, tbsecr);
            }

        }
        private void KnoxRemove()
        {
            //  Vibe();
            if (SaltSelected)
            {
                NXArray_Remove(knoxsalt, out knoxsalt);
                KnoxUpdateView(knoxsalt, tbsalt);
            }
            else
            {
                NXArray_Remove(knoxsecr, out knoxsecr);
                KnoxUpdateView(knoxsecr, tbsecr);
            }

        }
        private void KnoxUpdateView(int[] inputs, LinearLayout box)
        {
            box.RemoveAllViews();
            int cnt = 0;
            int gg = inputs.Length - 1;
            for (int i = gg; i \>\= 0; i--)
            {
                if (cnt \                {
                    ImageView imageView = new ImageView(ApplicationContext);
                    imageView.SetImageBitmap(Sclear.SCharImage(inputs[i]).BITMAP);
                    box.AddView(imageView, 0);
                    cnt = cnt + 1;
                }
                else
                {
                    return;
                }
            }
        }
        private void vbw_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                string sgamsg = WS.GetSGAMessage();
                if (!string.IsNullOrEmpty(sgamsg))
                {
                    using (NoxKeys.NoxKeyGen keys = new NoxKeyGen())
                    {
                        string tkmsg = "";
                        string adrlist = "";
                        if (action == "accounts")
                        {
                            string[] adrresp = keys.SignSGAMessageOut(knoxsalt, knoxsecr, sgamsg);
                            tkmsg = adrresp[0];
                            adrlist = adrresp[1];
                        }
                        if (action == "overview") tkmsg = keys.SignSGAMessage(knoxsalt, knoxsecr, sgamsg);
                        if (action == "signin") tkmsg = keys.SignSGAMessageWithCode(knoxsalt, knoxsecr, sgamsg, actiontask);
                        if (tkmsg == "0")
                        {
                            e.Result = new string[2] { "0", "" };
                            return;
                        }
                        if (tkmsg == "10" || tkmsg == "-10")
                        {
                            e.Result = new string[2] { "5", "" };
                            return;
                        }
                        if (string.IsNullOrEmpty(tkmsg))
                        {
                            e.Result = new string[2] { "1", "" };
                            return;
                        }
                        if (tkmsg.Length \                        {
                            e.Result = new string[2] { "2", "" };
                            return;
                        }
                        else
                        {
                            e.Result = new string[2] { tkmsg, adrlist };
                            return;
                        }
                    }
                }
                else
                {
                    e.Result = new string[2] { "3", "" };
                    return;
                }
            }
            catch (Exception)
            {
                e.Result = new string[2] { "4", "" };
                return;
            }
        }
        private void vbw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string[] result = e.Result as string[];
            if (result[0].Length \>\ 10)
            {
                var nxact = new Intent(Application.Context, typeof(NoxViewModel));

                nxact.PutExtra("action", action);
                nxact.PutExtra("token", result[0]);
                nxact.PutExtra("adrlist", result[1]);
                bwalert.Dismiss();
                StartActivity(nxact);
                NoxDispose(false);
            }
            else
            {
                bwalert.Dismiss();
                Sclear.EraseIntegers(knoxsalt);
                Sclear.EraseIntegers(knoxsecr);
                tbsalt.RemoveAllViews();
                tbsecr.RemoveAllViews();
                knoxsalt = new int[0];
                knoxsecr = new int[0];
                LinearLayout ltmain = FindViewById\\(Resource.Id.pos0);
                ltmain.Visibility = Android.Views.ViewStates.Visible;
                if (result[0] == "0")
                {
                    DisplayMSG("0");
                    return;
                }
                if (result[0] == "5")
                {
                    DisplayMSG("Invalid signin for this session.");
                    NoxFinish();
                    return;
                }
                string sresult = "Error;";
                if (result[0] == "1") sresult = "Error requesting profile.";
                if (result[0] == "2") sresult = "No profile available.";
                if (result[0] == "3") sresult = "Communication error.";
                if (result[0] == "4") sresult = "Unexpected error, please try again.";
                DisplayMSG(sresult);
            }
        }
        private void StartCompute()
        {
            tbsalt.RemoveAllViews();
            tbsecr.RemoveAllViews();
            if (action == "overview" || action == "accounts" || action == "signin")
            {
                string sdmsg2 = "";
                string sdMsg = "working, please wait...";

                if (action == "signin")
                {
                    sdMsg = "|\>\ DMA-2 LOGIN";
                    sdmsg2 = "authenticating...";
                }
                vbw.RunWorkerAsync();
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetTitle(sdMsg);
                builder.SetCancelable(false);
                builder.SetMessage(sdmsg2);
                bwalert = builder.Create();
                bwalert.Show();
                LinearLayout ltmain = FindViewById\\(Resource.Id.pos0);
                ltmain.Visibility = Android.Views.ViewStates.Invisible;
            }
            if (action == "address")
            {
                bw.RunWorkerAsync();
                bwalert.Show();
            }
            if (action == "sign")
            {
                if (tproc == null)
                {
                    AlertDialog.Builder tbuilder = new AlertDialog.Builder(this).SetTitle("Sign").SetMessage("Sorry, error getting request.").SetCancelable(false)
                   .SetPositiveButton("Ok", (EventHandler\\)null);
                    AlertDialog talert = tbuilder.Create();
                    talert.Show();
                    var okBtn = talert.GetButton((int)DialogButtonType.Positive);
                    okBtn.Click += (asender, args) =\>\
                    {
                        NoxFinish();
                    };
                }
                else
                {
                    abw.RunWorkerAsync();
                    bwalert.Show();
                }
            }
            if (action == "image")
            {
                ibw.RunWorkerAsync();
                bwalert.Show();
            }
            if (action == "gas")
            {
                gbw.RunWorkerAsync();
                bwalert.Show();
            }
            if (action == "message")
            {
                ProcessMsgTask();
                bwalert.Show();
            }
        }
        private void ProcessMsgTask()
        {
            Task t = Task.Factory.StartNew(() =\>\
            {
                string resp = "";
                try
                {
                    NOXWS.NoxMsgProcess req = WS.GetMsgRequest(actiontask);
                    if (req != null)
                    {
                        using (NoxKeyGen noxKeyGen = new NoxKeyGen())
                        {
                            var msgTaskTransferResponse = noxKeyGen.SignUserMsg(knoxsalt, knoxsecr, req);
                            if (!string.IsNullOrEmpty(msgTaskTransferResponse.Error))
                            {
                                resp = msgTaskTransferResponse.Error;
                            }
                            else
                            {
                                resp = WS.SubmitMsgResponse(req.TXNLineID, msgTaskTransferResponse.MsgSig);
                            }
                        }
                    }
                    else
                    {
                        resp = "Error";
                    }
                }
                catch (Exception ex)
                {
                    resp = ex.Message;
                }
                finally
                {
                    this.RunOnUiThread(() =\>\
                    {
                        bwalert.Dismiss();
                        Sclear.EraseIntegers(knoxsalt);
                        Sclear.EraseIntegers(knoxsecr);
                        tbsalt.RemoveAllViews();
                        tbsecr.RemoveAllViews();
                        knoxsalt = new int[0];
                        knoxsecr = new int[0];
                        DisplayMSG(resp);
                    });
                }
            });
        }
        public override void OnBackPressed()
        {
            Sclear.EraseIntegers(knoxsalt);
            Sclear.EraseIntegers(knoxsecr);
            tbsalt.RemoveAllViews();
            tbsecr.RemoveAllViews();
            knoxsalt = new int[0];
            knoxsecr = new int[0];
              AlertDialog.Builder tbuilder = new AlertDialog.Builder(this).SetTitle("Exit").SetMessage("Close wallet now?").SetCancelable(false).SetNegativeButton("NO", (EventHandler\\)null).SetPositiveButton("YES", (EventHandler\\)null);
               AlertDialog talert = tbuilder.Create();
              talert.Show();
              LinearLayout ltmain = FindViewById\\(Resource.Id.pos0);
              ltmain.Visibility = Android.Views.ViewStates.Invisible;
            var okBtn = talert.GetButton((int)DialogButtonType.Positive);
            okBtn.Click += (asender, args) =\>\
             {
              talert.Dismiss();
                 ReleaseBW();
            GC.Collect(0);
            GC.Collect();
            GC.WaitForPendingFinalizers();
                 Finish();
            base.OnBackPressed();
             };
             var noBtn = talert.GetButton((int)DialogButtonType.Negative);
             noBtn.Click += (asender, args) =\>\
             {
                talert.Dismiss();
                Sclear.EraseIntegers(knoxsalt);
                Sclear.EraseIntegers(knoxsecr);
                tbsalt.RemoveAllViews();
                tbsecr.RemoveAllViews();
               knoxsalt = new int[0];
               knoxsecr = new int[0];
                ltmain.Visibility = Android.Views.ViewStates.Visible;
             };
        }
        private void NoxFinish()
        {
            NoxDispose();
        }
        private void ReleaseBW()
        {
            try
            {
                vbw.DoWork -= new DoWorkEventHandler(vbw_DoWork);
                vbw.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(vbw_RunWorkerCompleted);

            }
            catch { }
            try
            {
                bw.DoWork -= new DoWorkEventHandler(bw_DoWork);
                bw.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);

            }
            catch { }
            try
            {
                tbw.DoWork -= new DoWorkEventHandler(tbw_DoWork);
                tbw.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(tbw_RunWorkerCompleted);

            }
            catch { }
            try
            {
                abw.DoWork -= new DoWorkEventHandler(abw_DoWork);
                abw.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(abw_RunWorkerCompleted);

            }
            catch { }
            try
            {
                ibw.DoWork -= new DoWorkEventHandler(ibw_DoWork);
                ibw.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(ibw_RunWorkerCompleted);

            }
            catch { }
            try
            {
                gbw.DoWork -= new DoWorkEventHandler(gbw_DoWork);
                gbw.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(gbw_RunWorkerCompleted);

            }
            catch { }
        }
        private void NoxDispose(bool WAlert = true)
        {
            try
            {
                Sclear.EraseIntegers(knoxsalt);
                Sclear.EraseIntegers(knoxsecr);
                tbsalt.RemoveAllViews();
                tbsecr.RemoveAllViews();
                knoxsalt = new int[0];
                knoxsecr = new int[0];  
            }
            catch
            {
            }
            ReleaseBW();
             GC.Collect(0);
             GC.Collect();
             GC.WaitForPendingFinalizers();

             Finish();

        }
        private void TryExpMsg()
        {
            if (!Sclear.SendExpMsg) return;
            AlertDialog.Builder abuilder = new AlertDialog.Builder(this).SetTitle("")
.SetMessage("It is possible that one or more experimental security checks have failed, please inform with dev/messages. Thanks!")
.SetCancelable(false)
.SetPositiveButton("ok", (EventHandler\\)null);
            AlertDialog aalert = abuilder.Create();
            aalert.Show();
            var okBtn = aalert.GetButton((int)DialogButtonType.Positive);
            okBtn.Click += (asender, args) =\>\
            {
                aalert.Dismiss();
            };
        }
        private void ibw_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                e.Result = "1";
                NOXWS.NoxImageRequests req = WS.GetImageRequest(actiontask);
                if (req == null)
                {
                    e.Result = "1";
                    return;
                }
                using (NoxKeys.NoxKeyGen keys = new NoxKeyGen())
                {
                    var resp = keys.XMR_GetImages(knoxsalt, knoxsecr, req.PublicTable);
                    if (!string.IsNullOrEmpty(resp.Error))
                    {
                        e.Result = resp.Error;
                    }
                    else
                    {
                        string respONSE = WS.RespondImageRequest(req.TXNLineID, resp.SpendKeyImages, resp.WalletAddress);
                        if (respONSE == "0")
                        {
                            e.Result = "0";
                        }
                        else
                        {
                            e.Result = "1";
                        }
                    }
                }
            }
            catch
            {
                e.Result = "1";
            }
        }
        private void ibw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            knoxsalt = new int[0];
            knoxsecr = new int[0];
            bwalert.Dismiss();
            if (e.Result == null)
            {
                DisplayMSG("Connect Error.");
                return;
            }
            int result = Convert.ToInt16(e.Result);
            if (result == 0)
            {
                DisplayMSGSuccess("Success!");
                return;
            }
            DisplayMSG("Error.");
        }
        private void gbw_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                e.Result = "1";
                NOXWS.NoxGasRequests req = WS.GetGasRequest(actiontask);
                if (req == null)
                {
                    e.Result = "Error getting request, please retry.";
                    return;
                }
                using (NoxKeyGen keys = new NoxKeyGen())
                {
                    var resp = keys.NEO_ClaimGas(knoxsalt, knoxsecr, req);
                    if (!string.IsNullOrEmpty(resp.Error))
                    {
                        e.Result = resp.Error;
                    }
                    else
                    {
                        var respONSE = WS.SubmitGasResponse(req.TXNLineID, resp.TxnHex);
                        e.Result = respONSE;
                    }
                }
            }
            catch (Exception ex)
            {
                e.Result = ex.Message;
            }
        }
        private void gbw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            knoxsalt = new int[0];
            knoxsecr = new int[0];
            bwalert.Dismiss();
            DisplayMSG(Convert.ToString(e.Result));
        }
        private void abw_DoWork(object sender, DoWorkEventArgs e)
        {
            SignTransferResponse signTransferResponse = new SignTransferResponse();
            using (NoxKeys.NoxKeyGen keys = new NoxKeyGen())
            {
                if (tproc == null)
                {
                    signTransferResponse.Error = "Error getting request.";
                    e.Result = signTransferResponse;
                    return;
                }
                signTransferResponse.LineID = tproc.TXNLineID;
                signTransferResponse.Amount = tproc.Amount;
                signTransferResponse.Blk = tproc.BlkNet;
                signTransferResponse.BlkDisplayName = tproc.BlkNet;
                signTransferResponse.ToAddress = tproc.ToAddress;
                if (!string.IsNullOrEmpty(tproc.ETCToken))
                {
                    signTransferResponse.BlkDisplayName = tproc.ETCTokenName;
                }
                try
                {
                    if (tproc.BlkNet == "neo" || tproc.BlkNet == "gas")
                    {
                        var resp = keys.NEO_Sign(knoxsalt, knoxsecr, tproc.MXTxn, tproc.BlkNet, tproc.ToAddress, tproc.Amount, tproc.NoxAddress.BTCAddress);
                        signTransferResponse.Error = resp.Error;
                        signTransferResponse.TxnHex = resp.TxnHex;
                    }
                    if (tproc.BlkNet == "eth")
                    {
                        var resp = keys.ETH_Sign(knoxsalt, knoxsecr, tproc);
                        signTransferResponse.Error = resp.Error;
                        signTransferResponse.TxnHex = resp.TxnHex;
                    }
                    if (tproc.BlkNet == "apl")
                    {
                        var resp = keys.APL_Sign(knoxsalt, knoxsecr, tproc);
                        signTransferResponse.Error = resp.Error;
                        signTransferResponse.TxnHex = resp.TxnHex;
                    }
                    if (tproc.BlkNet == "xrp")
                    {
                        var resp = keys.Rip_Sign(knoxsalt, knoxsecr, tproc);
                        signTransferResponse.Error = resp.Error;
                        signTransferResponse.TxnHex = resp.TxnHex;
                    }
                    if (tproc.BlkNet == "xmr")
                    {
                        var resp = keys.XMR_Sign(knoxsalt, knoxsecr, tproc.ToAddress, tproc.Amount, tproc.MXTxn, tproc.NoxAddress.BTCAddress);
                        signTransferResponse.Error = resp.Error;
                        signTransferResponse.TxnHex = resp.TxnHex;
                        signTransferResponse.SpendKeyImages = resp.SpendKeyImages;
                        if (string.IsNullOrEmpty(signTransferResponse.Error))
                        {
                            if (signTransferResponse.SpendKeyImages == null || signTransferResponse.SpendKeyImages.Length == 0 || string.IsNullOrEmpty(signTransferResponse.SpendKeyImages[0]))
                            {
                                signTransferResponse.Error = "Unable to create all txn components.";
                            }
                        }
                    }
                    if (keys.GetBLKNetworkAlt(tproc.BlkNet) != null)
                    {
                        var resp = keys.Alt_Sign(knoxsalt, knoxsecr, tproc, tproc.BlkNet);
                        signTransferResponse.Error = resp.Error;
                        signTransferResponse.TxnHex = resp.TxnHex;
                        signTransferResponse.TotalFee = resp.FeeAmount;
                        signTransferResponse.FeeWarning = resp.FeeWarning;
                    }
                }
                catch (Exception ex)
                {
                    signTransferResponse.Error = ex.Message;
                }
            }
            e.Result = signTransferResponse;
        }
        private void abw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                Sclear.EraseIntegers(knoxsalt);
                Sclear.EraseIntegers(knoxsecr);
                tbsalt.RemoveAllViews();
                tbsecr.RemoveAllViews();
                knoxsalt = new int[0];
                knoxsecr = new int[0];
                bwalert.Dismiss();
                SignTransferResponse signTransferResponse = e.Result as SignTransferResponse;
                if (string.IsNullOrEmpty(signTransferResponse.Error))
                {
                    string msg = "Send " + signTransferResponse.Amount + " to " + signTransferResponse.ToAddress + "?";
                    if (!string.IsNullOrEmpty(signTransferResponse.TotalFee))
                    {
                        msg = "Send " + signTransferResponse.Amount + " to " + signTransferResponse.ToAddress + " with fee " + signTransferResponse.TotalFee + " ?";
                        if (!string.IsNullOrEmpty(signTransferResponse.FeeWarning))
                        {
                            msg = signTransferResponse.FeeWarning + " Continue anyway to send " + signTransferResponse.Amount + " to " + signTransferResponse.ToAddress + " with fee " + signTransferResponse.TotalFee + " ?";
                        }
                    }
                    AlertDialog.Builder tbuilder = new AlertDialog.Builder(this).SetTitle("|\>\ " + signTransferResponse.BlkDisplayName.ToUpper() + " Payment").SetMessage(msg).SetCancelable(false)
                    .SetNegativeButton("NO", (EventHandler\\)null).SetPositiveButton("Yes, Continue", (EventHandler\\)null);
                    AlertDialog talert = tbuilder.Create();
                    talert.Show();
                    var okBtn = talert.GetButton((int)DialogButtonType.Positive);
                    okBtn.Click += (asender, args) =\>\
                    {
                        SendResponse = signTransferResponse;
                        talert.Dismiss();
                        AlertDialog.Builder builder = new AlertDialog.Builder(this).SetTitle("|\>\ " + signTransferResponse.BlkDisplayName.ToUpper() + " Payment").SetMessage("Ok, sending now...").SetCancelable(false);
                        bwalert = builder.Create();
                        bwalert.Show();
                        tbw.RunWorkerAsync();
                    };
                    var NOTokBtn = talert.GetButton((int)DialogButtonType.Negative);
                    NOTokBtn.Click += (asender, args) =\>\
                    {
                        signTransferResponse = null;
                        SendResponse = null;
                        talert.Dismiss();
                        NoxFinish();
                    };
                }
                else
                {
                    DisplayMSG(signTransferResponse.Error);
                    signTransferResponse = null;
                    SendResponse = null;
                }
            }
            catch (Exception ex)
            {
                DisplayMSG(ex.Message);
            }
        }
        private void tbw_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (SendResponse.Blk != "xmr")
                {
                    string req = WS.SubmitTxnResponse(SendResponse.LineID, SendResponse.TxnHex);
                    e.Result = req;
                }
                else
                {
                    string req = WS.SubmitTxnResponseMX(SendResponse.LineID, SendResponse.TxnHex, SendResponse.SpendKeyImages);
                    e.Result = req;
                }
            }
            catch (Exception ex)
            {
                e.Result = ex.Message;
            }
            SendResponse = null;
        }
        private void tbw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            knoxsalt = new int[0];
            knoxsecr = new int[0];
            bwalert.Dismiss();
            DisplayMSG2(Convert.ToString(e.Result));
        }
        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            NOXWS.NoxAddressRequests req = WS.GetRequest(actiontask);
            e.Result = -1;
            if (req != null)
            {
                var afst = WS.GetFirstAddress(req.TXNLineID);
                if (afst == null)
                {
                    e.Result = 6; //invalid request or any ws error 
                    return;
                }
                string fst = afst[0];
                string fstblk = afst[1];
                if (!string.IsNullOrEmpty(fst))
                {
                    try
                    {
                        using (NoxKeys.NoxKeyGen keys = new NoxKeyGen())
                        {
                            if (keys.GetBLKNetworkAlt(req.BlkNet) == null && req.BlkNet != "apl" && req.BlkNet != "xrp")
                            {
                                e.Result = 77; return;
                            }
                            if (knoxsecr == null || knoxsalt == null)
                            {
                                e.Result = 6; return;
                            }
                            var adr = keys.GetNewAddress(knoxsalt, knoxsecr, req.HDIndex, req.BlkNet, fst, req.DoSegwit);
                            if (adr == null)
                            {
                                e.Result = 5;
                            }
                            else
                            {
                                var send = WS.RespondAddressRequest(req.TXNLineID, adr);
                                if (send == "0")
                                {
                                    e.Result = 0;
                                }
                                else
                                {
                                    e.Result = 2;
                                }
                            }
                        }
                    }
                    catch
                    {
                        e.Result = 5; return;
                    }
                }
                else
                {
                    try
                    {
                        using (NoxKeys.NoxKeyGen keys = new NoxKeyGen())
                        {
                            string b64 = keys.GetTestHash(knoxsalt, knoxsecr);
                            if (string.IsNullOrEmpty(TryAddressHash))  //instruct to repeat info
                            {
                                TryAddressHash = b64;
                                b64 = "0";
                                e.Result = 1001;
                                Sclear.Clear(knoxsalt);
                                Sclear.Clear(knoxsecr);
                                return;
                            }
                            else
                            {
                                if (b64 != TryAddressHash || string.IsNullOrEmpty(b64)) //repeat or cancel dialog if no match
                                {
                                    b64 = "0";
                                    e.Result = 8;
                                    return;
                                }
                                else
                                {
                                    Sclear.LastHashAtp = TryAddressHash;
                                    AdrCollection collection = keys.GetNewWalletCollection(knoxsalt, knoxsecr);
                                    TryAddressHash = "0";
                                    string btcadr = collection.BTC;
                                    string ltcadr = collection.LTC;
                                    string ethadr = collection.ETH;
                                    string[] moneroadr = collection.XMR;
                                    string neoaddress = collection.NEO;
                                    if (moneroadr == null || string.IsNullOrEmpty(moneroadr[0]) || string.IsNullOrEmpty(moneroadr[1]) || string.IsNullOrEmpty(btcadr) || string.IsNullOrEmpty(ltcadr) || string.IsNullOrEmpty(ethadr))
                                    {
                                        e.Result = 6;
                                        return;
                                    }
                                    if (string.IsNullOrEmpty(neoaddress))
                                    {
                                        e.Result = 6;
                                        return;
                                    }
                                    NOXWS.FirstAdrCollection Addresses = new NOXWS.FirstAdrCollection();
                                    Addresses.BTC = btcadr;
                                    Addresses.LTC = ltcadr;
                                    Addresses.ETH = ethadr;
                                    Addresses.NEO = neoaddress;
                                    Addresses.Monero = moneroadr;
                                    var send = WS.RespondFirstAddressRequest(req.TXNLineID, Addresses);
                                    if (send == "0")
                                    {
                                        e.Result = 0;
                                    }
                                    else
                                    {
                                        e.Result = 2;
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                        e.Result = 6;
                        return;
                    }
                }
            }
            else
            {
                e.Result = 3;
            }
        }
        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            knoxsalt = new int[0];
            knoxsecr = new int[0];
            bwalert.Dismiss();
            if (e.Result == null) return;
            int result = Convert.ToInt16(e.Result);
            if (result == 0)
            {
                DisplayMSGSuccess("Success!");
                return;
            }
            if (result == 1001)
            {
                DisplayMSG("FIRST ADDRESS ALERT \>\ Now repeat the passphrase and salt exactly as you have just entered, it's very important that you remember both since this is your first address.");
                return;
            }
            if (result == 8)
            {
                DisplayMSG("Information does not match, repeat the passphrase and salt exactly as you entered or start over by initiating a new address request in your dashboard.");
                return;
            }
            if (result == 1)
            {
                DisplayMSG("Incorrect info, please use the same pw and salt.");
                return;
            }
            if (result == 2)
            {
                DisplayMSG("Server error when adding new address, please try again.");
                return;
            }
            if (result == 5)
            {
                DisplayMSG("Invalid info, try again.");
                return;
            }
            if (result == 6)
            {
                DisplayMSG("Invalid length");
                return;
            }
            if (result == 7)
            {
                DisplayMSG("Single address currency type.");
                return;
            }
            if (result == 77)
            {
                DisplayMSG("Currency not supported in this wallet version.");
                return;
            }
            else
            {
                DisplayMSG("Error");
                return;
            }
        }
        public void DisplayMSGInfo(string msg)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this).SetTitle("").SetMessage(msg).SetCancelable(false).SetPositiveButton("Ok", (EventHandler\\)null);
            AlertDialog alert = builder.Create();
            alert.Show();
            var okBtn = alert.GetButton((int)DialogButtonType.Positive);
            okBtn.Click += (asender, args) =\>\
            {
                alert.Dismiss();
            };
        }
        public void DisplayMSG(string msg)
        {
            string imsg = msg;
            if (msg == "0") msg = "Success!";
            if (string.IsNullOrEmpty(msg)) msg = "Connect Error";
            AlertDialog.Builder builder = new AlertDialog.Builder(this).SetTitle("Request").SetMessage(msg).SetCancelable(false).SetPositiveButton("Ok", (EventHandler\\)null);
            AlertDialog alert = builder.Create();
            alert.Show();
            var okBtn = alert.GetButton((int)DialogButtonType.Positive);
            okBtn.Click += (asender, args) =\>\
            {
                alert.Dismiss();
                if (msg == "Success!")
                {
                    NoxFinish();
                }
            };
        }
        public void DisplayMSG2(string msg)
        {
            string imsg = msg;
            if (msg == "0") msg = "Success!";
            if (string.IsNullOrEmpty(msg)) msg = "Connect Error";
            AlertDialog.Builder builder = new AlertDialog.Builder(this).SetTitle("Request").SetMessage(msg).SetCancelable(false).SetPositiveButton("Ok", (EventHandler\\)null);
            AlertDialog alert = builder.Create();
            alert.Show();
            var okBtn = alert.GetButton((int)DialogButtonType.Positive);
            okBtn.Click += (asender, args) =\>\
            {
                alert.Dismiss();
                NoxFinish();
            };
        }
        public void DisplayMSGSuccess(string msg)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this).SetTitle("Request").SetMessage(msg).SetCancelable(false).SetPositiveButton("Ok", (EventHandler\\)null);
            AlertDialog alert = builder.Create();
            alert.Show();
            var okBtn = alert.GetButton((int)DialogButtonType.Positive);
            okBtn.Click += (asender, args) =\>\
            {
                alert.Dismiss();
                NoxFinish();
            };
        }
        public void DisplayMSGQuest(string msg)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this).SetTitle("Request").SetMessage(msg).SetCancelable(false).SetNegativeButton("NO", (EventHandler\\)null).SetPositiveButton("Yes, Continue", (EventHandler\\)null);
            AlertDialog alert = builder.Create();
            alert.Show();
            var okBtn = alert.GetButton((int)DialogButtonType.Positive);
            okBtn.Click += (asender, args) =\>\
            {
                alert.Dismiss();
            };
        }
        private void NXArray_Add(int addval, int[] nxlist, out int[] updated)
        {
            updated = new int[nxlist.Length + 1];
            for (int i = 0; i \            {
                updated[i] = nxlist[i];
                nxlist[i] = -1;
            }
            updated[nxlist.Length] = addval;
            Array.Clear(nxlist, 0, nxlist.Length);
            nxlist = null;
        }
        private void NXArray_Remove(int[] nxlist, out int[] updated)
        {
            if (nxlist.Length \            {
                updated = nxlist;
                return;
            }
            updated = new int[nxlist.Length - 1];
            for (int i = 0; i \            {
                updated[i] = nxlist[i];
                nxlist[i] = -1;
            }
            nxlist[nxlist.Length - 1] = -1;
            Array.Clear(nxlist, 0, nxlist.Length);
            nxlist = null;
        }
        public void LoadEEvent()
        {
            LinearLayout toptbsalt = FindViewById\\(Resource.Id.ttag1);
            LinearLayout toptbsecr = FindViewById\\(Resource.Id.ttag2);
            TextView tagsalt = FindViewById\\(Resource.Id.tag1);
            TextView tagsecr = FindViewById\\(Resource.Id.tag2);
            SaltSelected = true;
            toptbsalt.SetBackgroundResource(Resource.Color.space);
            tagsalt.SetBackgroundResource(Resource.Color.space);
            toptbsecr.SetBackgroundResource(Resource.Color.black);
            tagsecr.SetBackgroundResource(Resource.Color.black);
            Button mButtonEnter;
            mButtonEnter = (Button)FindViewById(Resource.Id.button_enter);
            mButtonEnter.Tag = 1005;
            mButtonEnter.Click += delegate
            {
                if (SaltSelected)
                {
                    SaltSelected = false;
                    SecrSelected = true;
                    toptbsecr.SetBackgroundResource(Resource.Color.space);
                    tagsecr.SetBackgroundResource(Resource.Color.space);
                    toptbsalt.SetBackgroundResource(Resource.Color.black);
                    tagsalt.SetBackgroundResource(Resource.Color.black);
                }
                else
                {
                    if (knoxsecr.Length \                    {
                        DisplayMSG("Secret. At least 30 characters required.");
                        SaltSelected = false;
                        SecrSelected = true;
                        toptbsecr.SetBackgroundResource(Resource.Color.space);
                        tagsecr.SetBackgroundResource(Resource.Color.space);
                        toptbsalt.SetBackgroundResource(Resource.Color.black);
                        tagsalt.SetBackgroundResource(Resource.Color.black);
                        return;
                    }
                    if (knoxsalt.Length \                    {
                        DisplayMSG("Salt. At least 6 characters required.");
                        SecrSelected = false;
                        SaltSelected = true;
                        toptbsalt.SetBackgroundResource(Resource.Color.space);
                        tagsalt.SetBackgroundResource(Resource.Color.space);
                        toptbsecr.SetBackgroundResource(Resource.Color.black);
                        tagsecr.SetBackgroundResource(Resource.Color.black);
                        return;
                    }
                    SecrSelected = false;
                    SaltSelected = true;
                    toptbsalt.SetBackgroundResource(Resource.Color.space);
                    tagsalt.SetBackgroundResource(Resource.Color.space);
                    toptbsecr.SetBackgroundResource(Resource.Color.black);
                    tagsecr.SetBackgroundResource(Resource.Color.black);
                    StartCompute();
                }
            };
            tbsalt.Click += delegate
            {
                SaltSelected = true;
                SecrSelected = false;
                toptbsalt.SetBackgroundResource(Resource.Color.space);
                tagsalt.SetBackgroundResource(Resource.Color.space);
                toptbsecr.SetBackgroundResource(Resource.Color.black);
                tagsecr.SetBackgroundResource(Resource.Color.black);
            };
            tagsalt.Click += delegate
            {
                SaltSelected = true;
                SecrSelected = false;
                toptbsalt.SetBackgroundResource(Resource.Color.space);
                tagsalt.SetBackgroundResource(Resource.Color.space);
                toptbsecr.SetBackgroundResource(Resource.Color.black);
                tagsecr.SetBackgroundResource(Resource.Color.black);
            };
            tbsecr.Click += delegate
            {
                SaltSelected = false;
                SecrSelected = true;
                toptbsecr.SetBackgroundResource(Resource.Color.space);
                tagsecr.SetBackgroundResource(Resource.Color.space);
                toptbsalt.SetBackgroundResource(Resource.Color.black);
                tagsalt.SetBackgroundResource(Resource.Color.black);
            };
            tagsecr.Click += delegate
            {
                SaltSelected = false;
                SecrSelected = true;
                toptbsecr.SetBackgroundResource(Resource.Color.space);
                tagsecr.SetBackgroundResource(Resource.Color.space);
                toptbsalt.SetBackgroundResource(Resource.Color.black);
                tagsalt.SetBackgroundResource(Resource.Color.black);
            };
        }
        public void Vibe()
        {
            Task t = Task.Factory.StartNew(() =\>\
            {
                try
                {
                    using (Vibrator v = (Vibrator)GetSystemService(Context.VibratorService))
                    {
                        v.Vibrate(VibrationEffect.CreateOneShot(50, VibrationEffect.DefaultAmplitude));
                    }
                }
                catch
                {
                }
            });
        }
        private void LoadBEvents()
        {
            buttonLIst = new List\\();
            Button mButton1; Button mButton2; Button mButton3; Button mButton4; Button mButton5; Button mButton6; Button mButton7; Button mButton8; Button mButton9; Button mButton0; Button mButtonA; Button mButtonB; Button mButtonC; Button mButtonD; Button mButtonE; Button mButtonF; Button mButtonG; Button mButtonH; Button mButtonI; Button mButtonJ; Button mButtonK; Button mButtonL; Button mButtonM; Button mButtonN; Button mButtonO; Button mButtonP; Button mButtonQ; Button mButtonR; Button mButtonS; Button mButtonT; Button mButtonU; Button mButtonV; Button mButtonW; Button mButtonX; Button mButtonY; Button mButtonZ; Button mButtonDelete; Button mButtonSpace; Button mButtonShift; Button mButtonCaps;
            mButton1 = (Button)FindViewById(Resource.Id.button_1); mButton2 = (Button)FindViewById(Resource.Id.button_2); mButton3 = (Button)FindViewById(Resource.Id.button_3); mButton4 = (Button)FindViewById(Resource.Id.button_4); mButton5 = (Button)FindViewById(Resource.Id.button_5); mButton6 = (Button)FindViewById(Resource.Id.button_6); mButton7 = (Button)FindViewById(Resource.Id.button_7); mButton8 = (Button)FindViewById(Resource.Id.button_8); mButton9 = (Button)FindViewById(Resource.Id.button_9); mButton0 = (Button)FindViewById(Resource.Id.button_0); mButtonA = (Button)FindViewById(Resource.Id.button_A); mButtonB = (Button)FindViewById(Resource.Id.button_B); mButtonC = (Button)FindViewById(Resource.Id.button_C); mButtonD = (Button)FindViewById(Resource.Id.button_D); mButtonE = (Button)FindViewById(Resource.Id.button_E); mButtonF = (Button)FindViewById(Resource.Id.button_F); mButtonG = (Button)FindViewById(Resource.Id.button_G); mButtonH = (Button)FindViewById(Resource.Id.button_H); mButtonI = (Button)FindViewById(Resource.Id.button_I); mButtonJ = (Button)FindViewById(Resource.Id.button_J); mButtonK = (Button)FindViewById(Resource.Id.button_K); mButtonL = (Button)FindViewById(Resource.Id.button_L); mButtonM = (Button)FindViewById(Resource.Id.button_M); mButtonN = (Button)FindViewById(Resource.Id.button_N); mButtonO = (Button)FindViewById(Resource.Id.button_O); mButtonP = (Button)FindViewById(Resource.Id.button_P); mButtonQ = (Button)FindViewById(Resource.Id.button_Q); mButtonR = (Button)FindViewById(Resource.Id.button_R); mButtonS = (Button)FindViewById(Resource.Id.button_S); mButtonT = (Button)FindViewById(Resource.Id.button_T); mButtonU = (Button)FindViewById(Resource.Id.button_U); mButtonV = (Button)FindViewById(Resource.Id.button_V); mButtonW = (Button)FindViewById(Resource.Id.button_W); mButtonX = (Button)FindViewById(Resource.Id.button_X); mButtonY = (Button)FindViewById(Resource.Id.button_Y); mButtonZ = (Button)FindViewById(Resource.Id.button_Z);
            buttonLIst.Add(mButton0); buttonLIst.Add(mButton1); buttonLIst.Add(mButton2); buttonLIst.Add(mButton3); buttonLIst.Add(mButton4); buttonLIst.Add(mButton5); buttonLIst.Add(mButton6); buttonLIst.Add(mButton7); buttonLIst.Add(mButton8); buttonLIst.Add(mButton9); buttonLIst.Add(mButtonA); buttonLIst.Add(mButtonB); buttonLIst.Add(mButtonC); buttonLIst.Add(mButtonD); buttonLIst.Add(mButtonE); buttonLIst.Add(mButtonF); buttonLIst.Add(mButtonG); buttonLIst.Add(mButtonH); buttonLIst.Add(mButtonI); buttonLIst.Add(mButtonJ); buttonLIst.Add(mButtonK); buttonLIst.Add(mButtonL); buttonLIst.Add(mButtonM); buttonLIst.Add(mButtonN); buttonLIst.Add(mButtonO); buttonLIst.Add(mButtonP); buttonLIst.Add(mButtonQ); buttonLIst.Add(mButtonR); buttonLIst.Add(mButtonS); buttonLIst.Add(mButtonT); buttonLIst.Add(mButtonU); buttonLIst.Add(mButtonV); buttonLIst.Add(mButtonW); buttonLIst.Add(mButtonX); buttonLIst.Add(mButtonY); buttonLIst.Add(mButtonZ);
            mButtonA.Tag = 0; mButtonB.Tag = 1; mButtonC.Tag = 2; mButtonD.Tag = 3; mButtonE.Tag = 4; mButtonF.Tag = 5; mButtonG.Tag = 6; mButtonH.Tag = 7; mButtonI.Tag = 8; mButtonJ.Tag = 9; mButtonK.Tag = 10; mButtonL.Tag = 11; mButtonM.Tag = 12; mButtonN.Tag = 13; mButtonO.Tag = 14; mButtonP.Tag = 15; mButtonQ.Tag = 16; mButtonR.Tag = 17; mButtonS.Tag = 18; mButtonT.Tag = 19; mButtonU.Tag = 20; mButtonV.Tag = 21; mButtonW.Tag = 22; mButtonX.Tag = 23; mButtonY.Tag = 24; mButtonZ.Tag = 25; mButton0.Tag = 52; mButton1.Tag = 53; mButton2.Tag = 54; mButton3.Tag = 55; mButton4.Tag = 56; mButton5.Tag = 57; mButton6.Tag = 58; mButton7.Tag = 59; mButton8.Tag = 60; mButton9.Tag = 61;
            mButtonSpace = (Button)FindViewById(Resource.Id.button_SPACE);
            mButtonSpace.Tag = 88;
            mButtonSpace.Click += delegate
            {
                KnoxAdd((int)mButtonSpace.Tag);
            };
            mButtonDelete = (Button)FindViewById(Resource.Id.button_delete);
            mButtonDelete.Tag = 1002;
            mButtonDelete.Click += delegate
            {
                KnoxRemove();
            };
            mButtonShift = (Button)FindViewById(Resource.Id.button_SHIFT);
            mButtonCaps = (Button)FindViewById(Resource.Id.button_CAPS);
            mButtonShift.Tag = 1001;
            mButtonCaps.Tag = 1000;
            mButtonCaps.Click += delegate
            {
                mButtonShift.Text = "^";
                if (mButtonCaps.Text == "CAPS")
                {
                    mButtonCaps.Text = "SCAP";
                    capson = true;
                    mButtonA.Tag = 26; mButtonB.Tag = 27; mButtonC.Tag = 28; mButtonD.Tag = 29; mButtonE.Tag = 30; mButtonF.Tag = 31; mButtonG.Tag = 32; mButtonH.Tag = 33; mButtonI.Tag = 34; mButtonJ.Tag = 35; mButtonK.Tag = 36; mButtonL.Tag = 37; mButtonM.Tag = 38; mButtonN.Tag = 39; mButtonO.Tag = 40; mButtonP.Tag = 41; mButtonQ.Tag = 42; mButtonR.Tag = 43; mButtonS.Tag = 44; mButtonT.Tag = 45; mButtonU.Tag = 46; mButtonV.Tag = 47; mButtonW.Tag = 48; mButtonX.Tag = 49; mButtonY.Tag = 50; mButtonZ.Tag = 51;
                }
                else
                {
                    mButtonCaps.Text = "CAPS";
                    capson = false;
                    mButtonA.Tag = 0; mButtonB.Tag = 1; mButtonC.Tag = 2; mButtonD.Tag = 3; mButtonE.Tag = 4; mButtonF.Tag = 5; mButtonG.Tag = 6; mButtonH.Tag = 7; mButtonI.Tag = 8; mButtonJ.Tag = 9; mButtonK.Tag = 10; mButtonL.Tag = 11; mButtonM.Tag = 12; mButtonN.Tag = 13; mButtonO.Tag = 14; mButtonP.Tag = 15; mButtonQ.Tag = 16; mButtonR.Tag = 17; mButtonS.Tag = 18; mButtonT.Tag = 19; mButtonU.Tag = 20; mButtonV.Tag = 21; mButtonW.Tag = 22; mButtonX.Tag = 23; mButtonY.Tag = 24; mButtonZ.Tag = 25;
                }
                foreach (Button b in buttonLIst)
                {
                    b.SetText(new char[] { Sclear.SCharImage((int)b.Tag).SCHAR }, 0, 1);
                }
            };
            foreach (Button b in buttonLIst)
            {
                b.SetText(new char[] { Sclear.SCharImage((int)b.Tag).SCHAR }, 0, 1);
                b.Click += delegate
                {
                    if ((int)b.Tag \                    {
                        if (capson)
                        {
                            if ((int)b.Tag \                        }
                        else
                        {
                            if ((int)b.Tag \>\ 25) b.Tag = (int)b.Tag - 26;
                        }
                    }
                    KnoxAdd((int)b.Tag);
                };
            }
            mButtonShift.Click += delegate
            {
                if (mButtonShift.Text == "^")
                {
                    mButtonShift.Text = "\                    mButtonA.Tag = 62; mButtonB.Tag = 63; mButtonC.Tag = 64; mButtonD.Tag = 65; mButtonE.Tag = 66; mButtonF.Tag = 67; mButtonG.Tag = 68; mButtonH.Tag = 69; mButtonI.Tag = 70; mButtonJ.Tag = 71; mButtonK.Tag = 72; mButtonL.Tag = 73; mButtonM.Tag = 74; mButtonN.Tag = 75; mButtonO.Tag = 76; mButtonP.Tag = 77; mButtonQ.Tag = 78; mButtonR.Tag = 79; mButtonS.Tag = 80; mButtonT.Tag = 81; mButtonU.Tag = 82; mButtonV.Tag = 83; mButtonW.Tag = 84; mButtonX.Tag = 85; mButtonY.Tag = 86; mButtonZ.Tag = 87;
                }
                else
                {
                    mButtonShift.Text = "^";
                    if (capson)
                    {
                        mButtonA.Tag = 26; mButtonB.Tag = 27; mButtonC.Tag = 28; mButtonD.Tag = 29; mButtonE.Tag = 30; mButtonF.Tag = 31; mButtonG.Tag = 32; mButtonH.Tag = 33; mButtonI.Tag = 34; mButtonJ.Tag = 35; mButtonK.Tag = 36; mButtonL.Tag = 37; mButtonM.Tag = 38; mButtonN.Tag = 39; mButtonO.Tag = 40; mButtonP.Tag = 41; mButtonQ.Tag = 42; mButtonR.Tag = 43; mButtonS.Tag = 44; mButtonT.Tag = 45; mButtonU.Tag = 46; mButtonV.Tag = 47; mButtonW.Tag = 48; mButtonX.Tag = 49; mButtonY.Tag = 50; mButtonZ.Tag = 51;
                    }
                    else
                    {
                        mButtonA.Tag = 0; mButtonB.Tag = 1; mButtonC.Tag = 2; mButtonD.Tag = 3; mButtonE.Tag = 4; mButtonF.Tag = 5; mButtonG.Tag = 6; mButtonH.Tag = 7; mButtonI.Tag = 8; mButtonJ.Tag = 9; mButtonK.Tag = 10; mButtonL.Tag = 11; mButtonM.Tag = 12; mButtonN.Tag = 13; mButtonO.Tag = 14; mButtonP.Tag = 15; mButtonQ.Tag = 16; mButtonR.Tag = 17; mButtonS.Tag = 18; mButtonT.Tag = 19; mButtonU.Tag = 20; mButtonV.Tag = 21; mButtonW.Tag = 22; mButtonX.Tag = 23; mButtonY.Tag = 24; mButtonZ.Tag = 25;
                    }
                }
                foreach (Button b in buttonLIst)
                {
                    b.SetText(new char[] { Sclear.SCharImage((int)b.Tag).SCHAR }, 0, 1);
                }
            };
        }

    }

}