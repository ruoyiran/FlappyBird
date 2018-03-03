
public class AgentMessage
{
    public int Action { get; set; }
}

public class AgentStepMessage
{
    public float Reward { get; set; }
    public bool IsDone { get; set; }
    public string ImagePath { get; set; }
}