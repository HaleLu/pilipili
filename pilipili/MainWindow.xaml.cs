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
    public partial class MainWindow
    {
        //颜色列表
        private static readonly SolidColorBrush[] ColorList =
        {
            Brushes.AliceBlue, Brushes.Aqua, Brushes.Black, Brushes.Blue,
            Brushes.BlueViolet, Brushes.Red, Brushes.Gold, Brushes.White, Brushes.White, Brushes.White, Brushes.White,
            Brushes.White, Brushes.White, Brushes.White, Brushes.White, Brushes.White
        };

        //弹幕库
/*        private static readonly string[] MessageLib =
        {
            "23333", "我从未见过如此厚颜无耻之人", "前方高能，弹幕护体", "前方高能反应，非战斗人员迅速撤退", "这不科学", "666666666", "是在下输了", "BGM爱的供养，再问自杀",
            "硬币已投", "马克", "放学别走", "我裤子都脱了，你就给我看这个？", "暂停学表情", "以上企业均已破产", "在座的各位都是辣鸡", "不就是膝盖吗，拿去好了"
        };*/

        //弹幕标签列表（循环队列）
        private static readonly Label[] LabelList = new Label[1024];
        //初始化布局
        private readonly Grid _g = new Grid {Margin = new Thickness(0, 0, 0, 0)};
        //随机数生成器
        private readonly Random _rd = new Random();
        private const int Speed = 4;

        //发送GET请求的定时器
        private readonly Timer _timerGet = new Timer();
        //移动弹幕的定时器
        private readonly Timer _timerMove = new Timer();

        //循环队列的头和尾序号
        private int _begin;
        private int _end;

        //当前已从服务器接收到的弹幕id
        private int _id;

        //房间号
        private readonly int _roomId;

        public MainWindow(int roomId)
        {
            _roomId = roomId;
            InitializeComponent();
        }

        //初始化两个计时器
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _timerMove.Elapsed += MoveLabelEvent;
            _timerMove.Interval = 10;
            _timerMove.Start();
            _timerMove.Enabled = true;

            _timerGet.Elapsed += SendGetEvent;
//            _timerGet.Elapsed += GenMessageEvent;
            _timerGet.Interval = 1000;
            _timerGet.Start();
            _timerGet.Enabled = true;
            AddChild(_g);

            GC.KeepAlive(_timerMove);
            GC.KeepAlive(_timerGet);
        }

        //弹幕移动
        public void MoveLabelEvent(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(
                delegate
                {
                    for (var i = _begin; Between(i, _begin, _end); i++, i %= 1024)
                    {
                        LabelList[i].Margin = new Thickness(LabelList[i].Margin.Left - Speed, LabelList[i].Margin.Top, 0,
                            0);

                        if (!(LabelList[i].Margin.Left < -Width)) continue;
                        _g.Children.Remove(LabelList[i]);
                        LabelList[i] = null;
                        _begin++;
                        _begin %= 1024;
                    }
                }
                );
        }

        //弹幕生成器（测试用）
/*
        private void GenMessageEvent(object sender, ElapsedEventArgs e)
        {
            if (_rd.Next()%5 != 0) return;
            var num = _rd.Next()%5;
            if (num == 0)
            {
                if (_rd.Next()%4 == 0)
                {
                    var content = MessageLib[_rd.Next()%16];
                    for (var i = 0; i < 20; i++)
                    {
                        var level = i;
                        Dispatcher.Invoke(
                            delegate
                            {
                                var l = new Label
                                {
                                    Content = content,
                                    HorizontalAlignment = HorizontalAlignment.Left,
                                    VerticalAlignment = VerticalAlignment.Top,
                                    Margin = new Thickness(Width, level * 40, 0, 0),
                                    UseLayoutRounding = true,
                                    Foreground = ColorList[_rd.Next()%16],
                                    FontSize = 30
                                };
                                LabelList[_end] = l;
                                _end++;
                                _end %= 1024;
                                _g.Children.Add(l);
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
                            Content = MessageLib[_rd.Next()%16],
                            HorizontalAlignment = HorizontalAlignment.Left,
                            VerticalAlignment = VerticalAlignment.Top,
                            Margin = new Thickness(Width, _rd.NextDouble()*(Height - 100), 0, 0),
                            UseLayoutRounding = true,
                            Foreground = ColorList[_rd.Next()%16],
                            FontSize = 30
                        };
                        LabelList[_end] = l;
                        _end++;
                        _end %= 1024;
                        _g.Children.Add(l);
                    }
                    );
            }
        }
*/

        //发送GET请求并用返回值生成弹幕
        private void SendGetEvent(object sender, ElapsedEventArgs e)
        {
            var json = HttpGet("http://pilipili.azurewebsites.net/index.php", "id=" + _id + "&roomId=" + _roomId);

            try
            {
                var res = JArray.Parse(((JObject)JsonConvert.DeserializeObject(json))["data"].ToString());
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
                                Margin = new Thickness(Width, _rd.NextDouble() * (Height - 100), 0, 0),
                                UseLayoutRounding = true,
                                Foreground = ColorList[_rd.Next() % 16],
                                FontSize = 30
                            };
                            LabelList[_end] = l;
                            _end++;
                            _end %= 1024;
                            _g.Children.Add(l);
                        }
                        );
                    _id = _id > int.Parse(message["id"].ToString()) ? _id : int.Parse(message["id"].ToString());
                }
            }
            catch
            {
                // ignored
            }
        }

        //GET方法模板
        public string HttpGet(string url, string postDataStr)
        {
            var request = (HttpWebRequest) WebRequest.Create(url + (postDataStr == "" ? "" : "?") + postDataStr);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            try
            {
                var response = (HttpWebResponse) request.GetResponse();
                var myResponseStream = response.GetResponseStream();
                if (myResponseStream != null)
                {
                    var myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                    var retString = myStreamReader.ReadToEnd();
                    myStreamReader.Close();
                    myResponseStream.Close();
                    return retString;
                }
            }
            catch (Exception)
            {
                // ignored
            }
            return null;
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