namespace ScriptableSurfaceNamespace
{
	public class BinaryNode
	{
		public BinaryNode LeftNode { get; set; }

		public BinaryNode RightNode { get; set; }

		public SurfaceData Data { get; }

		public BinaryNode(ref SurfaceData data)
		{
			Data = data;
			LeftNode = null;
			RightNode = null;
		}
	}
}
