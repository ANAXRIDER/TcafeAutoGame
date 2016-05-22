using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace TcafeAutoGame
{
    public partial class Form1 : Form
    {
        private bool typingGameContinue;
        private bool checkingAddr;

        private int step;        
        private int tryCount;
        private int dalinCount;
        private int attentionGameTotal;
        private int errFlag;
        private int inputCount;
        private int earnPoint;

        private string address;
        private string attentionAddr;
        private string oldString;

        private JavaScripts js;

        public string temp;        
        

        public Form1()
        {
            InitializeComponent();
            js = new JavaScripts();
        }

        private void RunButton_Click(object sender, EventArgs e)
        {
            SetControlAttr(false);
            step = 1;
            errFlag = 0;
            inputCount = 11;
            oldString = "";
            address = EditAddress.Text;
            Random r = new Random();
            double n = r.Next() / 100000000000000;
            attentionAddr = "/bbs/login_check.php?" + n + "&url=%2F&mb_id=" + EditID.Text + "&mb_password=" + EditPassword.Text + "&x=0&y=0";
            CtrlBrowser.SupressCookiePersist();
            webBrowser1.Navigate(address);
        }
        
        private void AttentionGameTimer_Tick(object sender, EventArgs e)
        {
            this.Invoke(new MethodInvoker(delegate ()
            {
                HtmlDocument doc = webBrowser1.Document;
                HtmlElementCollection elems = doc.GetElementsByTagName("center");
                int readyIdx = -1;
                int completeIdx = -1;

                foreach (HtmlElement ele in elems)
                {
                    string str1 = ele.InnerHtml;
                    readyIdx = str1.IndexOf("오늘은");
                    if (0 <= readyIdx)
                    {
                        break;
                    }

                    completeIdx = str1.IndexOf("마지막");
                    if (0 <= completeIdx)
                    {
                        break;
                    }
                }
                
                if ((-1 == readyIdx) && (-1 == completeIdx))
                {
                    ++tryCount;
                    if (5 <= tryCount)
                    {
                        errFlag = 1;
                        step = 5;
                        tryCount = 0;
                        webBrowser1.Navigate(address + "/tazza/");
                        AttentionGameTimer.Stop();
                    }

                    return;
                }

                if (-1 != completeIdx)
                {
                    AddLogMsg("이미 출석게임을 완료한 상태입니다.");
                    step = 5;
                    tryCount = 0;
                    webBrowser1.Navigate(address + "/tazza/");
                    AttentionGameTimer.Stop();
                    return;
                }

                step = 4;
                tryCount = 0;

                int i = 0;
                foreach (HtmlElement ele in doc.All)
                {
                    if (ele.GetAttribute("className") == "pnt_money")
                    {
                        ++i;
                        if (9 == i)
                        {
                            dalinCount = int.Parse(ele.InnerText.Replace("회", ""));
                            attentionGameTotal = dalinCount;
                        }
                        else if ((10 <= i) && (14 >= i))
                        {
                            attentionGameTotal += int.Parse(ele.InnerText.Replace("회", ""));
                            break;
                        }
                    }
                }

                int goal = ((dalinCount * 100 / attentionGameTotal) < 25) ? 1 : 100;
                if (1 == goal)
                    AddLogMsg("달인(1)을 노립니다.");
                else
                    AddLogMsg("능력자(100)를 노립니다.");

                webBrowser1.Navigate(js.GetAttentionGameScript(goal));
                AttentionGameTimer.Stop();
            }));
        }

        private void TypingGameTimer_Tick(object sender, EventArgs e)
        {
            if (typingGameContinue)
            {
                ++tryCount;
                if (3 == tryCount)
                {
                    typingGameContinue = false;
                    TypingGameTimer.Enabled = false;
                    AddLogMsg("Error: 페이지 리로드합니다.");
                    webBrowser1.Refresh();
                    return;
                }
            }

            BeginInvoke(new MethodInvoker(delegate ()
            {
                HtmlElement ele = webBrowser1.Document.GetElementById("tzStr");
                if (null != ele)
                    temp = ele.InnerText;
                else
                    temp = "";

                bool toNextPage = false;               
                if ((null != temp) && (0 < temp.Length))
                {
                    if (oldString != temp)
                    {
                        tryCount = 0;
                        oldString = temp;
                        webBrowser1.Navigate(js.GetTypingGameScript());
                        --inputCount;
                        if (0 < inputCount)
                            AddLogMsg("타자게임: " + (11 - inputCount) + "번째 입력했습니다. ");
                    }
                    else
                    {                        
                        if (4 == tryCount)
                        {
                            AddLogMsg(" - 내용 초기화");
                            oldString = "";
                        }
                        else if (22 <= tryCount)
                        {
                            errFlag += 2;
                            AddLogMsg(" <타자 게임 타임 오버 발생>");
                            AddLogMsg(" -- " + temp);
                            AddLogMsg(" -- " + oldString);
                            toNextPage = true;                            
                        }
                        
                    }
                }
                else
                {
                    if (30 <= tryCount)
                    {
                        AddLogMsg("오류가 발생되어 게임을 패스합니다.");
                        errFlag += 2;
                        toNextPage = true;
                    }
                    
                }

                if (toNextPage || (0 >= inputCount))
                {
                    webBrowser1.Navigate(address + "/index.php");
                    TypingGameTimer.Enabled = false;
                }
                else
                    typingGameContinue = true;
            })); 
        }

        private void FormShowTimer_Tick(object sender, EventArgs e)
        {
            webBrowser1.Stop();
            checkingAddr = true;
            step = 1;
            FormShowTimer.Enabled = false;
            EditAddress.Enabled = true;
            AddLogMsg("주소를 확인 시간 초과 했습니다.");
            AddLogMsg("기본 주소로 대체합니다.");
            EditAddress.Text = "http://tcafeh.com";
            SetControlAttr();
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            InjectAlertBlocker();
            switch (step)
            {
                case 0:
                    {                        
                        if (false == checkingAddr)
                        {
                            FormShowTimer.Enabled = false;
                            HtmlDocument doc = webBrowser1.Document;
                            foreach (HtmlElement ele in doc.All)
                            {
                                if (ele.GetAttribute("className") == "js-display-url")
                                {
                                    string temp = ele.InnerText.ToString().ToLower();
                                    if (-1 != temp.IndexOf("http", 0))
                                    {
                                        EditAddress.Text = temp;
                                    }
                                    else
                                    {
                                        EditAddress.Text = "http://" + temp;
                                    }
                                    
                                    checkingAddr = true;
                                    step = 1;
                                    SetControlAttr();
                                    AddLogMsg(EditID.Text + "Tcafe 주소 확인 완료하였습니다.");
                                    break;
                                }
                            }
                        }
                        break;
                    }
                case 1:
                    {
                        if (webBrowser1.Url.Equals(address))
                        {
                            AddLogMsg(EditID.Text + " 계정으로 로그인 시도 중입니다.");
                            step = 2;
                            webBrowser1.Navigate(address + attentionAddr);
                        }
                        break;
                    }
                case 2:
                    {
                        if (webBrowser1.Url.Equals(address + "/?udt=1"))
                        {
                            AddLogMsg("로그인 성공했습니다.");
                            earnPoint = GetAccountPoint();
                            step = 3;
                            tryCount = 0;
                            webBrowser1.Navigate(address + "/attendance/attendance.php?3");
                            AddLogMsg("출석게임하러 이동 합니다.");                            
                        }
                        else
                        {
                            ++tryCount;
                            AddLogMsg("로그인 시도 중(" + tryCount + ") ");
                        }
                        break;
                    }
                case 3:
                    {
                        if (webBrowser1.Url.Equals(address + "/attendance/attendance.php?3"))
                        {
                            if (false == AttentionGameTimer.Enabled)
                            {
                                AddLogMsg("출석게임 시작합니다.");
                                tryCount = 0;
                                AttentionGameTimer.Start();
                            }
                        }
                        else
                        {
                            ++tryCount;
                            AddLogMsg("출석게임으로 연결 시도중(" + tryCount + ")");
                        }
                        break;
                    }
                case 4:
                    {
                        step = 5;
                        tryCount = 0;
                        HtmlDocument doc = webBrowser1.Document;
                        HtmlElement elem;
                        HtmlElementCollection elems = doc.GetElementsByTagName("center");
                        elem = elems[4];
                        string str1 = elem.InnerHtml;

                        if (-1 != str1.IndexOf("오늘은"))
                        {                            
                            errFlag = 1;
                        }
                        break;
                    }
                case 5:
                    {
                        if (webBrowser1.Url.Equals(address + "/tazza/"))
                        {
                            AddLogMsg("타자게임 진행중입니다. ");

                            HtmlDocument doc = webBrowser1.Document;
                            foreach (HtmlElement ele in doc.All)
                            {
                                if (ele.GetAttribute("className") == "tzl_chld")
                                {
                                    if ((ele.OuterHtml == null))
                                    {
                                        webBrowser1.Refresh();
                                        return;
                                    }
;
                                    break;
                                }
                            }

                            step = 6;                            
                        }
                        else
                        {
                            ++tryCount;
                            AddLogMsg("타자게임 접속 시도중(" + tryCount + ")");
                            webBrowser1.Navigate(address + "/tazza/");
                        }
                        break;
                    }

                case 6:
                    {
                        if (webBrowser1.Url.Equals(address + "/index.php"))
                        {
                            if (TypingGameTimer.Enabled)
                            {
                                TypingGameTimer.Stop();
                                if (1 < inputCount)
                                {
                                    AddLogMsg(" - 일일 타자게임 횟수를 초과한 것 같습니다.");
                                }
                            }

                            earnPoint = GetAccountPoint() - earnPoint;
                            step = 7;
                            AddLogMsg("--[결 과]-------------------------");
                            if (errFlag == 1)
                                AddLogMsg(" -- 출석 게임을 완료하지 못 했습니다.");
                            else if (errFlag == 2)
                                AddLogMsg(" -- 타자 게임을 완료하지 못 했습니다.");
                            else if (errFlag == 3)
                                AddLogMsg(" -- 출석 게임과 타자 게임을 완료하지 못 했습니다.");

                            AddLogMsg(" 총 " + earnPoint + " 포인트를 획득했습니다.");                            
                            AddLogMsg("----------------------------------");
                            AddLogMsg("Tcafe 로그아웃 합니다.");

                            CtrlBrowser.EndBrowserSession();
                            webBrowser1.Navigate(address + "/bbs/logout.php");
                            SetControlAttr();
                        }
                        else if (webBrowser1.Url.Equals(address + "/tazza/"))
                        {
                            if (false == TypingGameTimer.Enabled)
                            {
                                TypingGameTimer.Interval = 100;
                                TypingGameTimer.Start();
                            }
                        }                            

                        break;
                    }
            }
        }

        private void webBrowser1_ProgressChanged(object sender, WebBrowserProgressChangedEventArgs e)
        {
            if (typingGameContinue)
            {
                typingGameContinue = false;
            }
        }        

        private void Form1_Shown(object sender, EventArgs e)
        {
            if ((false == checkingAddr) && (0 == step))
            {
                AddLogMsg("트위터에서 Tcafe 주소를 확인중입니다.");
                AddLogMsg("잠시만(3초) 기다려주세요.");
            }
        }        

        private void SetControlAttr(bool isEnabled = true)
        {
            EditAddress.Enabled = isEnabled;
            EditID.Enabled = isEnabled;
            EditPassword.Enabled = isEnabled;
            RunButton.Enabled = isEnabled;
        }

        private void InjectAlertBlocker()
        {
            HtmlElement head = webBrowser1.Document.GetElementsByTagName("head")[0];
            HtmlElement scriptEl = webBrowser1.Document.CreateElement("script");
            string alertBlocker = "window.alert = function () {}";
            scriptEl.SetAttribute("text", alertBlocker);
            head.AppendChild(scriptEl);
        }

        private void AddLogMsg(string msg)
        {
            LogBox.Items.Add(msg);
            LogBox.SetSelected(LogBox.Items.Count - 1, true);
            LogBox.SetSelected(LogBox.Items.Count - 1, false);
        }

        private int GetAccountPoint()
        {
            HtmlDocument doc = webBrowser1.Document;
            foreach (HtmlElement ele in doc.All)
            {
                if (ele.GetAttribute("className") == "lg_pnt_n pnt_money")
                {
                    return int.Parse(ele.InnerText.Replace(",", ""));
                }
            }

            return -99999999;
        }
    }    
}
