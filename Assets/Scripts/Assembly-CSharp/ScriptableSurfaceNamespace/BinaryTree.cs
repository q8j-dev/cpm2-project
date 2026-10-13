namespace ScriptableSurfaceNamespace
{
	public class BinaryTree
	{
		public BinaryNode Root { get; set; }

		public bool Add(SurfaceData newSurfaceData)
		{
			BinaryNode binaryNode = null;
			BinaryNode binaryNode2 = Root;
			while (binaryNode2 != null)
			{
				binaryNode = binaryNode2;
				if (newSurfaceData.hash < binaryNode2.Data.hash)
				{
					binaryNode2 = binaryNode2.LeftNode;
					continue;
				}
				if (newSurfaceData.hash > binaryNode2.Data.hash)
				{
					binaryNode2 = binaryNode2.RightNode;
					continue;
				}
				return false;
			}
			BinaryNode binaryNode3 = new BinaryNode(ref newSurfaceData);
			if (Root == null)
			{
				Root = binaryNode3;
			}
			else if (newSurfaceData.hash < binaryNode.Data.hash)
			{
				binaryNode.LeftNode = binaryNode3;
			}
			else
			{
				binaryNode.RightNode = binaryNode3;
			}
			return true;
		}

		public Surface Find(int hash)
		{
			return Find(hash, Root)?.Data.surface ?? Surface.None;
		}

		private BinaryNode Find(int hash, BinaryNode parent)
		{
			if (parent != null)
			{
				if (hash == parent.Data.hash)
				{
					return parent;
				}
				if (hash < parent.Data.hash)
				{
					return Find(hash, parent.LeftNode);
				}
				return Find(hash, parent.RightNode);
			}
			return null;
		}
	}
}
