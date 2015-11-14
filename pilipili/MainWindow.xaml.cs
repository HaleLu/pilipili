using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace pilipili
{
    /// <summary>
    ///     MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly SolidColorBrush[] colorList =
        {
            Brushes.AliceBlue, Brushes.Aqua, Brushes.Black, Brushes.Blue,
            Brushes.BlueViolet, Brushes.Red, Brushes.Gold, Brushes.White, Brushes.White, Brushes.White, Brushes.White,
            Brushes.White, Brushes.White, Brushes.White, Brushes.White, Brushes.White
        };

        private static readonly Label[] labelList = new Label[1024];
        private readonly Grid g = new Grid {Margin = new Thickness(0, 0, 0, 0)};
        private int id = 0;

        private readonly Random rd = new Random();
        private readonly Timer timer_get = new Timer();
        private readonly Timer timer_move = new Timer();

        private int begin;
        private int end;

        public MainWindow()
        {
            InitializeComponent();

            timer_move.Elapsed += MoveLabelEvent;
            timer_move.Interval = 10;
            timer_move.Start();
            timer_move.Enabled = true;

            timer_get.Elapsed += SendGetEvent;
            timer_get.Interval = 1000;
            timer_get.Start();
            timer_get.Enabled = true;
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AddChild(g);
        }

        public void MoveLabelEvent(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(
                delegate
                {
                    for (var i = begin; i < end; i++)
                    {
                        labelList[i].Margin = new Thickness(labelList[i].Margin.Left + 4, labelList[i].Margin.Top, 0, 0);

                        if (labelList[i].Margin.Left >= this.Width)
                        {
                            g.Children.Remove(labelList[i]);
                            labelList[i] = null;
                            begin++;
                            begin %= 1024;
                        }
                    }
                }
                );
        }

        private void SendGetEvent(object sender, ElapsedEventArgs e)
        {
            var json = HttpGet("http://2.countdown2014.sinaapp.com/index.php", "id=" + id);
            JObject jo = (JObject)JsonConvert.DeserializeObject(json);
            JArray res = JArray.Parse(jo["data"].ToString());
            foreach (var message in res)
            {
                Dispatcher.Invoke(
                    delegate
                    {
                        var l = new Label
                        {
                            Content = message["content"],
                            HorizontalAlignment = HorizontalAlignment.Left,
                            Margin = new Thickness(-Width, rd.NextDouble()*Height, 0, 0),
                            VerticalAlignment = VerticalAlignment.Top,
                            UseLayoutRounding = true,
                            Foreground = colorList[rd.Next()%16],
                            FontSize = 30
                        };
                        labelList[end] = l;
                        end++;
                        end %= 1024;
                        g.Children.Add(l);
                    }
                    );
                id = id > Int32.Parse(message["id"].ToString()) ? id : Int32.Parse(message["id"].ToString());
            }
        }

        public string HttpGet(string Url, string postDataStr)
        {
            var request = (HttpWebRequest) WebRequest.Create(Url + (postDataStr == "" ? "" : "?") + postDataStr);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";

            var response = (HttpWebResponse) request.GetResponse();
            var myResponseStream = response.GetResponseStream();
            var myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            var retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }
    }
}