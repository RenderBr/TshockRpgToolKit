﻿using RpgToolsEditor.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace RpgToolsEditor.Controls
{
	public partial class ModelTreeEditor : UserControl, INotifyPropertyChanged
	{
		string currentFilePath = "";
		public string CurrentFilePath
		{
			get => currentFilePath;
			set
			{
				currentFilePath = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentFilePath)));
			}
		}

		public bool HasCurrentFilePath => !string.IsNullOrWhiteSpace(CurrentFilePath);

		bool isTreeDirty;
		public bool IsTreeDirty
		{
			get => isTreeDirty;
			set 
			{
				isTreeDirty = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsTreeDirty)));
			}
		}

		public string Caption => CurrentFilePath;// + ( IsTreeDirty ? "*" : "" );

		public OpenFileDialog OpenFileDialog { get; set; }
		public SaveFileDialog SaveFileDialog { get; set; }
		public ModelTree ModelTree { get; set; }

		protected PropertyGrid PropertyGrid => propertyGridItemEditor;

		public event PropertyChangedEventHandler PropertyChanged;

		public ModelTreeEditor()
		{
			InitializeComponent();

			//removing this, really slows down file loads for some reason...
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
		}
		
		public void Clear()
		{
			propertyGridItemEditor.SelectedObject = null;
			//listBoxItems.DataSource = null;

			treeViewItems.Nodes.Clear();

			IsTreeDirty = false;
			//toolStripLabelFileName.Text = "";
			CurrentFilePath = "";
		}

		public void CreateItem()
		{
			var selectedNode = treeViewItems.SelectedNode as ModelTreeNode;

			if(selectedNode!=null)
			{
				if( selectedNode.CanAddChild )
				{
					selectedNode.AddItem();
					IsTreeDirty = true;
					return;
				}
			}
			
			var defaultNode = ModelTree.CreateDefaultItem();
			treeViewItems.Nodes.Add(defaultNode);
			IsTreeDirty = true;
		}

		public void CopySelectedItem()
		{
			var selectedNode = treeViewItems.SelectedNode as ModelTreeNode;

			if( selectedNode != null )
			{
				if( selectedNode.CanCopy )
				{
					var copy = selectedNode.Copy();

					copy.Model.TryAddCopySuffix();
					
					selectedNode.InsertAfter(copy);
					IsTreeDirty = true;
				}
			}
		}

		public void DeleteSelectedItem()
		{
			var selectedNode = treeViewItems.SelectedNode as ModelTreeNode;

			if( selectedNode != null )
			{
				if( selectedNode.CanDelete )
				{
					selectedNode.Remove();
					IsTreeDirty = true;
				}
			}
		}
		
		protected virtual void OnFileLoad(string fileName)
		{
			var nodes = ModelTree.LoadTree(fileName);
			SetTreeViewModels(nodes);
		}

		protected virtual void OnFileSave(string fileName)
		{
			throw new NotImplementedException();
		}
				
		protected void SetTreeViewModels<T>(IList<T> boundTreeNodes) where T : ModelTreeNode
		{
			treeViewItems.Nodes.Clear();

			foreach( var bt in boundTreeNodes )
			{
				treeViewItems.Nodes.Add(bt);
			}
		}

		protected IList<ModelTreeNode> GetTreeViewModels()
		{
			var nodes = treeViewItems.Nodes;
			var modelTreeNodes = nodes.Cast<ModelTreeNode>().ToList();
			return modelTreeNodes;
		}
		
		public void NewFile()
		{
			if( IsTreeDirty )
			{
				var result = MessageBox.Show("There are unsaved changes present. Proceed?", "Delete Items?", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

				if( result != DialogResult.OK )
					return;
			}

			Clear();
			
			//if(!SupportMultipleItems)
			//{
			//	//we're in single item mode, so create a default item...
			//	var item = OnCreateItem();
			//	propertyGridItemEditor.SelectedObject = item;
			//}
		}

		private void toolStripButtonNewFile_Click(object sender, EventArgs e)
		{
			NewFile();
		}
		
		private void toolStripButtonAddItem_Click(object sender, EventArgs e)
		{
			//var selectedNode = treeViewItems.SelectedNode as ModelTreeNode;
			CreateItem();


			//var node = new BoundTreeNode();

			//if we've already selected a node, what type of model to create, and how to add it?
			//if(selectedNode!=null)
			//{
			//	var boundNode = (BoundTreeNode)selectedNode;
			//	var selectedModel = boundNode.BoundObject;

			//	if(selectedModel is CategoryModel)
			//	{
			//		node.BoundObject = new IncludeModel();
			//		selectedNode.Nodes.Add(node);
			//		selectedNode.Expand();
			//		//treeViewItems.SelectedNode = node;
			//		IsTreeDirty = true;
			//		return;
			//	}
			//	else if(selectedModel is IncludeModel)
			//	{
			//		node.BoundObject = (IModel)OnCreateItem();
			//		selectedNode.Nodes.Add(node);
			//		selectedNode.Expand();
			//		//treeViewItems.SelectedNode = node;
			//		IsTreeDirty = true;
			//		return;
			//	}
			//}

			//node.BoundObject = (IModel)OnCreateItem();

			////insert or add to root?
			//if( selectedNode != null )
			//{
			//	selectedNode.InsertAfter(node);
			//	IsTreeDirty = true;
			//}
			//else
			//{
			//	treeViewItems.Nodes.Add(node);
			//	IsTreeDirty = true;
			//}

			//treeViewItems.SelectedNode = node;
		}
		
		private void toolStripButtonCopy_Click(object sender, EventArgs e)
		{
			//var index = listBoxItems.SelectedIndex;

			//if( index > -1 && BoundItems!=null )
			//{
			//	var copy = OnCopyItem(listBoxItems.SelectedItem);

			//	BoundItems.Insert(++index,copy);
			//}

			CopySelectedItem();


			//treeview
			//var selectedNode = (BoundTreeNode)treeViewItems.SelectedNode;

			//if(selectedNode!=null && selectedNode.CanCopy())
			//{
			//	var src = ((BoundTreeNode)selectedNode).BoundObject;
			//	var copy = OnCopyItem(src);

			//	var newNode = new BoundTreeNode();
			//	newNode.BoundObject = (IModel)copy;

			//	selectedNode.InsertAfter(newNode);
			//	IsTreeDirty = true;
			//}
		}

		private void toolStripButtonDeleteItem_Click(object sender, EventArgs e)
		{
			var selectedNode = treeViewItems.SelectedNode;

			if(selectedNode!=null)
			{
				treeViewItems.Nodes.Remove(selectedNode);
				IsTreeDirty = true;
			}
		}

		public void OpenFile()
		{
			if( IsTreeDirty )
			{
				var confirm = MessageBox.Show("There are unsaved changes present. This will replace the current data. Proceed?",
												"Unsaved Data",
												MessageBoxButtons.OKCancel,
												MessageBoxIcon.Warning);

				if( confirm == DialogResult.Cancel )
					return;
			}

			var result = OpenFileDialog.ShowDialog();

			if( result == DialogResult.OK )
			{
				try
				{
					Clear();
					OnFileLoad(OpenFileDialog.FileName);
					//toolStripLabelFileName.Text = OpenFileDialog.FileName;
					IsTreeDirty = false;
					CurrentFilePath = OpenFileDialog.FileName;
				}
				catch( Exception ex )
				{
					MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		private void toolStripButtonFileOpen_Click(object sender, EventArgs e)
		{
			OpenFile();
		}

		public void SaveFileAsImpl(string fileName)
		{
			try
			{
				OnFileSave(fileName);
				IsTreeDirty = false;
				CurrentFilePath = fileName; // SaveFileDialog.FileName;
			}
			catch( Exception ex )
			{
				MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		public void SaveFileAs()
		{
			var result = SaveFileDialog.ShowDialog();

			if( result == DialogResult.OK )
			{
				SaveFileAsImpl(SaveFileDialog.FileName);
			}
		}

		public void SaveFile()
		{
			if( string.IsNullOrWhiteSpace(CurrentFilePath) )
				SaveFileAs();
			else
				SaveFileAsImpl(CurrentFilePath);
		}

		private void toolStripButtonFileSave_Click(object sender, EventArgs e)
		{
			SaveFile();
		}

		private void toolStripButtonFileSaveAs_Click(object sender, EventArgs e)
		{
			SaveFileAs();
		}
		
		private void treeViewItems_AfterSelect(object sender, TreeViewEventArgs e)
		{
			var selected = (ModelTreeNode)e.Node;
			object target;

			if( selected.CanEditModel )
				target = selected.Model;
			else
				target = null;

			propertyGridItemEditor.SelectedObject = target;
		}

		private void treeViewItems_ItemDrag(object sender, ItemDragEventArgs e)
		{
			DoDragDrop(e.Item, DragDropEffects.Move);
		}

		private void treeViewItems_DragEnter(object sender, DragEventArgs e)
		{
			e.Effect = DragDropEffects.Move;
		}

		private void treeViewItems_DragDrop(object sender, DragEventArgs e)
		{
			// Retrieve the client coordinates of the drop location.
			Point targetPoint = treeViewItems.PointToClient(new Point(e.X, e.Y));

			// Retrieve the node at the drop location.
			BoundTreeNode targetNode = (BoundTreeNode)treeViewItems.GetNodeAt(targetPoint);

			// Retrieve the node that was dragged.
			BoundTreeNode draggedNode = (BoundTreeNode)e.Data.GetData(typeof(BoundTreeNode));

			// Confirm that the node at the drop location is not 
			// the dragged node and that target node isn't null
			// (for example if you drag outside the control)
			if( !draggedNode.Equals(targetNode) && targetNode != null )
			{
				if(targetNode.CanAcceptDraggedNode(draggedNode))
				{
					// Remove the node from its current 
					// location and add it to the node at the drop location.
					draggedNode.Remove();
					//targetNode.Nodes.Add(draggedNode);

					if(targetNode.ShouldInsertDraggedNodeAsChild(draggedNode))
					{
						targetNode.Nodes.Add(draggedNode);
						// Expand the node at the location 
						// to show the dropped node.
						targetNode.Expand();
						IsTreeDirty = true;
					}
					else
					{
						targetNode.InsertAfter(draggedNode);
						IsTreeDirty = true;
					}
				}
			}
			else if(targetNode == null)
			{
				//were dropping at top level... ie, the treeview itself
				if( draggedNode.BoundObject is IncludeModel )
					return;//includes can only live within categories.

				draggedNode.Remove();
				treeViewItems.Nodes.Add(draggedNode);
				IsTreeDirty = true;
			}
		}
		
		private void propertyGridItemEditor_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			IsTreeDirty = true;
		}
	}
}
