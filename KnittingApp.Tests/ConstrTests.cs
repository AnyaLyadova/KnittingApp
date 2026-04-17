namespace KnittingApp.Tests;

[TestClass]
public class ConstrTests
{

    [TestMethod]
    public void CreateNewModel_ShouldCompleteSuccessfully()
    {
        List<string> parts = new List<string> { "body", "sleeve", "oneck" };

        string name = "testmodel";
        double width = 12;
        double height = 12;
        int loopInWidht = 12;
        int loopInHeight = 12;

        var constructor = new Constructor();
        var model = constructor.CreateNewModel(name, parts, height, width, loopInHeight, loopInWidht);
        Assert.IsNotNull(model);
        Assert.AreEqual(model.Parts.Count, 3);
        Assert.AreEqual(constructor.GetModels()[0], model);
    }
}
