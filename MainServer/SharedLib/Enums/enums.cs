using Microsoft.EntityFrameworkCore.Metadata;
using SharedLib.Models;
using System.Collections.Generic;

namespace SharedLib.Enums
{
    public enum Tables
    {
        User,
        Comment,
        Video,
        ServerLog
    }
    public enum LogType
    {
        загрузка,
        просмотр
    }

    public enum ErrorType
    {
        NotFound,       // 404
        ValidationError,// 400  
        Unauthorized,   // 401
        Forbidden,      // 403
        Conflict,       // 409
        ServerError     // 500
    }

    public enum UserRole
    {
        Admin,
        Virifier
    }

    public enum AdminTables
    {
        User,
        Comment,
        Video,
        ServerLog,
        Dislike,
        Like,
        View,
        Employee
    }

    public enum FerifierTables
    {
        User,
        Comment,
        Video,
        Dislike,
        Like,
        View
    }

    public static class TableToClassMapper
    {
        private static readonly Dictionary<string, Type> tableToClassMap = new()
        {
            { "User", typeof(User) },
            { "Comment", typeof(Comment) },
            { "Video", typeof(Video) },
            { "ServerLog", typeof(ServerLog) },
            { "Dislike", typeof(DisLike) },
            { "Like", typeof(Like) },
            { "View", typeof(View) },
            { "Employee", typeof(Employee) }
        };

        public static Dictionary<string, Type> TableToClassMap => tableToClassMap;
    }
}
