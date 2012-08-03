using System;

namespace Iridio.Helpers
{
  public static class CombGuid
  {
    public static Guid GetNewCombGuid()
    {
      byte[] destinationArray = Guid.NewGuid().ToByteArray();
      DateTime time = new DateTime(0x76c, 1, 1);
      DateTime now = DateTime.Now;
      TimeSpan span = new TimeSpan(now.Ticks - time.Ticks);
      TimeSpan timeOfDay = now.TimeOfDay;
      byte[] bytes = BitConverter.GetBytes(span.Days);
      byte[] array = BitConverter.GetBytes((long)(timeOfDay.TotalMilliseconds / 3.333333));
      Array.Reverse(bytes);
      Array.Reverse(array);
      Array.Copy(bytes, bytes.Length - 2, destinationArray, destinationArray.Length - 6, 2);
      Array.Copy(array, array.Length - 4, destinationArray, destinationArray.Length - 4, 4);
      return new Guid(destinationArray);
    }
  }
}
