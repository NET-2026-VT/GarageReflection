using GarageDI.Entities;

namespace GarageDI.Tests.Entities;

/// <summary>
/// Unit tests for <see cref="Vehicle"/> base class validation.
/// </summary>
public class VehicleTests
{
 
    private static Car CreateCar(string regNo = "ABC123", string color = "RED", int nrOfWheels = 4)
        => new Car { RegNo = regNo, Color = color, NrOfWheels = nrOfWheels };

    // -------------------------------------------------------------------------
    // RegNo – formatvalidering i settern
    // -------------------------------------------------------------------------

    [Fact]
    public void RegNo_StoresValueInUpperCase()
    {
        var car = CreateCar(regNo: "abc123");

        Assert.Equal("ABC123", car.RegNo);
    }

    [Theory]
    [InlineData("")]        // tom sträng
    [InlineData("   ")]     // bara blanksteg
    public void RegNo_ThrowsArgumentException_WhenValueIsEmptyOrWhitespace(string invalidRegNo)
    {
        Assert.Throws<ArgumentException>(() =>
            CreateCar(regNo: invalidRegNo));
    }
    

    // -------------------------------------------------------------------------
    // Color – normalisering
    // -------------------------------------------------------------------------

    [Fact]
    public void Color_StoresValueInUpperCase()
    {
        var car = CreateCar(color: "blue");

        Assert.Equal("BLUE", car.Color);
    }

    // -------------------------------------------------------------------------
    // Indexer
    // -------------------------------------------------------------------------

    [Fact]
    public void Indexer_Get_ReturnsPropertyValue()
    {
        var car = CreateCar();

        Assert.Equal("ABC123", car["RegNo"]);
    }

    [Fact]
    public void Indexer_Set_UpdatesPropertyValue()
    {
        var car = CreateCar();

        car["Color"] = "BLUE";

        Assert.Equal("BLUE", car.Color);
    }

    [Fact]
    public void Indexer_Get_ThrowsArgumentException_ForInvalidPropertyName()
    {
        var car = CreateCar();

        Assert.Throws<ArgumentException>(() => _ = car["DoesNotExist"]);
    }
}
