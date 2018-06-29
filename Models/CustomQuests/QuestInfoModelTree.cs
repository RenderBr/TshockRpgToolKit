﻿using Newtonsoft.Json;
using RpgToolsEditor.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgToolsEditor.Models.CustomQuests
{
	public class QuestInfoModelTree : ModelTree
	{
		public override List<ModelTreeNode> CreateTree()
		{
			var nodes = new List<ModelTreeNode>(1);
			var item = new QuestInfo();
			var node = new QuestInfoTreeNode(item);

			nodes.Add(node);

			return nodes;
		}

		public override List<ModelTreeNode> LoadTree(string path)
		{
			var json = File.ReadAllText(path);
			var items = JsonConvert.DeserializeObject<List<QuestInfo>>(json);

			var nodes = items.Select(i => (ModelTreeNode)new QuestInfoTreeNode(i)).ToList();

			return nodes;
		}

		public override void SaveTree(List<ModelTreeNode> tree, string path)
		{
			throw new NotImplementedException();
		}

		public override ModelTreeNode CreateDefaultItem()
		{
			var item = new QuestInfo();
			return new QuestInfoTreeNode(item);
		}
	}

	public class QuestInfoTreeNode : ModelTreeNode
	{
		public QuestInfoTreeNode() : base()
		{
			CanEditModel = true;
			CanAddChild = false;
			CanCopy = true;
			CanDelete = true;
			CanDrag = true;
		}

		public QuestInfoTreeNode(QuestInfo model) : this()
		{
			Model = model;
		}

		public override ModelTreeNode Copy()
		{
			var dstItem = new QuestInfo((QuestInfo)Model);
			var dstNode = new QuestInfoTreeNode(dstItem);
			
			return dstNode;
		}
	}
}
