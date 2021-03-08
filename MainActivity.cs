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
                    String API = "https://www.googleapis.com/customsearch/v1?key=" + Key.Google_API() + "&cx=" + Key.CSE_ID() + "&q=" + SW_url;
                    WebRequest request = WebRequest.Create(API);
                    WebResponse Res = request.GetResponse();
                    StreamReader reader = new StreamReader(Res.GetResponseStream(), new UTF8Encoding(true));
                    var google_json = JObject.Parse(reader.ReadToEnd());
                    for (int i = 0; i < 10; i++)
                    {
                        try
                        {
                            results[0, 0, i] = google_json["items"][i]["link"].ToString();
                            results[1, 0, i] = google_json["items"][i]["title"].ToString();
                            url[i] = results[0, 0, i];
                        }
                        catch
                        {
                            break;
                        }
                    }
                    button_google.Enabled = true;
                });
                Task twitter = Task.Run(async () =>
                {
                    button_twitter.Enabled = false;
                    int i = 0;
                    var token = CoreTweet.Tokens.Create(Key.Twitter_API(), Key.Twitter_API_SECRET(), Key.Twitter_Token(), Key.Twitter_Token_SECRET());
                    var result = await token.Search.TweetsAsync(count => 10, q => searchword.Text.ToString());

                    foreach(var tweet in result)
                    {
                        results[0, 1, i] = "https://twitter.com/" + tweet.User.Id + "/status/" + tweet.Id;
                        try
                        {
                            results[1, 1, i] = tweet.Text.Substring(0, 40) + "...";
                        }
                        catch (Exception)
                        {
                            results[1, 1, i] = tweet.Text;
                        }
                        url[i] = results[0, 1, i];
                        i++;
                    }
                    button_twitter.Enabled = true;
                });
                Task qiita = Task.Run(() =>
                {
                    button_qiita.Enabled = false;
                    string SW_url = HttpUtility.UrlEncode(searchword.Text);
                    String API = "https://qiita.com/api/v2/items?page=1&per_page=10&query=tag%3A" + SW_url + " HTTP/1.1";
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
                    String API = "https://www.googleapis.com/youtube/v3/search?type=video&part=snippet&maxResults=10&q=" + searchword.Text + "&key=" + Key.Google_API();
                    WebRequest request = WebRequest.Create(API);
                    WebResponse Res = request.GetResponse();
                    StreamReader reader = new StreamReader(Res.GetResponseStream(), new UTF8Encoding(false));
                    var youtube_json = JObject.Parse(reader.ReadToEnd());
                    for (int i = 0; i < 10; i++)
                    {
                        try
                        {
                            results[0, 3, i] = "https://www.youtube.com/watch?v=" + youtube_json["items"][i]["id"]["videoid"].ToString();
                        }
                        catch (Exception) { }
                        results[1, 3, i] = youtube_json["items"][i]["snipets"]["title"].ToString();
                        url[i] = results[0, 3, i];
                    }
                    button_youtube.Enabled = true;
                });
                string result_view_url = HttpUtility.UrlEncode(searchword.Text);
            }
        }

        public TextView[] result_view { get; private set; } = new TextView[10];
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


            Task thread1 = Task.Run(
                () =>
                {
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

            Task thread2 = Task.Run(
                () =>
                {
                    button_qiita = FindViewById<ImageButton>(Resource.Id.button_qiita);
                    button_qiita.Click += button_qiita_Click;

                    button_youtube = FindViewById<ImageButton>(Resource.Id.button_youtube);
                    button_youtube.Click += button_youtube_Click;

                    button_search = FindViewById<ImageButton>(Resource.Id.button_search);
                    button_search.Click += button_search_Click;
                });
            result_view[0] = FindViewById<TextView>(Resource.Id.result_view0);
            result_view[1] = FindViewById<TextView>(Resource.Id.result_view1);
            result_view[2] = FindViewById<TextView>(Resource.Id.result_view2);
            result_view[3] = FindViewById<TextView>(Resource.Id.result_view3);
            result_view[4] = FindViewById<TextView>(Resource.Id.result_view4);
            result_view[5] = FindViewById<TextView>(Resource.Id.result_view5);
            result_view[6] = FindViewById<TextView>(Resource.Id.result_view6);
            result_view[7] = FindViewById<TextView>(Resource.Id.result_view7);
            result_view[8] = FindViewById<TextView>(Resource.Id.result_view8);
            result_view[9] = FindViewById<TextView>(Resource.Id.result_view9);

            result_view[0].Click += Click0;
            result_view[1].Click += Click1;
            result_view[2].Click += Click2;
            result_view[3].Click += Click3;
            result_view[4].Click += Click4;
            result_view[5].Click += Click5;
            result_view[6].Click += Click6;
            result_view[7].Click += Click7;
            result_view[8].Click += Click8;
            result_view[9].Click += Click9;
        }

        private void Click0(object sender, EventArgs e)
        {
            web(url[0]);
        }

        private void Click1(object sender, EventArgs e)
        {
            web(url[1]);
        }

        private void Click2(object sender, EventArgs e)
        {
            web(url[2]);
        }

        private void Click3(object sender, EventArgs e)
        {
            web(url[3]);
        }

        private void Click4(object sender, EventArgs e)
        {
            web(url[4]);
        }

        private void Click5(object sender, EventArgs e)
        {
            web(url[5]);
        }

        private void Click6(object sender, EventArgs e)
        {
            web(url[6]);
        }

        private void Click7(object sender, EventArgs e)
        {
            web(url[7]);
        }

        private void Click8(object sender, EventArgs e)
        {
            web(url[8]);
        }

        private void Click9(object sender, EventArgs e)
        {
            web(url[9]);
        }

        void resultview(int s)
        {
            for (int i = 0; i < 10; i++)
            {
                result_view[i].Text = results[1, s, i] + "\n";
            }
        }

        private void button_google_Click(object sender, EventArgs e)
        {
            resultview(0);
        }

        private void button_twitter_Click(object sender, EventArgs e)
        {
            resultview(1);
        }

        private void button_qiita_Click(object sender, EventArgs e)
        {
            resultview(2);
        }

        private void button_youtube_Click(object sender, EventArgs e)
        {
            resultview(3);
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