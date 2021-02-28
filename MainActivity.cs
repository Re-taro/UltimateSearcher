using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Views;
using System;
using System.Web.NBitcoin;
using System.Threading;


namespace UltimateSearcher
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity
    {
        struct Result
        {
            public string google;
            public string twitter;
            public string qiita;
            public string youtube;
        }

        enum Searchmode
        {
            google,twitter,qiita,youtube,none
        }

        Result[] result = new Result[10];
        Searchmode searchmode = new Searchmode();


        public TextView result_view { get; private set; }
        public EditText searchword { get; private set; }

        public ImageButton button_google { get; private set; }
        public ImageButton button_twitter { get; private set; }
        public ImageButton button_qiita { get; private set; }
        public ImageButton button_youtube { get; private set; }
        public ImageButton button_search { get; private set; }

        public bool Handled { get; set; }


        protected override void OnCreate(Bundle savedInstanceState)
        {  base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            var systemUiFlags = SystemUiFlags.LayoutStable
            | SystemUiFlags.LayoutHideNavigation
            | SystemUiFlags.LayoutFullscreen
            | SystemUiFlags.HideNavigation
            | SystemUiFlags.Fullscreen
            | SystemUiFlags.ImmersiveSticky;
            Window.DecorView.SystemUiVisibility = (StatusBarVisibility)(int)systemUiFlags;


            Thread thread1 = new Thread(
                () =>
                {
                    result_view = FindViewById<TextView>(Resource.Id.result_view);

                    searchword = FindViewById<EditText>(Resource.Id.search_bar);

                    button_google = FindViewById<ImageButton>(Resource.Id.button_google);
                    button_google.Click += button_google_Click;

                    button_twitter = FindViewById<ImageButton>(Resource.Id.button_twitter);
                    button_twitter.Click += button_twitter_Click;
                });
            thread1.Start();

            Thread thread2 = new Thread(
                () =>
                {
                    button_qiita = FindViewById<ImageButton>(Resource.Id.button_qiita);
                    button_qiita.Click += button_qiita_Click;

                    button_youtube = FindViewById<ImageButton>(Resource.Id.button_youtube);
                    button_youtube.Click += button_youtube_Click;

                    button_search = FindViewById<ImageButton>(Resource.Id.button_search);
                    button_search.Click += button_search_Click;
                });
            thread2.Start();

            for (int i = 0; i < 10; i++)
            {
                result[i] = new Result();
            }

            searchword.KeyPress += (object sender, View.KeyEventArgs e) =>
            {
                e.Handled = false;
                if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
                {

                    e.Handled = true;
                }
            };
        }

        void resultview()
        {
            result_view.Text = searchword.Text;
            for (int i = 0; i < 10; i++)
            {
                result_view.Text += "\n";
                switch (searchmode)
                {
                    case Searchmode.google:
                        result_view.Text += result[i].google;
                        break;
                    case Searchmode.twitter:
                        result_view.Text += result[i].twitter;
                        break;
                    case Searchmode.qiita:
                        result_view.Text += result[i].qiita;
                        break;
                    case Searchmode.youtube:
                        result_view.Text += result[i].youtube;
                        break;
                    default:
                        break;
                }
            }
        }

        private void button_google_Click(object sender, EventArgs e)
        {
            searchmode = Searchmode.google;
            resultview();
        }

        private void button_twitter_Click(object sender, EventArgs e)
        {
            searchmode = Searchmode.twitter;
            resultview();
        }

        private void button_qiita_Click(object sender, EventArgs e)
        {
            searchmode = Searchmode.qiita;
            resultview();
        }

        private void button_youtube_Click(object sender, EventArgs e)
        {
            searchmode = Searchmode.youtube;
            resultview();
        }

        private void button_search_Click(object sender, EventArgs e)
        {
            if (searchword.Text.Length == 0)
            {
                button_google.Enabled = false;
                button_twitter.Enabled = false;
                button_qiita.Enabled = false;
                button_youtube.Enabled = false;
            }
            else
            {
                string result_viewurl = HttpUtility.UrlEncode(searchword.Text);
                button_google.Enabled = true;
                button_twitter.Enabled = true;
                button_qiita.Enabled = true;
                button_youtube.Enabled = true;
                for (int i = 0; i < 10; i++)
                {
                    result[i].google = "google" + i.ToString();
                    result[i].twitter = "twitter" + i.ToString();
                    result[i].qiita = "qiita" + i.ToString();
                    result[i].youtube = "youtube" + i.ToString();
                }
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}