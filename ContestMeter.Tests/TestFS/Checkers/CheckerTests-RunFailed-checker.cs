using System.IO;
class CheckerTests_RunFailed_checker
{
    public static void Main()
    {
        var xml = "<result outcome=\"failed\"></result>";
        File.WriteAllText("result.xml", xml);
    }
}

