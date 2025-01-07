using RimWorld;

namespace RandomFactions.filters;

public class AllowedTemperatureFactionDefFilter(float temperature) : FactionDefFilter
{
    protected override bool Matches(FactionDef f)
    {
        return f.allowedArrivalTemperatureRange.Includes(temperature);
    }
}