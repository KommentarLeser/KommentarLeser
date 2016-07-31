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


namespace BlogReader
{
	public partial class Form1 : Form
	{
		private AngleSharp.Parser.Html.HtmlParser parser;
		private string html;
		private AngleSharp.Dom.Html.IHtmlDocument doc;
		private AngleSharp.Dom.IElement list;
		private bool expanded = false;
		private SortedSet<string> seenSet = new SortedSet<string>();

		private System.Net.WebClient webClient = new System.Net.WebClient();

		public Form1()
		{
			InitializeComponent();
			parser = new AngleSharp.Parser.Html.HtmlParser();
			//string htmlname = @"D:\temp\_________\BlogReader\html.htm";
			// System.IO.TextReader reader = new System.IO.StreamReader(htmlname);
			//html = System.IO.File.ReadAllText(htmlname);
			//doc = parser.Parse(html);
			//var div = doc.QuerySelector(".social-comments");
			//list = div.QuerySelector(".social-commentlist");
			// MessageBox.Show(html.Substring(0, 100));
		}

		private void Form1_Load(object sender, EventArgs e)
		{

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
			public string text;
			public bool seen = false;
			//public List<entry> subentries;
		}

		private void loadButton_Click(object sender, EventArgs e)
		{
			string htmlname = textBoxUrl.Text;
			byte[] bytes = webClient.DownloadData(htmlname);
			html = System.Text.Encoding.UTF8.GetString(bytes);
			doc = parser.Parse(html);
			var div = doc.QuerySelector(".social-comments");
			list = div.QuerySelector(".social-commentlist");
			var items = list.Children; // <li></li>
			foreach(var item in items)
			{
				entry ent = new entry();
				ent.name = item.QuerySelector("cite.fn").TextContent;
				ent.id = item.QuerySelector(".social-comment-inner").Id;
				ent.when = item.QuerySelector(".social-posted-when").TextContent;
				ent.text = item.QuerySelector(".social-comment-body").TextContent;
				ent.text = ent.text.Trim().Replace("\n", "\r\n\r\n");
				TreeNode tn = new TreeNode(ent.name + "  -  " + ent.when);
				tn.Tag = ent;
				treeView1.Nodes.Add(tn);
				var sublist = item.QuerySelector("ul"); // Unterliste?
				if(sublist != null)
					filltree(sublist, tn);
			}
		}
		private void filltreeX(AngleSharp.Dom.IElement subList, TreeNode topnode)
		{
		}
		private void filltree(AngleSharp.Dom.IElement subList, TreeNode topnode)
		{
			var items = subList.Children;
			foreach(var item in items)
			{
				entry ent = new entry();
				ent.name = item.QuerySelector("cite.fn").TextContent;
				ent.when = item.QuerySelector(".social-posted-when").TextContent;
				ent.text = item.QuerySelector(".social-comment-body").TextContent;
				ent.text = ent.text.Trim().Replace("\n", "\r\n\r\n");
				TreeNode tn = new TreeNode(ent.name + "  -  " + ent.when);
				tn.Tag = ent;
				topnode.Nodes.Add(tn);
				var sublist = item.QuerySelector("ul"); // Unterliste?
				if(sublist != null)
					filltree(sublist, tn);
			}
		}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			textBox1.Clear();
			entry ent = (entry)treeView1.SelectedNode.Tag;
			textBox1.Text = ent.text;
			if(!ent.seen)
			{
				ent.seen = true;
				treeView1.SelectedNode.ForeColor = Color.Red;
				treeView1.SelectedNode.Text += " <#";
				seenSet.Add(ent.id);
			}
		}

		private void expandButton_Click(object sender, EventArgs e)
		{
			if(expanded)
			{
				treeView1.CollapseAll();
				expanded = false;
				expandButton.Text = "Alles auf";
			}
			else
			{
				treeView1.ExpandAll();
				expanded = true;
				expandButton.Text = "Alles zu";
			}
		}
	}
}
