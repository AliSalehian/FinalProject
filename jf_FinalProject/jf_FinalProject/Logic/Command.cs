namespace jf_FinalProject.Logic
{
    public enum BrakeType
    {
        Moment = 0,
        Pressure = 1
    }

    public enum RunDirection
    {
        Forward = 0,
        Backward = 1
    }
    public class Command
    {
        public static void Brake(double momentValue, double pressureValue, BrakeType brakeType)
        {
            if(brakeType == BrakeType.Moment)
            {
                Logger.Logger.Log($"brake in moment mode with value: {momentValue}");
            }
            else
            {
                Logger.Logger.Log($"brake in pressure mode with value: {pressureValue}");
            }
        }

        public static void UnloadBrake()
        {
            Logger.Logger.Log("unload brake");
        }

        public static void Run(double speed, RunDirection direction)
        {
            if(direction == RunDirection.Forward)
            {
                Logger.Logger.Log($"run with speed: {speed}");
            }
            else
            {
                Logger.Logger.Log($"run with speed: {speed * -1}");
            }
        }
    }
}
