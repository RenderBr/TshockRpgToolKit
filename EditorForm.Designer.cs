﻿namespace RpgToolsEditor
{
	partial class EditorForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if( disposing && ( components != null ) )
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditorForm));
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
			this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
			this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.tabControlMain = new System.Windows.Forms.TabControl();
			this.tabPageInvasions = new System.Windows.Forms.TabPage();
			this.invasionEditor1 = new RpgToolsEditor.Controls.InvasionEditor();
			this.tabPageNpcs = new System.Windows.Forms.TabPage();
			this.npcEditor1 = new RpgToolsEditor.Controls.NpcEditor();
			this.tabPageProjectiles = new System.Windows.Forms.TabPage();
			this.modelTreeEditor4 = new RpgToolsEditor.Controls.ModelTreeEditor();
			this.tabPageNpcShopsX = new System.Windows.Forms.TabPage();
			this.modelTreeEditor2 = new RpgToolsEditor.Controls.ModelTreeEditor();
			this.tabPageLevelingX = new System.Windows.Forms.TabPage();
			this.modelTreeEditor3 = new RpgToolsEditor.Controls.ModelTreeEditor();
			this.tabPageQuestsX = new System.Windows.Forms.TabPage();
			this.modelTreeEditor1 = new RpgToolsEditor.Controls.ModelTreeEditor();
			this.imageListTabIcons = new System.Windows.Forms.ImageList(this.components);
			this.openFileDialogProjectiles = new System.Windows.Forms.OpenFileDialog();
			this.saveFileDialogProjectiles = new System.Windows.Forms.SaveFileDialog();
			this.openFileDialogNpcs = new System.Windows.Forms.OpenFileDialog();
			this.saveFileDialogNpcs = new System.Windows.Forms.SaveFileDialog();
			this.openFileDialogNpcShop = new System.Windows.Forms.OpenFileDialog();
			this.saveFileDialogNpcShop = new System.Windows.Forms.SaveFileDialog();
			this.openFileDialogLeveling = new System.Windows.Forms.OpenFileDialog();
			this.saveFileDialogLeveling = new System.Windows.Forms.SaveFileDialog();
			this.openFileDialogQuests = new System.Windows.Forms.OpenFileDialog();
			this.saveFileDialogQuests = new System.Windows.Forms.SaveFileDialog();
			this.openFileDialogInvasions = new System.Windows.Forms.OpenFileDialog();
			this.saveFileDialogInvasions = new System.Windows.Forms.SaveFileDialog();
			this.menuStrip1.SuspendLayout();
			this.tabControlMain.SuspendLayout();
			this.tabPageInvasions.SuspendLayout();
			this.tabPageNpcs.SuspendLayout();
			this.tabPageProjectiles.SuspendLayout();
			this.tabPageNpcShopsX.SuspendLayout();
			this.tabPageLevelingX.SuspendLayout();
			this.tabPageQuestsX.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.helpToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(1008, 24);
			this.menuStrip1.TabIndex = 0;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.toolStripSeparator,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "&File";
			// 
			// newToolStripMenuItem
			// 
			this.newToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("newToolStripMenuItem.Image")));
			this.newToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.newToolStripMenuItem.Name = "newToolStripMenuItem";
			this.newToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
			this.newToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
			this.newToolStripMenuItem.Text = "&New";
			this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
			// 
			// openToolStripMenuItem
			// 
			this.openToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openToolStripMenuItem.Image")));
			this.openToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.openToolStripMenuItem.Name = "openToolStripMenuItem";
			this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
			this.openToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
			this.openToolStripMenuItem.Text = "&Open";
			this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
			// 
			// toolStripSeparator
			// 
			this.toolStripSeparator.Name = "toolStripSeparator";
			this.toolStripSeparator.Size = new System.Drawing.Size(143, 6);
			// 
			// saveToolStripMenuItem
			// 
			this.saveToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripMenuItem.Image")));
			this.saveToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
			this.saveToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
			this.saveToolStripMenuItem.Text = "&Save";
			this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
			// 
			// saveAsToolStripMenuItem
			// 
			this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
			this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
			this.saveAsToolStripMenuItem.Text = "Save &As";
			this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(143, 6);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
			this.exitToolStripMenuItem.Text = "E&xit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// editToolStripMenuItem
			// 
			this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem,
            this.toolStripSeparator3,
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.toolStripSeparator4,
            this.selectAllToolStripMenuItem});
			this.editToolStripMenuItem.Name = "editToolStripMenuItem";
			this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
			this.editToolStripMenuItem.Text = "&Edit";
			// 
			// undoToolStripMenuItem
			// 
			this.undoToolStripMenuItem.Enabled = false;
			this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
			this.undoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
			this.undoToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
			this.undoToolStripMenuItem.Text = "&Undo";
			// 
			// redoToolStripMenuItem
			// 
			this.redoToolStripMenuItem.Enabled = false;
			this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
			this.redoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
			this.redoToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
			this.redoToolStripMenuItem.Text = "&Redo";
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(141, 6);
			// 
			// cutToolStripMenuItem
			// 
			this.cutToolStripMenuItem.Enabled = false;
			this.cutToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("cutToolStripMenuItem.Image")));
			this.cutToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
			this.cutToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
			this.cutToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
			this.cutToolStripMenuItem.Text = "Cu&t";
			// 
			// copyToolStripMenuItem
			// 
			this.copyToolStripMenuItem.Enabled = false;
			this.copyToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("copyToolStripMenuItem.Image")));
			this.copyToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
			this.copyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
			this.copyToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
			this.copyToolStripMenuItem.Text = "&Copy";
			// 
			// pasteToolStripMenuItem
			// 
			this.pasteToolStripMenuItem.Enabled = false;
			this.pasteToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("pasteToolStripMenuItem.Image")));
			this.pasteToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
			this.pasteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
			this.pasteToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
			this.pasteToolStripMenuItem.Text = "&Paste";
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(141, 6);
			// 
			// selectAllToolStripMenuItem
			// 
			this.selectAllToolStripMenuItem.Enabled = false;
			this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
			this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
			this.selectAllToolStripMenuItem.Text = "Select &All";
			// 
			// helpToolStripMenuItem
			// 
			this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator5,
            this.aboutToolStripMenuItem});
			this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			this.helpToolStripMenuItem.Text = "&Help";
			// 
			// toolStripSeparator5
			// 
			this.toolStripSeparator5.Name = "toolStripSeparator5";
			this.toolStripSeparator5.Size = new System.Drawing.Size(113, 6);
			// 
			// aboutToolStripMenuItem
			// 
			this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			this.aboutToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
			this.aboutToolStripMenuItem.Text = "&About...";
			this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
			// 
			// statusStrip1
			// 
			this.statusStrip1.Location = new System.Drawing.Point(0, 659);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(1008, 22);
			this.statusStrip1.TabIndex = 2;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// tabControlMain
			// 
			this.tabControlMain.Controls.Add(this.tabPageInvasions);
			this.tabControlMain.Controls.Add(this.tabPageNpcs);
			this.tabControlMain.Controls.Add(this.tabPageProjectiles);
			this.tabControlMain.Controls.Add(this.tabPageNpcShopsX);
			this.tabControlMain.Controls.Add(this.tabPageLevelingX);
			this.tabControlMain.Controls.Add(this.tabPageQuestsX);
			this.tabControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControlMain.ImageList = this.imageListTabIcons;
			this.tabControlMain.Location = new System.Drawing.Point(0, 24);
			this.tabControlMain.Name = "tabControlMain";
			this.tabControlMain.SelectedIndex = 0;
			this.tabControlMain.Size = new System.Drawing.Size(1008, 635);
			this.tabControlMain.TabIndex = 3;
			this.tabControlMain.SelectedIndexChanged += new System.EventHandler(this.tabControlMain_SelectedIndexChanged);
			this.tabControlMain.TabIndexChanged += new System.EventHandler(this.tabControlMain_SelectedIndexChanged);
			// 
			// tabPageInvasions
			// 
			this.tabPageInvasions.Controls.Add(this.invasionEditor1);
			this.tabPageInvasions.ImageIndex = 0;
			this.tabPageInvasions.Location = new System.Drawing.Point(4, 23);
			this.tabPageInvasions.Name = "tabPageInvasions";
			this.tabPageInvasions.Size = new System.Drawing.Size(1000, 608);
			this.tabPageInvasions.TabIndex = 0;
			this.tabPageInvasions.Text = "Invasions";
			this.tabPageInvasions.UseVisualStyleBackColor = true;
			// 
			// invasionEditor1
			// 
			this.invasionEditor1.CanAddCategory = true;
			this.invasionEditor1.CurrentFilePath = "";
			this.invasionEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.invasionEditor1.IsTreeDirty = false;
			this.invasionEditor1.Location = new System.Drawing.Point(0, 0);
			this.invasionEditor1.Name = "invasionEditor1";
			this.invasionEditor1.OpenFileDialog = null;
			this.invasionEditor1.SaveFileDialog = null;
			this.invasionEditor1.Size = new System.Drawing.Size(1000, 608);
			this.invasionEditor1.SupportMultipleItems = true;
			this.invasionEditor1.TabIndex = 0;
			// 
			// tabPageNpcs
			// 
			this.tabPageNpcs.Controls.Add(this.npcEditor1);
			this.tabPageNpcs.ImageIndex = 1;
			this.tabPageNpcs.Location = new System.Drawing.Point(4, 23);
			this.tabPageNpcs.Name = "tabPageNpcs";
			this.tabPageNpcs.Size = new System.Drawing.Size(1000, 608);
			this.tabPageNpcs.TabIndex = 1;
			this.tabPageNpcs.Text = "NPCs";
			this.tabPageNpcs.UseVisualStyleBackColor = true;
			// 
			// npcEditor1
			// 
			this.npcEditor1.CanAddCategory = true;
			this.npcEditor1.CurrentFilePath = "";
			this.npcEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.npcEditor1.IsTreeDirty = false;
			this.npcEditor1.Location = new System.Drawing.Point(0, 0);
			this.npcEditor1.Name = "npcEditor1";
			this.npcEditor1.OpenFileDialog = null;
			this.npcEditor1.SaveFileDialog = null;
			this.npcEditor1.Size = new System.Drawing.Size(1000, 608);
			this.npcEditor1.SupportMultipleItems = true;
			this.npcEditor1.TabIndex = 0;
			// 
			// tabPageProjectiles
			// 
			this.tabPageProjectiles.Controls.Add(this.modelTreeEditor4);
			this.tabPageProjectiles.ImageIndex = 2;
			this.tabPageProjectiles.Location = new System.Drawing.Point(4, 23);
			this.tabPageProjectiles.Name = "tabPageProjectiles";
			this.tabPageProjectiles.Size = new System.Drawing.Size(1000, 608);
			this.tabPageProjectiles.TabIndex = 2;
			this.tabPageProjectiles.Text = "ProjectilesX";
			this.tabPageProjectiles.UseVisualStyleBackColor = true;
			// 
			// modelTreeEditor4
			// 
			this.modelTreeEditor4.CurrentFilePath = "";
			this.modelTreeEditor4.Dock = System.Windows.Forms.DockStyle.Fill;
			this.modelTreeEditor4.IsTreeDirty = false;
			this.modelTreeEditor4.Location = new System.Drawing.Point(0, 0);
			this.modelTreeEditor4.ModelTree = null;
			this.modelTreeEditor4.Name = "modelTreeEditor4";
			this.modelTreeEditor4.OpenFileDialog = null;
			this.modelTreeEditor4.SaveFileDialog = null;
			this.modelTreeEditor4.Size = new System.Drawing.Size(1000, 608);
			this.modelTreeEditor4.TabIndex = 0;
			// 
			// tabPageNpcShopsX
			// 
			this.tabPageNpcShopsX.Controls.Add(this.modelTreeEditor2);
			this.tabPageNpcShopsX.ImageIndex = 3;
			this.tabPageNpcShopsX.Location = new System.Drawing.Point(4, 23);
			this.tabPageNpcShopsX.Name = "tabPageNpcShopsX";
			this.tabPageNpcShopsX.Size = new System.Drawing.Size(1000, 608);
			this.tabPageNpcShopsX.TabIndex = 7;
			this.tabPageNpcShopsX.Text = "NpcShopsX";
			this.tabPageNpcShopsX.UseVisualStyleBackColor = true;
			// 
			// modelTreeEditor2
			// 
			this.modelTreeEditor2.CurrentFilePath = "";
			this.modelTreeEditor2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.modelTreeEditor2.IsTreeDirty = false;
			this.modelTreeEditor2.Location = new System.Drawing.Point(0, 0);
			this.modelTreeEditor2.ModelTree = null;
			this.modelTreeEditor2.Name = "modelTreeEditor2";
			this.modelTreeEditor2.OpenFileDialog = null;
			this.modelTreeEditor2.SaveFileDialog = null;
			this.modelTreeEditor2.Size = new System.Drawing.Size(1000, 608);
			this.modelTreeEditor2.TabIndex = 0;
			// 
			// tabPageLevelingX
			// 
			this.tabPageLevelingX.Controls.Add(this.modelTreeEditor3);
			this.tabPageLevelingX.ImageIndex = 4;
			this.tabPageLevelingX.Location = new System.Drawing.Point(4, 23);
			this.tabPageLevelingX.Name = "tabPageLevelingX";
			this.tabPageLevelingX.Size = new System.Drawing.Size(1000, 608);
			this.tabPageLevelingX.TabIndex = 8;
			this.tabPageLevelingX.Text = "LevelingX";
			this.tabPageLevelingX.UseVisualStyleBackColor = true;
			// 
			// modelTreeEditor3
			// 
			this.modelTreeEditor3.CurrentFilePath = "";
			this.modelTreeEditor3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.modelTreeEditor3.IsTreeDirty = false;
			this.modelTreeEditor3.Location = new System.Drawing.Point(0, 0);
			this.modelTreeEditor3.ModelTree = null;
			this.modelTreeEditor3.Name = "modelTreeEditor3";
			this.modelTreeEditor3.OpenFileDialog = null;
			this.modelTreeEditor3.SaveFileDialog = null;
			this.modelTreeEditor3.Size = new System.Drawing.Size(1000, 608);
			this.modelTreeEditor3.TabIndex = 0;
			// 
			// tabPageQuestsX
			// 
			this.tabPageQuestsX.Controls.Add(this.modelTreeEditor1);
			this.tabPageQuestsX.ImageIndex = 5;
			this.tabPageQuestsX.Location = new System.Drawing.Point(4, 23);
			this.tabPageQuestsX.Name = "tabPageQuestsX";
			this.tabPageQuestsX.Size = new System.Drawing.Size(1000, 608);
			this.tabPageQuestsX.TabIndex = 6;
			this.tabPageQuestsX.Text = "QuestsX";
			this.tabPageQuestsX.UseVisualStyleBackColor = true;
			// 
			// modelTreeEditor1
			// 
			this.modelTreeEditor1.CurrentFilePath = "";
			this.modelTreeEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.modelTreeEditor1.IsTreeDirty = false;
			this.modelTreeEditor1.Location = new System.Drawing.Point(0, 0);
			this.modelTreeEditor1.ModelTree = null;
			this.modelTreeEditor1.Name = "modelTreeEditor1";
			this.modelTreeEditor1.OpenFileDialog = null;
			this.modelTreeEditor1.SaveFileDialog = null;
			this.modelTreeEditor1.Size = new System.Drawing.Size(1000, 608);
			this.modelTreeEditor1.TabIndex = 0;
			// 
			// imageListTabIcons
			// 
			this.imageListTabIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListTabIcons.ImageStream")));
			this.imageListTabIcons.TransparentColor = System.Drawing.Color.Transparent;
			this.imageListTabIcons.Images.SetKeyName(0, "Goblin-16.png");
			this.imageListTabIcons.Images.SetKeyName(1, "Green_Slime-16.png");
			this.imageListTabIcons.Images.SetKeyName(2, "Shuriken.png");
			this.imageListTabIcons.Images.SetKeyName(3, "Merchant-16.png");
			this.imageListTabIcons.Images.SetKeyName(4, "Wooden_Sword-16.png");
			this.imageListTabIcons.Images.SetKeyName(5, "Skeletron_Head-16.png");
			// 
			// openFileDialogProjectiles
			// 
			this.openFileDialogProjectiles.Filter = "Json files|*.json|All files|*.*";
			this.openFileDialogProjectiles.Title = "Open Custom Projectiles";
			// 
			// saveFileDialogProjectiles
			// 
			this.saveFileDialogProjectiles.Filter = "Json files|*.json|All files|*.*";
			this.saveFileDialogProjectiles.SupportMultiDottedExtensions = true;
			this.saveFileDialogProjectiles.Title = "Save Custom Projectiles";
			// 
			// openFileDialogNpcs
			// 
			this.openFileDialogNpcs.Filter = "Json files|*.json|All files|*.*";
			this.openFileDialogNpcs.Title = "Open Custom NPCs";
			// 
			// saveFileDialogNpcs
			// 
			this.saveFileDialogNpcs.Filter = "Json files|*.json|All files|*.*";
			this.saveFileDialogNpcs.SupportMultiDottedExtensions = true;
			this.saveFileDialogNpcs.Title = "Save Custom NPCs";
			// 
			// openFileDialogNpcShop
			// 
			this.openFileDialogNpcShop.Filter = "Shop files|*.shop|Json files|*.json|All files|*.*";
			this.openFileDialogNpcShop.Title = "Open Npc Shop";
			// 
			// saveFileDialogNpcShop
			// 
			this.saveFileDialogNpcShop.Filter = "Shop files|*.shop|Json files|*.json|All files|*.*";
			this.saveFileDialogNpcShop.SupportMultiDottedExtensions = true;
			this.saveFileDialogNpcShop.Title = "Save Npc Shop";
			// 
			// openFileDialogLeveling
			// 
			this.openFileDialogLeveling.Filter = "Class files|*.class|Json files|*.json|All files|*.*";
			this.openFileDialogLeveling.Title = "Open Class";
			// 
			// saveFileDialogLeveling
			// 
			this.saveFileDialogLeveling.Filter = "Class files|*.class|Json files|*.json|All files|*.*";
			this.saveFileDialogLeveling.SupportMultiDottedExtensions = true;
			this.saveFileDialogLeveling.Title = "Save Class";
			// 
			// openFileDialogQuests
			// 
			this.openFileDialogQuests.Filter = "Json files|*.json|All files|*.*";
			this.openFileDialogQuests.Title = "Open Quests";
			// 
			// saveFileDialogQuests
			// 
			this.saveFileDialogQuests.Filter = "Json files|*.json|All files|*.*";
			this.saveFileDialogQuests.SupportMultiDottedExtensions = true;
			this.saveFileDialogQuests.Title = "Save Quests";
			// 
			// openFileDialogInvasions
			// 
			this.openFileDialogInvasions.Filter = "Json files|*.json|All files|*.*";
			this.openFileDialogInvasions.Title = "Open Invasions";
			// 
			// saveFileDialogInvasions
			// 
			this.saveFileDialogInvasions.Filter = "Json files|*.json|All files|*.*";
			this.saveFileDialogInvasions.SupportMultiDottedExtensions = true;
			this.saveFileDialogInvasions.Title = "Save Invasions";
			// 
			// EditorForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1008, 681);
			this.Controls.Add(this.tabControlMain);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.menuStrip1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "EditorForm";
			this.Text = "RPG Tools Editor";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EditorForm_FormClosing);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.tabControlMain.ResumeLayout(false);
			this.tabPageInvasions.ResumeLayout(false);
			this.tabPageNpcs.ResumeLayout(false);
			this.tabPageProjectiles.ResumeLayout(false);
			this.tabPageNpcShopsX.ResumeLayout(false);
			this.tabPageLevelingX.ResumeLayout(false);
			this.tabPageQuestsX.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
		private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
		private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.TabControl tabControlMain;
		private System.Windows.Forms.TabPage tabPageInvasions;
		private System.Windows.Forms.TabPage tabPageNpcs;
		private System.Windows.Forms.TabPage tabPageProjectiles;
		private System.Windows.Forms.OpenFileDialog openFileDialogProjectiles;
		private System.Windows.Forms.SaveFileDialog saveFileDialogProjectiles;
		private Controls.NpcEditor npcEditor1;
		private System.Windows.Forms.OpenFileDialog openFileDialogNpcs;
		private System.Windows.Forms.SaveFileDialog saveFileDialogNpcs;
		private Controls.InvasionEditor invasionEditor1;
		private System.Windows.Forms.ImageList imageListTabIcons;
		private System.Windows.Forms.OpenFileDialog openFileDialogNpcShop;
		private System.Windows.Forms.SaveFileDialog saveFileDialogNpcShop;
		private System.Windows.Forms.OpenFileDialog openFileDialogLeveling;
		private System.Windows.Forms.SaveFileDialog saveFileDialogLeveling;
		private System.Windows.Forms.OpenFileDialog openFileDialogQuests;
		private System.Windows.Forms.SaveFileDialog saveFileDialogQuests;
		private System.Windows.Forms.OpenFileDialog openFileDialogInvasions;
		private System.Windows.Forms.SaveFileDialog saveFileDialogInvasions;
		private System.Windows.Forms.TabPage tabPageQuestsX;
		private Controls.ModelTreeEditor modelTreeEditor1;
		private System.Windows.Forms.TabPage tabPageNpcShopsX;
		private Controls.ModelTreeEditor modelTreeEditor2;
		private System.Windows.Forms.TabPage tabPageLevelingX;
		private Controls.ModelTreeEditor modelTreeEditor3;
		private Controls.ModelTreeEditor modelTreeEditor4;
	}
}

