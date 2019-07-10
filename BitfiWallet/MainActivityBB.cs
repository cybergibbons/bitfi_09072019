using Android.App;
using Android.Content;
using Android.OS;
using System.Threading;
using System.Threading.Tasks;
using Android.Runtime;
using Android.Graphics;
using Android.Views;
namespace BitfiWallet
{


    [Activity(Name = "my.MainActivityN", MainLauncher = true, Label = "", Theme = "@style/FullscreenTheme", LaunchMode = Android.Content.PM.LaunchMode.SingleTop)]
    [MetaData("android.support.PARENT_ACTIVITY", Value = "com.rokits.noxadmin.NoxCosu")]

    public class MainActivityN : Activity
    {
        private bool isRunning = false;
        private bool isStarted = false;
        protected override void OnStart()
        {
            base.OnStart();
            if (isStarted)
            {
                FinishAndRemoveTask();
            }
            else
            {
                isStarted = true;
            }

        }
        private void GetAExt()
        {
            try
            {
                if (Intent.Extras != null && NoxKeys.Sclear.noxPrivateKey == null)
                {
                    var bund = Intent?.Extras;
                    var aprefuser = bund.GetString("aprefuser", "");
                    NoxKeys.Sclear.InitiateWS(new NBitcoin.BitcoinSecret(bund.GetString("token"), NBitcoin.Network.Main), aprefuser);
                    NoxKeys.Sclear.typeface = Typeface.CreateFromAsset(Assets, "Rubik-Regular.ttf");
                    NoxKeys.Sclear.typefaceI = Typeface.CreateFromAsset(Assets, "Rubik-Italic.ttf");
                }
            }
            catch { isRunning = false; }
        }
        private void StartSplash()
        {

            Task t = Task.Factory.StartNew(() =>
            {


                Thread.Sleep(500);
                GetAExt();
                var nxact = new Intent(this, typeof(LaunchActivity));
                if (isRunning)
                {
                    StartActivity(nxact);

                }
                else
                {
                    FinishAndRemoveTask();
                }
            });
        }

        public override void OnBackPressed()
        {
           isRunning = false;

            base.OnBackPressed();

        }
        protected override void OnCreate(Bundle bundle)
        {
            try
            {

                Window.AddFlags(Android.Views.WindowManagerFlags.LayoutNoLimits | WindowManagerFlags.LayoutInOverscan);
                SetContentView(Resource.Layout.splashmodel);
                base.OnCreate(bundle);

                isRunning = true;
                StartSplash();
            }
            catch
            {
                isRunning = false;
            }
        }

    }
}

