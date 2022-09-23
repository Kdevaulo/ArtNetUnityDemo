namespace ArtNet
{
	public static class Extensions
	{
		public static byte[] Block(this byte[] data, int offset, int length)
		{
			var tmp = new byte[length];

			for (int i = offset; i < length; i++)
				tmp[i - offset] = data[i];

			return tmp;
		}
	}
}

