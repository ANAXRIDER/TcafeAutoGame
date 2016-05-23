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
        private bool skipAttentionGame;
        private bool skipTypingGame;
        private int tryCount;
        private int attentionGameTotal;
        private int typingCount;
        private int initPoint;        
        private string attentionAddr;
        private string oldString;                    
        private JavaScripts js;

        public Form1()
        {
            InitializeComponent();
            js = new JavaScripts();
            initPoint = 0;
        }

        private void RunButton_Click(object sender, EventArgs e)
        {
            SetEnabledControls(false);
            skipAttentionGame = false;
            skipTypingGame = false;
            typingCount = 11;
            oldString = "";
            initPoint = 0;
            Random r = new Random();
            double n = r.Next() / 100000000000000;
            attentionAddr = "/bbs/login_check.php?" + n + "&url=%2F&mb_id=" + EditID.Text + "&mb_password=" + EditPassword.Text + "&x=0&y=0";
            CtrlBrowser.SupressCookiePersist();
            webBrowser1.DocumentCompleted += LoginTcafe;
            webBrowser1.Navigate(EditAddress.Text);
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

                bool isSkipGame = false;
                if (-1 != completeIdx)
                {
                    AddLogMsg("이미 출석게임을 완료한 상태입니다.");
                    isSkipGame = true;
                }
                else if ((-1 == readyIdx) && (-1 == completeIdx))
                {
                    ++tryCount;
                    if (5 <= tryCount)
                    {
                        isSkipGame = true;                        
                    }
                    else
                    {
                        return;
                    }
                }

                if (isSkipGame)
                {                    
                    tryCount = 0;
                    webBrowser1.DocumentCompleted += this.PlayTypingGame;
                    webBrowser1.Navigate(EditAddress.Text + "/tazza/");
                    AttentionGameTimer.Stop();
                    return;
                }
                
                tryCount = 0;

                int i = 0;
                int dalInCount = 0;
                foreach (HtmlElement ele in doc.All)
                {
                    if (ele.GetAttribute("className") == "pnt_money")
                    {
                        ++i;
                        if (9 == i)
                        {
                            dalInCount = int.Parse(ele.InnerText.Replace("회", ""));
                            attentionGameTotal = dalInCount;
                        }
                        else if ((10 <= i) && (14 >= i))
                        {
                            attentionGameTotal += int.Parse(ele.InnerText.Replace("회", ""));
                            break;
                        }
                    }
                }

                int goal = ((dalInCount * 100 / attentionGameTotal) < 25) ? 1 : 100;
                if (1 == goal)
                    AddLogMsg("달인(1)을 노립니다.");
                else
                    AddLogMsg("능력자(100)를 노립니다.");

                webBrowser1.DocumentCompleted += this.PlayTypingGame;
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
                String temp = "";

                if (null != ele)
                    temp = ele.InnerText;                

                bool toNextPage = false;               
                if ((null != temp) && (0 < temp.Length))
                {
                    if (oldString != temp)
                    {
                        tryCount = 0;
                        oldString = temp;
                        webBrowser1.Navigate(js.GetTypingGameScript());
                        --typingCount;
                        if (0 < typingCount)
                            AddLogMsg("타자게임: " + (11 - typingCount) + "번째 입력했습니다. ");
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
                            skipTypingGame = true;
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
                        skipTypingGame = true;
                        AddLogMsg("오류가 발생되어 게임을 패스합니다.");                        
                        toNextPage = true;
                    }
                    
                }

                if (toNextPage || (0 >= typingCount))
                {
                    webBrowser1.Navigate(EditAddress.Text + "/index.php");
                    TypingGameTimer.Enabled = false;
                }
                else
                    typingGameContinue = true;
            })); 
        }

        private void FormShowTimer_Tick(object sender, EventArgs e)
        {
            webBrowser1.Stop();
            FormShowTimer.Enabled = false;
            EditAddress.Enabled = true;
            AddLogMsg("주소 확인애 실패(타임오버) 했습니다.");
            AddLogMsg("기본 주소로 대체합니다.");
            EditAddress.Text = "http://tcafeh.com";
            SetEnabledControls();
        }


        private void webBrowser1_ProgressChanged(object sender, WebBrowserProgressChangedEventArgs e)
        {
            if (typingGameContinue)
            {
                typingGameContinue = false;
            }
        }        

        private void SetEnabledControls(bool isEnabled = true)
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
            if (-99999999 == initPoint)
            {
                return -99999999;
            }

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

        private void Form1_Load(object sender, EventArgs e)
        {            
            webBrowser1.DocumentCompleted += InjectAlertMsgBlock;
            AddLogMsg("트위터에서 Tcafe 주소를 확인중입니다.");
            AddLogMsg("잠시만 기다려주세요.");
        }

        private void InjectAlertMsgBlock(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            InjectAlertBlocker();
        }

        private void CheckTwitterAddress(object sender, WebBrowserDocumentCompletedEventArgs e)
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

                    SetEnabledControls();
                    AddLogMsg(EditID.Text + "Tcafe 주소 확인 완료하였습니다.");
                    webBrowser1.DocumentCompleted -= this.CheckTwitterAddress;
                    break;
                }
            }
        }

        private void LoginTcafe(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (webBrowser1.Url.Equals(EditAddress.Text))
            {
                AddLogMsg(EditID.Text + " 계정으로 로그인 시도 중입니다.");
                webBrowser1.DocumentCompleted -= this.LoginTcafe;
                webBrowser1.DocumentCompleted += this.CheckLogin;
                webBrowser1.Navigate(EditAddress.Text + attentionAddr);
            }
        }

        private void CheckLogin(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (webBrowser1.Url.Equals(EditAddress.Text + "/?udt=1"))
            {
                AddLogMsg("로그인 성공했습니다.");
                initPoint = GetAccountPoint();
                tryCount = 0;
                webBrowser1.DocumentCompleted -= this.CheckLogin;
                webBrowser1.DocumentCompleted += this.MoveAttentionGamePage;
                webBrowser1.Navigate(EditAddress.Text + "/attendance/attendance.php?3");
                AddLogMsg("출석게임하러 이동 합니다.");
            }
            else
            {
                if (0 < tryCount)
                    AddLogMsg("로그인 시도 중(" + tryCount + ") ");

                ++tryCount;
            }
        }

        private void MoveAttentionGamePage(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (webBrowser1.Url.Equals(EditAddress.Text + "/attendance/attendance.php?3"))
            {
                if (false == AttentionGameTimer.Enabled)
                {
                    AddLogMsg("출석게임 진행합니다.");
                    tryCount = 0;
                    webBrowser1.DocumentCompleted -= this.MoveAttentionGamePage;
                    webBrowser1.DocumentCompleted += this.CheckAttentionGame;
                    AttentionGameTimer.Start();
                }
            }
            else
            {
                if (0 < tryCount)
                {
                    AddLogMsg("출석게임으로 연결 시도중(" + tryCount + ")");
                }

                ++tryCount;
            }
        }

        private void CheckAttentionGame(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            tryCount = 0;
            HtmlDocument doc = webBrowser1.Document;
            HtmlElement elem;
            HtmlElementCollection elems = doc.GetElementsByTagName("center");
            elem = elems[4];
            string str1 = elem.InnerHtml;

            if (-1 != str1.IndexOf("오늘은"))
            {
                skipAttentionGame = true;
            }

            webBrowser1.DocumentCompleted -= this.CheckAttentionGame;
        }

        private void PlayTypingGame(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (webBrowser1.Url.Equals(EditAddress.Text + "/tazza/"))
            {
                AddLogMsg("타자게임 진행합니다. ");

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

                        break;
                    }
                }

                webBrowser1.DocumentCompleted -= this.PlayTypingGame;
                webBrowser1.DocumentCompleted += this.ShowResult;
            }
            else
            {
                ++tryCount;
                AddLogMsg("타자게임 접속 시도중(" + tryCount + ")");
                webBrowser1.Navigate(EditAddress.Text + "/tazza/");
            }
        }

        public void ShowResult(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (webBrowser1.Url.Equals(EditAddress.Text + "/index.php"))
            {
                if (TypingGameTimer.Enabled)
                {
                    TypingGameTimer.Stop();
                    if (1 < typingCount)
                    {
                        AddLogMsg(" - 일일 타자게임 횟수를 초과한 것 같습니다.");
                    }
                }

                AddLogMsg("--[결 과]-------------------------");
                if (skipAttentionGame || skipTypingGame)
                {
                    AddLogMsg(" -- 출석 게임과 타자 게임을 완료하지 못 했습니다.");
                }
                else if (skipAttentionGame)
                {
                    AddLogMsg(" -- 출석 게임을 완료하지 못 했습니다.");
                }
                else if (skipTypingGame)
                {
                    AddLogMsg(" -- 타자 게임을 완료하지 못 했습니다.");
                }

                if (-99999999 == initPoint)
                {
                    AddLogMsg(" 포인트를 확인할 수 없습니다.");
                }
                else
                {
                    AddLogMsg(" 총 " + (GetAccountPoint() - initPoint) + " 포인트를 획득했습니다.");
                }

                AddLogMsg("----------------------------------");
                AddLogMsg("Tcafe 로그아웃 합니다.");

                webBrowser1.DocumentCompleted -= this.ShowResult;
                CtrlBrowser.EndBrowserSession();
                webBrowser1.Navigate(EditAddress.Text + "/bbs/logout.php");
                SetEnabledControls();
            }
            else if (webBrowser1.Url.Equals(EditAddress.Text + "/tazza/"))
            {
                if (false == TypingGameTimer.Enabled)
                {
                    TypingGameTimer.Interval = 100;
                    TypingGameTimer.Start();
                }
            }
        }
    }    
}
