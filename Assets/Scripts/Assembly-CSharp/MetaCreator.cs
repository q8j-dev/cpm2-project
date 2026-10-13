using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

public class MetaCreator : IConfigCreator
{
	public void Create(string relativePath, string MIME_TYPE)
	{
		CustomComparer comparer = new CustomComparer();
		List<string> list = Directory.GetFiles(relativePath + "/", "*." + MIME_TYPE).ToList();
		string[] directories = Directory.GetDirectories(relativePath);
		string[] array2;
		using (FileStream fileStream = File.Create(relativePath + "/meta.txt"))
		{
			List<string> list2 = new List<string>();
			int num = list.Count - 1;
			foreach (string item2 in list)
			{
				bool flag = false;
				string[] array = item2.Split('/');
				string text = "";
				array2 = array;
				foreach (string text2 in array2)
				{
					if (flag)
					{
						text += text2;
					}
					if (flag && !text2.Contains('.'))
					{
						text += "/";
					}
					if (text2 == "Resources_moved")
					{
						flag = true;
					}
				}
				string item = item2.Remove(0, 1);
				list2.Add(item);
			}
			list2.Sort(comparer);
			foreach (string item3 in list2)
			{
				string text3 = item3.Split(new string[1] { "Assets/Resources_moved" }, StringSplitOptions.None)[1];
				text3 = text3.Remove(0, 1);
				byte[] array3 = ((num == 0) ? new UTF8Encoding(encoderShouldEmitUTF8Identifier: true).GetBytes(text3) : new UTF8Encoding(encoderShouldEmitUTF8Identifier: true).GetBytes(text3 + "\n"));
				fileStream.Write(array3, 0, array3.Length);
				num--;
			}
		}
		array2 = directories;
		foreach (string relativePath2 in array2)
		{
			Create(relativePath2, MIME_TYPE);
		}
	}
}
