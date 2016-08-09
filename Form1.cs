﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using AngleSharp;

using System.Reflection;  // for Version
using System.Diagnostics; // for Version

namespace KommentarLeser
{
	public partial class Form1 : Form
	{
		private AngleSharp.Parser.Html.HtmlParser parser;
		private string url;
		private string html;
		//private string urlFileName = "";
		private string progOptionPath;
		private string _version = "";
		private AngleSharp.Dom.Html.IHtmlDocument doc;
		private AngleSharp.Dom.IElement list;
		private bool expanded = false;
		private SortedSet<string> seenSet = new SortedSet<string>();
		private string selectedCommentId = "";
		private System.Net.WebClient webClient = new System.Net.WebClient();

		public Form1()
		{
			InitializeComponent();
			parser = new AngleSharp.Parser.Html.HtmlParser();
#if DEBUG
			//textBoxUrl.Text = @"http://vineyardsaker.de/analyse/die-spaltung-der-linken-ganz-im-sinne-der-herrschenden/";
			textBoxUrl.Text = @"http://vineyardsaker.de/analyse/der-gescheiterte-putsch-in-der-tuerkei-einige-erste-gedanken/";
#endif
			progOptionPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			progOptionPath += @"\KommentarLeser\";
			if(!System.IO.Directory.Exists(progOptionPath))
				System.IO.Directory.CreateDirectory(progOptionPath);
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			Assembly assembly = Assembly.GetExecutingAssembly();
			FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
			_version = "v" + fileVersionInfo.ProductVersion;
			Text = "KommentarLeser - " + _version;
			enableAll(false);
		}

		private int findInCombobox(string surl)
		{
			int ctr = 0;
			foreach(articleEntry ent in comboBoxArticles.Items)
			{
				if(ent.url == surl)
					return ctr;
				++ctr;
			}
			return -1;
		}

		private void readArticleList(Progress p)
		{
			try
			{
				string num = "1";
				string mainUrl = "http://vineyardsaker.de";
				for(int i = 0; i < 5; ++i)
				{
					p.set(num);
					byte[] bytes = webClient.DownloadData(mainUrl);
					string html = Encoding.UTF8.GetString(bytes);
					var doc = parser.Parse(html);
					var list = doc.QuerySelectorAll("h1.entry-title");
					foreach(var entry in list)
					{
						Form1.articleEntry ent = new Form1.articleEntry();
						ent.name = entry.TextContent;
						ent.setUrl(entry.QuerySelector("a").GetAttribute("href"));
						comboBoxArticles.Items.Add(ent);
					}
					num = (i + 2).ToString();
					mainUrl = "http://vineyardsaker.de/page/" + num;
#if DEBUG
					break;
#endif
				}
			}
			catch(Exception ex)
			{
				MessageBox.Show("Fehler beim Lesen der Artikelliste\n" + ex.ToString());
			}
			p.set("");
		}

		private void comboBoxArticles_SelectedIndexChanged(object sender, EventArgs e)
		{
			textBoxUrl.Text = ((Form1.articleEntry)comboBoxArticles.SelectedItem).url;
			selectedCommentId = "";
			Properties.Settings.Default.lastComment = selectedCommentId;
			Properties.Settings.Default.Save();
			loadButton_Click(null, null);
		}
		private class articleEntry
		{
			public string name;
			public string url;
			public void setUrl(string inUrl)
			{
				url = Utilities.sanitizeUrl(inUrl);
			}
			public override string ToString()
			{
				return name;
			}
		}

		private void exitButton_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private class entry
		{
			public string name;
			public string id;
			public string when;
			public string link;
			public string text;
			public bool seen = false;
		}

		private string filenameFromUrl(string htmlUrl)
		{
			Utilities.sanitizeUrl(htmlUrl);
			htmlUrl = htmlUrl.Replace(":", "%3A").Replace("/", "%2F").Trim();
			return htmlUrl;
		}
		private void setUrl(string inUrl)
		{
			url = Utilities.sanitizeUrl(inUrl);
		}
		private void saveState()
		{
			string urlFileName = filenameFromUrl(url);
			string saveFileNAme = progOptionPath + urlFileName;

			System.IO.FileStream file = System.IO.File.Create(saveFileNAme);
			System.IO.StreamWriter sw = new System.IO.StreamWriter(file);
			foreach(var id in seenSet)
			{
				sw.WriteLine(id);
			}
			sw.Close();
			string lastUrlFile = this.progOptionPath + "lastUrl";
			System.IO.FileStream urlfile = System.IO.File.Create(lastUrlFile);
			System.IO.StreamWriter urlsw = new System.IO.StreamWriter(urlfile);
			urlsw.WriteLine(this.textBoxUrl.Text);
			urlsw.Close();
			Properties.Settings.Default.expanded = expanded;
			Properties.Settings.Default.Save();
		}
		void updateText()
		{
			TreeNode tn = treeView1.SelectedNode;
			entry ent = (entry)tn?.Tag;
			if(ent == null)
				return;
			richTextBox1.Clear();
			richTextBox1.Text = ent.text;
			if(!ent.seen)
				markTreeNode(tn);
		}
		void loadSeenSet()
		{
			string urlFileName = filenameFromUrl(url);
			string loadFile = progOptionPath + urlFileName;
			string[] lines;
			try
			{
				lines = System.IO.File.ReadAllLines(loadFile);
				seenSet.Clear();
				foreach(string id in lines)
					seenSet.Add(id);
			}
			catch(Exception)
			{
				return;
			}
		}
		private void loadButton_Click(object sender, EventArgs e)
		{
			try
			{
				enableAll(false);
				UseWaitCursor = true;
				Application.DoEvents();
				setUrl(textBoxUrl.Text);
				byte[] bytes = webClient.DownloadData(url);
				html = System.Text.Encoding.UTF8.GetString(bytes);
				doc = parser.Parse(html);
				var div = doc?.QuerySelector(".social-comments");
				list = div?.QuerySelector(".social-commentlist");
				if(list == null)
					list = div?.QuerySelector(".commentlist");
				if(list == null)
					throw new Exception("list == null");
			}
			catch(Exception ex)
			{
				url = "";
				list = null;
				MessageBox.Show("Der Kommentarleser konnte die Adresse nicht interpretieren.");
			}
			finally
			{
				if(list != null) // wir konnten parsen
				{
					if(treeView1.Nodes.Count != 0)
					{
						saveState(); // Altes seenSet abspeichern
						treeView1.Nodes.Clear();
					}
					loadSeenSet(); // Liste der bereits gesehenen IDs laden
					filltree(list, treeView1.Nodes);
					if(expanded)
						expand();
					else
						collapse();
					if(selectedCommentId != "")
						treeView1.SelectedNode = findNodeById(ref selectedCommentId, treeView1.Nodes);
					else
						richTextBox1.Clear();
					expandButton.Select();
					UseWaitCursor = false;
					enableAll(true);
				}
			}
		}
		private void filltree(AngleSharp.Dom.IElement subList, TreeNodeCollection nodelist)
		{
			var items = subList.Children;
			foreach(var item in items)
			{
				entry ent = new entry();
				ent.name = item.QuerySelector("cite.fn").TextContent;
				ent.id = item.QuerySelector(".social-comment-inner").Id;
				ent.when = item.QuerySelector(".social-posted-when").TextContent;
				ent.link = item.QuerySelector(".social-posted-when").GetAttribute("href");
				ent.text = item.QuerySelector(".social-comment-body").TextContent;
				ent.text = ent.text.Trim().Replace("\n", "\r\n\r\n");
				TreeNode tn = new TreeNode(ent.name + "  -  " + ent.when);
				tn.Tag = ent;
				if(seenSet.Contains(ent.id))
					markTreeNode(tn);
				nodelist.Add(tn);
				var sublist = item.QuerySelector("ul"); // Unterliste?
				if(sublist != null)
					filltree(sublist, tn.Nodes);
			}
		}

		private void markTreeNode(TreeNode tn)
		{
			entry ent = (entry)tn.Tag;
			ent.seen = true;
			tn.ForeColor = Color.Red;
			seenSet.Add(ent.id);
		}

		private void expandButton_Click(object sender, EventArgs e)
		{
			if(expanded)
			{
				var temp = treeView1.SelectedNode;
				bool isex = treeView1.SelectedNode.IsExpanded;
				collapse();
				treeView1.SelectedNode = temp;
				if(isex)
					treeView1.SelectedNode.ExpandAll();
			}
			else
			{
				expand();
			}
		}

		private void richTextBox1_LinkClicked(object sender, LinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start(e.LinkText);
		}

		private void textBoxUrl_KeyPress(object sender, KeyPressEventArgs e)
		{
			if(e.KeyChar == '\r')
			{
				e.Handled = true;
				loadButton_Click(null, null);
			}
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			if(treeView1.Nodes.Count > 0 && url != "")
			{
				saveState();
			}
		}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			updateText();
			selectedCommentId = ((entry)treeView1.SelectedNode.Tag).id;
			Properties.Settings.Default.lastComment = selectedCommentId;
			Properties.Settings.Default.Save();
		}

		private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			var node = treeView1.SelectedNode;
			if(node.IsExpanded)
				node.Collapse();
			else
				node.Expand();
			System.Diagnostics.Process.Start(((entry)(node.Tag)).link);
		}

		private void Form1_Shown(object sender, EventArgs e)
		{
			UseWaitCursor = true;
			Progress p = null;
			try
			{
				p = new Progress();
				p.StartPosition = FormStartPosition.Manual;
				Point pos = new Point((Width / 2) + Left - (p.Width / 2), ((Height) / 2) + Top - p.Height / 2);
				p.Location = pos;
				p.Show(this);
				p.Text = "Lade Artikelliste...";
				restoreArticleList(p);
				restoreUrl(p);
				restoreState();
			}
			finally
			{
				p?.Close();
				p = null;
				expandButton.Select();
				enableAll(true);
				UseWaitCursor = false;
			}
		}

		string restoreArticleList(Progress p)
		{
			if(Properties.Settings.Default.upgradeRequired)
			{
				Properties.Settings.Default.Upgrade();
				Properties.Settings.Default.upgradeRequired = false;
				Properties.Settings.Default.Save();
			}
			readArticleList(p);
			try
			{
				// zuletzt besuchte URL wiederherstellen
				string loadUrlFile = this.progOptionPath + "lastUrl";
				using(System.IO.StreamReader urlsw = new System.IO.StreamReader(loadUrlFile))
				{
					setUrl(urlsw.ReadLine());
					this.textBoxUrl.Text = url;
				}
			}
			catch(Exception ex)
			{
				url = "";
				this.textBoxUrl.Text = "";
			}
			return url;
		}

		void restoreUrl(Progress p)
		{
			p.Text = "Lade Artikel...";
			if(url == "")
			{
				if(this.comboBoxArticles.Items.Count > 0)
				{
					this.comboBoxArticles.SelectedIndex = 0;
					this.textBoxUrl.Text = ((Form1.articleEntry)this.comboBoxArticles.SelectedItem).url;
				}
			}
			else
			{
				string temp = Properties.Settings.Default.lastComment;
				comboBoxArticles.SelectedIndex = findInCombobox(url);
				Properties.Settings.Default.lastComment = temp;
			}
		}
		void expand() {
			expanded = true;
			treeView1.ExpandAll();
			expandButton.Text = "alles zuklappen";
		}
		void collapse() {
			expanded = false;
			treeView1.CollapseAll();
			expandButton.Text = "alles aufklappen";
			updateText();
		}
		void restoreState()
		{
			selectedCommentId = Properties.Settings.Default.lastComment;
			if(selectedCommentId != "")
			{
				treeView1.SelectedNode = findNodeById(ref selectedCommentId, treeView1.Nodes);
			}
			bool expanded = Properties.Settings.Default.expanded;
			if(expanded)
				expand();
		}
		TreeNode findNodeById(ref string id, TreeNodeCollection nodes)
		{
			foreach(TreeNode node in nodes)
			{
				if(((entry)(node.Tag)).id == id)
					return node;
				var found = findNodeById(ref id, node.Nodes);
				if(found != null)
					return found; 
			}
			return null;
		}
		private void enableAll(bool enable)
		{
			if(enable)
			{
				foreach(Control c in Controls)
					c.Enabled = true;
				richTextBox1.ScrollBars = RichTextBoxScrollBars.None;
				richTextBox1.ScrollBars = RichTextBoxScrollBars.Vertical;
			}
			else
			{
				foreach(Control c in Controls)
					c.Enabled = false;
			}
		}
	}
	//class ProgressForm
	class Utilities
	{
		public static string sanitizeUrl(string inUrl)
		{
			int raute = inUrl.LastIndexOf('#');
			if(raute != -1)
				inUrl = inUrl.Substring(0, raute);
			char[] trims = { '/' };
			inUrl = inUrl.TrimEnd(trims);
			return inUrl;
		}
	}
}
