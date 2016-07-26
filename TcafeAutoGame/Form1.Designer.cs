namespace TcafeAutoGame
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label1 = new System.Windows.Forms.Label();
            this.EditAddress = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.EditID = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.EditPassword = new System.Windows.Forms.TextBox();
            this.LogBox = new System.Windows.Forms.ListBox();
            this.RunButton = new System.Windows.Forms.Button();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.AttentionGameTimer = new System.Windows.Forms.Timer(this.components);
            this.TypingGameTimer = new System.Windows.Forms.Timer(this.components);
            this.FormShowTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "주소:";
            // 
            // EditAddress
            // 
            this.EditAddress.Enabled = false;
            this.EditAddress.Location = new System.Drawing.Point(51, 8);
            this.EditAddress.Name = "EditAddress";
            this.EditAddress.Size = new System.Drawing.Size(146, 21);
            this.EditAddress.TabIndex = 4;
            this.EditAddress.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.EditAddress.WordWrap = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(24, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "ID :";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // EditID
            // 
            this.EditID.Enabled = false;
            this.EditID.Location = new System.Drawing.Point(52, 36);
            this.EditID.Name = "EditID";
            this.EditID.Size = new System.Drawing.Size(145, 21);
            this.EditID.TabIndex = 1;
            this.EditID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.EditID.WordWrap = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(33, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "암호:";
            // 
            // EditPassword
            // 
            this.EditPassword.Enabled = false;
            this.EditPassword.Location = new System.Drawing.Point(52, 64);
            this.EditPassword.Name = "EditPassword";
            this.EditPassword.Size = new System.Drawing.Size(145, 21);
            this.EditPassword.TabIndex = 2;
            this.EditPassword.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.EditPassword.UseSystemPasswordChar = true;
            this.EditPassword.WordWrap = false;
            // 
            // LogBox
            // 
            this.LogBox.FormattingEnabled = true;
            this.LogBox.ItemHeight = 12;
            this.LogBox.Location = new System.Drawing.Point(15, 98);
            this.LogBox.Name = "LogBox";
            this.LogBox.Size = new System.Drawing.Size(264, 88);
            this.LogBox.TabIndex = 5;
            this.LogBox.TabStop = false;
            this.LogBox.UseTabStops = false;
            // 
            // RunButton
            // 
            this.RunButton.Enabled = false;
            this.RunButton.Location = new System.Drawing.Point(204, 5);
            this.RunButton.Name = "RunButton";
            this.RunButton.Size = new System.Drawing.Size(75, 80);
            this.RunButton.TabIndex = 3;
            this.RunButton.Text = "시작";
            this.RunButton.UseVisualStyleBackColor = true;
            this.RunButton.Click += new System.EventHandler(this.RunButton_Click);
            // 
            // webBrowser1
            // 
            this.webBrowser1.Location = new System.Drawing.Point(15, 160);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.ScriptErrorsSuppressed = true;
            this.webBrowser1.Size = new System.Drawing.Size(71, 59);
            this.webBrowser1.TabIndex = 7;
            this.webBrowser1.TabStop = false;
            this.webBrowser1.Url = new System.Uri("https://twitter.com/tcafenet", System.UriKind.Absolute);
            this.webBrowser1.Visible = false;
            this.webBrowser1.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.CheckTwitterAddress);
            this.webBrowser1.ProgressChanged += new System.Windows.Forms.WebBrowserProgressChangedEventHandler(this.webBrowser1_ProgressChanged);
            // 
            // AttentionGameTimer
            // 
            this.AttentionGameTimer.Interval = 250;
            this.AttentionGameTimer.Tick += new System.EventHandler(this.AttentionGameTimer_Tick);
            // 
            // TypingGameTimer
            // 
            this.TypingGameTimer.Interval = 250;
            this.TypingGameTimer.Tick += new System.EventHandler(this.TypingGameTimer_Tick);
            // 
            // FormShowTimer
            // 
            this.FormShowTimer.Enabled = true;
            this.FormShowTimer.Interval = 3000;
            this.FormShowTimer.Tick += new System.EventHandler(this.FormShowTimer_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(290, 197);
            this.Controls.Add(this.webBrowser1);
            this.Controls.Add(this.RunButton);
            this.Controls.Add(this.LogBox);
            this.Controls.Add(this.EditPassword);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.EditID);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.EditAddress);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(306, 236);
            this.MinimumSize = new System.Drawing.Size(306, 236);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TcafeAutoGame v1.1.4";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox EditAddress;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox EditID;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox EditPassword;
        private System.Windows.Forms.ListBox LogBox;
        private System.Windows.Forms.Button RunButton;
        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.Timer AttentionGameTimer;
        private System.Windows.Forms.Timer TypingGameTimer;
        private System.Windows.Forms.Timer FormShowTimer;
    }
}

