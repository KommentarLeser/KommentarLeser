using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KommentarLeser
{
	public partial class Progress : Form
	{
		public Progress()
		{
			InitializeComponent();
			label1.Text = "";
		}
		public void set(string text)
		{
			label1.Text = text;
			Application.DoEvents();
		}
	}
}
