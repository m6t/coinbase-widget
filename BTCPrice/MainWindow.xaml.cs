using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;

namespace BTCPrice
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            System.Windows.Forms.Timer tmr = new System.Windows.Forms.Timer();
            tmr.Tick += Tmr_Tick; tmr.Interval = 5000;
            tmr.Enabled = true;
            this.openheight = this.Height;
            this.Topmost = true;
            this.Height = closeheight;
            Tmr_Tick(null, null);
        }
        TimeSpan animduration = TimeSpan.FromSeconds(0.5);
        double closeheight = 35;
        double openheight = 100;
        public static bool skip = false;
        public static object _lock = new object();
        CoinbaseApi api = new CoinbaseApi();
        public string selectedCurrency = "USD";
        bool isopen = true;
        double lastBtc = 0;
        double lastEth = 0;
        double lastLtc = 0;
        SolidColorBrush durgun = new SolidColorBrush(Color.FromArgb(255, 110, 110, 110));
        SolidColorBrush yukselis = new SolidColorBrush(Color.FromArgb(255, 25, 141, 0));
        SolidColorBrush dusus = new SolidColorBrush(Color.FromArgb(255, 232, 53, 0));

        private void Tmr_Tick(object sender, EventArgs e)
        {
            try
            {
                if (skip == true)
                    return;
                lock (_lock)
                {
                    if (skip == true)
                        return;
                    skip = true;
                    var prices = api.getPrices(selectedCurrency);


                    var btcItem = prices.data.First(x => x.@base == "BTC");
                    var ethItem = prices.data.First(x => x.@base == "ETH");
                    var ltcItem = prices.data.First(x => x.@base == "LTC");


                    SetCoin(btcItem, btc, lastBtc, out lastBtc);
                    SetCoin(ethItem, eth, lastEth, out lastEth);
                    SetCoin(ltcItem, ltc, lastLtc, out lastLtc);

                    skip = false;
                }
            }
            catch
            {

            }
        }

        private void SetCoin(SpotItem btcItem, Label btc, double lastBtc, out double setbtc)
        {
            var currentBtc = btcItem.amount.ToDouble();

            if (lastBtc == currentBtc)
            {
                btc.Foreground = durgun;
                //BackgroundAnim(btc,durgun);
            }
            else if (lastBtc < currentBtc)
            {
                btc.Foreground = yukselis;
                BackgroundAnim(btc, yukselis);

            }
            else
            {
                btc.Foreground = dusus;
                BackgroundAnim(btc, dusus);

            }


            setbtc = currentBtc;

            btc.Content = btcItem.amount + " " + selectedCurrency;
        }

        private void BackgroundAnim(Label btc, SolidColorBrush color)
        {
            ColorAnimation animation = new ColorAnimation();
            animation.From = color.Color;
            animation.To = Colors.Transparent;
            animation.Duration = animduration;

            btc.Background = color.Clone();
            btc.Background.BeginAnimation(SolidColorBrush.ColorProperty, animation);
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Grid_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            if (isopen)
            {
                isopen = false;
                DoubleAnimation ani = new DoubleAnimation(closeheight, animduration);
                this.BeginAnimation(HeightProperty, ani);
                DoubleAnimation aniAngle = new DoubleAnimation(0, animduration);
                arrowangle.BeginAnimation(RotateTransform.AngleProperty, aniAngle);
            }
            else
            {
                isopen = true;
                DoubleAnimation ani = new DoubleAnimation(openheight, animduration);
                this.BeginAnimation(HeightProperty, ani);
                DoubleAnimation aniAngle = new DoubleAnimation(180, animduration);
                arrowangle.BeginAnimation(RotateTransform.AngleProperty, aniAngle);
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.selectedCurrency = (sender as ComboBox).SelectedItem.ToString();
            lastBtc = 0;
            lastEth = 0;
            lastLtc = 0;
            Tmr_Tick(null, null);
        }
    }

    public class RestOperationDefinition
    {
        public string Uri
        {
            get;
            set;
        }

        public HttpMethod Method
        {
            get;
            set;
        }

        public RestOperationDefinition(string Uri, HttpMethod Method)
        {
            this.Uri = Uri;
            this.Method = Method;
        }
    }

    public class SpotItem
    {
        public string @base { get; set; }
        public string currency { get; set; }
        public string amount { get; set; }
    }

    public class SpotResponse
    {
        public List<SpotItem> data { get; set; }
    }

    public class NoResponse
    {
    }

    public class CoinbaseApi
    {
        public SpotResponse getPrices(string currency)
        {
            return SendRequest<SpotResponse>(new NoResponse(), new RestOperationDefinition("prices/" + currency.ToUpper() + "/spot", HttpMethod.Get));
        }
        public readonly HttpClient client = new HttpClient();
        public T SendRequest<T>(object requestBody, RestOperationDefinition operation, params string[] operationArguments)
        {
            List<string> list = new List<string>();
            list.AddRange(operationArguments);
            list.Insert(0, operation.Uri);
            list.Insert(0, "https://api.coinbase.com/v2");
            StringContent stringContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            int num = 0;
            T result3;
            while (true)
            {
                try
                {
                    using (HttpRequestMessage httpRequestMessage = new HttpRequestMessage(operation.Method, System.IO.Path.Combine(list.ToArray())))
                    {
                        bool flag = operation.Method != HttpMethod.Get;
                        if (flag)
                        {
                            httpRequestMessage.Content = stringContent;
                        }
                        HttpResponseMessage result = this.client.SendAsync(httpRequestMessage).Result;
                        string result2 = result.Content.ReadAsStringAsync().Result;
                        bool flag2 = result.StatusCode == HttpStatusCode.OK;
                        if (flag2)
                        {
                            bool flag3 = typeof(T) == typeof(NoResponse);
                            if (flag3)
                            {
                                result3 = default(T);
                                break;
                            }
                            T t = JsonConvert.DeserializeObject<T>(result2);
                            bool flag4 = t == null;
                            if (flag4)
                            {
                                throw new NullReferenceException("Request Cevabı null\n url = " + System.IO.Path.Combine(list.ToArray()) + " \n data : " + JsonConvert.SerializeObject(stringContent));
                            }
                            result3 = t;
                            break;
                        }
                        else
                        {
                            Debugger.Break();

                            result.EnsureSuccessStatusCode();
                        }
                    }
                }
                catch (AggregateException ex)
                {
                    int num2 = num;
                    num = num2 + 1;
                    bool flag5 = num == 5;
                    if (flag5)
                    {
                        throw ex;
                    }
                }
            }
            return result3;
        }

    }
}
