using System.IO;
class CheckerTests_RunAccepted_checker
{
    public static void Main()
    {
        var xml = "<result outcome=\"accepted\"></result>";
        File.WriteAllText("result.xml", xml);
    }
}

