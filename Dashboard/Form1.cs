using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace Dashboard
{
    public partial class Dashboard : Form
    {
        public Dashboard()
        {
            InitializeComponent();
        }

        /// <summary>
        /// takes a dictionary of links and adds them to the ResourceLinks control.
        /// </summary>
        /// <param name="links">key = link caption; value = url</param>
        private void SetLinks(SortedDictionary<String, String> links)
        {
            ResourceLinks.Text = "";
            ResourceLinks.Links.Clear();
            foreach (KeyValuePair<String, String> link in links)
            {
                int startIndex = ResourceLinks.Text.Length;
                ResourceLinks.Text += link.Key + "\n";
                ResourceLinks.Links.Add(startIndex, link.Key.Length, link.Value);
            }
        }

        private void ResourceLinks_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ResourceLinks.Links[ResourceLinks.Links.IndexOf(e.Link)].Visited = true;
            string target = e.Link.LinkData as string;
            if (null != target && (target.Contains("://") || target.Contains("www.")))
            {
                Process.Start(target);
            }
        }

    }
}
