using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Views;
using System.Web.NBitcoin;
using System;

namespace UltimateSearcher
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity
    {
        string result_word;

        struct Result
        {
            string google, twitter, qiita, youtube;
        }

        Result[] result = new Result[10];

        int searchmode = 0;

        void resultview()
        {
            result_view.Text = result_word;
            switch (searchmode)
            {
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
                default:
                    break;
            }
        }

        void search()
        {
            string result_wordurl = HttpUtility.UrlEncode(result_word);
            if (result_word == "")
            {
                button_google.Enabled = false;
                button_twitter.Enabled = false;
                button_qiita.Enabled = false;
                button_youtube.Enabled = false;
            }
            else
            {
                button_google.Enabled = true;
                button_twitter.Enabled = true;
                button_qiita.Enabled = true;
                button_youtube.Enabled = true;
            }
        }

        public TextView result_view { get; private set; }
        public EditText searchword { get; private set; }

        public ImageButton button_google { get; private set; }
        public ImageButton button_twitter { get; private set; }
        public ImageButton button_qiita { get; private set; }
        public ImageButton button_youtube { get; private set; }
        public ImageButton button_search { get; private set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            var systemUiFlags = SystemUiFlags.LayoutStable
                | SystemUiFlags.LayoutHideNavigation
                | SystemUiFlags.LayoutFullscreen
                | SystemUiFlags.HideNavigation
                | SystemUiFlags.Fullscreen
                | SystemUiFlags.ImmersiveSticky;
            Window.DecorView.SystemUiVisibility = (StatusBarVisibility)(int)systemUiFlags;

            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);


            result_view = FindViewById<TextView>(Resource.Id.result_view);

            searchword = FindViewById<EditText>(Resource.Id.search_bar);

            button_google = FindViewById<ImageButton>(Resource.Id.button_google);
            button_google.Click += button_google_Click;

            button_twitter = FindViewById<ImageButton>(Resource.Id.button_twitter);
            button_twitter.Click += button_twitter_Click;

            button_qiita = FindViewById<ImageButton>(Resource.Id.button_qiita);
            button_qiita.Click += button_qiita_Click;

            button_youtube = FindViewById<ImageButton>(Resource.Id.button_youtube);
            button_youtube.Click += button_youtube_Click;

            button_search = FindViewById<ImageButton>(Resource.Id.button_search);
            button_search.Click += button_search_Click;

            search();
        }

        private void button_google_Click(object sender, EventArgs e)
        {
            searchmode = 1;
            resultview();
        }

        private void button_twitter_Click(object sender, EventArgs e)
        {
            searchmode = 2;
            resultview();
        }

        private void button_qiita_Click(object sender, EventArgs e)
        {
            searchmode = 3;
            resultview();
        }

        private void button_youtube_Click(object sender, EventArgs e)
        {
            searchmode = 4;
            resultview();
        }

        private void button_search_Click(object sender, EventArgs e)
        {
            search();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}