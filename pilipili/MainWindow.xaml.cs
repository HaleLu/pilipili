using System;
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
        //颜色列表
        private static readonly SolidColorBrush[] colorList =
        {
            Brushes.AliceBlue, Brushes.Aqua, Brushes.Black, Brushes.Blue,
            Brushes.BlueViolet, Brushes.Red, Brushes.Gold, Brushes.White, Brushes.White, Brushes.White, Brushes.White,
            Brushes.White, Brushes.White, Brushes.White, Brushes.White, Brushes.White
        };

        //弹幕库
        private static readonly string[] messageLib =
        {
            "23333", "我从未见过如此厚颜无耻之人", "前方高能，弹幕护体", "前方高能反应，非战斗人员迅速撤退", "这不科学", "666666666", "是在下输了", "BGM爱的供养，再问自杀",
            "硬币已投", "马克", "放学别走", "我裤子都脱了，你就给我看这个？", "暂停学表情", "以上企业均已破产", "在座的各位都是辣鸡", "不就是膝盖吗，拿去好了"
        };

        //弹幕标签列表（循环队列）
        private static readonly Label[] labelList = new Label[1024];
        //初始化布局
        private readonly Grid g = new Grid {Margin = new Thickness(0, 0, 0, 0)};
        //随机数生成器
        private readonly Random rd = new Random();
        private readonly int speed = 4;

        //发送GET请求的定时器
        private readonly Timer timer_get = new Timer();
        //移动弹幕的定时器
        private readonly Timer timer_move = new Timer();

        //循环队列的头和尾序号
        private int begin;
        private int end;

        //当前已从服务器接收到的弹幕id
        private int id;

        public MainWindow()
        {
            InitializeComponent();
        }

        //初始化两个计时器
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            timer_move.Elapsed += MoveLabelEvent;
            timer_move.Interval = 10;
            timer_move.Start();
            timer_move.Enabled = true;

            timer_get.Elapsed += SendGetEvent;
            timer_get.Elapsed += GenMessageEvent;
            timer_get.Interval = 1000;
            timer_get.Start();
            timer_get.Enabled = true;
            AddChild(g);

            GC.KeepAlive(timer_move);
            GC.KeepAlive(timer_get);
        }

        //弹幕移动
        public void MoveLabelEvent(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(
                delegate
                {
                    for (var i = begin; Between(i, begin, end); i++, i %= 1024)
                    {
                        labelList[i].Margin = new Thickness(labelList[i].Margin.Left - speed, labelList[i].Margin.Top, 0,
                            0);

                        if (labelList[i].Margin.Left < -Width)
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

        //弹幕生成器（测试用）
        private void GenMessageEvent(object sender, ElapsedEventArgs e)
        {
            if (rd.Next()%5 != 0) return;
            var num = rd.Next()%5;
            if (num == 0)
            {
                if (rd.Next()%4 == 0)
                {
                    var content = messageLib[rd.Next()%16];
                    for (var i = 0; i < 10; i++)
                    {
                        Dispatcher.Invoke(
                            delegate
                            {
                                var l = new Label
                                {
                                    Content = content,
                                    HorizontalAlignment = HorizontalAlignment.Left,
                                    VerticalAlignment = VerticalAlignment.Top,
                                    Margin = new Thickness(Width, i*60, 0, 0),
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
                    }
                }
            }
            for (var i = 0; i < num; i++)
            {
                Dispatcher.Invoke(
                    delegate
                    {
                        var l = new Label
                        {
                            Content = messageLib[rd.Next()%16],
                            HorizontalAlignment = HorizontalAlignment.Left,
                            VerticalAlignment = VerticalAlignment.Top,
                            Margin = new Thickness(Width, rd.NextDouble()*(Height - 100), 0, 0),
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
            }
        }

        //发送GET请求并用返回值生成弹幕
        private void SendGetEvent(object sender, ElapsedEventArgs e)
        {
            var json = HttpGet("http://2.countdown2014.sinaapp.com/index.php", "id=" + id);
            var jo = (JObject) JsonConvert.DeserializeObject(json);
            var res = JArray.Parse(jo["data"].ToString());
            foreach (var message in res)
            {
                Dispatcher.Invoke(
                    delegate
                    {
                        var l = new Label
                        {
                            Content = message["content"],
                            HorizontalAlignment = HorizontalAlignment.Left,
                            VerticalAlignment = VerticalAlignment.Top,
                            Margin = new Thickness(Width, rd.NextDouble() * (Height - 100), 0, 0),
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
                id = id > int.Parse(message["id"].ToString()) ? id : int.Parse(message["id"].ToString());
            }
        }

        //GET方法模板
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

        private static bool Between(int i, int begin, int end)
        {
            if (begin <= end)
            {
                if (i < end) return true;
            }
            else
            {
                if (i >= begin || i < end) return true;
            }
            return false;
        }
    }
}