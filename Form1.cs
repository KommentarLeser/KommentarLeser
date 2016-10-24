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
using System.Collections;

namespace KommentarLeser
{
	public partial class Form1 : Form
	{
		private AngleSharp.Parser.Html.HtmlParser parser;
		private string _url;
		private string html;
		//private string urlFileName = "";
		private string progOptionPath;
		private string _version = "";
		private AngleSharp.Dom.Html.IHtmlDocument doc;
		private AngleSharp.Dom.IElement commentList;
		private System.Collections.Generic.SortedDictionary<string, System.Collections.Generic.List<TreeNode>> _userNames;
		private bool expanded = false;
		private SortedSet<string> seenSet = new SortedSet<string>();
		private SortedSet<string> checkedSet = new SortedSet<string>();
		private string selectedCommentId = "";
		private System.Net.WebClient webClient = new System.Net.WebClient();

		private Font regularFont;
		private Font boldFont;
		private Font underlineFont;

		public Form1()
		{
			InitializeComponent();
			regularFont = treeView1.Font;
			boldFont = new Font(regularFont, FontStyle.Bold);
			underlineFont = new Font(regularFont, FontStyle.Underline);
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
			_userNames = new SortedDictionary<string, List<TreeNode>>();
			if(Properties.Settings.Default.size.Height != -1)
			{
				Location = Properties.Settings.Default.location;
				Size = Properties.Settings.Default.size;
				splitContainer1.SplitterDistance = Properties.Settings.Default.splitterDistance;
			}
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
			string num = "1";
			try
			{
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
					break; // im DEBUG-Modus nur eine Seite Laden
#endif
				}
			}
			catch(Exception ex)
			{
				int nnum;
				int.TryParse(num, out nnum);
				if(nnum == 1)
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
			_url = Utilities.sanitizeUrl(inUrl);
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
		private void saveState()
		{
			string urlFileName = filenameFromUrl(_url);
			string saveFileName = progOptionPath + urlFileName;

			System.IO.FileStream file = System.IO.File.Create(saveFileName);
			System.IO.StreamWriter sw = new System.IO.StreamWriter(file);
			foreach(var id in seenSet)
			{
				sw.WriteLine(id);
			}
			sw.Close();
			saveFileName += ".checked";
			System.IO.FileStream checkedfile = System.IO.File.Create(saveFileName);
			System.IO.StreamWriter checkedsw = new System.IO.StreamWriter(checkedfile);
			foreach(var id in checkedSet)
			{
				checkedsw.WriteLine(id);
			}
			checkedsw.Close();
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
			string urlFileName = filenameFromUrl(_url);
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
		void loadCheckedSet()
		{
			string urlFileName = filenameFromUrl(_url);
			string loadFile = progOptionPath + urlFileName + ".checked";
			string[] lines;
			try
			{
				lines = System.IO.File.ReadAllLines(loadFile);
				checkedSet.Clear();
				foreach(string id in lines)
					checkedSet.Add(id);
			}
			catch(Exception)
			{
				return;
			}
		}

		private void loadButton_Click(object sender, EventArgs e)
		{
			string oldUrl = _url;
			string oldUser = (string)comboBoxNutzer.SelectedItem;
			comboBoxNutzer.Text = "";
			AngleSharp.Dom.IElement div;
			try
			{
				enableAll(false);
				UseWaitCursor = true;

				if(treeView1.Nodes.Count != 0)
				{
					saveState(); // Altes seenSet/checkedSet abspeichern
					treeView1.Nodes.Clear();
				}
				_userNames.Clear();
				comboBoxNutzer.Items.Clear();
				comboBoxNutzer.SelectedIndex = -1;
				richTextBox1.Clear();
				textBoxSuche.Clear();
				Application.DoEvents();

				setUrl(textBoxUrl.Text);
				byte[] bytes = webClient.DownloadData(_url);
				html = System.Text.Encoding.UTF8.GetString(bytes);
				doc = parser.Parse(html);

				div = doc?.QuerySelector(".entry-content"); // Artikel
				if(div == null)
					throw new Exception("Kann Artikel nicht finden.");

				entry ent = new entry();
				ent.name = "Artikel";
				ent.id = "Artikel";
				ent.when = "";
				ent.link = _url;
				ent.text = div.TextContent;
				ent.text = ent.text.Trim().Replace("\n", "\r\n\r\n");
				TreeNode tn = new TreeNode(ent.name);
				tn.Tag = ent;
				treeView1.Nodes.Add(tn);

				//div = doc?.QuerySelector(".social-comments");
				div = doc?.QuerySelector(".comments-area");
				//commentList = div?.QuerySelector(".social-commentlist");
				commentList = div?.QuerySelector(".comment-list");
				if(commentList == null)
					commentList = div?.QuerySelector(".commentlist");
			}
			catch(Exception ex)
			{
				_url = "";
				commentList = null;
				treeView1.Nodes.Clear();
			
				_userNames.Clear();
				comboBoxNutzer.Items.Clear();
				comboBoxNutzer.SelectedIndex = -1;
				MessageBox.Show("Kein Artikel gefunden.");
			}
			finally
			{
				if(commentList != null) // wir habenm Kommentare
				{
					loadSeenSet(); // Liste der bereits gesehenen IDs laden
					loadCheckedSet(); // Liste der mrkierten IDs laden

					_userNames.Add("-- kein --", new List<TreeNode>());
					treeView1.BeginUpdate();
					filltree(commentList, treeView1.Nodes[0].Nodes);
					treeView1.EndUpdate();
					comboBoxNutzer.Items.Clear();
					foreach(var nutzer in _userNames)
					{
						comboBoxNutzer.Items.Add(nutzer.Key);
					}
					int idx = comboBoxNutzer.Items.IndexOf("Russophilus");
					if(idx != -1)
					{
						comboBoxNutzer.Items.Insert(1, "Russophilus");
#if false
						comboBoxNutzer.SelectedIndex = 1;
#endif
					}
					if(oldUser != null)
					{
						int index = comboBoxNutzer.FindStringExact(oldUser);
						if(index != -1)
							comboBoxNutzer.SelectedIndex = index;
						else
							comboBoxNutzer.SelectedIndex = -1;
					}
					if(expanded)
						expand();
					else
						collapse();
					if(selectedCommentId != "") // wird bei Artikelwechsel auf "" gesetzt.
						treeView1.SelectedNode = findNodeById(ref selectedCommentId, treeView1.Nodes);
					else
					{
						richTextBox1.Clear();
						treeView1.SelectedNode = treeView1.Nodes?[0];
					}
					//expandButton.Select();
				}
				UseWaitCursor = false;
				enableAll(true);
				richTextBox1.Focus();
			}
		}
		private void filltree(AngleSharp.Dom.IElement subList, TreeNodeCollection nodelist)
		{
			var items = subList.Children;
			foreach(var item in items)
			{
				entry ent = new entry();
				//ent.name = item.QuerySelector("cite.fn").TextContent;
				var fn = item.QuerySelector(".fn");
				if(fn == null)
					continue;
				ent.name = fn.TextContent;
				//ent.id = item.QuerySelector(".social-comment-inner").Id;
				ent.id = item.QuerySelector(".comment-body").Id;//.Trim();
				//ent.when = item.QuerySelector(".social-posted-when").TextContent;
				ent.when = item.QuerySelector(".comment-metadata time").TextContent.Trim();
				//ent.link = item.QuerySelector(".social-posted-when").GetAttribute("href");
				ent.link = item.QuerySelector(".comment-metadata a").GetAttribute("href");
				//ent.text = item.QuerySelector(".social-comment-body").TextContent;
				ent.text = item.QuerySelector(".comment-content").TextContent;
				ent.text = ent.text.Trim().Replace("\n", "\r\n\r\n");
				TreeNode tn = new TreeNode(ent.name + "  -  " + ent.when);
				tn.Tag = ent;
				if(seenSet.Contains(ent.id))
					markTreeNode(tn);
				if(checkedSet.Contains(ent.id))
					tn.Checked = true;

				nodelist.Add(tn);
				if(!_userNames.ContainsKey(ent.name))
					_userNames.Add(ent.name, new System.Collections.Generic.List<TreeNode>());
				_userNames[ent.name].Add(tn);
				var sublist = item.QuerySelector("ol"); // Unterliste?
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
			Properties.Settings.Default.location = Location;
			Properties.Settings.Default.size = Size;
			Properties.Settings.Default.splitterDistance = splitContainer1.SplitterDistance;
			if(treeView1.Nodes.Count > 0 && _url != "")
			{
				saveState();
			}
		}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			treeView1.SelectedNode.NodeFont = regularFont;
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
				p.Text = "Lade Artikelliste...";
				p.Show(this);
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
					this.textBoxUrl.Text = _url;
				}
			}
			catch(Exception ex)
			{
				_url = "";
				this.textBoxUrl.Text = "";
			}
			return _url;
		}

		void restoreUrl(Progress p)
		{
			p.Text = "Lade Artikel...";
			if(_url == "")
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
				comboBoxArticles.SelectedIndex = findInCombobox(_url);
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
			updateText(); // beim Zuklappen wechselt u.U. der Node
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

		private void richTextBox1_MouseEnter(object sender, EventArgs e)
		{
			richTextBox1.Select();
		}

		private void treeView1_MouseEnter(object sender, EventArgs e)
		{
			treeView1.Select();
		}

		private void comboBoxArticles_MouseDown(object sender, MouseEventArgs e)
		{
			comboBoxArticles.DroppedDown = true;
		}

		private void loadButton_MouseEnter(object sender, EventArgs e)
		{
			loadButton.Select();
		}

		private void loadButton_MouseLeave(object sender, EventArgs e)
		{
			BringToFront();
			richTextBox1.Select();
			//Select(true, true);
		}

		private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
		{
			if(e.Node.Checked)
				checkedSet.Add(((entry)e.Node.Tag).id);
			else
				checkedSet.Remove(((entry)e.Node.Tag).id);
		}

		private void Form1_KeyDown(object sender, KeyEventArgs e)
		{
			if(e.KeyCode == Keys.F5)
			{
				e.Handled = true;
				loadButton_Click(null, null);
			}
		}
		System.Collections.Generic.List<TreeNode> _lastSelectedUserList = new System.Collections.Generic.List<TreeNode>();
		private void comboBoxNutzer_SelectedIndexChanged(object sender, EventArgs e)
		{
			foreach(TreeNode node in _lastSelectedUserList)
			{
				node.BackColor = treeView1.BackColor;
				node.NodeFont = regularFont;
			}
			_lastSelectedUserList = _userNames[(string)comboBoxNutzer.SelectedItem];
			foreach(TreeNode node in _lastSelectedUserList)
			{
				node.BackColor = Color.PeachPuff;
				if(!((entry)node.Tag).seen)
					node.NodeFont = underlineFont;
			}
		}

		private void comboBoxNutzer_MouseDown(object sender, MouseEventArgs e)
		{
			//comboBoxNutzer.DroppedDown = true;
		}

		private void comboBoxNutzer_KeyDown(object sender, KeyEventArgs e)
		{
			if(e.KeyCode == Keys.Enter)
			{
				//e.SuppressKeyPress = true;
				e.Handled = true;
				//comboBoxNutzer.DroppedDown = false;
				//treeView1.Select();
			}
		}

		private void buttonSuche_Click(object sender, EventArgs e)
		{
			textBoxSuche.Text = textBoxSuche.Text.Trim();
			string suchtext = textBoxSuche.Text.ToLower();
			if(treeView1.Nodes.Count == 0)
				return;
			TVEnumerable enu = new TVEnumerable(treeView1.Nodes[0].Nodes);
			//var nodeFont = treeView1.Nodes[0].NodeFont;
			//var nodeFont = treeView1.Font;
			//Font regularFont = new Font(nodeFont, FontStyle.Regular);

			if(textBoxSuche.Text == "")
			{
				foreach(TreeNode node in enu)
					node.NodeFont = regularFont;
				return;
			}
			//Font boldFont = new Font(nodeFont, FontStyle.Bold);
			foreach(TreeNode node in enu)
			{
				var lowercase = ((entry)node.Tag).text.ToLower();
				if(lowercase.Contains(suchtext))
				{
					node.NodeFont = boldFont;
					node.Text = node.Text;
				}
				else
					node.NodeFont = regularFont;
			}
		}

		private void textBoxSuche_KeyPress(object sender, KeyPressEventArgs e)
		{
			if(e.KeyChar == '\r')
			{
				e.Handled = true;
				buttonSuche_Click(null, null);
			}
		}
	}

	//##############################################################################################
	//##############################################################################################
	//##############################################################################################
	//##############################################################################################
	//##############################################################################################
	//##############################################################################################
	//##############################################################################################
	//##############################################################################################
	//##############################################################################################

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
	class TVEnumerable : IEnumerable<TreeNode>
	{
		TreeNodeCollection _nodes;
		public TVEnumerable(TreeNodeCollection nodes)
		{
			_nodes = nodes;
		}
		public IEnumerator<TreeNode> GetEnumerator()
		{
			return new TVEnumerator(_nodes);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new TVEnumerator(_nodes);
		}
	}

	class TVEnumerator : IEnumerator<TreeNode>
	{
		System.Collections.Generic.List<TreeNode> _nodes = new System.Collections.Generic.List<TreeNode>();
		int _pos = -1;
		public TVEnumerator(TreeNodeCollection nodes)
		{
			iterate(nodes);
		}
		void iterate(TreeNodeCollection nodes)
		{
			foreach(TreeNode node in nodes)
			{
				_nodes.Add(node);
				iterate(node.Nodes);
			}
		}
		public TreeNode Current
		{
			get
			{
				return _nodes[_pos];
			}
		}
		TreeNode IEnumerator<TreeNode>.Current
		{
			get
			{
				return Current;
			}
		}

		object IEnumerator.Current
		{
			get
			{
				return Current;
			}
		}

		public void Dispose()
		{
			_nodes = null;
		}

		public bool MoveNext()
		{
			return (++_pos < _nodes.Count);
		}

		public void Reset()
		{
			_pos = -1;
		}
	}
}
