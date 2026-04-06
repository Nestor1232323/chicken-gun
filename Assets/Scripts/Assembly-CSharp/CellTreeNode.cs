using System.Collections.Generic;
using UnityEngine;

public class CellTreeNode
{
	public enum ENodeType
	{
		Root = 0,
		Node = 1,
		Leaf = 2
	}

	public byte Id;

	public Vector3 Center;

	public Vector3 Size;

	public Vector3 TopLeft;

	public Vector3 BottomRight;

	public ENodeType NodeType;

	public CellTreeNode Parent;

	public List<CellTreeNode> Childs;

	private float maxDistance;

	public CellTreeNode()
	{
	}

	public CellTreeNode(byte id, ENodeType nodeType, CellTreeNode parent)
	{
		Id = id;
		NodeType = nodeType;
		Parent = parent;
	}

	public void AddChild(CellTreeNode child)
	{
		if (Childs == null)
		{
			Childs = new List<CellTreeNode>(1);
		}
		Childs.Add(child);
	}

	public void Draw()
	{
	}

	public void GetActiveCells(List<byte> activeCells, bool yIsUpAxis, Vector3 position)
	{
		if (NodeType != ENodeType.Leaf)
		{
			foreach (CellTreeNode child in Childs)
			{
				child.GetActiveCells(activeCells, yIsUpAxis, position);
			}
			return;
		}
		if (!IsPointNearCell(yIsUpAxis, position))
		{
			return;
		}
		if (IsPointInsideCell(yIsUpAxis, position))
		{
			activeCells.Insert(0, Id);
			for (CellTreeNode parent = Parent; parent != null; parent = parent.Parent)
			{
				activeCells.Insert(0, parent.Id);
			}
		}
		else
		{
			activeCells.Add(Id);
		}
	}

	public bool IsPointInsideCell(bool yIsUpAxis, Vector3 point)
	{
		if (point.x < TopLeft.x || point.x > BottomRight.x)
		{
			return false;
		}
		if (yIsUpAxis)
		{
			if (point.y >= TopLeft.y && point.y <= BottomRight.y)
			{
				return true;
			}
		}
		else if (point.z >= TopLeft.z && point.z <= BottomRight.z)
		{
			return true;
		}
		return false;
	}

	public bool IsPointNearCell(bool yIsUpAxis, Vector3 point)
	{
		if (maxDistance == 0f)
		{
			maxDistance = (Size.x + Size.y + Size.z) / 2f;
		}
		return (point - Center).sqrMagnitude <= maxDistance * maxDistance;
	}
}
