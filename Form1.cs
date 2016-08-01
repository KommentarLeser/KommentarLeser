using System;
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

namespace BlogReader
{
	public partial class Form1 : Form
	{
		private AngleSharp.Parser.Html.HtmlParser parser;
		private string html;
		private string htmlUrl = "";
		private string progOptionPath;
		private string _version = "";
		private AngleSharp.Dom.Html.IHtmlDocument doc;
		private AngleSharp.Dom.IElement list;
		private bool expanded = false;
		private SortedSet<string> seenSet = new SortedSet<string>();

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
		string sanitizeUrl(string htmlUrl)
		{
			int raute = htmlUrl.LastIndexOf('#');
			if(raute != -1)
				htmlUrl = htmlUrl.Substring(0, raute);
			char[] trims = { '/' };
			htmlUrl = htmlUrl.TrimEnd(trims);
			htmlUrl = htmlUrl.Replace(":", "%3A").Replace("/", "%2F").Trim();
			return htmlUrl;
		}
		void saveState()
		{
			htmlUrl = sanitizeUrl(htmlUrl);
			string saveFile = progOptionPath + htmlUrl;

			System.IO.FileStream file = System.IO.File.Create(saveFile);
			System.IO.StreamWriter sw = new System.IO.StreamWriter(file);
			foreach(var id in seenSet)
			{
				sw.WriteLine(id);
			}
			sw.Close();
		}
		void loadState()
		{
			htmlUrl = sanitizeUrl(htmlUrl);
			string loadFile = progOptionPath + htmlUrl;
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
			UseWaitCursor = true;
			string htmlname = textBoxUrl.Text;
			try
			{
				byte[] bytes = webClient.DownloadData(htmlname);
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
				MessageBox.Show("Der Kommnetarleser konnte die Adresse nicht interpretieren.");
				return;
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
					htmlUrl = htmlname;
					expandButton.Text = "Alles aufklappen";
					expanded = false;
					loadState(); // Liste der bereits gesehenen IDs laden
					filltree(list, treeView1.Nodes);
				}
				UseWaitCursor = false;
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
				treeView1.CollapseAll();
				expanded = false;
				expandButton.Text = "Alles aufklappen";
			}
			else
			{
				treeView1.ExpandAll();
				expanded = true;
				expandButton.Text = "Alles zuklappen";
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
			if(treeView1.Nodes.Count > 0 && htmlUrl != "")
			{
				saveState();
			}
		}

		private void label1_Click(object sender, EventArgs e)
		{

		}

		private void treeView1_BeforeSelect(object sender, TreeViewCancelEventArgs e)
		{
			var node = treeView1.SelectedNode;
			if(node != null)
			{
				//node.Text = node.Text.Substring(0, node.Text.Length - 13);
				//node.BackColor = Color.White;
			}
		}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			richTextBox1.Clear();
			TreeNode tn = treeView1.SelectedNode;
			//tn.BackColor = Color.LightGray;
			//tn.Text += "  <==========";
			entry ent = (entry)tn.Tag;
			richTextBox1.Text = ent.text;
			if(!ent.seen)
				markTreeNode(tn);
		}

		private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			var node = treeView1.SelectedNode;
			System.Diagnostics.Process.Start(((entry)(node.Tag)).link);
		}
	}
}
