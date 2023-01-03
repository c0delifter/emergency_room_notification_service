using System;

class Solution
{
    static void Main(string[] args)
    {
        //For testing purposes. Coderpad doesn't allow me to overwrite which arguments get submitted to the .NET console

        string[] vals = {
                         "2021-01-10T01:30:00.000Z",
                         "2021-01-10T02:05:23.000Z",
                         "2021-01-10T02:05:23.000Z"
                        };

        if (vals.Length < 2)
        {
            throw new ArgumentException("Insufficient number of arguments supplied");
        }

        DateTime currentTime = Helpers.ParseInputTimeStamp(vals[0]);
        DateTime lowerEstimate = Helpers.ParseInputTimeStamp(vals[1]);
        DateTime upperEstimate = Helpers.ParseInputTimeStamp(vals[2]);

        //Scenario #1 - We are dealing with estimates that are both in the past
        //Scenario #2 - We are dealing with a scenario where one of the two estimates is in the past
        //Scenario #3 - We are dealing with a valid scenario where both time stamps are in the future

        if (lowerEstimate < currentTime && upperEstimate < currentTime)
        {
            Helpers.PrintUpdatedStatusPastEdgeCase();
            return;
        }

        if (lowerEstimate < currentTime && upperEstimate > currentTime)
        {
            Helpers.PrintUpdatedStatusSemiPastEdgeCase(currentTime, upperEstimate);
            return;
        }

        Helpers.PrintUpdatedStatus(currentTime, lowerEstimate, upperEstimate);
    }
}

/*HELPER FUNCTIONS*/
public static class Helpers
{
    public static void PrintUpdatedStatus(DateTime currentTimeStamp, DateTime lowerEstimate, DateTime upperEstimate) 
    {
        string toPrint = "";
        WaitTime lowerEstimateHumanReadable = ToWaitTimeObject(currentTimeStamp, lowerEstimate);
        WaitTime upperEstimateHumanReadable = ToWaitTimeObject(currentTimeStamp, upperEstimate);

        bool hoursMatch = (lowerEstimateHumanReadable.GetHours() == upperEstimateHumanReadable.GetHours());

        bool minutesMatch = (lowerEstimateHumanReadable.GetMinutes() == upperEstimateHumanReadable.GetMinutes());

        if (hoursMatch && minutesMatch)
        {
            toPrint += BuildEstimatedTimeframe(lowerEstimateHumanReadable);
        }
        else
        {
            toPrint += BuildEstimatedTimeframe(lowerEstimateHumanReadable);
            toPrint += " - ";
            toPrint += BuildEstimatedTimeframe(upperEstimateHumanReadable);
        }

        toPrint = SanitizeTimeFormat(toPrint);

        Console.WriteLine($"You will be moved to a treatment area within the next {toPrint}");
    }

    public static void PrintUpdatedStatusSemiPastEdgeCase(DateTime currentTimeStamp, DateTime upperEstimate) 
    {
        PrintUpdatedStatus(currentTimeStamp, upperEstimate, upperEstimate);
    }

    public static void PrintUpdatedStatusPastEdgeCase() 
    {
        Console.WriteLine($"You will be moved to a treatment area as soon as possible");
    }

    public static WaitTime ToWaitTimeObject(DateTime currentTimeStamp, DateTime estimateTimeStamp)
    {
        TimeSpan waitTime = estimateTimeStamp - currentTimeStamp;
        WaitTime toReturn = new WaitTime(waitTime.Hours, waitTime.Minutes);
        return toReturn;
    }

    public static DateTime ParseInputTimeStamp(string timestamp)
    {
        if (DateTime.TryParse(timestamp, out DateTime toReturn))
        {
            return RoundToNearestFiveMinutes(toReturn);
        }
        else
        {
            throw new InvalidCastException("Please provide a valid timestamp");
        }
    }

    public static DateTime RoundToNearestFiveMinutes(DateTime dt)
    {
        long fiveMinuteTicks = TimeSpan.FromMinutes(5).Ticks;
        return (dt.Ticks % fiveMinuteTicks) == 0 ? dt : new DateTime((dt.Ticks / fiveMinuteTicks) * fiveMinuteTicks);
    }

    public static string BuildEstimatedTimeframe(WaitTime wt)
    {
        string toReturn = "";
        int hours = wt.GetHours();
        int minutes = wt.GetMinutes();

        if (hours > 0)
            toReturn += $"{hours}h";

        if (minutes > 0)
            toReturn += (hours > 0) ? $" {minutes}min" : $"{minutes}min";

        return toReturn;
    }

    public static string SanitizeTimeFormat(string input)
    {
        string toReturn = input;
        bool hasMinutes = toReturn.Contains("min");
        bool hasHours = toReturn.Contains("h");

        if (hasHours && !hasMinutes)
        {
            toReturn = RemoveDuplicate("h", toReturn);
        }

        if (!hasHours && hasMinutes)
        {
            toReturn = RemoveDuplicate("min", toReturn);
        }        

        return toReturn;
    }

    public static string RemoveDuplicate(string searchValue, string input)
    {
        int indexFirst = input.IndexOf(searchValue);
        int indexLast = input.LastIndexOf(searchValue);

        if (indexFirst != indexLast)
        {
            return input.Remove(indexFirst, searchValue.Length).Insert(indexFirst, "");
        }

        return input;
    }
}

/* MODELS */
public class WaitTime 
{
    private readonly int _hours;
    private readonly int _minutes;
    private readonly bool _capHours;

    public WaitTime(int hours, int minutes)
    {
        _hours = hours;
        _minutes = minutes;
        _capHours = hours >= 2;
    }

    public int GetHours()
    {
        return this._capHours ? 2 : this._hours;
    }

    public int GetMinutes()
    {
        return this._capHours ? 0 : this._minutes;
    } 
}
