using GarageDI.Contracts;
using GarageDI.Entities;
using GarageDI.Garage;
using Moq;
using System.Collections;

namespace GarageDI.Tests.Garage;

/// <summary>
/// Unit tests for <see cref="InMemoryGarage{T}"/>.
///
/// Naming convention:  MethodName_Scenario_ExpectedBehavior
/// Structure:          AAA  – Arrange / Act / Assert
/// Isolation:          Each test creates its own mocks via the factory helpers
///                     below, so no state leaks between tests.
/// </summary>
public class InMemoryGarageTests
{
    // -------------------------------------------------------------------------
    // Factory helpers
    // -------------------------------------------------------------------------

    /// <summary>
    /// Creates a fresh garage with its own mock dependencies.
    /// Using local mocks per test avoids shared-state problems that can make
    /// tests order-dependent or hard to debug.
    /// </summary>
    private static (InMemoryGarage<Car> garage, Mock<IGaragePersistenceService> persistenceMock)
        CreateGarage(int capacity = 3, string name = "TestGarage")
    {
        var settingsMock = new Mock<ISettings>();
        settingsMock.Setup(s => s.Name).Returns(name);
        settingsMock.Setup(s => s.Capacity).Returns(capacity);

        var persistenceMock = new Mock<IGaragePersistenceService>();

        var garage = new InMemoryGarage<Car>(settingsMock.Object, persistenceMock.Object);
        return (garage, persistenceMock);
    }

    /// <summary>
    /// Creates a <see cref="Car"/> with a given registration number.
    /// </summary>
    private static Car CreateCar(string regNo)
        => new Car { RegNo = regNo, Color = "Red", NrOfWheels = 4 };

    // -------------------------------------------------------------------------
    // Constructor
    // -------------------------------------------------------------------------

    [Fact]
    public void Constructor_WithNullSettings_ThrowsArgumentNullException()
    {
        var persistenceMock = new Mock<IGaragePersistenceService>();

        Assert.Throws<ArgumentNullException>(() =>
            new InMemoryGarage<Car>(null!, persistenceMock.Object));
    }

    [Fact]
    public void Constructor_SetsNameFromSettings()
    {
        var (garage, _) = CreateGarage(name: "MyGarage");

        Assert.Equal("MyGarage", garage.Name);
    }

    /// <summary>
    /// The garage enforces a minimum capacity of 2 regardless of configuration.
    /// </summary>
    [Theory]
    [InlineData(1, 2)]   // below minimum  =  2
    [InlineData(0, 2)]   // zero           =  2
    [InlineData(5, 5)]   // above minimum  =  5
    public void Constructor_SetsCapacityToAtLeastTwo(int configuredCapacity, int expectedCapacity)
    {
        var (garage, _) = CreateGarage(capacity: configuredCapacity);

        Assert.Equal(expectedCapacity, garage.Capacity);
    }

    // -------------------------------------------------------------------------
    // Initial state
    // -------------------------------------------------------------------------

    [Fact]
    public void Count_StartsAtZero()
    {
        var (garage, _) = CreateGarage();

        Assert.Equal(0, garage.Count);
    }

    [Fact]
    public void IsFull_IsFalseWhenEmpty()
    {
        var (garage, _) = CreateGarage();

        Assert.False(garage.IsFull);
    }

    // -------------------------------------------------------------------------
    // Park
    // -------------------------------------------------------------------------

    [Fact]
    public void Park_ReturnsTrueWhenGarageHasSpace()
    {
        var (garage, _) = CreateGarage();

        var result = garage.Park(CreateCar("ABC123"));

        Assert.True(result);
    }

    [Fact]
    public void Park_IncreasesCount()
    {
        var (garage, _) = CreateGarage();

        garage.Park(CreateCar("ABC123"));

        Assert.Equal(1, garage.Count);
    }

    [Fact]
    public void Park_CallsSaveGarageOnPersistenceWithCorrectArguments()
    {
        var (garage, persistenceMock) = CreateGarage(capacity: 3, name: "TestGarage");
        var car = CreateCar("ABC123");

        garage.Park(car);

        // Verify that persistence is called with the right name, capacity,
        // and a collection containing exactly the parked vehicle.
        persistenceMock.Verify(
            p => p.SaveGarage(
                "TestGarage",
                3,
                It.Is<IEnumerable<IVehicle>>(vehicles => vehicles.Single() == car)),
                Times.Once);
    }

    [Fact]
    public void Park_ReturnsFalseWhenGarageIsFull()
    {
        var (garage, _) = CreateGarage(capacity: 2);

        garage.Park(CreateCar("A"));
        garage.Park(CreateCar("B"));
        var result = garage.Park(CreateCar("C"));  // garage is now full

        Assert.True(garage.IsFull);
        Assert.False(result);
    }

    [Fact]
    public void Park_DoesNotIncreaseCountWhenFull()
    {
        var (garage, _) = CreateGarage(capacity: 2);

        garage.Park(CreateCar("A"));
        garage.Park(CreateCar("B"));
        garage.Park(CreateCar("C"));  // rejected

        Assert.Equal(2, garage.Count);
    }

    [Fact]
    public void Park_DoesNotCallPersistenceWhenGarageIsFull()
    {
        var (garage, persistenceMock) = CreateGarage(capacity: 2);

        garage.Park(CreateCar("A"));
        garage.Park(CreateCar("B"));
        persistenceMock.Invocations.Clear();  // reset after the two successful parks

        garage.Park(CreateCar("C"));  // should be rejected without saving

        persistenceMock.Verify(
            p => p.SaveGarage(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<IEnumerable<IVehicle>>()),
            Times.Never);
    }

    [Fact]
    public void Park_ReturnsFalseWhenRegNoAlreadyExists()
    {
        var (garage, _) = CreateGarage();

        garage.Park(CreateCar("ABC123"));
        var result = garage.Park(CreateCar("ABC123"));  // samma reg-nummer

        Assert.False(result);
    }

    [Fact]
    public void Park_DoesNotIncreaseCountWhenRegNoAlreadyExists()
    {
        var (garage, _) = CreateGarage();

        garage.Park(CreateCar("ABC123"));
        garage.Park(CreateCar("ABC123"));  // avvisas

        Assert.Equal(1, garage.Count);
    }

    [Fact]
    public void Park_DoesNotCallPersistenceWhenRegNoAlreadyExists()
    {
        var (garage, persistenceMock) = CreateGarage();
        garage.Park(CreateCar("ABC123"));
        persistenceMock.Invocations.Clear();

        garage.Park(CreateCar("ABC123"));  // dublett — ska inte spara

        persistenceMock.Verify(
            p => p.SaveGarage(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<IEnumerable<IVehicle>>()),
            Times.Never);
    }

    // -------------------------------------------------------------------------
    // Leave
    // -------------------------------------------------------------------------

    [Fact]
    public void Leave_ReturnsFalseWhenVehicleIsNull()
    {
        var (garage, _) = CreateGarage();

        var result = garage.Leave(null!);

        Assert.False(result);
    }

    [Fact]
    public void Leave_ReturnsTrueWhenVehicleIsParked()
    {
        var (garage, _) = CreateGarage();
        var car = CreateCar("ABC123");
        garage.Park(car);

        var result = garage.Leave(car);

        Assert.True(result);
    }

    [Fact]
    public void Leave_DecreasesCount()
    {
        var (garage, _) = CreateGarage();
        var car = CreateCar("ABC123");
        garage.Park(car);

        garage.Leave(car);

        Assert.Equal(0, garage.Count);
    }

    [Fact]
    public void Leave_CallsSaveGarageOnPersistenceWithEmptyCollection()
    {
        var (garage, persistenceMock) = CreateGarage(capacity: 3, name: "TestGarage");
        var car = CreateCar("ABC123");
        garage.Park(car);
        persistenceMock.Invocations.Clear();  // isolate – only verify the Leave call

        garage.Leave(car);

        persistenceMock.Verify(
            p => p.SaveGarage(
                "TestGarage",
                3,
                It.Is<IEnumerable<IVehicle>>(vehicles => !vehicles.Any())),
            Times.Once);
    }

    [Fact]
    public void Leave_ReturnsFalseWhenVehicleNotInGarage()
    {
        var (garage, _) = CreateGarage();
        var parkedCar = CreateCar("ABC123");
        var otherCar  = CreateCar("XYZ789");
        garage.Park(parkedCar);

        var result = garage.Leave(otherCar);

        Assert.False(result);
        Assert.Equal(1, garage.Count);  // original vehicle is still there
    }

    [Fact]
    public void Leave_DoesNotCallPersistenceWhenVehicleNotInGarage()
    {
        var (garage, persistenceMock) = CreateGarage();
        garage.Park(CreateCar("ABC123"));
        persistenceMock.Invocations.Clear();

        garage.Leave(CreateCar("XYZ789"));

        persistenceMock.Verify(
            p => p.SaveGarage(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<IEnumerable<IVehicle>>()),
            Times.Never);
    }

    // -------------------------------------------------------------------------
    // IEnumerable
    // -------------------------------------------------------------------------

    [Fact]
    public void GetEnumerator_ReturnsOnlyParkedVehicles()
    {
        var (garage, _) = CreateGarage();
        var first  = CreateCar("A");
        var second = CreateCar("B");
        garage.Park(first);
        garage.Park(second);

        var vehicles = garage.ToList();

        Assert.Equal(2, vehicles.Count);
        Assert.Contains(first,  vehicles);
        Assert.Contains(second, vehicles);
    }

    [Fact]
    public void GetEnumerator_IsEmptyWhenNoVehiclesParked()
    {
        var (garage, _) = CreateGarage();

        Assert.Empty(garage);
    }

    [Fact]
    public void NonGenericGetEnumerator_ReturnsSameVehiclesAsGenericEnumerator()
    {
        var (garage, _) = CreateGarage();
        var car = CreateCar("ABC123");
        garage.Park(car);

        // Cast via the non-generic IEnumerable interface to verify both paths
        // return the same data – important since the class implements both.
        var viaNonGeneric = ((IEnumerable)garage).Cast<Car>().ToList();

        Assert.Single(viaNonGeneric);
        Assert.Equal(car, viaNonGeneric[0]);
    }
}
