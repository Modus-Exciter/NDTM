﻿using System.Windows.Forms;

namespace Notung.Helm.Controls
{
  public partial class InfoBufferView : UserControl
  {
    public InfoBufferView()
    {
      InitializeComponent();
    }

    public void SetInfoBuffer(InfoBuffer buffer)
    {
      SetInfoBuffer(buffer, m_tree.Nodes);
    }

    private void SetInfoBuffer(InfoBuffer buffer, TreeNodeCollection nodes)
    {
      foreach (var info in buffer)
      {
        var node = nodes.Add(info.Message);
        node.ImageKey = info.Level.ToString();
        node.SelectedImageKey = node.ImageKey;

        node.Tag = info.Details;

        SetInfoBuffer(info.InnerMessages, node.Nodes);

        node.Expand();
      }
    }
  }
}
