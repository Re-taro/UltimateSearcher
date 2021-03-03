using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Views;
using System;
using System.Web.NBitcoin;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using Xamarin.Essentials;


namespace UltimateSearcher
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity
    {
        /*
         * 配列メモ
         * 
         * google:0
         * twitter:1
         * qiita:2
         * youtube:3
         * 
         * url:0
         * title:1
         */

        string[,,] results = new string[2, 4, 10];
        string[] url = new string[10];

        public async void web(string url)
        {
            try
            {
                await Browser.OpenAsync(url, BrowserLaunchMode.SystemPreferred);
            }
            catch (Exception) { }
        }

        public void search()
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
                Task google = Task.Run(() =>
                {
                    button_google.Enabled = false;
                    string SW_url = HttpUtility.UrlEncode(searchword.Text);
                    String API = "https://www.googleapis.com/customsearch/v1?/key=" + Key.Google_API() + "&cx=" + Key.CSE_ID() + "&q=" + SW_url;
                    WebRequest request = WebRequest.Create(API);
                    WebResponse Res = request.GetResponse();
                    StreamReader reader = new StreamReader(Res.GetResponseStream(), new UTF8Encoding(false));
                    var google_json = JArray.Parse(reader.ReadToEnd());
                    for (int i = 0; i < 10; i++)
                    {
                        results[0, 0, i] = google_json[i]["link"].ToString();
                        results[1, 0, i] = google_json[i]["title"].ToString();
                        url[i] = results[0, 0, i];
                    }
                    button_google.Enabled = true;
                });
                Task twitter = Task.Run(async () =>
                {
                    button_twitter.Enabled = false;
                    int i = 0;
                    var token = CoreTweet.Tokens.Create(Key.Twitter_API(), Key.Twitter_API_SECRET(), Key.Twitter_Token(), Key.Twitter_Token_SECRET());
                    var result_T = await token.Search.TweetsAsync(count => 10, q => searchword.Text);

                    foreach(var tweet in result_T)
                    {
                        results[0, 1, i] = "https://twitter.com/" + tweet.User.Id + "/status/" + tweet.Id;
                        results[1, 1, i] = tweet.Text.Substring(0, 40) + "...";
                        url[i] = results[0, 1, i];
                        i++;
                    }
                    button_twitter.Enabled = true;
                });
                Task qiita = Task.Run(() =>
                {
                    button_qiita.Enabled = false;
                    string SW_url = HttpUtility.UrlEncode(searchword.Text);
                    String API = "https://qiita.com/api/v2/items?/page=1&per_page=10&query=tag%3A" + SW_url + "HTTP/1.1";
                    WebRequest request = WebRequest.Create(API);
                    WebResponse Res = request.GetResponse();
                    StreamReader reader = new StreamReader(Res.GetResponseStream(), new UTF8Encoding(false));
                    var qiita_json = JArray.Parse(reader.ReadToEnd());
                    for (int i = 0; i < 10; i++)
                    {
                        results[0, 2, i] = qiita_json[i]["url"].ToString();
                        results[1, 2, i] = qiita_json[i]["title"].ToString();
                        url[i] = results[0, 2, i];
                    }
                    button_qiita.Enabled = true;
                });
                Task youtube = Task.Run(() =>
                {
                    button_youtube.Enabled = false;
                    String API = "https://www.googleapis.com/youtube/v3/search?type=video&part=snippet&q=" + searchword.Text + "&key=" + Key.Google_API();
                });
            }
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

        public void search()
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
                Task google = Task.Run(() =>
                {
                    string SW_url = HttpUtility.UrlEncode(searchword.Text);
                    String API = "https://www.googleapis.com/customsearch/v1?/key="+ 
                })
                string result_viewurl = HttpUtility.UrlEncode(searchword.Text);
                button_google.Enabled = true;
                button_twitter.Enabled = true;
                button_qiita.Enabled = true;
                button_youtube.Enabled = true;
            }
        }


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
                    searchword.KeyPress += (object sender, View.KeyEventArgs e) =>
                    {
                        e.Handled = false;
                        if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
                        {
                            search();
                            e.Handled = true;
                        }
                    };

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
            search();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}