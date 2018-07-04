﻿using RpgToolsEditor.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgToolsEditor.Models.CustomNpcs
{
	public class ProjectileTreeNode : ModelTreeNode
	{
		public ProjectileTreeNode() : base()
		{
			CanEditModel = true;
			CanAdd = true;
			CanCopy = true;
			CanDelete = true;
			CanDrag = true;
		}

		public ProjectileTreeNode(Projectile model) : this()
		{
			Model = model;
		}

		public override ModelTreeNode AddItem()
		{
			var model = new Projectile();
			var node = new ProjectileTreeNode(model);

			this.AddSibling(node);

			return node;
		}

		public override bool CanAcceptDraggedNode(ModelTreeNode node)
		{
			return node is ProjectileTreeNode;
		}

		public override bool TryAcceptDraggedNode(ModelTreeNode draggedNode)
		{
			if( CanAcceptDraggedNode(draggedNode) )
			{
				AddSibling(draggedNode);

				return true;
			}

			return false;
		}
	}
}
