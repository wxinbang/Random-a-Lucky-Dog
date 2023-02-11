﻿using CommunityToolkit.Mvvm.Input;
using Microsoft.Xaml.Interactivity;
using System.Collections.Generic;
using System.Windows.Input;
using WinUI = Microsoft.UI.Xaml.Controls;

namespace Select_Lucky_Dog.Behaviors
{
	public class TreeViewCollapseBehavior : Behavior<WinUI.TreeView>
	{
		public ICommand CollapseAllCommand { get; }

		public TreeViewCollapseBehavior()
		{
			CollapseAllCommand = new RelayCommand(() => CollapseNodes(AssociatedObject.RootNodes));
		}

		private void CollapseNodes(IList<WinUI.TreeViewNode> nodes)
		{
			foreach (var node in nodes)
			{
				CollapseNodes(node.Children);
				AssociatedObject.Collapse(node);
			}
		}
	}
}