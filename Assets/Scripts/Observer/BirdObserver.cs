public class BirdObserver : Observer {

    public override void Quit()
    {
        Logger.Print("BirdObserver.Quit.");
    }

    public override void Reset()
    {
        Logger.Print("BirdObserver.Reset.");
    }

    public override void Step()
    {
        Logger.Print("BirdObserver.Step.");
    }
}
