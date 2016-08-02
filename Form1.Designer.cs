namespace KommentarLeser
{
	partial class Form1
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if(disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.treeView1 = new System.Windows.Forms.TreeView();
			this.loadButton = new System.Windows.Forms.Button();
			this.exitButton = new System.Windows.Forms.Button();
			this.expandButton = new System.Windows.Forms.Button();
			this.textBoxUrl = new System.Windows.Forms.TextBox();
			this.richTextBox1 = new System.Windows.Forms.RichTextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.comboBoxArticles = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// treeView1
			// 
			this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.treeView1.HideSelection = false;
			this.treeView1.Location = new System.Drawing.Point(12, 70);
			this.treeView1.Name = "treeView1";
			this.treeView1.Size = new System.Drawing.Size(372, 655);
			this.treeView1.TabIndex = 0;
			this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
			this.treeView1.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseDoubleClick);
			// 
			// loadButton
			// 
			this.loadButton.Location = new System.Drawing.Point(9, 12);
			this.loadButton.Name = "loadButton";
			this.loadButton.Size = new System.Drawing.Size(97, 23);
			this.loadButton.TabIndex = 1;
			this.loadButton.Text = "laden";
			this.loadButton.UseVisualStyleBackColor = true;
			this.loadButton.Click += new System.EventHandler(this.loadButton_Click);
			// 
			// exitButton
			// 
			this.exitButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.exitButton.Location = new System.Drawing.Point(1041, 15);
			this.exitButton.Name = "exitButton";
			this.exitButton.Size = new System.Drawing.Size(72, 26);
			this.exitButton.TabIndex = 2;
			this.exitButton.Text = "beenden";
			this.exitButton.UseVisualStyleBackColor = true;
			this.exitButton.Click += new System.EventHandler(this.exitButton_Click);
			// 
			// expandButton
			// 
			this.expandButton.Location = new System.Drawing.Point(9, 41);
			this.expandButton.Name = "expandButton";
			this.expandButton.Size = new System.Drawing.Size(97, 23);
			this.expandButton.TabIndex = 4;
			this.expandButton.Text = "Alles aufklappen";
			this.expandButton.UseVisualStyleBackColor = true;
			this.expandButton.Click += new System.EventHandler(this.expandButton_Click);
			// 
			// textBoxUrl
			// 
			this.textBoxUrl.AcceptsReturn = true;
			this.textBoxUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxUrl.Location = new System.Drawing.Point(166, 15);
			this.textBoxUrl.Name = "textBoxUrl";
			this.textBoxUrl.Size = new System.Drawing.Size(869, 20);
			this.textBoxUrl.TabIndex = 5;
			this.textBoxUrl.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxUrl_KeyPress);
			// 
			// richTextBox1
			// 
			this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.richTextBox1.BackColor = System.Drawing.Color.Gainsboro;
			this.richTextBox1.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.richTextBox1.Location = new System.Drawing.Point(390, 70);
			this.richTextBox1.Name = "richTextBox1";
			this.richTextBox1.ReadOnly = true;
			this.richTextBox1.Size = new System.Drawing.Size(723, 655);
			this.richTextBox1.TabIndex = 6;
			this.richTextBox1.Text = "";
			this.richTextBox1.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.richTextBox1_LinkClicked);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(112, 18);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(48, 13);
			this.label1.TabIndex = 7;
			this.label1.Text = "Adresse:";
			// 
			// comboBoxArticles
			// 
			this.comboBoxArticles.FormattingEnabled = true;
			this.comboBoxArticles.Location = new System.Drawing.Point(166, 41);
			this.comboBoxArticles.Name = "comboBoxArticles";
			this.comboBoxArticles.Size = new System.Drawing.Size(623, 21);
			this.comboBoxArticles.TabIndex = 8;
			this.comboBoxArticles.SelectedIndexChanged += new System.EventHandler(this.comboBoxArticles_SelectedIndexChanged);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1125, 737);
			this.Controls.Add(this.comboBoxArticles);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.richTextBox1);
			this.Controls.Add(this.textBoxUrl);
			this.Controls.Add(this.expandButton);
			this.Controls.Add(this.exitButton);
			this.Controls.Add(this.loadButton);
			this.Controls.Add(this.treeView1);
			this.Name = "Form1";
			this.Text = "Sakers Kommentarleser";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
			this.Load += new System.EventHandler(this.Form1_Load);
			this.Shown += new System.EventHandler(this.Form1_Shown);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TreeView treeView1;
		private System.Windows.Forms.Button loadButton;
		private System.Windows.Forms.Button exitButton;
		private System.Windows.Forms.Button expandButton;
		private System.Windows.Forms.TextBox textBoxUrl;
		private System.Windows.Forms.RichTextBox richTextBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox comboBoxArticles;
	}
}

