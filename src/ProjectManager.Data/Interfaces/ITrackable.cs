using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Data.Interfaces;

public interface ITrackable
{
    public Instant CreatedAt { get; set; }
    public string CreatedBy { get; set; } 
    public Instant ModifiedAt { get; set; }
    public string ModifiedBy { get; set; } 
    public Instant? DeletedAt { get; set; }
    public string? DeletedBy { get; set; } 
}
public static class ITrackableExtensions
{
    private const string SYSTEM = "System";
    public static T SetCreatedBySystem<T>(this T trackable, Instant now)
        where T : class,ITrackable
        => trackable.SetCreatedBy(SYSTEM, now);
    public static T SetModyfyBySystem<T>(this T trackable, Instant now)
        where T : class, ITrackable
        => trackable.SetModifyBy(SYSTEM, now);
    public static T SetDeletedBySystem<T>(this T trackable, Instant now)
    where T : class, ITrackable
    => trackable.SetDeletedBy(SYSTEM, now);

    public static T SetCreatedBy<T>(this T trackable, string author, Instant now)
        where T : class, ITrackable
    {
        trackable.CreatedAt = now;
        trackable.CreatedBy = author;

        return trackable.SetModifyBy(author, now); ;
    }
    public static T SetModifyBy<T>(this T trackable, string author, Instant now)
            where T : class, ITrackable
    {
        trackable.ModifiedAt = now;
        trackable.ModifiedBy = author;

        return trackable;
    }
    public static T SetDeletedBy<T>(this T trackable, string author, Instant now)
                where T : class, ITrackable
    {
        trackable.DeletedAt = now;
        trackable.DeletedBy = author;

        return trackable;
    }
}
