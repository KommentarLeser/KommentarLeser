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
			this.expandButton = new System.Windows.Forms.Button();
			this.textBoxUrl = new System.Windows.Forms.TextBox();
			this.richTextBox1 = new System.Windows.Forms.RichTextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.comboBoxArticles = new System.Windows.Forms.ComboBox();
			this.comboBoxNutzer = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textBoxSuche = new System.Windows.Forms.TextBox();
			this.buttonSuche = new System.Windows.Forms.Button();
			this.mainSplitContainer = new System.Windows.Forms.SplitContainer();
			this.treeSplitContainer = new System.Windows.Forms.SplitContainer();
			this.listView1 = new System.Windows.Forms.ListView();
			this.column1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			((System.ComponentModel.ISupportInitialize)(this.mainSplitContainer)).BeginInit();
			this.mainSplitContainer.Panel1.SuspendLayout();
			this.mainSplitContainer.Panel2.SuspendLayout();
			this.mainSplitContainer.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.treeSplitContainer)).BeginInit();
			this.treeSplitContainer.Panel1.SuspendLayout();
			this.treeSplitContainer.Panel2.SuspendLayout();
			this.treeSplitContainer.SuspendLayout();
			this.SuspendLayout();
			// 
			// treeView1
			// 
			this.treeView1.CheckBoxes = true;
			this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeView1.HideSelection = false;
			this.treeView1.Location = new System.Drawing.Point(0, 0);
			this.treeView1.Name = "treeView1";
			this.treeView1.Size = new System.Drawing.Size(390, 448);
			this.treeView1.TabIndex = 2;
			this.treeView1.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterCheck);
			this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
			this.treeView1.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseDoubleClick);
			this.treeView1.MouseEnter += new System.EventHandler(this.treeView1_MouseEnter);
			// 
			// loadButton
			// 
			this.loadButton.Location = new System.Drawing.Point(9, 12);
			this.loadButton.Name = "loadButton";
			this.loadButton.Size = new System.Drawing.Size(97, 23);
			this.loadButton.TabIndex = 1;
			this.loadButton.Text = "(neu) laden";
			this.loadButton.UseVisualStyleBackColor = true;
			this.loadButton.Click += new System.EventHandler(this.loadButton_Click);
			this.loadButton.MouseEnter += new System.EventHandler(this.loadButton_MouseEnter);
			this.loadButton.MouseLeave += new System.EventHandler(this.loadButton_MouseLeave);
			// 
			// expandButton
			// 
			this.expandButton.Location = new System.Drawing.Point(9, 41);
			this.expandButton.Name = "expandButton";
			this.expandButton.Size = new System.Drawing.Size(97, 23);
			this.expandButton.TabIndex = 4;
			this.expandButton.Text = "alles aufklappen";
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
			this.textBoxUrl.Size = new System.Drawing.Size(763, 20);
			this.textBoxUrl.TabIndex = 5;
			this.textBoxUrl.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxUrl_KeyPress);
			// 
			// richTextBox1
			// 
			this.richTextBox1.BackColor = System.Drawing.Color.Gainsboro;
			this.richTextBox1.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.richTextBox1.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.richTextBox1.Location = new System.Drawing.Point(0, 0);
			this.richTextBox1.Name = "richTextBox1";
			this.richTextBox1.ReadOnly = true;
			this.richTextBox1.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.richTextBox1.Size = new System.Drawing.Size(601, 639);
			this.richTextBox1.TabIndex = 3;
			this.richTextBox1.Text = "";
			this.richTextBox1.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.richTextBox1_LinkClicked);
			this.richTextBox1.MouseEnter += new System.EventHandler(this.richTextBox1_MouseEnter);
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
			this.comboBoxArticles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxArticles.FormattingEnabled = true;
			this.comboBoxArticles.Location = new System.Drawing.Point(166, 41);
			this.comboBoxArticles.Name = "comboBoxArticles";
			this.comboBoxArticles.Size = new System.Drawing.Size(363, 21);
			this.comboBoxArticles.TabIndex = 8;
			this.comboBoxArticles.SelectedIndexChanged += new System.EventHandler(this.comboBoxArticles_SelectedIndexChanged);
			this.comboBoxArticles.MouseDown += new System.Windows.Forms.MouseEventHandler(this.comboBoxArticles_MouseDown);
			// 
			// comboBoxNutzer
			// 
			this.comboBoxNutzer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxNutzer.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
			this.comboBoxNutzer.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
			this.comboBoxNutzer.FormattingEnabled = true;
			this.comboBoxNutzer.Location = new System.Drawing.Point(582, 41);
			this.comboBoxNutzer.Name = "comboBoxNutzer";
			this.comboBoxNutzer.Size = new System.Drawing.Size(162, 21);
			this.comboBoxNutzer.TabIndex = 9;
			this.comboBoxNutzer.SelectedIndexChanged += new System.EventHandler(this.comboBoxNutzer_SelectedIndexChanged);
			this.comboBoxNutzer.KeyDown += new System.Windows.Forms.KeyEventHandler(this.comboBoxNutzer_KeyDown);
			this.comboBoxNutzer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.comboBoxNutzer_MouseDown);
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(535, 44);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(41, 13);
			this.label2.TabIndex = 10;
			this.label2.Text = "Nutzer:";
			// 
			// textBoxSuche
			// 
			this.textBoxSuche.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxSuche.Location = new System.Drawing.Point(750, 41);
			this.textBoxSuche.Name = "textBoxSuche";
			this.textBoxSuche.Size = new System.Drawing.Size(179, 20);
			this.textBoxSuche.TabIndex = 11;
			this.textBoxSuche.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxSuche_KeyPress);
			// 
			// buttonSuche
			// 
			this.buttonSuche.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonSuche.Location = new System.Drawing.Point(935, 38);
			this.buttonSuche.Name = "buttonSuche";
			this.buttonSuche.Size = new System.Drawing.Size(71, 23);
			this.buttonSuche.TabIndex = 12;
			this.buttonSuche.Text = "such!";
			this.buttonSuche.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			this.buttonSuche.UseVisualStyleBackColor = true;
			this.buttonSuche.Click += new System.EventHandler(this.buttonSuche_Click);
			// 
			// mainSplitContainer
			// 
			this.mainSplitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.mainSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.mainSplitContainer.Location = new System.Drawing.Point(9, 70);
			this.mainSplitContainer.Name = "mainSplitContainer";
			// 
			// mainSplitContainer.Panel1
			// 
			this.mainSplitContainer.Panel1.Controls.Add(this.treeSplitContainer);
			// 
			// mainSplitContainer.Panel2
			// 
			this.mainSplitContainer.Panel2.Controls.Add(this.richTextBox1);
			this.mainSplitContainer.Size = new System.Drawing.Size(995, 639);
			this.mainSplitContainer.SplitterDistance = 390;
			this.mainSplitContainer.TabIndex = 13;
			// 
			// treeSplitContainer
			// 
			this.treeSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeSplitContainer.Location = new System.Drawing.Point(0, 0);
			this.treeSplitContainer.Name = "treeSplitContainer";
			this.treeSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// treeSplitContainer.Panel1
			// 
			this.treeSplitContainer.Panel1.Controls.Add(this.treeView1);
			this.treeSplitContainer.Panel1MinSize = 0;
			// 
			// treeSplitContainer.Panel2
			// 
			this.treeSplitContainer.Panel2.Controls.Add(this.listView1);
			this.treeSplitContainer.Panel2MinSize = 0;
			this.treeSplitContainer.Size = new System.Drawing.Size(390, 639);
			this.treeSplitContainer.SplitterDistance = 448;
			this.treeSplitContainer.TabIndex = 6;
			// 
			// listView1
			// 
			this.listView1.CheckBoxes = true;
			this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.column1});
			this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.listView1.HideSelection = false;
			this.listView1.Location = new System.Drawing.Point(0, 0);
			this.listView1.MultiSelect = false;
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size(390, 187);
			this.listView1.TabIndex = 5;
			this.listView1.UseCompatibleStateImageBehavior = false;
			this.listView1.View = System.Windows.Forms.View.Details;
			this.listView1.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.listView1_ItemChecked);
			this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
			this.listView1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseDoubleClick);
			this.listView1.MouseEnter += new System.EventHandler(this.listView1_MouseEnter);
			// 
			// column1
			// 
			this.column1.Text = "";
			this.column1.Width = 30;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1016, 721);
			this.Controls.Add(this.mainSplitContainer);
			this.Controls.Add(this.buttonSuche);
			this.Controls.Add(this.textBoxSuche);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.comboBoxNutzer);
			this.Controls.Add(this.comboBoxArticles);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textBoxUrl);
			this.Controls.Add(this.expandButton);
			this.Controls.Add(this.loadButton);
			this.KeyPreview = true;
			this.Name = "Form1";
			this.Text = "Sakers Kommentarleser";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
			this.Load += new System.EventHandler(this.Form1_Load);
			this.Shown += new System.EventHandler(this.Form1_Shown);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
			this.mainSplitContainer.Panel1.ResumeLayout(false);
			this.mainSplitContainer.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.mainSplitContainer)).EndInit();
			this.mainSplitContainer.ResumeLayout(false);
			this.treeSplitContainer.Panel1.ResumeLayout(false);
			this.treeSplitContainer.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.treeSplitContainer)).EndInit();
			this.treeSplitContainer.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TreeView treeView1;
		private System.Windows.Forms.Button loadButton;
		private System.Windows.Forms.Button expandButton;
		private System.Windows.Forms.TextBox textBoxUrl;
		private System.Windows.Forms.RichTextBox richTextBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox comboBoxArticles;
		private System.Windows.Forms.ComboBox comboBoxNutzer;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBoxSuche;
		private System.Windows.Forms.Button buttonSuche;
		private System.Windows.Forms.SplitContainer mainSplitContainer;
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.ColumnHeader column1;
		private System.Windows.Forms.SplitContainer treeSplitContainer;
	}
}

