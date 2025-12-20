using Roblox;

namespace RobloxValidation.Tests;

/// <summary>
/// Tests Vector3, CFrame, Color3 creation
/// </summary>
public class DataTypesTest {
    public static void TestVector3() {
        var v1 = Vector3.@new(1, 2, 3);
        var v2 = Vector3.@new(10);
        var zero = Vector3.zero;
        var one = Vector3.one;
        System.Console.WriteLine("X: " + v1.X + " Y: " + v1.Y + " Z: " + v1.Z);
    }

    public static void TestCFrame() {
        var cf1 = CFrame.@new(0, 10, 0);
        var cf2 = CFrame.identity;
        var position = cf1.Position;
    }

    public static void TestColor3() {
        var c1 = Color3.@new(1.0, 0.5, 0.25);
        var c2 = Color3.fromRGB(255, 128, 64);
        var c3 = Color3.fromHSV(0.5, 1.0, 1.0);
        System.Console.WriteLine("R: " + c1.R);
    }

    public static void TestUDim2() {
        var ud1 = UDim2.@new(0, 100, 0, 50);
        var ud2 = UDim2.fromScale(0.5, 0.5);
        var ud3 = UDim2.fromOffset(100, 100);
    }
}
