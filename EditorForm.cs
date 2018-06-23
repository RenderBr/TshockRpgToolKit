﻿using CustomNpcsEdit.Controls;
using CustomNpcsEdit.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CustomNpcsEdit
{
	internal partial class EditorForm : Form
	{
		List<ObjectEditor> objectEditors;
		InvasionEditor invasionsEditor;
		NpcEditor npcsEditor;
		ProjectileEditor projectilesEditor;

		NpcShopsEditor npcShopsEditor;

		public EditorForm()
		{
			InitializeComponent();
			
			invasionsEditor = (InvasionEditor)tabControlMain.TabPages[0].Controls[0];
			invasionsEditor.OpenFileDialog = openFileDialogNpcs;
			invasionsEditor.SaveFileDialog = saveFileDialogNpcs;
			
			npcsEditor = (NpcEditor)tabControlMain.TabPages[1].Controls[0];
			npcsEditor.OpenFileDialog = openFileDialogNpcs;
			npcsEditor.SaveFileDialog = saveFileDialogNpcs;
			
			projectilesEditor = (ProjectileEditor)tabControlMain.TabPages[2].Controls[0];
			projectilesEditor.OpenFileDialog = openFileDialogProjectiles;
			projectilesEditor.SaveFileDialog = saveFileDialogProjectiles;

			npcShopsEditor = (NpcShopsEditor)tabControlMain.TabPages[3].Controls[0];
			npcShopsEditor.OpenFileDialog = openFileDialogProjectiles;
			npcShopsEditor.SaveFileDialog = saveFileDialogProjectiles;
			npcShopsEditor.CanAddCategory = false;
			npcShopsEditor.SupportMultipleItems = false;
			
			objectEditors = new List<ObjectEditor>()
			{
				invasionsEditor,
				npcsEditor,
				projectilesEditor,
				npcShopsEditor
			};

			foreach( var editor in objectEditors )
			{
				//refresh on property change
				editor.PropertyChanged += (s, a) =>
				{
					//...but only if its the currently selected tab.
					var selectedIndex = tabControlMain.SelectedIndex;

					if( s == tabControlMain.TabPages[selectedIndex].Controls[0] )
						refreshObjectEditorExternalDisplay(selectedIndex);
				};
			}
			
			//start on projectiles page for now...
			tabControlMain.SelectedIndex = 3;
		}
		
		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void EditorForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			var unsavedData = objectEditors.FirstOrDefault(ed => ed.IsTreeDirty) != null;

			if(unsavedData)
			{
				var result = MessageBox.Show("There are unsaved changes present. Are you sure you want to exit?", "Unsaved Data", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

				if( result != DialogResult.OK )
				{
					e.Cancel = true;
				}
			}
		}

		private void refreshObjectEditorExternalDisplay(int selectedIndex)
		{
			var editor = objectEditors[selectedIndex];
			var value = editor.Caption;

			Text = $"CustomNpcsEdit - {value}";
		}

		private void tabControlMain_SelectedIndexChanged(object sender, EventArgs e)
		{
			refreshObjectEditorExternalDisplay(tabControlMain.SelectedIndex);
		}

		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var version = Assembly.GetExecutingAssembly().GetName().Version;
			
			MessageBox.Show($"CustomNpcsEdit {version}",
							"About",
							MessageBoxButtons.OK,
							MessageBoxIcon.Information);
		}

		private void newToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var editor = objectEditors[tabControlMain.SelectedIndex];
			editor.NewFile();
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var editor = objectEditors[tabControlMain.SelectedIndex];
			editor.OpenFile();
		}

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var editor = objectEditors[tabControlMain.SelectedIndex];
			editor.SaveFileAs();
		}
	}
}
