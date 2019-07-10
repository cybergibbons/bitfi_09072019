using System;
using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using System.ComponentModel;
using Android.Views;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
namespace BitfiWallet
{
    [Activity(Name = "my.MessageActivity", Theme = "@style/FullscreenTheme", Label = "", ExcludeFromRecents = true)]
    public class MessageActivity : Activity
    {
        BackgroundWorker BW = new BackgroundWorker();
        BackgroundWorker BW2 = new BackgroundWorker();
        public string EndPoint;
        AlertDialog bwalert;
        NoxKeys.NWS WS = new NoxKeys.NWS();
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            try
            {

                SetContentView(Resource.Layout.message);
                EditText tbMessage = FindViewById<EditText>(Resource.Id.tvMessage);
                tbMessage.Enabled = false;
                tbMessage.SetFocusable(ViewFocusability.NotFocusable);
                tbMessage.SetCursorVisible(false);
                tbMessage.Clickable = false;



                BW.DoWork += new DoWorkEventHandler(BW_DoWork);
                BW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BW_RunWorkerCompleted);

                BW2.DoWork += new DoWorkEventHandler(BW2_DoWork);
                BW2.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BW2_RunWorkerCompleted);
                LoadBClicks();
                AlertDialog.Builder builder = new AlertDialog.Builder(this).SetTitle("").SetMessage("Please wait...").SetCancelable(false);
                bwalert = builder.Create();
                Button send = (Button)FindViewById(Resource.Id.btnSendMessage);
                send.Click += delegate
                {
                    if (BW.IsBusy == false)
                    {
                        bwalert.Show();
                        BW.RunWorkerAsync();
                    }
                };
                BW2.RunWorkerAsync();

            }
            catch
            {
                Finish();
            }
        }
        private void StartService()
        {
            FindViewById(Resource.Id.llMessages).Visibility = ViewStates.Visible;


        }
        private void BW2_DoWork(object sender, DoWorkEventArgs e)
        {

            e.Result = WS.GetMessagesConfig();
        }
        private void BW2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result == null)
            {
                NoxDispose();

                return;
            }
            SGADWS.NoxMessagesConfig noxMessagesConfig = e.Result as SGADWS.NoxMessagesConfig;
            EndPoint = noxMessagesConfig.EndPoint;
            AlertDialog.Builder abuilder = new AlertDialog.Builder(this).SetTitle("")
  .SetMessage(noxMessagesConfig.WelcomeMessage)
  .SetCancelable(false)
  .SetPositiveButton("ok", (EventHandler<DialogClickEventArgs>)null);
            AlertDialog aalert = abuilder.Create();
            aalert.Show();
            var okBtn = aalert.GetButton((int)DialogButtonType.Positive);
            okBtn.Click += (asender, args) => {
                aalert.Dismiss();
                if (noxMessagesConfig.ServiceEnabled == false)
                {
                    NoxDispose();

                }
                else
                {
                    StartService();
                }
            };
        }
        private void BW_DoWork(object sender, DoWorkEventArgs e)
        {

            EditText tbMessage = FindViewById<EditText>(Resource.Id.tvMessage);
            e.Result = WS.SendMessage(EndPoint, tbMessage.Text);
        }
        private void BW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            bwalert.Dismiss();
            TxnResponse resp = e.Result as TxnResponse;
            TextView tbMessage = FindViewById<TextView>(Resource.Id.tvMessage);
            tbMessage.Text = "";
            secr = "";
            kbstr = "";
            string message = "Sent!";
            if (resp.success == false)
            {
                message = "Error. " + resp.error_message;
            }
            AlertDialog.Builder builder = new AlertDialog.Builder(this).SetTitle("")
                .SetMessage(message)
                .SetCancelable(false)
                .SetPositiveButton("ok", (EventHandler<DialogClickEventArgs>)null);
            AlertDialog alert = builder.Create();
            alert.Show();

            var okBtn = alert.GetButton((int)DialogButtonType.Positive);
            okBtn.Click += (asender, args) => {

                alert.Dismiss();
                if (resp.success)
                {
                    NoxDispose();

                }

            };
        }
        private void NoxDispose()
        {
            BW.DoWork -= new DoWorkEventHandler(BW_DoWork);
            BW.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(BW_RunWorkerCompleted);
            BW2.DoWork -= new DoWorkEventHandler(BW2_DoWork);
            BW2.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(BW2_RunWorkerCompleted);
            Finish();
        }
        public override void OnBackPressed()
        {
            NoxDispose();
            base.OnBackPressed();
        }
        public void Vibe()
        {
            new Thread(() =>
            {
                try
            {
                Vibrator v = (Vibrator)GetSystemService(Context.VibratorService);
                {
                    v.Vibrate(VibrationEffect.CreateOneShot(50, VibrationEffect.DefaultAmplitude));
                }
            }
            catch
            {
            }
        }).Start();
    }
    private Button mButton1;
    private Button mButton2;
    private Button mButton3;
    private Button mButton4;
    private Button mButton5;
    private Button mButton6;
    private Button mButton7;
    private Button mButton8;
    private Button mButton9;
    private Button mButton0;
    private Button mButtonA;
    private Button mButtonB;
    private Button mButtonC;
    private Button mButtonD;
    private Button mButtonE;
    private Button mButtonF;
    private Button mButtonG;
    private Button mButtonH;
    private Button mButtonI;
    private Button mButtonJ;
    private Button mButtonK;
    private Button mButtonL;
    private Button mButtonM;
    private Button mButtonN;
    private Button mButtonO;
    private Button mButtonP;
    private Button mButtonQ;
    private Button mButtonR;
    private Button mButtonS;
    private Button mButtonT;
    private Button mButtonU;
    private Button mButtonV;
    private Button mButtonW;
    private Button mButtonX;
    private Button mButtonY;
    private Button mButtonZ;
    private Button mButtonDelete;
    private Button mButtonEnter;
    private Button mButtonSpace;
    private Button mButtonShift;
    private Button mButtonCaps;
    private Button mButtonSemiC;
    private Button mButtonQuote;
    private Button mButtonUnderS;
    private Button mButtonAT;
    List<Button> buttonLIst;
        public string secr;
    public string kbstr;
    public bool capson;
    private void LoadBClicks()
    {
        buttonLIst = new List<Button>();
        mButton1 = (Button)FindViewById(Resource.Id.mwbutton_1);
        mButton2 = (Button)FindViewById(Resource.Id.mwbutton_2);
        mButton3 = (Button)FindViewById(Resource.Id.mwbutton_3);
        mButton4 = (Button)FindViewById(Resource.Id.mwbutton_4);
        mButton5 = (Button)FindViewById(Resource.Id.mwbutton_5);
        mButton6 = (Button)FindViewById(Resource.Id.mwbutton_6);
        mButton7 = (Button)FindViewById(Resource.Id.mwbutton_7);
        mButton8 = (Button)FindViewById(Resource.Id.mwbutton_8);
        mButton9 = (Button)FindViewById(Resource.Id.mwbutton_9);
        mButton0 = (Button)FindViewById(Resource.Id.mwbutton_0);
        mButtonA = (Button)FindViewById(Resource.Id.mwbutton_A);
        mButtonB = (Button)FindViewById(Resource.Id.mwbutton_B);
        mButtonC = (Button)FindViewById(Resource.Id.mwbutton_C);
        mButtonD = (Button)FindViewById(Resource.Id.mwbutton_D);
        mButtonE = (Button)FindViewById(Resource.Id.mwbutton_E);
        mButtonF = (Button)FindViewById(Resource.Id.mwbutton_F);
        mButtonG = (Button)FindViewById(Resource.Id.mwbutton_G);
        mButtonH = (Button)FindViewById(Resource.Id.mwbutton_H);
        mButtonI = (Button)FindViewById(Resource.Id.mwbutton_I);
        mButtonJ = (Button)FindViewById(Resource.Id.mwbutton_J);
        mButtonK = (Button)FindViewById(Resource.Id.mwbutton_K);
        mButtonL = (Button)FindViewById(Resource.Id.mwbutton_L);
        mButtonM = (Button)FindViewById(Resource.Id.mwbutton_M);
        mButtonN = (Button)FindViewById(Resource.Id.mwbutton_N);
        mButtonO = (Button)FindViewById(Resource.Id.mwbutton_O);
        mButtonP = (Button)FindViewById(Resource.Id.mwbutton_P);
        mButtonQ = (Button)FindViewById(Resource.Id.mwbutton_Q);
        mButtonR = (Button)FindViewById(Resource.Id.mwbutton_R);
        mButtonS = (Button)FindViewById(Resource.Id.mwbutton_S);
        mButtonT = (Button)FindViewById(Resource.Id.mwbutton_T);
        mButtonU = (Button)FindViewById(Resource.Id.mwbutton_U);
        mButtonV = (Button)FindViewById(Resource.Id.mwbutton_V);
        mButtonW = (Button)FindViewById(Resource.Id.mwbutton_W);
        mButtonX = (Button)FindViewById(Resource.Id.mwbutton_X);
        mButtonY = (Button)FindViewById(Resource.Id.mwbutton_Y);
        mButtonZ = (Button)FindViewById(Resource.Id.mwbutton_Z);
        mButtonSemiC = (Button)FindViewById(Resource.Id.mwbutton_SemiC);
        mButtonQuote = (Button)FindViewById(Resource.Id.mwbutton_Quote);
        mButtonUnderS = (Button)FindViewById(Resource.Id.mwbutton_UnderS);
        mButtonAT = (Button)FindViewById(Resource.Id.mwbutton_AT);

        buttonLIst.Add(mButton0);
        buttonLIst.Add(mButton1);
        buttonLIst.Add(mButton2);
        buttonLIst.Add(mButton3);
        buttonLIst.Add(mButton4);
        buttonLIst.Add(mButton5);
        buttonLIst.Add(mButton6);
        buttonLIst.Add(mButton7);
        buttonLIst.Add(mButton8);
        buttonLIst.Add(mButton9);
        buttonLIst.Add(mButtonA);
        buttonLIst.Add(mButtonB);
        buttonLIst.Add(mButtonC);
        buttonLIst.Add(mButtonD);
        buttonLIst.Add(mButtonE);
        buttonLIst.Add(mButtonF);
        buttonLIst.Add(mButtonG);
        buttonLIst.Add(mButtonH);
        buttonLIst.Add(mButtonI);
        buttonLIst.Add(mButtonJ);
        buttonLIst.Add(mButtonK);
        buttonLIst.Add(mButtonL);
        buttonLIst.Add(mButtonM);
        buttonLIst.Add(mButtonN);
        buttonLIst.Add(mButtonO);
        buttonLIst.Add(mButtonP);
        buttonLIst.Add(mButtonQ);
        buttonLIst.Add(mButtonR);
        buttonLIst.Add(mButtonS);
        buttonLIst.Add(mButtonT);
        buttonLIst.Add(mButtonU);
        buttonLIst.Add(mButtonV);
        buttonLIst.Add(mButtonW);
        buttonLIst.Add(mButtonX);
        buttonLIst.Add(mButtonY);
        buttonLIst.Add(mButtonZ);
        buttonLIst.Add(mButtonUnderS);
        buttonLIst.Add(mButtonQuote);
        buttonLIst.Add(mButtonSemiC);
        buttonLIst.Add(mButtonAT);
        EditText tbsecr = FindViewById<EditText>(Resource.Id.tvMessage);
        tbsecr.SetRawInputType(Android.Text.InputTypes.TextFlagNoSuggestions);
        kbstr = tbsecr.Text;
        mButtonSpace = (Button)FindViewById(Resource.Id.mwbutton_SPACE);
        mButtonSpace.Click += delegate
        {
            if (kbstr.Length > 0)
                {
                kbstr = kbstr + " ";
                tbsecr.Text = kbstr;
                tbsecr.SetSelection(kbstr.Length);
                Vibe();

            }
        };

        mButtonDelete = (Button)FindViewById(Resource.Id.mwbutton_delete);
        mButtonDelete.Click += delegate
        {
            if (kbstr.Length > 0)
                {
                kbstr = kbstr.Substring(0, kbstr.Length - 1);
                tbsecr.Text = kbstr;
                tbsecr.SetSelection(kbstr.Length);
            }
        };
        mButtonCaps = (Button)FindViewById(Resource.Id.mwbutton_CAPS);
        mButtonCaps.Visibility = ViewStates.Visible;

        mButtonUnderS.Visibility = ViewStates.Gone;
        mButtonQuote.Visibility = ViewStates.Gone;
        mButtonSemiC.Visibility = ViewStates.Gone;
        mButtonAT.Visibility = ViewStates.Gone;
        mButtonCaps.Click += delegate
        {
            if (mButtonCaps.Text == "CAPS")
            {
                mButtonCaps.Text = "SCAP";
                capson = true;
            }
            else
            {
                mButtonCaps.Text = "CAPS";
                capson = false;
            }
            foreach (Button b in buttonLIst)
            {
                if (capson)
                {
                    b.Text = b.Text.ToUpper();
                }
                else
                {
                    b.Text = b.Text.ToLower();
                }
            }
        };
        foreach (Button b in buttonLIst)
        {
            if (capson)
            {
                b.Text = b.Text.ToUpper();
            }
            else
            {
                b.Text = b.Text.ToLower();
            }
            b.Click += delegate
            {
                string val = b.Text;
                if (capson)
                {
                    val = val.ToUpper();
                }
                else
                {
                    val = val.ToLower();
                }
                kbstr = kbstr + val;
                tbsecr.Text = kbstr;
                tbsecr.SetSelection(kbstr.Length);
                Vibe();
            };
        }
        mButtonShift = (Button)FindViewById(Resource.Id.mwbutton_SHIFT);
        mButtonShift.Click += delegate
        {
            if (mButtonShift.Text == "^")
            {
                mButtonShift.Text = "<";
                    mButtonA.Text = "~";
                mButtonB.Text = "-";
                mButtonC.Text = "+";
                mButtonD.Text = "=";
                mButtonE.Text = "{";
                mButtonF.Text = "}";
                mButtonG.Text = "[";
                mButtonH.Text = "]";
                mButtonI.Text = "/";
                mButtonJ.Text = ">";
                    mButtonK.Text = "<";
                    mButtonL.Text = "?";
                mButtonM.Text = ".";
                mButtonN.Text = ",";
                mButtonO.Text = ")";
                mButtonP.Text = "(";
                mButtonQ.Text = "!";
                mButtonR.Text = "\\";
                mButtonS.Text = "#";
                mButtonT.Text = "$";
                mButtonU.Text = "%";
                mButtonV.Text = "^";
                mButtonW.Text = "&";
                mButtonX.Text = "*";
                mButtonY.Text = ";";
                mButtonZ.Text = "\"";

                mButtonUnderS.Visibility = ViewStates.Visible;
                mButtonQuote.Visibility = ViewStates.Visible;
                mButtonSemiC.Visibility = ViewStates.Visible;

                mButtonAT.Visibility = ViewStates.Visible;
                mButtonSpace.Visibility = ViewStates.Gone;
            }
            else
            {
                mButtonShift.Text = "^";
                mButtonA.Text = "A";
                mButtonB.Text = "B";
                mButtonC.Text = "C";
                mButtonD.Text = "D";
                mButtonE.Text = "E";
                mButtonF.Text = "F";
                mButtonG.Text = "G";
                mButtonH.Text = "H";
                mButtonI.Text = "I";
                mButtonJ.Text = "J";
                mButtonK.Text = "K";
                mButtonL.Text = "L";
                mButtonM.Text = "M";
                mButtonN.Text = "N";
                mButtonO.Text = "O";
                mButtonP.Text = "P";
                mButtonQ.Text = "Q";
                mButtonR.Text = "R";
                mButtonS.Text = "S";
                mButtonT.Text = "T";
                mButtonU.Text = "U";
                mButtonV.Text = "V";
                mButtonW.Text = "W";
                mButtonX.Text = "X";
                mButtonY.Text = "Y";
                mButtonZ.Text = "Z";
                foreach (Button b in buttonLIst)
                {
                    if (capson)
                    {
                        b.Text = b.Text.ToUpper();
                    }
                    else
                    {
                        b.Text = b.Text.ToLower();
                    }
                }

                mButtonUnderS.Visibility = ViewStates.Gone;
                mButtonQuote.Visibility = ViewStates.Gone;
                mButtonSemiC.Visibility = ViewStates.Gone;
                mButtonAT.Visibility = ViewStates.Gone;
                mButtonSpace.Visibility = ViewStates.Visible;
            }
        };
    }
}
}