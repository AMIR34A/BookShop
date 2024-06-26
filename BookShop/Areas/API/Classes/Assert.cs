﻿using System.Collections;

namespace BookShop.Areas.API.Classes;

public class Assert
{
    public static void NotNull<T>(T obj, string name, string? message) where T : class
    {
        if (obj is null)
            throw new ArgumentNullException($"{name} : {typeof(T)}", message);
    }

    public static void NotNull<T>(T? obj, string name, string? message) where T : struct
    {
        if (!obj.HasValue)
            throw new ArgumentNullException($"{name} : {typeof(T)}", message);
    }

    public static void NotEmpty<T>(T obj, string name, string? message, T? defaultValue) where T : class
    {
        if (obj == defaultValue
            || (obj is string str && string.IsNullOrWhiteSpace(str))
            || (obj is IEnumerable list && !list.Cast<object>().Any()))
        {
            throw new ArgumentException("Argument is empty : " + message, $"{name} : {typeof(T)}");
        }
    }
}

