using Roblox;

namespace RobloxValidation.Tests;

/// <summary>
/// Tests task, debug, os library calls
/// </summary>
public class LuaLibrariesTest {
    public static void TestTask() {
        task.spawn(() => {
            System.Console.WriteLine("Spawned!");
        });
        task.delay(1.0, () => {
            System.Console.WriteLine("Delayed!");
        });
        task.wait(0.5);
        task.defer(() => {
            System.Console.WriteLine("Deferred!");
        });
    }

    public static void TestDebug() {
        debug.profilebegin("MyProfile");
        // do work
        debug.profileend();
        var trace = debug.traceback();
    }

    public static void TestOs() {
        var time = os.time();
        var clock = os.clock();
        System.Console.WriteLine("Time: " + time + " Clock: " + clock);
    }
}
