using System;
using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
using System.Drawing;
//using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;

using AngleSharp;

using System.Reflection;  // for Version
using System.Diagnostics; // for Version
using System.Collections;

namespace KommentarLeser
{
	public partial class Form1 : Form
	{
		private class MyWebClient : System.Net.WebClient
		{
			protected override System.Net.WebRequest GetWebRequest(Uri uri)
			{
				System.Net.WebRequest w = base.GetWebRequest(uri);
				w.Timeout = (120 * 1000);
				return w;
			}
		}
		private AngleSharp.Parser.Html.HtmlParser parser;
		private string _url;
		private string html;
		//private string urlFileName = "";
		private string progOptionPath;
		private string _version = "";
		private AngleSharp.Dom.Html.IHtmlDocument doc;
		private AngleSharp.Dom.IElement commentList;
		private System.Collections.Generic.SortedDictionary<string, System.Collections.Generic.List<entry>> _userNames;
		private System.Collections.Generic.Dictionary<string, entry> _id2entry;
		private bool expanded = false;

		private List<entry> entryList = new List<entry>();
		private SortedSet<string> seenSet = new SortedSet<string>();
		private SortedSet<string> checkedSet = new SortedSet<string>();
		private string selectedCommentId = "";
		//private System.Net.WebClient webClient = new System.Net.WebClient();
		private MyWebClient webClient = new MyWebClient();

		private bool _handleEvents = true;

		private Font regularFont;
		private Font boldFont;
		private Font underlineFont;
		private Font underlineBoldFont;

		public Form1()
		{
			InitializeComponent();
			regularFont = treeView1.Font;
			boldFont = new Font(regularFont, FontStyle.Bold);
			// underline aus Ästhetischen Gründen raus
			//underlineFont = new Font(regularFont, FontStyle.Underline);
			underlineFont = regularFont;
			underlineBoldFont = new Font(regularFont, FontStyle.Underline | FontStyle.Bold);

			parser = new AngleSharp.Parser.Html.HtmlParser();
#if DEBUG
			//textBoxUrl.Text = @"http://vineyardsaker.de/analyse/die-spaltung-der-linken-ganz-im-sinne-der-herrschenden/";
			//textBoxUrl.Text = @"http://vineyardsaker.de/analyse/der-gescheiterte-putsch-in-der-tuerkei-einige-erste-gedanken/";
#endif
			progOptionPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			progOptionPath += @"\KommentarLeser\";
			if(!System.IO.Directory.Exists(progOptionPath))
				System.IO.Directory.CreateDirectory(progOptionPath);
			initiateSSLTrust();
			webClient.DownloadDataCompleted += new System.Net.DownloadDataCompletedEventHandler(downloadDataCallback);

		}
		//public delegate void DownloadDataCompletedEventHandler(Object sender,
		//														System.Net.DownloadDataCompletedEventArgs e);


		private void Form1_Load(object sender, EventArgs e)
		{
			Assembly assembly = Assembly.GetExecutingAssembly();
			FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
			_version = "v" + fileVersionInfo.ProductVersion;
			Text = "KommentarLeser - " + _version;
			_userNames = new SortedDictionary<string, List<entry>>();
			_id2entry = new Dictionary<string, entry>();
			System.Drawing.Point location = Properties.Settings.Default.location;
			if(location.X < 0)
				location.X = 0;
			if(location.Y < 0)
				location.Y = 0;
			Location = location;

			System.Drawing.Size size = Properties.Settings.Default.size;
			if(size.Height < 375)
				size.Height = 375;
			if(size.Width < 785)
				size.Width = 785;
			Size = size;

			treeSplitContainer.Panel1MinSize = 100;
			treeSplitContainer.Panel2MinSize = 100;
			mainSplitContainer.Panel1MinSize = 100;
			mainSplitContainer.Panel2MinSize = 100;
			if(Properties.Settings.Default.splitterDistance >= mainSplitContainer.Panel1MinSize
				&& Properties.Settings.Default.splitterDistance <= mainSplitContainer.Width - mainSplitContainer.Panel2MinSize)
				mainSplitContainer.SplitterDistance = Properties.Settings.Default.splitterDistance;
			else
				mainSplitContainer.SplitterDistance = mainSplitContainer.Panel1MinSize;

			if(Properties.Settings.Default.treeSplitterDistance >= treeSplitContainer.Panel1MinSize 
				&& Properties.Settings.Default.treeSplitterDistance <= treeSplitContainer.Height - treeSplitContainer.Panel2MinSize)
				treeSplitContainer.SplitterDistance = Properties.Settings.Default.treeSplitterDistance;
			else
				treeSplitContainer.SplitterDistance = treeSplitContainer.Panel1MinSize;
			enableAll(false);
		}
		private void initiateSSLTrust()
		{
			try
			{
				//Change SSL checks so that all checks pass
				System.Net.ServicePointManager.ServerCertificateValidationCallback =
				   new System.Net.Security.RemoteCertificateValidationCallback(
						delegate {
							return true;
						}
					);
			}
			catch(Exception ex)
			{
				MessageBox.Show(this, ex.Message);
			}
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
		private byte[] downloadBytes;
		private bool downloadOK;
		private bool downloadDone;
		void downloadDataCallback(Object Sender, System.Net.DownloadDataCompletedEventArgs e)
		{
			if(e.Error != null)
				downloadOK = false;
			else
			{
				downloadBytes = e.Result;
				downloadOK = true;
			}
			downloadDone = true;
		}
		private void readArticleList(Progress p)
		{
			string num = "1";
			try
			{
				string mainUrl = "https://vineyardsaker.de";
				for(int i = 0; i < 5; ++i)
				{
					Uri mainUri = new Uri(mainUrl);
					byte[] bytes = new byte[0]; // pro forma, doof
					p.set(num);
					//var prox = webClient.Proxy;
					//webClient.Proxy = System.Net.
					webClient.Proxy = System.Net.WebRequest.GetSystemWebProxy();
					//bytes = webClient.DownloadData(mainUrl);
					webClient.DownloadDataAsync(mainUri);
					for(int ii = 0; ii < _loadTimeoutIntervalCount; ++ii)
					{
						Application.DoEvents();
						if(downloadDone)
						{
							downloadDone = false;
							if(downloadOK)
							{
								bytes = downloadBytes;
								downloadBytes = null;
							}
							break;
						}
						System.Threading.Thread.Sleep(_loadTimeoutInterval);
					}
					if(!downloadOK)
					{
						webClient.CancelAsync();
						throw new Exception("Webserver hat nach " + (int)(_loadTimeoutIntervalCount * (_loadTimeoutInterval / 1000.0)) + " Sekunden nicht geantwortet.");
					}
					else
					{
						downloadOK = false;
						string html = Encoding.UTF8.GetString(bytes);
						var doc = parser.Parse(html);
						var list = doc.QuerySelectorAll("h1.entry-title");
						foreach(var h1entry in list)
						{
							Form1.articleEntry ent = new Form1.articleEntry();
							ent.name = h1entry.TextContent;
							ent.setUrl(h1entry.QuerySelector("a").GetAttribute("href"));
							comboBoxArticles.Items.Add(ent);
						}
						num = (i + 2).ToString();
						mainUrl = "https://vineyardsaker.de/page/" + num;
#if DEBUG
						if(i == 1)
							break; // im DEBUG-Modus nur zwei Seiten Laden
#endif
					}
				}
			}
			catch(Exception ex)
			{
				int nnum;
				int.TryParse(num, out nnum);
				if(nnum == 1)
					MessageBox.Show(this, "Fehler beim Lesen der Artikelliste\n" + ex.Message);
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
			public string timestamp;
			public string link;
			public string text;
			public bool seen = false;
			public bool isInSearch = false;
			public bool isUserSelected = false;
			public TreeNode tn;
			public ListViewItem lvi;
			public override string ToString()
			{
				return name + "  -  " + when;
			}
			public static int Compare(entry a, entry b)
			{
				int res = b.timestamp.CompareTo(a.timestamp);
				return res;
			}
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
		}
		void updateText(entry ent)
		{
			if(ent != null)
			{
				richTextBox1.Clear();
				richTextBox1.Text = ent.text;
				markEntryAsSeen(ent, true);
			}
		}
// 		void treeUpdateText()
// 		{
// 			TreeNode tn = treeView1.SelectedNode;
// 			entry ent = (entry)tn?.Tag;
// 			if(ent != null)
// 			{
// 				richTextBox1.Clear();
// 				richTextBox1.Text = ent.text;
// 				markEntryAsSeen(ent);
// 			}
// 		}
// 		void listUpdateText()
// 		{
// 			ListViewItem lvi = listView1.SelectedItems[0];// .SelectedItem;
// 			entry ent = (entry)lvi?.Tag;
// 			if(ent != null)
// 			{
// 				richTextBox1.Clear();
// 				richTextBox1.Text = ent.text;
// 				markEntryAsSeen(ent);
// 			}
// 		}
		void loadSeenSet()
		{
			string urlFileName = filenameFromUrl(_url);
			// Seit dem Wechsel zu https wurden die alten gelesen/Einträge nicht mehr geladen,
			// da sich der Dateiname geändert hat(von http... zu https...).
			// Daher hier otherFileName, um auch die evtl ander Datei zusätzlich zu laden.
			// SortedSet hat kein Problem mit nehrfachem Add()
			string otherUrlFileName;
			if(urlFileName[4] == 's') // https
			{
				otherUrlFileName = "http" + urlFileName.Substring(5, urlFileName.Length - 5);
			}
			else // http
			{
				otherUrlFileName = "https" + urlFileName.Substring(4, urlFileName.Length - 4);
			}
			string loadFile = progOptionPath + urlFileName;
			string otherLoadFile = progOptionPath + otherUrlFileName;
			string[] lines;
			try
			{
				lines = System.IO.File.ReadAllLines(loadFile);
				seenSet.Clear();
				foreach(string id in lines) // TODO: Leerzeilen ignorieren? oder beim Schreiben in saveState()?
				{
					var idd = id.Trim();
					if(!(idd.Length == 0))
						seenSet.Add(idd);
				}
			}
			catch(Exception)
			{
				;
			}
			try
			{
				lines = System.IO.File.ReadAllLines(otherLoadFile);
				foreach(string id in lines) // TODO: Leerzeilen ignorieren? oder beim Schreiben in saveState()?
				{
					var idd = id.Trim();
					if(!(idd.Length == 0))
						seenSet.Add(idd);
				}
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
					listView1.Items.Clear();
					entryList.Clear();
					_id2entry.Clear();
				}
				_userNames.Clear();
				comboBoxNutzer.Items.Clear();
				comboBoxNutzer.SelectedIndex = -1;
				richTextBox1.Clear();
				textBoxSuche.Clear();
				Application.DoEvents();

				setUrl(textBoxUrl.Text);

				byte[] bytes = new byte[0]; // pro forma, doof
				Uri uri = new Uri(textBoxUrl.Text);
				//byte[] bytes = webClient.DownloadData(_url);
				webClient.DownloadDataAsync(uri);
				for(int ii = 0; ii < _loadTimeoutIntervalCount; ++ii)
				{
					Application.DoEvents();
					if(downloadDone)
					{
						downloadDone = false;
						if(downloadOK)
						{
							bytes = downloadBytes;
							downloadBytes = null;
							break;
						}
					}
					System.Threading.Thread.Sleep(_loadTimeoutInterval);
				}
				if(!downloadOK)
				{
					webClient.CancelAsync();
					throw new Exception("Webserver hat nach " + (int)(_loadTimeoutIntervalCount * (_loadTimeoutInterval / 1000.0)) + " Sekunden	nicht geantwortet.");
				}
				downloadOK = false;
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
				ent.tn = tn;
				ent.lvi = new ListViewItem(""); // Dummy!
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
				listView1.Items.Clear();
				entryList.Clear();
				_id2entry.Clear();
				_userNames.Clear();
				comboBoxNutzer.Items.Clear();
				comboBoxNutzer.SelectedIndex = -1;
				MessageBox.Show(this, ex.Message);
			}
			finally
			{
				if(commentList != null) // wir habenm Kommentare
				{
					loadSeenSet(); // Liste der bereits gesehenen IDs laden
					loadCheckedSet(); // Liste der mrkierten IDs laden

					_userNames.Add("-- kein --", new List<entry>());
					treeView1.BeginUpdate();
					listView1.BeginUpdate();
					filltree(commentList, treeView1.Nodes[0].Nodes);
					entryList.Sort(entry.Compare);
					var g = this.CreateGraphics();
					g.PageUnit = GraphicsUnit.Pixel;
					SizeF size = new SizeF(0, 0);
					foreach(var entry in entryList)
					{
						listView1.Items.Add(entry.lvi);
						float width = g.MeasureString(entry.lvi.Text, entry.lvi.Font).Width;
						if(size.Width < width)
							size.Width = width;
					}
					listView1.Columns[0].Width = (int)size.Width;
					treeView1.EndUpdate();
					listView1.EndUpdate();
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
				var fn = item.QuerySelector(".fn");
				if(fn == null)
					continue;
				ent.name = fn.TextContent;
				ent.id = item.QuerySelector(".comment-body").Id;//.Trim();
				ent.when = item.QuerySelector(".comment-metadata time").TextContent.Trim();
				ent.timestamp = item.QuerySelector(".comment-metadata time").GetAttribute("datetime");
				ent.link = item.QuerySelector(".comment-metadata a").GetAttribute("href");
				ent.text = item.QuerySelector(".comment-content").TextContent;
				ent.text = ent.text.Trim().Replace("\n", "\r\n\r\n");

				string eintrag = ent.name + "  -  " + ent.when;
				TreeNode tn = new TreeNode(eintrag);
				tn.Tag = ent;
				tn.Name = ent.id;
				ListViewItem lvi = new ListViewItem(eintrag);
				lvi.Tag = ent;
				lvi.Name = ent.id;
				ent.tn = tn;
				ent.lvi = lvi;
				if(seenSet.Contains(ent.id))
					markEntryAsSeen(ent, true);
				if(checkedSet.Contains(ent.id))
				{
					tn.Checked = true;
					lvi.Checked = true;
				}
				nodelist.Add(tn);
				entryList.Add(ent);
				_id2entry.Add(ent.id, ent); // TODO: muß ich hier auf Duplikate achten? Eigentlich sollte das nicht vorkommen.
				if(!_userNames.ContainsKey(ent.name))
					_userNames.Add(ent.name, new System.Collections.Generic.List<entry>());
				_userNames[ent.name].Add((entry)tn.Tag);
				var sublist = item.QuerySelector("ol"); // Unterliste?
				if(sublist != null)
					filltree(sublist, tn.Nodes);
			}
		}

		private void markEntryAsSeen(entry ent, bool mark)
		{
			if(ent.seen == mark)
				return;
			ent.seen = mark;
			if(mark)
			{
				ent.tn.ForeColor = Color.Red;
				ent.lvi.ForeColor = Color.Red;
			}
			else
			{
				ent.tn.ForeColor = Color.Black;
				ent.lvi.ForeColor = Color.Black;
			}
			setFont(ent);
			seenSet.Add(ent.id);
		}
		void markEntryAsInSearch(entry ent, bool mark)
		{
			if(ent.isInSearch == mark)
				return;
			ent.isInSearch = mark;
			setFont(ent);
		}
		void markEntryAsUserSelected(entry ent, bool mark)
		{
			if(ent.isUserSelected == mark)
				return;
			ent.isUserSelected = mark;
			if(mark)
			{
				ent.tn.BackColor = Color.PeachPuff;
				ent.lvi.BackColor = Color.PeachPuff;
			}
			else
			{
				ent.tn.BackColor = treeView1.BackColor;
				ent.lvi.BackColor = listView1.BackColor;
			}
			setFont(ent);
		}
		void setFont(entry ent)
		{
			if(ent.seen)
			{
				if(ent.isInSearch)
				{
					ent.tn.NodeFont = boldFont;
					ent.lvi.Font = boldFont;
				}
				else
				{
					ent.tn.NodeFont = regularFont;
					ent.lvi.Font = regularFont;
				}
			}
			else // nicht gesehen
			{
				if(ent.isInSearch)
				{
					ent.tn.NodeFont = boldFont;
					ent.lvi.Font = boldFont;
				}
				else // nicht gesehen und nicht in Suche
				{
					if(ent.isUserSelected)
					{
						ent.tn.NodeFont = underlineFont;
						ent.lvi.Font = underlineFont;
					}
					else
					{
						ent.tn.NodeFont = regularFont;
						ent.lvi.Font = regularFont;
					}
				}
			}
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
			Properties.Settings.Default.splitterDistance = mainSplitContainer.SplitterDistance;
			Properties.Settings.Default.treeSplitterDistance = treeSplitContainer.SplitterDistance;
			if(treeView1.Nodes.Count > 0 && _url != "")
			{
				saveState();
			}
			Properties.Settings.Default.Save();
		}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			var node = treeView1.SelectedNode;
			//node.NodeFont = regularFont;
			updateText((entry)node.Tag);
			selectedCommentId = ((entry)node.Tag).id;
			Properties.Settings.Default.lastComment = selectedCommentId;
			Properties.Settings.Default.Save();
			var ent = (entry)node.Tag;
			if(ent.lvi == null)
				return;
			//ent.lvi.EnsureVisible();
			ent.lvi.Selected = true;
		}
		private void listView1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if(listView1.SelectedItems.Count == 0)
				return;
			var lvi = listView1.SelectedItems[0];
			updateText((entry)lvi.Tag);
			selectedCommentId = ((entry)lvi.Tag).id;
			Properties.Settings.Default.lastComment = selectedCommentId;
			Properties.Settings.Default.Save();
			treeView1.SelectedNode = ((entry)lvi.Tag).tn;
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
			if(treeView1.SelectedNode != null)
				updateText((entry)treeView1.SelectedNode.Tag);
			//treeUpdateText(); // beim Zuklappen wechselt u.U. der Node
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
		private void listView1_MouseEnter(object sender, EventArgs e)
		{
			listView1.Select();
		}
		private void loadButton_MouseEnter(object sender, EventArgs e)
		{
			loadButton.Select();
		}


		private void comboBoxArticles_MouseDown(object sender, MouseEventArgs e)
		{
			comboBoxArticles.DroppedDown = true;
		}


		private void loadButton_MouseLeave(object sender, EventArgs e)
		{
			BringToFront();
			richTextBox1.Select();
			//Select(true, true);
		}

		private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
		{
			if(!_handleEvents)
				return;
			entry ent = (entry)e.Node.Tag;
			if(e.Node.Checked)
				checkedSet.Add(ent.id);
			else
				checkedSet.Remove(ent.id);
			_handleEvents = false;
			ent.lvi.Checked = e.Node.Checked;
			_handleEvents = true;
		}
		private void listView1_ItemChecked(object sender, ItemCheckedEventArgs e)
		{
			if(!_handleEvents)
				return;
			entry ent = (entry)e.Item.Tag;
			if(e.Item.Checked)
				checkedSet.Add(ent.id);
			else
				checkedSet.Remove(ent.id);
			_handleEvents = false;
			ent.tn.Checked = e.Item.Checked;
			_handleEvents = true;

		}

		private void Form1_KeyDown(object sender, KeyEventArgs e)
		{
			if(e.KeyCode == Keys.F5)
			{
				e.Handled = true;
				loadButton_Click(null, null);
			}
		}
		System.Collections.Generic.List<entry> _lastSelectedUserList = new System.Collections.Generic.List<entry>();
		private int _loadTimeoutIntervalCount = 240;
		private int _loadTimeoutInterval = 250;

		private void comboBoxNutzer_SelectedIndexChanged(object sender, EventArgs e)
		{
			foreach(entry ent in _lastSelectedUserList)
			{
				markEntryAsUserSelected(ent, false);
// 				ent.tn.BackColor = treeView1.BackColor;
// 				ent.tn.NodeFont = regularFont;
// 				ent.lvi.BackColor = listView1.BackColor;
// 				ent.lvi.Font = regularFont;
			}
			_lastSelectedUserList = _userNames[(string)comboBoxNutzer.SelectedItem];
			foreach(entry ent in _lastSelectedUserList)
			{
				markEntryAsUserSelected(ent, true);
// 				ent.tn.BackColor = Color.PeachPuff;
// 				ent.lvi.BackColor = Color.PeachPuff;
// 				if(!ent.seen)
// 				{
// 					ent.tn.NodeFont = underlineFont;
// 					ent.lvi.Font = underlineFont;
// 				}
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
			
			if(textBoxSuche.Text == "")
			{
				foreach(entry ent in entryList)
				{
					markEntryAsInSearch(ent, false);
				}
				return;
			}
			foreach(entry ent in entryList)
			{
				var lowercase = ent.text.ToLower();
				if(lowercase.Contains(suchtext))
				{
					markEntryAsInSearch(ent, true);
				}
				else
				{
					markEntryAsInSearch(ent, false);
				}
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

		private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			var lvi = listView1.SelectedItems[0];
			if(lvi != null)
			{
				lvi.Checked = !lvi.Checked;
				System.Diagnostics.Process.Start(((entry)(lvi.Tag)).link);
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
