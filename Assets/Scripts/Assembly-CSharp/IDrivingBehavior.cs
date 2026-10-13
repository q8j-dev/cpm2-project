public interface IDrivingBehavior
{
	string Name { get; }

	float Score(in BotContext ctx, in PerceptionSnapshot snap, in RouteTarget route, in SpeedTarget speed, DriveBlackboard bb);

	DriveCommand Produce(in BotContext ctx, in PerceptionSnapshot snap, in RouteTarget route, in SpeedTarget speed, DriveBlackboard bb);
}
