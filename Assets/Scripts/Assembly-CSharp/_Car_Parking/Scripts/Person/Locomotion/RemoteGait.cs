namespace _Car_Parking.Scripts.Person.Locomotion
{
	public static class RemoteGait
	{
		private const float Margin = 0.25f;

		public static int Derive(float moveSpeed, int lastGait, bool crouching, float walkSpeed, float runSpeed, float sprintSpeed)
		{
			if (moveSpeed < 0.01f)
			{
				return 0;
			}
			if (crouching)
			{
				return 1;
			}
			float num = (walkSpeed + runSpeed) * 0.5f;
			float num2 = (runSpeed + sprintSpeed) * 0.5f;
			int num3 = ((lastGait < 1) ? 1 : lastGait);
			if (num3 < 2 && moveSpeed > num + 0.25f)
			{
				num3 = 2;
			}
			else if (num3 >= 2 && moveSpeed < num - 0.25f)
			{
				num3 = 1;
			}
			if (num3 < 3 && moveSpeed > num2 + 0.25f)
			{
				num3 = 3;
			}
			else if (num3 >= 3 && moveSpeed < num2 - 0.25f)
			{
				num3 = 2;
			}
			return num3;
		}
	}
}
